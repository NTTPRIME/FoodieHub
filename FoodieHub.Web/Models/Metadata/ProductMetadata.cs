// Khai báo thư viện chứa các Thuộc tính Đánh dấu dữ liệu (Data Annotations)
using System.ComponentModel.DataAnnotations;

// Namespace đặt theo đúng cấu trúc thư mục Models/Metadata của dự án
namespace FoodieHub.Web.Models
{
    // Sử dụng kĩ thuật Partial Class kết hợp MetadataType để không bị mất DataAnnotation khi CSDL được cập nhật
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
        // Lớp nội bộ ProductMetadata chứa toàn bộ luật Validation và cấu hình nhãn hiển thị cho Bảng Sản Phẩm
        internal class ProductMetadata
        {
            // Thiết lập nhãn hiển thị cho thuộc tính ProductID trên giao diện HTML
            [Display(Name = "Mã sản phẩm")]
            public int ProductID { get; set; }

            // Đánh dấu trường ProductName bắt buộc phải nhập
            [Required(ErrorMessage = "Tên món ăn không được để trống")]
            // Giới hạn độ dài tối đa của tên món ăn là 200 ký tự
            [StringLength(200, ErrorMessage = "Tên món ăn không được vượt quá 200 ký tự")]
            // Thiết lập tên hiển thị trên ô nhập liệu của Form
            [Display(Name = "Tên món ăn")]
            public string ProductName { get; set; }

            // Đánh dấu trường Price bắt buộc phải nhập
            [Required(ErrorMessage = "Giá bán không được để trống")]
            // Ràng buộc giá trị nhập vào phải trong khoảng từ 0 đến 100,000,000 VNĐ
            [Range(0, 100000000, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
            // Định dạng hiển thị kiểu tiền tệ hoặc số nguyên có phân cách
            [Display(Name = "Giá bán (VNĐ)")]
            public decimal Price { get; set; }

            // Đánh dấu trường Stock bắt buộc phải nhập
            [Required(ErrorMessage = "Số lượng tồn kho không được để trống")]
            // Ràng buộc số lượng tồn kho không được nhập số âm
            [Range(0, 100000, ErrorMessage = "Số lượng tồn kho không được âm")]
            // Thiết lập tên hiển thị cho trường Số lượng tồn
            [Display(Name = "Số lượng tồn kho")]
            public int Stock { get; set; }

            // Đánh dấu chọn danh mục là bắt buộc
            [Required(ErrorMessage = "Vui lòng chọn danh mục cho sản phẩm")]
            // Thiết lập tên hiển thị cho trường Khóa ngoại Danh mục
            [Display(Name = "Danh mục món ăn")]
            public int CategoryID { get; set; }
        }
    }
}