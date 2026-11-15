// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện làm việc với danh sách và tập hợp đối tượng (List, HashSet, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework hỗ trợ kỹ thuật nạp dữ liệu Eager Loading (.Include)
using System.Data.Entity;
// Khai báo thư viện truy vấn dữ liệu LINQ (Where, GroupBy, OrderBy, ToList...)
using System.Linq;
// Khai báo thư viện xử lý các yêu cầu Web HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC (Controller, ActionResult, ViewBag...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (Product, Category, Order, OrderDetail, FoodieHubDbContext)
using FoodieHub.Web.Models;
// Nhúng namespace chứa lớp ViewModel BestSellerItem để đóng gói dữ liệu Top món bán chạy
using FoodieHub.Web.Models.ViewModels;

// Định nghĩa không gian tên chứa các Controller phân hệ Khách hàng của FoodieHub
namespace FoodieHub.Web.Controllers
// Mở không gian tên FoodieHub.Web.Controllers
{
    // Lớp HomeController quản lý luồng hiển thị cho trang chủ, trang giới thiệu và liên hệ
    public class HomeController : Controller
    // Mở thân lớp HomeController
    {
        // Khởi tạo đối tượng db từ FoodieHubDbContext để giao tiếp dữ liệu với SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: TRANG CHỦ HỆ THỐNG - MẶT TIỀN WEBSITE
        // ----------------------------------------------------------------------
        // Chỉ định Action Index tiếp nhận yêu cầu truy cập trang chủ qua phương thức HTTP GET
        [HttpGet]
        // Khai báo phương thức Index trả về giao diện trang chủ kèm dữ liệu thực đơn và món bán chạy
        public ActionResult Index()
        // Mở thân phương thức Index
        {
            // Truy vấn lấy toàn bộ danh sách danh mục món ăn từ CSDL đưa vào ViewBag để hiển thị thanh lọc
            ViewBag.Categories = db.Categories.ToList();

            // Truy vấn danh sách tất cả các món có lượt bán hợp lệ (TotalSold > 0) và sắp xếp theo quy tắc cố định
            var rawSoldProducts = db.OrderDetails
                // Nạp kèm dữ liệu bảng Product và bảng Category liên kết (Eager Loading) để tránh truy vấn lặp N+1
                .Include(od => od.Product.Category)
                // Nạp kèm dữ liệu bảng Order để kiểm tra trạng thái đơn hàng của khách
                .Include(od => od.Order)
                // Lọc điều kiện lấy tất cả đơn hàng hợp lệ (tính cả đơn mới đặt, loại trừ các đơn đã bị hủy)
                .Where(od => od.Order != null
                          // Giữ lại các đơn hàng chưa gán trạng thái (null) hoặc trạng thái khác "Đã hủy" sau khi cắt khoảng trắng thừa
                          && (od.Order.Status == null || od.Order.Status.Trim() != "Đã hủy")
                          // Đảm bảo bản ghi món ăn liên kết không bị null (tránh lỗi tham chiếu dữ liệu)
                          && od.Product != null
                          // Chỉ đề xuất lên trang chủ những món hiện còn hàng trong kho (Stock > 0) để khách đặt được ngay
                          && od.Product.Stock > 0)
                // Gom nhóm các dòng chi tiết đơn hàng theo từng món ăn dựa trên các thuộc tính cố định của món
                .GroupBy(od => new
                // Mở khối khai báo khóa gom nhóm (Anonymous Type)
                {
                    // Gom nhóm theo Mã sản phẩm
                    od.ProductID,
                    // Gom nhóm theo Tên món ăn
                    od.Product.ProductName,
                    // Gom nhóm theo Đơn giá hiện tại của món
                    od.Product.Price,
                    // Gom nhóm theo Số lượng tồn kho hiện có
                    od.Product.Stock,
                    // Gom nhóm theo Đường dẫn hình ảnh món ăn
                    od.Product.ImageUrl,
                    // Gom nhóm theo Tên danh mục của món ăn
                    od.Product.Category.CategoryName
                    // Đóng khối khai báo khóa gom nhóm
                })
                // Chiếu dữ liệu từng nhóm đã gom sang đối tượng ViewModel BestSellerItem
                .Select(g => new BestSellerItem
                // Mở khối gán giá trị cho các thuộc tính của BestSellerItem
                {
                    // Gán Mã sản phẩm từ khóa nhóm
                    ProductID = g.Key.ProductID,
                    // Gán Tên món ăn từ khóa nhóm
                    ProductName = g.Key.ProductName,
                    // Gán Đơn giá món ăn từ khóa nhóm
                    Price = g.Key.Price,
                    // Gán Số lượng tồn kho từ khóa nhóm
                    Stock = g.Key.Stock,
                    // Gán Đường dẫn ảnh từ khóa nhóm
                    ImageUrl = g.Key.ImageUrl,
                    // Gán Tên danh mục, nếu món chưa phân loại (null) thì hiển thị mặc định là "Món ngon"
                    CategoryName = g.Key.CategoryName ?? "Món ngon",
                    // Tính tổng số lượng phần ăn đã bán ra của món này qua tất cả các đơn hàng hợp lệ
                    TotalSold = g.Sum(x => x.Quantity)
                    // Đóng khối gán giá trị thuộc tính
                })
                // Loại bỏ các món có tổng số lượng bán bằng 0 để không tự bịa thêm món chưa có đơn
                .Where(x => x.TotalSold > 0)
                // Sắp xếp ưu tiên 1: Theo tổng số lượng đã bán (TotalSold) giảm dần từ cao xuống thấp
                .OrderByDescending(x => x.TotalSold)
                // Sắp xếp ưu tiên 2 (khi bằng lượt bán): Theo đơn giá (Price) giảm dần để ưu tiên món giá trị cao
                .ThenByDescending(x => x.Price)
                // Sắp xếp ưu tiên 3 (khi bằng cả lượt bán và giá): Theo tên món (ProductName) tăng dần bảng chữ cái để tránh xếp ngẫu nhiên
                .ThenBy(x => x.ProductName)
                // Thực thi câu truy vấn SQL xuống CSDL và chuyển kết quả thành danh sách List<BestSellerItem>
                .ToList();

            // Khởi tạo danh sách rỗng để chứa các món đạt thứ hạng từ TOP #1 đến TOP #3 (bao gồm cả đồng hạng)
            var bestSellers = new List<BestSellerItem>();
            // Khởi tạo biến đếm thứ hạng thực tế hiện tại (bắt đầu từ 0, món đầu tiên sẽ tăng lên 1)
            int currentRank = 0;
            // Khởi tạo biến lưu lượt bán của món liền kề trước đó để so sánh đồng hạng (ban đầu là null)
            int? previousSold = null;

            // Duyệt tuần tự qua từng món trong danh sách đã sắp xếp theo lượt bán giảm dần
            foreach (var item in rawSoldProducts)
            // Mở khối vòng lặp duyệt từng món đã bán
            {
                // Kiểm tra nếu là món đầu tiên (previousSold == null) HOẶC lượt bán của món này thấp hơn món đứng ngay trước nó
                if (previousSold == null || item.TotalSold < previousSold.Value)
                // Mở khối điều kiện tăng bậc thứ hạng
                {
                    // Tăng thứ hạng lên 1 bậc (thuật toán Dense Ranking không nhảy bậc khi có đồng hạng)
                    currentRank++;
                    // Cập nhật mốc lượt bán mới để làm chuẩn so sánh cho các món kế tiếp
                    previousSold = item.TotalSold;
                    // Đóng khối điều kiện tăng bậc thứ hạng
                }

                // Kiểm tra nếu thứ hạng hiện tại đã vượt quá TOP #3 (tức currentRank > 3)
                if (currentRank > 3)
                // Mở khối điều kiện dừng vòng lặp
                {
                    // Thoát khỏi vòng lặp ngay lập tức vì đã lấy trọn vẹn tất cả các món thuộc Top 3 thứ hạng
                    break;
                    // Đóng khối điều kiện dừng vòng lặp
                }

                // Gán thứ hạng thực tế (1, 2 hoặc 3) cho món ăn hiện tại
                item.Rank = currentRank;
                // Thêm món ăn vào danh sách kết quả hiển thị Top 3 bán chạy
                bestSellers.Add(item);
                // Đóng khối vòng lặp duyệt từng món đã bán
            }

            // Lọc ra tập hợp các thứ hạng (Rank) xuất hiện từ 2 món trở lên để xác định các bậc bị đồng hạng
            var tiedRanks = bestSellers
                // Gom nhóm các món trong Top 3 theo thứ hạng Rank
                .GroupBy(x => x.Rank)
                // Chỉ giữ lại những nhóm thứ hạng có số lượng món lớn hơn 1 (tức có đồng hạng)
                .Where(g => g.Count() > 1)
                // Lấy ra con số thứ hạng (1, 2 hoặc 3) của các nhóm đồng hạng đó
                .Select(g => g.Key)
                // Chuyển sang cấu trúc HashSet<int> để tra cứu kiểm tra đồng hạng với tốc độ O(1)
                .ToHashSet();

            // Duyệt qua từng món trong danh sách Top 3 kết quả cuối cùng để gắn cờ nhận biết đồng hạng
            foreach (var item in bestSellers)
            // Mở khối vòng lặp gắn cờ đồng hạng
            {
                // Gán true nếu thứ hạng của món nằm trong tập hợp các hạng đồng hạng, ngược lại gán false
                item.IsTied = tiedRanks.Contains(item.Rank);
                // Đóng khối vòng lặp gắn cờ đồng hạng
            }

            // Truyền danh sách Top 3 món bán chạy (đã tính đủ Rank và IsTied) sang View thông qua ViewBag.BestSellers
            ViewBag.BestSellers = bestSellers;

            // Truy vấn lấy toàn bộ danh sách món ăn còn hàng (Stock > 0) kèm thông tin danh mục cho lưới thực đơn chính
            var products = db.Products.Include(p => p.Category)
                                      // Lọc chỉ lấy các sản phẩm có số lượng tồn kho lớn hơn 0
                                      .Where(p => p.Stock > 0)
                                      // Sắp xếp món mới thêm lên đầu theo ProductID giảm dần
                                      .OrderByDescending(p => p.ProductID)
                                      // Chuyển kết quả truy vấn thành danh sách List<Product>
                                      .ToList();

            // Gán chuỗi tiêu đề cho khu vực danh sách món ăn nổi bật trên trang chủ
            ViewBag.CategoryName = "Món Ngon Nổi Bật Hôm Nay";

            // Trả về giao diện Views/Home/Index.cshtml kèm danh sách món ăn truyền qua Model
            return View(products);
            // Đóng thân phương thức Index
        }

        // ----------------------------------------------------------------------
        // 2. ACTION ABOUT: TRANG GIỚI THIỆU THƯƠNG HIỆU FOODIEHUB
        // ----------------------------------------------------------------------
        // Chỉ định Action About tiếp nhận yêu cầu truy cập qua phương thức HTTP GET
        [HttpGet]
        // Khai báo phương thức About trả về giao diện trang giới thiệu
        public ActionResult About()
        // Mở thân phương thức About
        {
            // Đưa thông điệp sứ mệnh thương hiệu sang View thông qua ViewBag.Message
            ViewBag.Message = "FoodieHub - Nền tảng đặt món ngon & giao hàng siêu tốc hàng đầu.";

            // Trả về giao diện Views/Home/About.cshtml
            return View();
            // Đóng thân phương thức About
        }

        // ----------------------------------------------------------------------
        // 3. ACTION CONTACT: TRANG LIÊN HỆ & TƯ VẤN KHÁCH HÀNG
        // ----------------------------------------------------------------------
        // Chỉ định Action Contact tiếp nhận yêu cầu truy cập qua phương thức HTTP GET
        [HttpGet]
        // Khai báo phương thức Contact trả về giao diện trang liên hệ
        public ActionResult Contact()
        // Mở thân phương thức Contact
        {
            // Đưa thông điệp hỗ trợ khách hàng sang View thông qua ViewBag.Message
            ViewBag.Message = "Tổng đài hỗ trợ và trung tâm chăm sóc khách hàng FoodieHub 24/7.";

            // Trả về giao diện Views/Home/Contact.cshtml
            return View();
            // Đóng thân phương thức Contact
        }

        // ----------------------------------------------------------------------
        // 4. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè phương thức Dispose để tự động đóng kết nối SQL Server khi Controller kết thúc vòng đời
        protected override void Dispose(bool disposing)
        // Mở thân phương thức Dispose
        {
            // Kiểm tra cờ disposing để xác nhận có đang giải phóng tài nguyên được quản lý hay không
            if (disposing)
            // Mở khối lệnh giải phóng
            {
                // Ngắt kết nối CSDL và giải phóng bộ nhớ RAM của DbContext
                db.Dispose();
                // Đóng khối lệnh giải phóng
            }
            // Gọi phương thức Dispose của lớp cha Controller để hoàn tất dọn dẹp
            base.Dispose(disposing);
            // Đóng thân phương thức Dispose
        }
        // Đóng thân lớp HomeController
    }
    // Đóng không gian tên FoodieHub.Web.Controllers
}