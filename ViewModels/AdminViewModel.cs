using SKDJK.Models.enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SKDJK.ViewModels
{
    public class AdminTopicFormViewModel
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn ngôn ngữ")]
        [Display(Name = "Ngôn ngữ")]
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chủ đề")]
        [StringLength(100)]
        [Display(Name = "Tên chủ đề")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập cấp độ")]
        [StringLength(50)]
        [Display(Name = "Cấp độ")]
        public string Level { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [StringLength(500)]
        [Display(Name = "URL hình ảnh")]
        public string? ImageUrl { get; set; }

        // File chỉ đi giữa Razor và Controller; URL sau upload mới được đưa vào DTO cho Service.
        [Display(Name = "Ảnh chủ đề")]
        public IFormFile? ImageFile { get; set; }

        public List<LanguageOptionViewModel> Languages { get; set; } = new();
    }

    public class AdminLessonFormViewModel
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn chủ đề")]
        [Display(Name = "Chủ đề")]
        public int TopicId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài học")]
        [StringLength(255)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung tổng quan")]
        [Display(Name = "Nội dung tổng quan")]
        public string Content { get; set; } = string.Empty;

        public List<AdminTopicOptionViewModel> Topics { get; set; } = new();
    }

    public class AdminTopicOptionViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class AdminVocabularyFormViewModel
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        public int LessonId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Từ")]
        public string Word { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Nghĩa")]
        public string? Meaning { get; set; }

        [StringLength(100)]
        [Display(Name = "Phiên âm")]
        public string? Pronunciation { get; set; }

        [Display(Name = "Ví dụ")]
        public string? Example { get; set; }

        [StringLength(500)]
        [Display(Name = "URL âm thanh")]
        public string? AudioUrl { get; set; }
    }

    public class AdminLessonSectionFormViewModel
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        public int LessonId { get; set; }

        [Required]
        [Display(Name = "Loại nội dung")]
        public LessonSectionType Type { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nội dung")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "URL âm thanh")]
        public string? AudioUrl { get; set; }

        [Display(Name = "Thứ tự")]
        public int SortOrder { get; set; }
    }

    public class AdminTestFormViewModel
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn bài học")]
        [Display(Name = "Bài học")]
        public int LessonId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tên bài kiểm tra")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Range(1, 300)]
        [Display(Name = "Thời gian (phút)")]
        public int DurationMinutes { get; set; } = 30;

        [Required]
        [Display(Name = "Định dạng")]
        public TestFormat Format { get; set; } = TestFormat.ToeicStyle;

        [Required]
        [Display(Name = "Chế độ")]
        public TestMode Mode { get; set; } = TestMode.Practice;

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; } = true;

        public List<AdminLessonOptionViewModel> Lessons { get; set; } = new();
    }

    public class AdminLessonOptionViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
    }

    public class AdminQuestionFormViewModel
    {
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        public int TestId { get; set; }

        [Required]
        [Display(Name = "Nội dung câu hỏi")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Loại câu hỏi")]
        public QuestionType QuestionType { get; set; }

        [Required]
        [RegularExpression("^(Listening|Reading)$", ErrorMessage = "Section chỉ nhận Listening hoặc Reading")]
        [Display(Name = "Section")]
        public string SectionName { get; set; } = "Reading";

        [Range(1, 7)]
        [Display(Name = "Part/Section")]
        public int PartNumber { get; set; } = 1;

        [Range(0, int.MaxValue)]
        [Display(Name = "Thứ tự")]
        public int Order { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã nhóm")]
        public string? GroupCode { get; set; }

        [Display(Name = "Đoạn văn/ngữ cảnh")]
        public string? ContextText { get; set; }

        [StringLength(500)]
        [Display(Name = "URL hình ảnh")]
        public string? ImageUrl { get; set; }

        // File upload là dữ liệu của form nên thuộc ViewModel, không đưa IFormFile xuống DTO/Service nghiệp vụ.
        [Display(Name = "Ảnh câu hỏi")]
        public IFormFile? ImageFile { get; set; }

        [StringLength(500)]
        [Display(Name = "URL âm thanh")]
        public string? AudioUrl { get; set; }

        // Controller upload file này vào questionaudio rồi chỉ truyền URL qua DTO.
        [Display(Name = "Audio câu hỏi")]
        public IFormFile? AudioFile { get; set; }

        [StringLength(500)]
        [Display(Name = "Hướng dẫn")]
        public string? Instruction { get; set; }

        [Range(1, 10, ErrorMessage = "Giới hạn từ phải từ 1 đến 10")]
        [Display(Name = "Số từ tối đa")]
        public int? MaxWords { get; set; }

        public List<AdminAnswerInputViewModel> Answers { get; set; } = new();
    }

    public class AdminAnswerInputViewModel
    {
        public string? Content { get; set; }
        public bool IsCorrect { get; set; }
    }
}
