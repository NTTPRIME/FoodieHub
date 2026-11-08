// Khai báo thư viện chứa các Thuộc tính Đánh dấu dữ liệu (Data Annotations)
using System.ComponentModel.DataAnnotations;

// Namespace đặt theo đúng cấu trúc thư mục Models/Metadata của dự án
namespace FoodieHub.Web.Models
{
    // Sử dụng kĩ thuật Partial Class kết hợp MetadataType để không bị mất DataAnnotation khi CSDL được cập nhật (Barem Dòng 7)
    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category
    {
        // Lớp nội bộ CategoryMetadata chứa toàn bộ luật Validation và cấu hình nhãn hiển thị cho Bảng Danh Mục
        internal class CategoryMetadata
        {
            // Thiết lập nhãn hiển thị cho thuộc tính CategoryID trên giao diện HTML
            [Display(Name = "Mã danh mục")]
            public int CategoryID { get; set; }

            // Đánh dấu trường CategoryName bắt buộc phải nhập
            [Required(ErrorMessage = "Tên danh mục không được để trống")]
            // Giới hạn độ dài tối đa của tên danh mục là 100 ký tự
            [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
            // Thiết lập tên hiển thị trên ô nhập liệu của Form
            [Display(Name = "Tên danh mục")]
            public string CategoryName { get; set; }
        }
    }
}