// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện hỗ trợ làm việc với danh sách đối tượng (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework để sử dụng hàm Include nạp dữ liệu liên quan (Orders)
using System.Data.Entity;
// Khai báo thư viện hỗ trợ các câu lệnh truy vấn LINQ (ToList, Find, Where...)
using System.Linq;
// Khai báo thư viện xử lý yêu cầu Web HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC chính (Controller, ActionResult, HttpNotFound...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (User, Order, FoodieHubDbContext)
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller thuộc phân hệ Quản trị của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Khai báo lớp AdminCustomerController kế thừa từ lớp Controller mặc định của ASP.NET MVC
    public class AdminCustomerController : Controller
    {
        // Khởi tạo đối tượng db từ lớp FoodieHubDbContext để kết nối và thao tác dữ liệu với SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH KHÁCH HÀNG (Yêu cầu Barem Dòng 24)
        // ----------------------------------------------------------------------
        // Action xử lý yêu cầu GET mặc định khi quản trị viên truy cập trang Quản lý khách hàng
        [HttpGet]
        public ActionResult Index()
        {
            // Truy vấn lấy toàn bộ danh sách người dùng từ bảng Users trong CSDL
            // Hàm Include(u => u.Orders) giúp nạp kèm danh sách các đơn hàng của từng khách hàng để đếm số lượng
            var customersList = db.Users.Include(u => u.Orders).ToList();

            // Trả về giao diện View (Index.cshtml) và truyền danh sách customersList sang để hiển thị lên bảng
            return View(customersList);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION DETAILS: XEM CHI TIẾT VÀ LỊCH SỬ MUA HÀNG (Yêu cầu Barem Dòng 25)
        // ----------------------------------------------------------------------
        // Action nhận tham số id (Mã khách hàng) từ nút "Xem Đơn Đã Mua" trên giao diện danh sách
        [HttpGet]
        public ActionResult Details(int? id)
        {
            // Kiểm tra bảo mật: Nếu tham số id truyền vào bị rỗng (null) do người dùng gõ trực tiếp URL không đúng
            if (id == null)
            {
                // Lập tức trả về lỗi HttpNotFound (Mã lỗi 404) để dừng luồng xử lý
                return HttpNotFound();
            }

            // Dùng LINQ kết hợp Include để tìm chính xác khách hàng mang UserID = id
            // Nạp kèm danh sách tập hợp Orders (đơn hàng) của khách hàng đó từ CSDL
            User customer = db.Users.Include(u => u.Orders).FirstOrDefault(u => u.UserID == id);

            // Kiểm tra an toàn: Nếu không tìm thấy khách hàng nào khớp với mã id truyền vào
            if (customer == null)
            {
                // Trả về lỗi HttpNotFound (Mã lỗi 404) báo không tìm thấy bản ghi
                return HttpNotFound();
            }

            // Nếu tìm thấy khách hàng hợp lệ, truyền đối tượng customer sang View (Details.cshtml) để hiển thị chi tiết
            return View(customer);
        }

        // ----------------------------------------------------------------------
        // 3. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè hàm Dispose để tự động đóng kết nối cơ sở dữ liệu khi Controller kết thúc vòng đời
        protected override void Dispose(bool disposing)
        {
            // Kiểm tra nếu biến disposing là true (đang thực hiện giải phóng tài nguyên)
            if (disposing)
            {
                // Gọi hàm Dispose của đối tượng db để ngắt kết nối với SQL Server, giải phóng bộ nhớ RAM
                db.Dispose();
            }
            // Gọi tiếp phương thức Dispose của lớp cha (Controller)
            base.Dispose(disposing);
        }
    }
}