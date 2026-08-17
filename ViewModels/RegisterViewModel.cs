using System.ComponentModel.DataAnnotations;

namespace SKDJK.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui long nhap ho va ten")]
        [MaxLength(100, ErrorMessage = "So ky tu khong vuot qua 100 ky tu")]
        [Display(Name = "Ho va ten")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui long nhap email dang ky")]
        [EmailAddress(ErrorMessage = "Email khong hop le")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui long nhap mat khau")]
        [MinLength(6, ErrorMessage = "Mat khau co it nhat 6 ky tu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mat khau")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui long nhap xac nhan mat khau")]
        [MinLength(6, ErrorMessage = "Mat khau co it nhat 6 ky tu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mat lai mat khau")]
        [Compare(nameof(Password), ErrorMessage = "Mat khau khong khop")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
