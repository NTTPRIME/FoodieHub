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
    // THAY ĐỔI: Kế thừa từ BaseAdminController để bảo mật toàn bộ Action quản lý thông tin khách hàng
    public class AdminCustomerController : BaseAdminController
    {
        // Khởi tạo đối tượng db từ lớp FoodieHubDbContext để kết nối và thao tác dữ liệu với SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH KHÁCH HÀNG (Yêu cầu Barem Dòng 24)
        // ----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Index()
        {
            // Chỉ lấy tài khoản Khách hàng (loại trừ tài khoản Admin thông qua kiểm tra từ khóa admin trong email)
            var customersList = db.Users
                                  .Include(u => u.Orders)
                                  .Where(u => u.Email == null || !u.Email.ToLower().Contains("admin"))
                                  .ToList();

            // Trả về giao diện View (Index.cshtml) và truyền danh sách customersList sang để hiển thị lên bảng
            return View(customersList);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION DETAILS: XEM CHI TIẾT VÀ LỊCH SỬ MUA HÀNG (Yêu cầu Barem Dòng 25)
        // ----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            // Tìm chính xác khách hàng thường mang UserID = id (đảm bảo không phải là tài khoản Admin)
            User customer = db.Users
                              .Include(u => u.Orders)
                              .FirstOrDefault(u => u.UserID == id && (u.Email == null || !u.Email.ToLower().Contains("admin")));

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // ----------------------------------------------------------------------
        // 3. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}