// Khai báo thư viện hệ thống cơ bản của .NET Framework
using System;
// Khai báo thư viện làm việc với danh sách đối tượng (List, IEnumerable)
using System.Collections.Generic;
// Khai báo thư viện Entity Framework để nạp các đối tượng liên quan (Include)
using System.Data.Entity;
// Khai báo thư viện truy vấn LINQ (ToList, Find, FirstOrDefault, OrderByDescending...)
using System.Linq;
// Khai báo thư viện xử lý Web HTTP của ASP.NET
using System.Web;
// Khai báo thư viện ASP.NET MVC (Controller, ActionResult, HttpNotFound, ValidateAntiForgeryToken...)
using System.Web.Mvc;
// Nhúng namespace chứa các lớp Mô hình CSDL (Order, OrderDetail, Product, FoodieHubDbContext)
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller thuộc phân hệ Quản trị của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Lớp AdminOrderController kế thừa BaseAdminController để tự động kiểm tra quyền Admin trước khi thực thi
    public class AdminOrderController : BaseAdminController
    // Mở thân lớp AdminOrderController
    {
        // Khởi tạo đối tượng db từ FoodieHubDbContext để giao tiếp và truy vấn SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH ĐƠN HÀNG
        // ----------------------------------------------------------------------
        // Chỉ định Action Index tiếp nhận yêu cầu truy cập trang quản lý đơn hàng qua phương thức HTTP GET
        [HttpGet]
        // Khai báo phương thức Index trả về giao diện danh sách toàn bộ đơn hàng của hệ thống
        public ActionResult Index()
        // Mở thân phương thức Index
        {
            // Truy vấn bảng Orders từ CSDL
            var ordersList = db.Orders
                               // Nạp kèm thông tin tài khoản người đặt hàng (User) để hiển thị họ tên, số điện thoại
                               .Include(o => o.User)
                               // Sắp xếp danh sách theo thời gian đặt giảm dần (đơn mới nhất hiển thị trên cùng)
                               .OrderByDescending(o => o.OrderDate)
                               // Thực thi truy vấn và chuyển kết quả thành danh sách List<Order>
                               .ToList();

            // Trả về giao diện Views/AdminOrder/Index.cshtml kèm danh sách đơn hàng
            return View(ordersList);
            // Đóng thân phương thức Index
        }

        // ----------------------------------------------------------------------
        // 2. ACTION DETAILS: XEM CHI TIẾT ĐƠN HÀNG VÀ HÓA ĐƠN
        // ----------------------------------------------------------------------
        // Chỉ định Action Details tiếp nhận yêu cầu xem chi tiết qua phương thức HTTP GET
        [HttpGet]
        // Khai báo phương thức Details nhận tham số id kiểu nullable (int?) để tránh lỗi nếu thiếu tham số trên URL
        public ActionResult Details(int? id)
        // Mở thân phương thức Details
        {
            // Kiểm tra nếu không truyền mã đơn hàng (id bị null)
            if (id == null)
            // Mở khối xử lý thiếu tham số
            {
                // Trả về mã lỗi HTTP 404 Not Found
                return HttpNotFound();
                // Đóng khối xử lý thiếu tham số
            }

            // Truy vấn đơn hàng theo mã id được truyền vào
            Order order = db.Orders
                            // Nạp kèm thông tin khách hàng đặt đơn
                            .Include(o => o.User)
                            // Nạp kèm danh sách chi tiết món ăn (OrderDetails) và thông tin từng món (Product)
                            .Include(o => o.OrderDetails.Select(od => od.Product))
                            // Lấy bản ghi đầu tiên khớp OrderID, trả về null nếu không tồn tại
                            .FirstOrDefault(o => o.OrderID == id);

            // Kiểm tra nếu không tìm thấy đơn hàng tương ứng trong CSDL
            if (order == null)
            // Mở khối xử lý không tìm thấy đơn
            {
                // Trả về mã lỗi HTTP 404 Not Found
                return HttpNotFound();
                // Đóng khối xử lý không tìm thấy đơn
            }

            // Trả về giao diện Views/AdminOrder/Details.cshtml kèm đối tượng hóa đơn chi tiết
            return View(order);
            // Đóng thân phương thức Details
        }

        // ----------------------------------------------------------------------
        // 3. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè phương thức Dispose để tự động đóng kết nối SQL Server khi Controller kết thúc vòng đời
        protected override void Dispose(bool disposing)
        // Mở thân phương thức Dispose
        {
            // Kiểm tra cờ disposing để xác nhận giải phóng tài nguyên được quản lý
            if (disposing)
            // Mở khối giải phóng
            {
                // Ngắt kết nối CSDL và giải phóng bộ nhớ RAM của DbContext
                db.Dispose();
                // Đóng khối giải phóng
            }
            // Gọi phương thức Dispose của lớp cha để hoàn tất dọn dẹp
            base.Dispose(disposing);
            // Đóng thân phương thức Dispose
        }

        // ----------------------------------------------------------------------
        // 4. ACTION UPDATESTATUS: CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG & TỰ ĐỘNG HOÀN KHO KHI HỦY
        // ----------------------------------------------------------------------
        // Chỉ định Action chỉ nhận dữ liệu gửi lên qua phương thức HTTP POST
        [HttpPost]
        // Kích hoạt kiểm tra mã chống giả mạo yêu cầu liên trang (CSRF)
        [ValidateAntiForgeryToken]
        // Khai báo phương thức UpdateStatus tiếp nhận mã đơn hàng (id) và trạng thái mới (status)
        public ActionResult UpdateStatus(int id, string status)
        // Mở thân phương thức UpdateStatus
        {
            // Kiểm tra chặt đầu vào: Chặn chuỗi null, rỗng hoặc chỉ chứa khoảng trắng
            if (string.IsNullOrWhiteSpace(status))
            // Mở khối báo lỗi đầu vào
            {
                // Lưu thông báo lỗi vào TempData để hiển thị trên trang danh sách
                TempData["Error"] = "Trạng thái đơn hàng không được để trống!";
                // Chuyển hướng về trang danh sách đơn hàng
                return RedirectToAction("Index");
                // Đóng khối báo lỗi đầu vào
            }

            // Truy vấn đơn hàng và nạp kèm OrderDetails cùng Product để phục vụ thao tác hoàn tồn kho khi hủy đơn
            var order = db.Orders
                          // Nạp kèm danh sách món ăn trong đơn và bản ghi sản phẩm tương ứng trong kho
                          .Include(o => o.OrderDetails.Select(od => od.Product))
                          // Tìm chính xác đơn hàng có OrderID trùng với tham số id
                          .FirstOrDefault(o => o.OrderID == id);

            // Kiểm tra nếu đơn hàng không tồn tại trong CSDL
            if (order == null)
            // Mở khối xử lý không tìm thấy đơn
            {
                // Trả về mã lỗi HTTP 404 Not Found
                return HttpNotFound();
                // Đóng khối xử lý không tìm thấy đơn
            }

            // Chuẩn hóa chuỗi trạng thái cũ của đơn hàng (cắt khoảng trắng thừa nếu có)
            string oldStatus = order.Status != null ? order.Status.Trim() : "";
            // Chuẩn hóa chuỗi trạng thái mới Admin vừa chọn
            string newStatus = status.Trim();

            // Chặn trường hợp đơn đã hủy trước đó nhưng Admin lại đổi ngược sang trạng thái khác (tránh lệch kho)
            if (oldStatus == "Đã hủy" && newStatus != "Đã hủy")
            // Mở khối chặn khôi phục đơn đã hủy
            {
                // Lưu cảnh báo nghiệp vụ vào TempData
                TempData["Error"] = $"Đơn hàng #FH-{order.OrderID} đã bị hủy và hoàn kho trước đó, không thể khôi phục trạng thái!";
                // Quay lại trang danh sách đơn hàng mà không lưu thay đổi
                return RedirectToAction("Index");
                // Đóng khối chặn khôi phục đơn đã hủy
            }

            // Kiểm tra nếu đơn hàng đang hoạt động (chưa hủy) và Admin chuyển sang trạng thái "Đã hủy"
            if (oldStatus != "Đã hủy" && newStatus == "Đã hủy")
            // Mở khối xử lý hoàn trả tồn kho
            {
                // Đảm bảo danh sách chi tiết đơn hàng có dữ liệu
                if (order.OrderDetails != null)
                // Mở khối duyệt danh sách món trong đơn
                {
                    // Lặp qua từng dòng chi tiết món ăn thuộc đơn hàng bị hủy
                    foreach (var detail in order.OrderDetails)
                    // Mở thân vòng lặp foreach
                    {
                        // Kiểm tra món ăn gốc trong bảng Products còn tồn tại (không bị null)
                        if (detail.Product != null)
                        // Mở khối cộng hoàn tồn kho
                        {
                            // Cộng trả lại đúng số lượng khách đã đặt (Quantity) vào tồn kho hiện tại (Stock)
                            detail.Product.Stock += detail.Quantity;
                            // Đóng khối cộng hoàn tồn kho
                        }
                        // Đóng thân vòng lặp foreach
                    }
                    // Đóng khối duyệt danh sách món trong đơn
                }
                // Đóng khối xử lý hoàn trả tồn kho
            }

            // Cập nhật trạng thái mới cho đối tượng đơn hàng
            order.Status = newStatus;
            // Lưu đồng thời trạng thái đơn hàng và số lượng tồn kho đã hoàn trả xuống SQL Server
            db.SaveChanges();

            // Gán thông báo thành công tương ứng (nếu hủy đơn thì báo rõ đã tự động hoàn kho)
            TempData["SuccessMessage"] = newStatus == "Đã hủy"
                ? $"Đã hủy đơn hàng #FH-{order.OrderID} và tự động hoàn tồn kho thành công!"
                : "Cập nhật trạng thái đơn hàng thành công!";

            // Chuyển hướng về lại trang quản lý danh sách đơn hàng
            return RedirectToAction("Index");
            // Đóng thân phương thức UpdateStatus
        }
        // Đóng thân lớp AdminOrderController
    }
    // Đóng không gian tên FoodieHub.Web.Controllers
}