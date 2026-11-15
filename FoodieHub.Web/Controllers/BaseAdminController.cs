// Khai báo không gian tên cơ bản của hệ thống .NET
using System;
// Khai báo các thành phần cốt lõi của ASP.NET MVC (Controller, ActionExecutingContext...)
using System.Web.Mvc;
// Khai báo thư viện quản lý bản đồ định tuyến URL (RouteValueDictionary)
using System.Web.Routing;
// Nhúng namespace chứa lớp User (thực thể tài khoản) của dự án FoodieHub.Web
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller thuộc dự án FoodieHub.Web
namespace FoodieHub.Web.Controllers
{
    // Khai báo lớp BaseAdminController kế thừa từ Controller mặc định, đóng vai trò là lớp cha cho các Admin Controller
    public class BaseAdminController : Controller
    {
        // Ghi đè (override) sự kiện OnActionExecuting - tự động chạy TRƯỚC KHI bất kỳ Action nào trong Controller con thực thi
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy dữ liệu phiên làm việc từ Session["User"] và ép kiểu an toàn (as) về đối tượng User
            var currentUser = Session["User"] as User;

            // Kiểm tra điều kiện 1: Nếu Session bị rỗng (người dùng chưa đăng nhập hoặc Session đã bị hết hạn/clear)
            if (currentUser == null)
            {
                // Lưu thông báo lỗi tạm thời vào TempData["Error"] để hiển thị cảnh báo sau khi chuyển hướng sang trang đăng nhập
                TempData["Error"] = "Vui lòng đăng nhập bằng tài khoản Quản trị viên để tiếp tục!";

                // Can thiệp vào luồng xử lý bằng cách gán Result: Hủy Action sắp gọi và ép chuyển hướng trình duyệt
                filterContext.Result = new RedirectToRouteResult(
                    // Khởi tạo tập hợp tham số Route chỉ định đích đến là Action "Login" thuộc Controller "Account"
                    new RouteValueDictionary(new { controller = "Account", action = "Login" })
                );

                // Dừng ngay lập tức quá trình thực thi, không cho phép chạy tiếp các lệnh phía dưới
                return;
            }

            // Lấy chuỗi vai trò (Role) của người dùng hiện tại được lưu trong Session["UserRole"]
            var userRole = Session["UserRole"] as string;

            // Kiểm tra quyền Admin: Hợp lệ nếu Session["UserRole"] bằng "Admin" HOẶC Email khớp với tài khoản quản trị hệ thống (không phân biệt hoa thường)
            bool isAdmin = (userRole == "Admin") || (currentUser.Email != null && currentUser.Email.Equals("admin@foodiehub.com", StringComparison.OrdinalIgnoreCase));

            // Kiểm tra điều kiện 2: Nếu tài khoản đã đăng nhập nhưng không có quyền Quản trị viên (chống leo thang đặc quyền)
            if (!isAdmin)
            {
                // Lưu thông báo từ chối truy cập vào TempData["Error"] để cảnh báo người dùng khi bị đẩy về trang chủ
                TempData["Error"] = "Bạn không có quyền truy cập vào khu vực quản trị!";

                // Can thiệp luồng xử lý: Hủy Action quản trị sắp gọi và điều hướng trình duyệt về trang chủ
                filterContext.Result = new RedirectToRouteResult(
                    // Khởi tạo tập hợp tham số Route chỉ định đích đến là Action "Index" thuộc Controller "Home"
                    new RouteValueDictionary(new { controller = "Home", action = "Index" })
                );

                // Dừng ngay lập tức quá trình thực thi để bảo vệ tài nguyên trang quản trị
                return;
            }

            // Gọi phương thức OnActionExecuting của lớp cha (Controller) để tiếp tục tiến trình chuẩn nếu đã thỏa mãn toàn bộ quyền Admin
            base.OnActionExecuting(filterContext);
        }
    }
}