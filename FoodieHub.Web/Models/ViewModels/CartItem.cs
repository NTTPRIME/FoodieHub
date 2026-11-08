// Khai báo thư viện chứa các kiểu dữ liệu cơ bản của .NET Framework
using System;

// Định nghĩa không gian tên thuộc thư mục Models/ViewModels của dự án FoodieHub
namespace FoodieHub.Web.Models.ViewModels
{
    // Lớp ViewModel CartItem đại diện cho thông tin một món ăn được đưa vào giỏ hàng
    public class CartItem
    {
        // Thuộc tính lưu trữ mã định danh duy nhất (ID) của món ăn
        public int ProductID { get; set; }

        // Thuộc tính lưu trữ tên của món ăn hiển thị trong giỏ hàng
        public string ProductName { get; set; }

        // Thuộc tính lưu trữ đơn giá bán của món ăn (VNĐ)
        public decimal Price { get; set; }

        // Thuộc tính lưu trữ số lượng sản phẩm khách hàng chọn mua
        public int Quantity { get; set; }

        // Thuộc tính lưu trữ đường dẫn ảnh đại diện minh họa món ăn
        public string ImageUrl { get; set; }

        // 🟢 BỔ SUNG DÒNG NÀY: Lưu số lượng tồn kho của sản phẩm
        public int Stock { get; set; }

        // Thuộc tính tính toán tự động Thành tiền = Đơn giá nhân với Số lượng
        public decimal TotalPrice => Price * Quantity;       
    }
}