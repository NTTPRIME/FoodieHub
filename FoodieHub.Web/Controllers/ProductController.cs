// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện làm việc với tập hợp dữ liệu (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework để sử dụng kỹ thuật nạp dữ liệu Eager Loading (.Include)
using System.Data.Entity;
// Khai báo thư viện truy vấn LINQ (Where, FirstOrDefault, ToList...)
using System.Linq;
// Khai báo thư viện xử lý Web HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC (Controller, ActionResult, HttpNotFound...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (Product, Category, FoodieHubDbContext)
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller phân hệ Khách hàng của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Lớp ProductController quản lý toàn bộ luồng hiển thị danh sách thực đơn và chi tiết món ăn
    public class ProductController : Controller
    {
        // Khởi tạo đối tượng db từ FoodieHubDbContext để kết nối và thao tác với SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH MÓN ĂN, TÌM KIẾM, LỌC & PHÂN TRANG (Barem Dòng 31-34)
        // ----------------------------------------------------------------------
        // Action xử lý yêu cầu GET tiếp nhận mã danh mục, từ khóa tìm kiếm và số trang hiện tại
        [HttpGet]
        public ActionResult Index(int? categoryId, string searchString, int page = 1)
        {
            // Truy vấn lấy danh sách sản phẩm còn hàng (Stock > 0) từ bảng Products và nạp kèm Category
            var products = db.Products.Include(p => p.Category)
                                      .Where(p => p.Stock > 0)
                                      .AsQueryable();

            // ĐIỀU CHỈNH LỖI SAI: Lọc độc lập theo từ khóa tìm kiếm (nếu người dùng có nhập từ khóa)
            if (!string.IsNullOrEmpty(searchString))
            {
                // Lọc sản phẩm chứa từ khóa tìm kiếm trong tên món ăn
                products = products.Where(p => p.ProductName.ToLower().Contains(searchString.ToLower()));
            }

            // ĐIỀU CHỈNH LỖI SAI: Lọc độc lập theo mã danh mục (nếu người dùng có chọn danh mục)
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                // Lọc các sản phẩm thuộc mã danh mục tương ứng
                products = products.Where(p => p.CategoryID == categoryId.Value);
            }

            // Xây dựng thông báo tiêu đề danh mục / kết quả tìm kiếm hiển thị lên View
            if (!string.IsNullOrEmpty(searchString) && categoryId.HasValue && categoryId.Value > 0)
            {
                // Tìm thông tin danh mục trong CSDL để hiển thị tên
                var cat = db.Categories.Find(categoryId.Value);
                // Gán tiêu đề kết hợp cả từ khóa và tên danh mục
                ViewBag.CategoryName = "Kết quả tìm kiếm: \"" + searchString + "\" trong " + (cat != null ? cat.CategoryName : "Danh mục");
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                // Gán tiêu đề theo từ khóa tìm kiếm
                ViewBag.CategoryName = "Kết quả tìm kiếm: \"" + searchString + "\"";
            }
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                // Tìm đối tượng danh mục theo ID
                var category = db.Categories.Find(categoryId.Value);
                // Gán tiêu đề theo tên danh mục
                ViewBag.CategoryName = category != null ? category.CategoryName : "Thực Đơn Món Ăn";
            }
            else
            {
                // Mặc định hiển thị tiêu đề Thực Đơn Món Ăn
                ViewBag.CategoryName = "Thực Đơn Món Ăn";
            }

            // Truyền danh sách tất cả các danh mục sang ViewBag để nạp vào thẻ DropDownList trên View
            ViewBag.Categories = db.Categories.ToList();

            // BỔ SUNG CHỨC NĂNG PHÂN TRANG (Barem Dòng 32)
            // Khai báo số lượng món ăn tối đa hiển thị trên một trang
            int pageSize = 8;
            // Tính tổng số lượng bản ghi sau khi đã qua bộ lọc
            int totalItems = products.Count();
            // Tính tổng số trang (làm tròn lên)
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            // Đảm bảo chỉ số trang không nhỏ hơn 1
            page = page < 1 ? 1 : page;
            // Đảm bảo chỉ số trang không vượt quá tổng số trang nếu tổng số trang > 0
            page = (totalPages > 0 && page > totalPages) ? totalPages : page;

            // Thực thi phân trang LINQ bằng Skip và Take
            var pagedProducts = products.OrderBy(p => p.ProductID)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

            // Đưa các giá trị trạng thái phân trang và bộ lọc vào ViewBag để duy trì trên giao diện
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;
            ViewBag.SelectedCategoryID = categoryId;

            // Trả về View (Index.cshtml) kèm danh sách món ăn đã được phân trang
            return View(pagedProducts);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION DETAIL: XEM CHI TIẾT MÓN ĂN (Barem Dòng 37-40)
        // ----------------------------------------------------------------------
        // Action nhận tham số id (Mã món ăn) từ giao diện
        [HttpGet]
        public ActionResult Detail(int? id)
        {
            // Kiểm tra bảo mật: Nếu id bị rỗng (null) do gõ trực tiếp đường dẫn URL
            if (id == null)
            {
                // Trả về lỗi HttpNotFound (Mã 404) để dừng luồng xử lý
                return HttpNotFound();
            }

            // Truy vấn lấy chi tiết món ăn theo ProductID và nạp kèm thông tin Danh mục liên kết
            Product product = db.Products.Include(p => p.Category)
                                         .FirstOrDefault(p => p.ProductID == id);

            // Kiểm tra an toàn: Nếu không tìm thấy món ăn nào khớp với ID trong CSDL
            if (product == null)
            {
                // Trả về lỗi HttpNotFound (Mã 404) báo không tìm thấy sản phẩm
                return HttpNotFound();
            }

            // Trả về View (Detail.cshtml) và truyền đối tượng product sang giao diện
            return View(product);
        }

        // ----------------------------------------------------------------------
        // 3. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè hàm Dispose để ngắt kết nối CSDL khi Controller bị hủy
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Giải phóng RAM và đóng kết nối tới SQL Server
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}