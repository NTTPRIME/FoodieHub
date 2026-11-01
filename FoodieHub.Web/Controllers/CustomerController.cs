// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện chứa tập hợp dữ liệu (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework để sử dụng kỹ thuật Eager Loading (.Include)
using System.Data.Entity;
// Khai báo thư viện LINQ hỗ trợ lọc, sắp xếp dữ liệu (Where, OrderByDescending...)
using System.Linq;
// Khai báo thư viện xử lý Web HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC (Controller, ActionResult, RedirectToAction...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (User, Order, FoodieHubDbContext)
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller phục vụ phân hệ Khách hàng của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Lớp CustomerController quản lý các tính năng cá nhân của Khách hàng (Lịch sử giao dịch, thông tin tài khoản)
    public class CustomerController : Controller
    {
        // Khởi tạo đối tượng db từ FoodieHubDbContext để giao tiếp và truy vấn cơ sở dữ liệu SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // ACTION TRANSACTIONHISTORY: HIỂN THỊ LỊCH SỬ GIAO DỊCH / ĐƠN HÀNG CỦA KHÁCH HÀNG
        // ----------------------------------------------------------------------
        // Phương thức xử lý yêu cầu GET khi khách hàng bấm vào mục "Lịch sử giao dịch"
        [HttpGet]
        public ActionResult TransactionHistory()
        {
            // BƯỚC 1: KIỂM TRA PHIÊN ĐĂNG NHẬP (SESSION)
            // Lấy thông tin tài khoản khách hàng từ Session["User"] (được khởi tạo lúc đăng nhập ở AccountController)
            var currentUser = Session["User"] as User;

            // Kiểm tra an toàn: Nếu Session bị rỗng (người dùng chưa đăng nhập hoặc phiên làm việc đã hết hạn)
            if (currentUser == null)
            {
                // Tự động chuyển hướng trình duyệt sang trang Đăng nhập của AccountController để bảo mật
                return RedirectToAction("Login", "Account");
            }

            // BƯỚC 2: TRUY VẤN LỊCH SỬ ĐƠN HÀNG TỪ CƠ SỞ DỮ LIỆU
            // - db.Orders: Truy cập bảng danh sách đơn hàng
            // - Include(o => o.OrderDetails.Select(d => d.Product)): Nạp kèm bảng Chi tiết đơn hàng và Tên/Giá món ăn để hiển thị lên Modal
            // - Where(o => o.UserID == currentUser.UserID): Chỉ lấy các đơn hàng do chính tài khoản này thực hiện
            // - OrderByDescending(o => o.OrderDate): Sắp xếp thứ tự thời gian, đơn hàng mới đặt nhất đưa lên đầu
            var orderHistory = db.Orders
                                 .Include(o => o.OrderDetails.Select(d => d.Product))
                                 .Where(o => o.UserID == currentUser.UserID)
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToList();

            // BƯỚC 3: MÓC DỮ LIỆU TRẢ VỀ VIEW
            // Truyền tập hợp danh sách đơn hàng (orderHistory) sang View TransactionHistory.cshtml để đổ lên bảng HTML
            return View(orderHistory);
        }

        // ----------------------------------------------------------------------
        // GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè phương thức Dispose để đóng kết nối tới cơ sở dữ liệu khi Controller hoàn tất chu kỳ sống
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Giải phóng RAM và ngắt kết nối với SQL Server
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}