using System.ComponentModel.DataAnnotations;

namespace SKDJK.ViewModels
{
    // Trang danh sách ngôn ngữ cho quản trị viên.
    public sealed class AdminLanguageListViewModel
    {
        public string? Search { get; set; }
        public List<AdminLanguageItemViewModel> Items { get; set; } = [];
    }

    // Một dòng ngôn ngữ trên bảng quản trị.
    public sealed class AdminLanguageItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TopicCount { get; set; }
    }

    // Form dùng chung cho thao tác thêm và sửa ngôn ngữ.
    public sealed class AdminLanguageFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ngôn ngữ")]
        [StringLength(50)]
        [Display(Name = "Tên ngôn ngữ")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mã ngôn ngữ")]
        [StringLength(10)]
        [RegularExpression("^[a-zA-Z-]+$", ErrorMessage = "Mã chỉ gồm chữ cái và dấu gạch ngang")]
        [Display(Name = "Mã ngôn ngữ")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
