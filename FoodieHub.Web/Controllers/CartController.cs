// Khai báo thư viện hệ thống C# cơ bản
using System;
// Khai báo thư viện làm việc với các kiểu danh sách (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện LINQ hỗ trợ truy vấn dữ liệu (FirstOrDefault, Sum)
using System.Linq;
// Khai báo thư viện ASP.NET MVC hỗ trợ Controller, ActionResult, TempData
using System.Web.Mvc;
// Nhúng không gian tên chứa các Model CSDL của dự án FoodieHub
using FoodieHub.Web.Models;
// Nhúng không gian tên chứa ViewModel CartItem dùng cho giỏ hàng
using FoodieHub.Web.Models.ViewModels;

// Định nghĩa không gian tên chứa các Controller thuộc dự án FoodieHub.Web
namespace FoodieHub.Web.Controllers
{
    // Khai báo lớp CartController kế thừa từ Controller để quản lý toàn bộ chức năng Giỏ hàng
    public class CartController : Controller
    {
        // Khởi tạo đối tượng DbContext để làm việc và truy vấn CSDL SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. HÀM TRỢ GIÚP LẤY GIỎ HÀNG TỪ SESSION
        // ----------------------------------------------------------------------
        // Phương thức trợ giúp lấy danh sách giỏ hàng lưu trong Session của người dùng
        public List<CartItem> GetCart()
        {
            // Ép kiểu dữ liệu lưu trong Session["Cart"] về danh sách các đối tượng CartItem
            var cart = Session["Cart"] as List<CartItem>;

            // Kiểm tra nếu giỏ hàng trong Session chưa từng tồn tại (null)
            if (cart == null)
            {
                // Khởi tạo mới một danh sách CartItem rỗng
                cart = new List<CartItem>();
                // Gán danh sách rỗng vừa tạo vào lại trong Session["Cart"]
                Session["Cart"] = cart;
            }
            // Trả về danh sách giỏ hàng hiện tại cho phương thức gọi
            return cart;
        }

        // ----------------------------------------------------------------------
        // 2. ACTION INDEX: HIỂN THỊ TRANG GIỎ HÀNG
        // ----------------------------------------------------------------------
        // Chỉ định Action tiếp nhận phương thức HTTP GET
        [HttpGet]
        // Phương thức hiển thị giao diện chính của trang giỏ hàng
        public ActionResult Index()
        {
            // Trả về View giao diện kèm dữ liệu danh sách sản phẩm lấy từ Session giỏ hàng
            return View(GetCart());
        }

        // ----------------------------------------------------------------------
        // 3. ACTION ADDTOCART: THÊM MÓN ĂN VÀO GIỎ HÀNG (KIỂM TRA TỒN KHO)
        // ----------------------------------------------------------------------
        // Phương thức thêm sản phẩm vào giỏ hàng tiếp nhận mã ID và số lượng (cho phép null, mặc định = 1)
        public ActionResult AddToCart(int id, int? quantity = 1)
        {
            // Xử lý an toàn: Nếu số lượng truyền vào bị null hoặc <= 0 thì gán mặc định là 1
            int realQuantity = (quantity.HasValue && quantity.Value > 0) ? quantity.Value : 1;

            // Truy vấn CSDL để tìm sản phẩm tương ứng với ID được truyền vào
            var productInDb = db.Products.Find(id);

            // Kiểm tra an toàn: Nếu sản phẩm không tìm thấy hoặc số lượng tồn kho <= 0 (Đã hết hàng)
            if (productInDb == null || productInDb.Stock <= 0)
            {
                // Ghi thông báo lỗi sản phẩm hết hàng vào TempData để hiển thị thông báo alert
                TempData["Error"] = "Sản phẩm này hiện đã hết hàng trong kho!";
                // Chuyển hướng người dùng quay trở lại trang danh sách giỏ hàng
                return RedirectToAction("Index");
            }

            // Lấy danh sách giỏ hàng hiện tại từ Session
            var cart = GetCart();

            // Tìm xem sản phẩm đã có sẵn trong danh sách giỏ hàng hay chưa
            var item = cart.FirstOrDefault(p => p.ProductID == id);

            // Nếu món ăn này chưa có trong giỏ hàng
            if (item == null)
            {
                // Thêm mới một CartItem vào danh sách giỏ hàng với thông tin lấy từ CSDL
                cart.Add(new CartItem
                {
                    // Gán mã ID của sản phẩm
                    ProductID = productInDb.ProductID,
                    // Gán tên sản phẩm từ CSDL
                    ProductName = productInDb.ProductName,
                    // Gán đơn giá sản phẩm từ CSDL
                    Price = productInDb.Price,
                    // Gán đường dẫn hình ảnh đại diện của sản phẩm
                    ImageUrl = productInDb.ImageUrl,
                    // Gán số lượng sản phẩm hợp lệ đã được xử lý
                    Quantity = realQuantity,
                    // Gán số lượng tồn kho thực tế từ CSDL để kiểm tra giới hạn trên View
                    Stock = productInDb.Stock
                });
            }
            // Nếu món ăn này đã có sẵn trong giỏ hàng
            else
            {
                // BAREM CHECK TỒN KHO: Kiểm tra nếu tổng số lượng cũ + số lượng mua mới vượt quá tồn kho
                if (item.Quantity + realQuantity > productInDb.Stock)
                {
                    // Ghi thông báo lỗi vượt số lượng tồn kho vào TempData
                    TempData["Error"] = "Bạn không thể mua số lượng vượt quá hàng tồn kho hiện có (" + productInDb.Stock + " món)!";
                    // Chuyển hướng quay về lại trang giỏ hàng
                    return RedirectToAction("Index");
                }

                // Nếu thỏa mãn số lượng tồn kho, cộng thêm số lượng mới vào số lượng hiện tại
                item.Quantity += realQuantity;
            }

            // Chuyển hướng trình duyệt về lại Action Index để hiển thị giỏ hàng đã cập nhật
            return RedirectToAction("Index");
        }

