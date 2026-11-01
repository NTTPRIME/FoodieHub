// Khai báo thư viện chứa các tập hợp Generic như IEnumerable, List
using System.Collections.Generic;
// Nhúng namespace chứa các lớp Entity (Product, Category) của dự án FoodieHub
using FoodieHub.Web.Models;

// Định nghĩa không gian tên nằm trong thư mục Models/ViewModels của dự án
namespace FoodieHub.Web.Models.ViewModels
{
    // Lớp ProductListViewModel gom nhóm dữ liệu hiển thị cho trang danh sách món ăn kết hợp bộ lọc
    public class ProductListViewModel
    {
        // Danh sách các món ăn đã được lọc theo yêu cầu để gửi ra giao diện View
        public IEnumerable<Product> Products { get; set; }

        // Danh sách toàn bộ danh mục món ăn dùng để nạp vào thanh menu hoặc dropdown lọc
        public IEnumerable<Category> Categories { get; set; }

        // Mã danh mục đang được người dùng chọn trên giao diện (null nếu xem tất cả)
        public int? SelectedCategoryID { get; set; }

        // Từ khóa tìm kiếm do người dùng nhập vào ô tìm kiếm tên món ăn
        public string SearchString { get; set; }

        // Chỉ số trang hiện tại phục vụ phân trang danh sách
        public int CurrentPage { get; set; }

        // Tổng số trang được tính toán dựa trên tổng sản phẩm và số lượng hiển thị/trang
        public int TotalPages { get; set; }
    }
}