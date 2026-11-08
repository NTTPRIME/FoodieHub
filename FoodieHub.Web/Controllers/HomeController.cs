// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện làm việc với danh sách đối tượng (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework hỗ trợ kỹ thuật nạp dữ liệu Eager Loading (.Include)
using System.Data.Entity;
// Khai báo thư viện truy vấn dữ liệu LINQ (Where, OrderBy, ToList...)
using System.Linq;
// Khai báo thư viện xử lý các yêu cầu Web HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC (Controller, ActionResult, ViewBag...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (Product, Category, FoodieHubDbContext)
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller phân hệ Khách hàng của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Lớp HomeController quản lý luồng hiển thị cho trang chủ, trang giới thiệu và liên hệ
    public class HomeController : Controller
    {
        // Khởi tạo đối tượng db từ FoodieHubDbContext để giao tiếp dữ liệu với SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: TRANG CHỦ HỆ THỐNG - MẶT TIỀN WEBSITE (Barem Dòng 26, 27, 28, 29, 30)
        // ----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Index()
        {
            // BAREM DÒNG 26: Lấy danh sách tất cả các danh mục món ăn từ CSDL để nạp vào thanh Menu/Filter trên trang chủ
            ViewBag.Categories = db.Categories.ToList();

            // BAREM DÒNG 27 & 30: Truy vấn lấy danh sách sản phẩm còn hàng (Stock > 0)
            // Kỹ thuật Eager Loading (.Include) giúp nạp sẵn bảng Category tránh lỗi N+1 Query
            var products = db.Products.Include(p => p.Category)
                                      .Where(p => p.Stock > 0)
                                      .OrderByDescending(p => p.ProductID)
                                      .ToList();

            // Gán tiêu đề trang chủ hiển thị lên giao diện
            ViewBag.CategoryName = "Món Ngon Nổi Bật Hôm Nay";

            // Trả về View (Index.cshtml) kèm danh sách món ăn hợp lệ sang giao diện trang chủ
            return View(products);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION ABOUT: TRANG GIỚI THIỆU THƯƠNG HIỆU FOODIEHUB
        // ----------------------------------------------------------------------
        [HttpGet]
        public ActionResult About()
        {
            // Đưa thông điệp sứ mệnh thương hiệu sang View
            ViewBag.Message = "FoodieHub - Nền tảng đặt món ngon & giao hàng siêu tốc hàng đầu.";

            // Trả về giao diện Views/Home/About.cshtml
            return View();
        }

        // ----------------------------------------------------------------------
        // 3. ACTION CONTACT: TRANG LIÊN HỆ & TƯ VẤN KHÁCH HÀNG
        // ----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Contact()
        {
            // Đưa thông điệp hỗ trợ khách hàng sang View
            ViewBag.Message = "Tổng đài hỗ trợ và trung tâm chăm sóc khách hàng FoodieHub 24/7.";

            // Trả về giao diện Views/Home/Contact.cshtml
            return View();
        }

        // ----------------------------------------------------------------------
        // 4. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè phương thức Dispose để tự động đóng kết nối SQL Server khi Controller kết thúc vòng đời
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Ngắt kết nối CSDL và giải phóng bộ nhớ RAM
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}