        // ----------------------------------------------------------------------
        // 4. ACTION UPDATEQUANTITY: CẬP NHẬT SỐ LƯỢNG MÓN ĂN (SỬA LỖI NULL THAM SỐ)
        // ----------------------------------------------------------------------
        // Chỉ định Action tiếp nhận phương thức HTTP POST từ Form cập nhật số lượng
        [HttpPost]
        // Đổi int quantity thành int? quantity để tránh lỗi Server 500 ArgumentException khi input rỗng
        public ActionResult UpdateQuantity(int id, int? quantity)
        {
            // Tìm thông tin gốc của sản phẩm trong CSDL theo ID
            var productInDb = db.Products.Find(id);
            // Lấy danh sách sản phẩm trong giỏ hàng từ Session
            var cart = GetCart();
            // Tìm món ăn tương ứng cần cập nhật trong giỏ hàng
            var item = cart.FirstOrDefault(p => p.ProductID == id);

            // Xử lý an toàn: Nếu người dùng xóa trống ô nhập (quantity == null), gán mặc định bằng 1
            int newQuantity = quantity ?? 1;

            // Kiểm tra điều kiện nếu tìm thấy cả món ăn trong giỏ và trong CSDL
            if (item != null && productInDb != null)
            {
                // BAREM CHECK TỒN KHO: Kiểm tra nếu số lượng mới nhập vượt quá tồn kho hiện có trong CSDL
                if (newQuantity > productInDb.Stock)
                {
                    // Lưu thông báo lỗi vượt quá số lượng kho vào TempData
                    TempData["Error"] = "Số lượng vượt quá hàng tồn kho hiện có (" + productInDb.Stock + " món)!";
                    // Chuyển hướng quay về lại trang giỏ hàng để hiển thị Alert thông báo lỗi
                    return RedirectToAction("Index");
                }

                // Nếu người dùng nhập số lượng <= 0 thì tự động xóa sản phẩm khỏi giỏ hàng
                if (newQuantity <= 0)
                {
                    // Xóa món ăn khỏi danh sách giỏ hàng
                    cart.Remove(item);
                }
                // Nếu số lượng hợp lệ và nhỏ hơn hoặc bằng tồn kho
                else
                {
                    // Cập nhật số lượng mới cho món ăn trong giỏ
                    item.Quantity = newQuantity;
                }
            }

            // Chuyển hướng trình duyệt về lại trang giỏ hàng sau khi xử lý xong
            return RedirectToAction("Index");
        }

        // ----------------------------------------------------------------------
        // 5. ACTION REMOVEFROMCART: XÓA 1 MÓN ĂN KHỎI GIỎ HÀNG
        // ----------------------------------------------------------------------
        // Action xóa một sản phẩm cụ thể khỏi giỏ hàng theo ID
        public ActionResult RemoveFromCart(int id)
        {
            // Lấy danh sách sản phẩm hiện có trong giỏ hàng từ Session
            var cart = GetCart();
            // Tìm sản phẩm tương ứng trong giỏ theo ID
            var item = cart.FirstOrDefault(p => p.ProductID == id);

            // Kiểm tra nếu sản phẩm có tồn tại trong giỏ hàng
            if (item != null)
            {
                // Thực hiện xóa sản phẩm đó khỏi danh sách giỏ hàng
                cart.Remove(item);
            }

            // Chuyển hướng về lại trang giỏ hàng sau khi xóa thành công
            return RedirectToAction("Index");
        }

        // ----------------------------------------------------------------------
        // 6. ACTION CLEARCART: XÓA TOÀN BỘ GIỎ HÀNG
        // ----------------------------------------------------------------------
        // Action làm sạch toàn bộ giỏ hàng
        public ActionResult ClearCart()
        {
            // Gán Session["Cart"] về null để dọn sạch toàn bộ dữ liệu giỏ hàng
            Session["Cart"] = null;
            // Chuyển hướng về lại trang giỏ hàng rỗng
            return RedirectToAction("Index");
        }

        // ----------------------------------------------------------------------
        // GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL SQL SERVER
        // ----------------------------------------------------------------------
        // Ghi đè phương thức Dispose để giải phóng bộ nhớ khi Controller kết thúc làm việc
        protected override void Dispose(bool disposing)
        {
            // Kiểm tra nếu biến disposing bằng true
            if (disposing)
            {
                // Đóng kết nối và giải phóng tài nguyên của DbContext
                db.Dispose();
            }
            // Gọi phương thức Dispose của lớp cha Controller
            base.Dispose(disposing);
        }
    }
}