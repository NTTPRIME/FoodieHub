// Khai báo namespace chứa lớp cấu hình Migrations và dữ liệu mẫu Seed()
using FoodieHub.Web.Migrations;
// Khai báo namespace chứa lớp DbContext và các Entity Model của ứng dụng
using FoodieHub.Web.Models;
// Khai báo namespace chứa các kiểu dữ liệu cơ bản và ngoại lệ hệ thống .NET
using System;
// Khai báo namespace hỗ trợ các tập hợp dữ liệu Generic (List, Dictionary, IEnumerable)
using System.Collections.Generic;
// Khai báo namespace Entity Framework để làm việc với các cơ chế khởi tạo và cập nhật CSDL
using System.Data.Entity;
// Khai báo namespace hỗ trợ xử lý và truy vấn dữ liệu theo cú pháp LINQ
using System.Linq;
// Khai báo namespace xử lý các đối tượng ngữ cảnh Web (HttpRequest, HttpResponse, Server)
using System.Web;
// Khai báo namespace chứa các thành phần cốt lõi của kiến trúc ASP.NET MVC (Controller, View, Action)
using System.Web.Mvc;
// Khai báo namespace quản lý tối ưu hóa tài nguyên tĩnh (nén và nạp gói CSS/JS)
using System.Web.Optimization;
// Khai báo namespace quản lý hệ thống định tuyến đường dẫn URL (Routing System)
using System.Web.Routing;

// Không gian tên chính chứa cấu hình khởi chạy của ứng dụng web FoodieHub
namespace FoodieHub.Web
{
    // Lớp MvcApplication kế thừa HttpApplication để quản lý vòng đời và các sự kiện toàn cục của ứng dụng
    public class MvcApplication : System.Web.HttpApplication
    {
        // Phương thức tự động kích hoạt một lần duy nhất khi máy chủ ứng dụng web bắt đầu khởi động
        protected void Application_Start()
        {
            // Tự động tìm kiếm và đăng ký tất cả các khu vực chức năng (Areas) trong dự án MVC
            AreaRegistration.RegisterAllAreas();
            // Đăng ký các bộ lọc toàn cục (Global Filters) xử lý các vấn đề như ngoại lệ, xác thực
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            // Đăng ký sơ đồ đường dẫn URL (Route Table) để ánh xạ request tới đúng Controller/Action
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            // Đăng ký các gói tối ưu hóa tập tin giao diện CSS và JavaScript (Bundles)
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Tạm thời vô hiệu hóa cơ chế tự động khởi tạo CSDL để chủ động kiểm tra trạng thái thực tế trên SQL Server
            Database.SetInitializer<FoodieHubDbContext>(null);

            // Khởi tạo đối tượng ngữ cảnh CSDL tạm thời và tự động giải phóng tài nguyên sau khi kiểm tra xong
            using (var db = new FoodieHubDbContext())
            {
                // Khai báo biến cờ mặc định cho phép kích hoạt cơ chế tự động cập nhật bằng EF Migrations
                bool useMigration = true;

                // Kiểm tra xem cơ sở dữ liệu FoodieHubDB đã được tạo sẵn trên máy chủ SQL Server hay chưa
                if (db.Database.Exists())
                {
                    // Truy vấn hệ thống đếm số lượng bảng nghiệp vụ (Categories) đã tồn tại trong CSDL
                    int hasTables = db.Database.SqlQuery<int>(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories'").FirstOrDefault();

                    // Truy vấn hệ thống đếm sự tồn tại của bảng lịch sử phiên bản (__MigrationHistory) của Entity Framework
                    int hasMigrationHistory = db.Database.SqlQuery<int>(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'").FirstOrDefault();

                    // Nếu CSDL đã có bảng dữ liệu (do chạy script .sql độc lập) nhưng chưa có lịch sử Migration
                    if (hasTables > 0 && hasMigrationHistory == 0)
                    {
                        // Tắt cờ Migration để sử dụng trực tiếp cấu trúc bảng và dữ liệu từ file .sql, tránh lỗi xung đột trùng bảng
                        useMigration = false;
                    }
                }

                // Nếu CSDL chưa tồn tại hoặc được khởi tạo chuẩn từ Entity Framework Migrations
                if (useMigration)
                {
                    // Cấu hình Entity Framework tự động cập nhật CSDL lên phiên bản mới nhất và chạy hàm Seed() trong file Configuration.cs
                    Database.SetInitializer(new MigrateDatabaseToLatestVersion<FoodieHubDbContext, Configuration>());
                }
            }
        }
    }
}
