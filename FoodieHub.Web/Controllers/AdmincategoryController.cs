// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện làm việc với danh sách đối tượng (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework để sử dụng các phương thức như Include, EntityState
using System.Data.Entity;
// Khai báo thư viện truy vấn LINQ (ToList, Find, FirstOrDefault, Count...)
using System.Linq;
// Khai báo thư viện xử lý yêu cầu HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC (Controller, ActionResult, HttpNotFound...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (Category, Product, FoodieHubDbContext)
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller thuộc phân hệ Quản trị của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Khai báo lớp AdminCategoryController kế thừa từ lớp Controller mặc định của ASP.NET MVC
    public class AdminCategoryController : Controller
    {
        // Khởi tạo đối tượng db từ lớp FoodieHubDbContext để kết nối và thao tác với SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH DANH MỤC (Yêu cầu Barem)
        // ----------------------------------------------------------------------
        // Action xử lý yêu cầu GET mặc định để hiển thị toàn bộ danh mục món ăn
        [HttpGet]
        public ActionResult Index()
        {
            // Truy vấn lấy danh sách tất cả các danh mục từ bảng Categories
            // Hàm Include(c => c.Products) nạp kèm các sản phẩm để đếm số lượng món ăn trong danh mục
            var categoriesList = db.Categories.Include(c => c.Products).ToList();

            // Trả về View (Index.cshtml) và truyền danh sách categoriesList sang để hiển thị
            return View(categoriesList);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION DETAILS: XEM CHI TIẾT DANH MỤC & MÓN ĂN (Yêu cầu Barem Dòng 8)
        // ----------------------------------------------------------------------
        // Action nhận tham số id là Mã danh mục cần xem chi tiết
        [HttpGet]
        public ActionResult Details(int? id)
        {
            // Kiểm tra an toàn: Nếu id truyền vào bị rỗng (null)
            if (id == null)
            {
                // Trả về trang lỗi HttpNotFound (Mã 404) để dừng luồng
                return HttpNotFound();
            }

            // Tìm danh mục theo ID và nạp kèm danh sách các món ăn (Products) thuộc danh mục đó
            Category category = db.Categories.Include(c => c.Products)
                                             .FirstOrDefault(c => c.CategoryID == id);

            // Kiểm tra nếu không tìm thấy bản ghi danh mục nào khớp với ID
            if (category == null)
            {
                // Trả về lỗi HttpNotFound (Mã 404)
                return HttpNotFound();
            }

            // Trả về View (Details.cshtml) kèm đối tượng category tìm được
            return View(category);
        }

        // ----------------------------------------------------------------------
        // 3. ACTION CREATE: THÊM MỚI DANH MỤC
        // ----------------------------------------------------------------------
        // Action GET hiển thị giao diện Form thêm mới danh mục
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // Action POST tiếp nhận dữ liệu từ Form Submit để lưu danh mục mới vào CSDL
        [HttpPost]
        // Bảo mật chống tấn công giả mạo yêu cầu chéo trang (CSRF)
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            // Kiểm tra xem dữ liệu nhập trên Form có thỏa mãn DataAnnotations ở Model hay không
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên danh mục nhập vào có bị trùng lặp trong CSDL hay chưa
                var existingCategory = db.Categories.FirstOrDefault(c => c.CategoryName.ToLower() == category.CategoryName.ToLower());
                if (existingCategory != null)
                {
                    // Thêm thông báo lỗi riêng cho trường CategoryName nếu bị trùng tên
                    ModelState.AddModelError("CategoryName", "Tên danh mục này đã tồn tại trong hệ thống!");
                    return View(category);
                }

                // Đưa đối tượng danh mục mới vào bộ đệm của Entity Framework
                db.Categories.Add(category);
                // Thực thi câu lệnh SQL INSERT INTO để ghi xuống CSDL
                db.SaveChanges();
                // Đá trình duyệt về lại trang danh sách danh mục (Index)
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu vi phạm ràng buộc, nạp lại trang Form kèm dữ liệu người dùng vừa gõ
            return View(category);
        }

        // ----------------------------------------------------------------------
        // 4. ACTION EDIT: CHỈNH SỬA DANH MỤC
        // ----------------------------------------------------------------------
        // Action GET nhận id của danh mục cần sửa và kéo dữ liệu cũ lên Form
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            // Tìm bản ghi danh mục theo khóa chính ID
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // Action POST nhận đối tượng category chứa thông tin mới và lưu cập nhật xuống CSDL
        [HttpPost]
        // Bảo mật chống tấn công giả mạo yêu cầu chéo trang (CSRF)
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            // Kiểm tra xem dữ liệu nhập trên Form có thỏa mãn ràng buộc Validation DataAnnotations hay không
            if (ModelState.IsValid)
            {
                // BƯỚC CHECK TRÙNG TÊN: Kiểm tra xem tên danh mục mới có bị trùng với tên của danh mục KHÁC trong CSDL không
                var existingCategory = db.Categories.FirstOrDefault(c => c.CategoryName.ToLower() == category.CategoryName.ToLower() && c.CategoryID != category.CategoryID);
                // Nếu tìm thấy một danh mục khác đã sở hữu tên này
                if (existingCategory != null)
                {
                    // Ghi thông báo lỗi riêng vào thuộc tính CategoryName để hiển thị đỏ dưới ô nhập tên
                    ModelState.AddModelError("CategoryName", "Tên danh mục này đã trùng với một danh mục khác!");
                    // Nạp lại View Edit kèm dữ liệu đang nhập để người dùng chỉnh sửa lại
                    return View(category);
                }

                // Đánh dấu đối tượng category là đã bị thay đổi (Modified) trong Entity Framework
                db.Entry(category).State = EntityState.Modified;
                // Thực thi câu lệnh SQL UPDATE để cập nhật dữ liệu mới xuống CSDL SQL Server
                db.SaveChanges();
                // Chuyển hướng về trang danh sách danh mục (Index) sau khi cập nhật thành công
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, nạp lại trang Form Edit kèm dữ liệu hiện tại
            return View(category);
        }

        // ----------------------------------------------------------------------
        // 5. ACTION DELETE: XÓA DANH MỤC (KIỂM TRA RÀNG BUỘC KHÓA NGOẠI - BAREM DÒNG 10)
        // ----------------------------------------------------------------------
        // Action GET hiển thị trang xác nhận xóa danh mục
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            // Tìm danh mục và nạp danh sách sản phẩm liên quan để hiển thị cảnh báo
            var category = db.Categories.Include(c => c.Products).FirstOrDefault(c => c.CategoryID == id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // Action POST thực thi thao tác xóa danh mục
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Tìm đối tượng danh mục cần xóa trong CSDL
            var category = db.Categories.Find(id);
            if (category != null)
            {
                // BAREM CHECK CONSTRAINT: Trước khi xóa, đếm xem bảng Product có món ăn nào đang dùng CategoryID này không
                int checkProductsInUse = db.Products.Count(p => p.CategoryID == id);

                // Nếu > 0 nghĩa là danh mục đang chứa món ăn (bị ràng buộc khóa ngoại)
                if (checkProductsInUse > 0)
                {
                    // Ghi thông báo lỗi vào ViewBag để hiển thị chữ đỏ trên giao diện xác nhận xóa
                    ViewBag.Error = "Không thể xóa danh mục này vì đang chứa " + checkProductsInUse + " món ăn. Vui lòng xóa các món ăn trước!";
                    category = db.Categories.Include(c => c.Products).FirstOrDefault(c => c.CategoryID == id);
                    return View("Delete", category);
                }

                // Nếu an toàn (checkProductsInUse == 0), tiến hành xóa
                db.Categories.Remove(category);
                // Thực thi câu lệnh SQL DELETE
                db.SaveChanges();
            }

            // Chuyển hướng về trang danh sách danh mục
            return RedirectToAction("Index");
        }

        // ----------------------------------------------------------------------
        // 6. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè hàm Dispose để tự động đóng kết nối CSDL khi Controller kết thúc luồng
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Ngắt kết nối CSDL SQL Server để giải phóng tài nguyên hệ thống
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}