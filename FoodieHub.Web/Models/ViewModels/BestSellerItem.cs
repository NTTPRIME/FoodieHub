// Khai báo thư viện chứa các kiểu dữ liệu cơ bản của hệ thống .NET
using System;

// Định nghĩa không gian tên nằm trong thư mục Models/ViewModels của dự án FoodieHub.Web
namespace FoodieHub.Web.Models.ViewModels
// Mở khối định nghĩa không gian tên
{
    // Khai báo lớp ViewModel BestSellerItem dùng để đóng gói dữ liệu hiển thị cho mỗi món bán chạy nhất
    public class BestSellerItem
    // Mở khối định nghĩa lớp BestSellerItem
    {
        // Thuộc tính lưu trữ mã định danh duy nhất (khóa chính) của món ăn
        public int ProductID { get; set; }

        // Thuộc tính lưu trữ tên hiển thị của món ăn
        public string ProductName { get; set; }

        // Thuộc tính lưu trữ đơn giá bán hiện tại của món ăn (đơn vị VNĐ)
        public decimal Price { get; set; }

        // Thuộc tính lưu trữ số lượng tồn kho còn lại của món ăn
        public int Stock { get; set; }

        // Thuộc tính lưu trữ đường dẫn hình ảnh đại diện của món ăn
        public string ImageUrl { get; set; }

        // Thuộc tính lưu trữ tên danh mục mà món ăn trực thuộc
        public string CategoryName { get; set; }

        // Thuộc tính lưu trữ tổng số phần đã bán được cộng dồn từ các đơn hàng hợp lệ
        public int TotalSold { get; set; }

        // Thuộc tính lưu trữ thứ hạng bán chạy thực tế của món ăn (1, 2 hoặc 3) sau khi xét đồng hạng
        public int Rank { get; set; }

        // Thuộc tính cờ đánh dấu món ăn có đang đồng hạng với món khác hay không (true: đồng hạng, false: hạng duy nhất)
        public bool IsTied { get; set; }
        // Đóng khối định nghĩa lớp BestSellerItem
    }
    // Đóng khối định nghĩa không gian tên
}