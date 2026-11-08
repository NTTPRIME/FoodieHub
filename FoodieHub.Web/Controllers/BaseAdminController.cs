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

            // Kiểm tra điều kiện: Nếu Session bị rỗng (người dùng chưa đăng nhập hoặc Session đã bị hết hạn/clear)
            if (currentUser == null)
            {
                // Can thiệp vào luồng xử lý bằng cách gán Result: Hủy Action sắp gọi và ép chuyển hướng trình duyệt
                filterContext.Result = new RedirectToRouteResult(
                    // Khởi tạo tập hợp tham số Route chỉ định đích đến là Action "Login" thuộc Controller "Account"
                    new RouteValueDictionary(new { controller = "Account", action = "Login" })
                );
            }

            // Gọi phương thức OnActionExecuting của lớp cha (Controller) để tiếp tục tiến trình chuẩn nếu đã đăng nhập thành công
            base.OnActionExecuting(filterContext);
        }
    }
}