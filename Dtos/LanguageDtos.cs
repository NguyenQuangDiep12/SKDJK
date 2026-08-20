namespace SKDJK.Dtos
{
    // DTO chuyển bộ lọc và danh sách ngôn ngữ từ Service sang Controller.
    public sealed class AdminLanguageListDto
    {
        public string? Search { get; set; }
        public List<AdminLanguageItemDto> Items { get; set; } = [];
    }

    // DTO đại diện một dòng ngôn ngữ, không chứa logic hiển thị Razor.
    public sealed class AdminLanguageItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TopicCount { get; set; }
    }

    // DTO đọc một ngôn ngữ từ Service để Controller điền form sửa.
    public sealed class AdminLanguageFormDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // DTO lệnh tạo ngôn ngữ không có Id vì database sẽ sinh khóa chính.
    public sealed class CreateLanguageDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // DTO lệnh cập nhật không có Id; Id luôn được truyền riêng vào UpdateAsync.
    public sealed class UpdateLanguageDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
