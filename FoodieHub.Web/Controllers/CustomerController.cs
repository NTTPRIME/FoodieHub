// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện chứa tập hợp dữ liệu (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework để sử dụng kỹ thuật Eager Loading (.Include)
using System.Data.Entity;
// Khai báo thư viện LINQ hỗ trợ lọc, sắp xếp dữ liệu (Where, OrderByDescending, FirstOrDefault...)
using System.Linq;
// Khai báo thư viện xử lý Web HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC (Controller, ActionResult, RedirectToAction, ValidateAntiForgeryToken...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (User, Order, OrderDetail, Product, FoodieHubDbContext)
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller phục vụ phân hệ Khách hàng của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Lớp CustomerController quản lý các tính năng cá nhân của Khách hàng (Lịch sử giao dịch, tự hủy đơn hàng)
    public class CustomerController : Controller
    {
        // Khởi tạo đối tượng db từ FoodieHubDbContext để giao tiếp và truy vấn cơ sở dữ liệu SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION TRANSACTIONHISTORY: HIỂN THỊ LỊCH SỬ GIAO DỊCH / ĐƠN HÀNG CỦA KHÁCH HÀNG
        // ----------------------------------------------------------------------
        // Phương thức xử lý yêu cầu GET khi khách hàng bấm vào mục "Lịch sử giao dịch"
        [HttpGet]
        // Khai báo Action TransactionHistory trả về giao diện danh sách đơn hàng cá nhân
        public ActionResult TransactionHistory()
        // Mở thân phương thức TransactionHistory
        {
            // BƯỚC 1: KIỂM TRA PHIÊN ĐĂNG NHẬP (SESSION)
            // Lấy thông tin tài khoản khách hàng từ Session["User"] (được khởi tạo lúc đăng nhập ở AccountController)
            var currentUser = Session["User"] as User;

            // Kiểm tra an toàn: Nếu Session bị rỗng (người dùng chưa đăng nhập hoặc phiên làm việc đã hết hạn)
            if (currentUser == null)
            // Mở khối lệnh xử lý khi chưa đăng nhập
            {
                // Tự động chuyển hướng trình duyệt sang trang Đăng nhập của AccountController để bảo mật
                return RedirectToAction("Login", "Account");
                // Đóng khối lệnh xử lý khi chưa đăng nhập
            }

            // BƯỚC 2: TRUY VẤN LỊCH SỬ ĐƠN HÀNG TỪ CƠ SỞ DỮ LIỆU
            // Truy cập bảng danh sách đơn hàng db.Orders
            var orderHistory = db.Orders
                                 // Nạp kèm bảng Chi tiết đơn hàng và thông tin món ăn (Product) để hiển thị lên Modal
                                 .Include(o => o.OrderDetails.Select(d => d.Product))
                                 // Lọc điều kiện bảo mật: Chỉ lấy các đơn hàng thuộc sở hữu của chính tài khoản đang đăng nhập
                                 .Where(o => o.UserID == currentUser.UserID)
                                 // Sắp xếp thứ tự thời gian giảm dần, đơn hàng mới đặt nhất đưa lên đầu danh sách
                                 .OrderByDescending(o => o.OrderDate)
                                 // Thực thi câu truy vấn và chuyển kết quả thành danh sách List<Order>
                                 .ToList();

            // BƯỚC 3: MÓC DỮ LIỆU TRẢ VỀ VIEW
            // Truyền tập hợp danh sách đơn hàng (orderHistory) sang View TransactionHistory.cshtml để đổ lên bảng HTML
            return View(orderHistory);
            // Đóng thân phương thức TransactionHistory
        }

        // ----------------------------------------------------------------------
        // 2. ACTION CANCELORDER: KHÁCH HÀNG TỰ HỦY ĐƠN HÀNG (CHỈ KHI "CHỜ XÁC NHẬN")
        // ----------------------------------------------------------------------
        // Chỉ định Action chỉ tiếp nhận yêu cầu gửi lên qua phương thức HTTP POST để tránh lỗi hủy nhầm qua URL
        [HttpPost]
        // Kích hoạt bộ lọc xác thực mã chống giả mạo yêu cầu liên trang (CSRF) từ Form trên giao diện
        [ValidateAntiForgeryToken]
        // Khai báo Action CancelOrder tiếp nhận tham số id (Mã đơn hàng cần hủy)
        public ActionResult CancelOrder(int id)
        // Mở thân phương thức CancelOrder
        {
            // BƯỚC 1: KIỂM TRA ĐĂNG NHẬP
            // Đọc thông tin người dùng hiện tại từ Session["User"]
            var currentUser = Session["User"] as User;
            // Nếu người dùng chưa đăng nhập hoặc phiên làm việc đã hết hạn
            if (currentUser == null)
            // Mở khối xử lý hết phiên đăng nhập
            {
                // Lưu thông báo lỗi tạm thời qua TempData để hiển thị sau khi chuyển hướng
                TempData["Error"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!";
                // Chuyển hướng khách hàng về trang Đăng nhập
                return RedirectToAction("Login", "Account");
                // Đóng khối xử lý hết phiên đăng nhập
            }

            // BƯỚC 2: TRUY VẤN ĐƠN HÀNG KÈM CHI TIẾT MÓN ĂN & SẢN PHẨM ĐỂ HOÀN KHO
            // Tìm đơn hàng theo Mã đơn (OrderID) và đối chiếu đúng chủ sở hữu (UserID) để chống hủy chéo đơn người khác
            var order = db.Orders
                          // Nạp kèm danh sách OrderDetails và thực thể Product tương ứng để cộng lại số lượng tồn kho
                          .Include(o => o.OrderDetails.Select(d => d.Product))
                          // Lấy bản ghi đầu tiên khớp đồng thời cả OrderID và UserID, trả về null nếu không tồn tại
                          .FirstOrDefault(o => o.OrderID == id && o.UserID == currentUser.UserID);

            // Kiểm tra nếu không tìm thấy đơn hàng hoặc đơn hàng thuộc tài khoản khác
            if (order == null)
            // Mở khối xử lý không tìm thấy đơn
            {
                // Ghi thông báo lỗi vào TempData
                TempData["Error"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền thao tác đơn hàng này!";
                // Quay lại trang Lịch sử giao dịch
                return RedirectToAction("TransactionHistory");
                // Đóng khối xử lý không tìm thấy đơn
            }

            // BƯỚC 3: KIỂM TRA QUY TẮC NGHIỆP VỤ F&B (CHỈ HỦY KHI "CHỜ XÁC NHẬN")
            // Chuẩn hóa chuỗi trạng thái hiện tại của đơn hàng (cắt khoảng trắng thừa nếu có)
            string currentStatus = order.Status != null ? order.Status.Trim() : "Chờ xác nhận";
            // Nếu đơn hàng đã chuyển sang trạng thái khác (Đang xử lý, Đang giao, Hoàn thành, Đã hủy...)
            if (currentStatus != "Chờ xác nhận")
            // Mở khối chặn hủy đơn
            {
                // Thông báo rõ lý do không thể tự hủy trực tiếp trên hệ thống
                TempData["Error"] = $"Đơn hàng #FH-{order.OrderID} hiện đang ở trạng thái \"{currentStatus}\", bếp đã tiếp nhận hoặc đang giao nên không thể tự hủy trực tiếp. Vui lòng liên hệ Hotline quán nếu cần hỗ trợ gấp!";
                // Chuyển hướng về lại trang Lịch sử giao dịch
                return RedirectToAction("TransactionHistory");
                // Đóng khối chặn hủy đơn
            }

            // BƯỚC 4: HOÀN TRẢ SỐ LƯỢNG TỒN KHO (RESTOCK) CHO TỪNG MÓN TRONG ĐƠN
            // Kiểm tra danh sách chi tiết món ăn của đơn hàng có tồn tại dữ liệu hay không
            if (order.OrderDetails != null)
            // Mở khối duyệt danh sách chi tiết đơn
            {
                // Lặp qua từng dòng món ăn khách đã đặt trong đơn hàng này
                foreach (var detail in order.OrderDetails)
                // Mở thân vòng lặp foreach
                {
                    // Đảm bảo món ăn gốc trong bảng Products vẫn tồn tại (không bị null)
                    if (detail.Product != null)
                    // Mở khối cộng hoàn tồn kho
                    {
                        // Cộng trả lại số lượng khách đã đặt (Quantity) vào số lượng tồn kho hiện tại (Stock)
                        detail.Product.Stock += detail.Quantity;
                        // Đóng khối cộng hoàn tồn kho
                    }
                    // Đóng thân vòng lặp foreach
                }
                // Đóng khối duyệt danh sách chi tiết đơn
            }

            // BƯỚC 5: CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG SANG "ĐÃ HỦY" VÀ LƯU CSDL
            // Chuyển trạng thái đơn hàng thành "Đã hủy"
            order.Status = "Đã hủy";
            // Lưu đồng thời cả trạng thái đơn hàng và số lượng tồn kho mới xuống SQL Server
            db.SaveChanges();

            // Gửi thông báo hủy đơn và hoàn kho thành công sang giao diện
            TempData["Success"] = $"Đã hủy thành công đơn hàng #FH-{order.OrderID} và hoàn lại số lượng món vào hệ thống!";
            // Chuyển hướng người dùng trở lại trang Lịch sử giao dịch để xem trạng thái mới
            return RedirectToAction("TransactionHistory");
            // Đóng thân phương thức CancelOrder
        }

        // ----------------------------------------------------------------------
        // 3. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè phương thức Dispose để đóng kết nối tới cơ sở dữ liệu khi Controller hoàn tất chu kỳ sống
        protected override void Dispose(bool disposing)
        // Mở thân phương thức Dispose
        {
            // Kiểm tra cờ disposing để xác nhận giải phóng các tài nguyên được quản lý
            if (disposing)
            // Mở khối lệnh giải phóng
            {
                // Giải phóng RAM và ngắt kết nối với SQL Server
                db.Dispose();
                // Đóng khối lệnh giải phóng
            }
            // Gọi phương thức Dispose của lớp cha Controller để hoàn tất dọn dẹp
            base.Dispose(disposing);
            // Đóng thân phương thức Dispose
        }
        // Đóng thân lớp CustomerController
    }
    // Đóng không gian tên FoodieHub.Web.Controllers
}