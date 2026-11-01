// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện làm việc với danh sách đối tượng (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework để nạp các đối tượng liên quan (Include)
using System.Data.Entity;
// Khai báo thư viện truy vấn LINQ (ToList, Find, FirstOrDefault...)
using System.Linq;
// Khai báo thư viện xử lý Web HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC (Controller, ActionResult, HttpNotFound...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (Order, OrderDetail, FoodieHubDbContext)
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller thuộc phân hệ Quản trị của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Khai báo lớp AdminOrderController kế thừa từ lớp Controller mặc định của ASP.NET MVC
    public class AdminOrderController : Controller
    {
        // Khởi tạo đối tượng db từ lớp FoodieHubDbContext để kết nối và thao tác với SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH ĐƠN HÀNG (Yêu cầu Barem Dòng 22)
        // ----------------------------------------------------------------------
        // Action xử lý yêu cầu GET mặc định khi quản trị viên vào trang Quản lý đơn hàng
        [HttpGet]
        public ActionResult Index()
        {
            // Truy vấn lấy toàn bộ danh sách đơn hàng từ bảng Orders trong CSDL
            // Hàm Include(o => o.User) giúp nạp kèm thông tin tài khoản khách hàng liên kết
            // Hàm OrderByDescending sắp xếp các đơn hàng mới đặt lên trên cùng
            var ordersList = db.Orders
                               .Include(o => o.User)
                               .OrderByDescending(o => o.OrderDate)
                               .ToList();

            // Trả về View (Index.cshtml) và truyền danh sách ordersList sang để hiển thị lên bảng
            return View(ordersList);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION DETAILS: XEM CHI TIẾT ĐƠN HÀNG VÀ HÓA ĐƠN (Yêu cầu Barem Dòng 24, 25)
        // ----------------------------------------------------------------------
        // Action nhận tham số id (Mã đơn hàng) từ nút "Xem Hóa Đơn" trên giao diện danh sách
        [HttpGet]
        public ActionResult Details(int? id)
        {
            // Kiểm tra bảo mật: Nếu id truyền vào bị rỗng (null) do truy cập trực tiếp URL
            if (id == null)
            {
                // Trả về trang lỗi HttpNotFound (Mã 404) để ngắt luồng
                return HttpNotFound();
            }

            // Dùng LINQ kết hợp Include nạp kèm bảng User và danh sách OrderDetails (kèm thông tin món ăn Product)
            Order order = db.Orders
                            .Include(o => o.User)
                            .Include(o => o.OrderDetails.Select(od => od.Product))
                            .FirstOrDefault(o => o.OrderID == id);

            // Kiểm tra an toàn: Nếu không tìm thấy đơn hàng nào khớp với mã id truyền vào
            if (order == null)
            {
                // Trả về lỗi HttpNotFound (Mã 404) báo không tìm thấy bản ghi
                return HttpNotFound();
            }

            // Trả về View (Details.cshtml) và truyền đối tượng order sang để hiển thị thông tin hóa đơn
            return View(order);
        }

        // ----------------------------------------------------------------------
        // 3. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè hàm Dispose để giải phóng đối tượng DbContext khi Controller bị hủy
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Ngắt kết nối CSDL SQL Server để giải phóng RAM cho Server
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}