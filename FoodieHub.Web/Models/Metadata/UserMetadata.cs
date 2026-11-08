// Khai báo thư viện chứa các Thuộc tính Đánh dấu dữ liệu (Data Annotations)
using System.ComponentModel.DataAnnotations;

// Namespace đặt theo đúng cấu trúc thư mục Models/Metadata của dự án
namespace FoodieHub.Web.Models
{
    // Sử dụng kĩ thuật Partial Class kết hợp MetadataType để không bị mất DataAnnotation khi CSDL được cập nhật (Barem Dòng 7)
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        // Lớp nội bộ UserMetadata chứa toàn bộ luật Validation và cấu hình nhãn hiển thị cho Bảng Người Dùng
        internal class UserMetadata
        {
            // Thiết lập nhãn hiển thị cho thuộc tính UserID trên giao diện HTML
            [Display(Name = "Mã người dùng")]
            public int UserID { get; set; }

            // Đánh dấu trường Email bắt buộc phải nhập
            [Required(ErrorMessage = "Email không được để trống")]
            // Kiểm tra cấu trúc Email nhập vào phải đúng định dạng địa chỉ Email hợp lệ
            [EmailAddress(ErrorMessage = "Địa chỉ Email không đúng định dạng")]
            // Thiết lập tên hiển thị trên ô nhập liệu của Form
            [Display(Name = "Địa chỉ Email")]
            public string Email { get; set; }

            // Đánh dấu trường Password bắt buộc phải nhập
            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            // Ràng buộc độ dài tối thiểu của mật khẩu từ 6 ký tự trở lên để đảm bảo an toàn bảo mật
            [MinLength(6, ErrorMessage = "Mật khẩu phải chứa ít nhất 6 ký tự")]
            // Đặt tên hiển thị cho trường Mật khẩu
            [Display(Name = "Mật khẩu")]
            // Chỉ định kiểu dữ liệu Password để Razor tự động ẩn ký tự khi gõ
            [DataType(DataType.Password)]
            public string Password { get; set; }

            // Đánh dấu bắt buộc nhập tên người dùng
            [Required(ErrorMessage = "Họ và tên không được để trống")]
            // Đặt tên hiển thị cho trường Họ và tên
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; }
        }
    }
}