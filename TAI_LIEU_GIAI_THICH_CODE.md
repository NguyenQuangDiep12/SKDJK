# Tài liệu triển khai và giải thích code SKDJK

## 1. Mục tiêu và nguyên tắc triển khai

Ứng dụng được phát triển tiếp trên kiến trúc có sẵn: ASP.NET Core MVC, Razor View, Entity Framework Core và SQL Server. Không thay framework, không thêm SPA, không dùng React/Blazor và không tạo tầng service phức tạp hơn nhu cầu của đặc tả.

Các chú thích trong mã nguồn được đặt ngay trên lớp, hàm hoặc khối lệnh có nghiệp vụ để giải thích **vì sao khối đó tồn tại**. Những dòng gán thuộc tính hiển nhiên được giải thích tập trung trong tài liệu này để tránh biến code thành một tệp chỉ toàn chú thích lặp lại.

CSS dùng cho giao diện nằm trong `Views/Shared/_InlineStyles.cshtml` và được nhúng trực tiếp vào layout. JavaScript dùng chung nằm trong `Views/Shared/_Layout.cshtml`; JavaScript riêng của bài học và Test nằm trong chính View tương ứng. Toàn bộ file `.css` và `.js` trong `wwwroot` đã được xóa. Mục tiêu là bám bố cục wireframe bằng nền trắng/xám, một màu nhấn, viền đơn giản, không gradient và không đổ bóng.

## 2. Kiến trúc xử lý chung

```text
Trình duyệt
    -> ViewModel: nhận dữ liệu form từ Razor và kiểm tra DataAnnotations
    -> Controller: ánh xạ ViewModel thành DTO, kiểm tra đăng nhập và phân quyền
    -> Service: chỉ nhận/trả DTO, truy vấn dữ liệu và xử lý nghiệp vụ
    -> ApplicationDbContext/EF Core: đọc ghi SQL Server
    -> Service trả DTO về Controller
    -> Controller ánh xạ DTO thành ViewModel
    -> Razor View: chỉ nhận ViewModel để render HTML/CSS đơn giản
```

Ý nghĩa từng tầng:

- `Models`: cấu trúc dữ liệu lưu trong cơ sở dữ liệu.
- `Data/Configurations`: khai báo kiểu cột, độ dài, quan hệ và ràng buộc EF Core.
- `Dtos`: đối tượng thuần dữ liệu duy nhất được chuyển hai chiều giữa Controller và Service; không chứa DataAnnotations hay logic giao diện.
- `ViewModels`: đối tượng chuyển hai chiều giữa Controller và Razor View; chứa validation form và thuộc tính tính toán phục vụ hiển thị.
- `Services`: nghiệp vụ ngắn gọn, truy vấn trực tiếp bằng EF Core, không tham chiếu namespace `ViewModels` và không tách hàm private.
- `Controllers`: điều phối HTTP và chịu trách nhiệm ánh xạ `ViewModel -> DTO` trước khi gọi service, rồi ánh xạ `DTO -> ViewModel` trước khi gọi view.
- `Views`: giao diện động lấy từ model, không hard-code danh sách nghiệp vụ.

Ví dụ luồng lưu câu hỏi:

```text
QuestionForm.cshtml
    -> AdminQuestionFormViewModel
    -> TestController.SaveQuestion
    -> AdminQuestionFormDto
    -> TestService.SaveQuestionAsync
```

Ví dụ luồng hiển thị kết quả:

```text
TestService.GetResultAsync
    -> TestResultDto
    -> TestController.Result
    -> TestResultViewModel
    -> Result.cshtml
```

Các hàm ánh xạ có thể là hàm private trong Controller vì chúng thuộc ranh giới HTTP/giao diện. Riêng Service không có hàm private; phần lọc, chuẩn hóa, validation, gom nhóm và đếm từ được viết trực tiếp trong hàm public đang xử lý nghiệp vụ để người đọc theo dõi một mạch.

## 3. Các nhóm chức năng đã triển khai

### 3.1. Đăng nhập và phân quyền

- Cookie Authentication giữ nguyên từ dự án gốc.
- Cookie có `HttpOnly`, `SameSite=Lax`, `Secure` trong môi trường production.
- Route người học có `[Authorize]`.
- Route quản trị có `[Authorize(Roles = "ADMIN")]`.
- `ControllerClaimsExtensions.TryGetUserId` đọc ID người dùng từ claim và không tin ID do client gửi lên.
- Người chưa đăng nhập truy cập trang quản trị sẽ được chuyển đến trang đăng nhập.

### 3.2. Trang chủ người học

Các file chính:

- `Services/HomeService.cs`
- `Controllers/HomeController.cs`
- `Views/Home/Index.cshtml`

Ý nghĩa:

- Đếm số chủ đề đã học từ các bài có tiến độ khác `NOTSTARTED`.
- Đếm bài kiểm tra đã nộp từ `TestResult`.
- Tính phần trăm hoàn thành từ số bài học `COMPLETED` trên tổng số bài học.
- Lấy bài đang học gần nhất làm mục “Tiếp tục học”.
- Gợi ý các chủ đề chưa bắt đầu, giới hạn số card để giao diện gọn.

### 3.3. Ngôn ngữ và chủ đề

Các file chính:

- `Services/LanguageService.cs`
- `Services/TopicService.cs`
- `Controllers/LanguageController.cs`
- `Controllers/TopicController.cs`
- `Views/Topic/Index.cshtml`
- `Views/Topic/Details.cshtml`
- `Views/Admin/Language/*`
- `Views/Admin/Topic/*`

Ý nghĩa:

- Người học có thể tìm chủ đề, lọc theo ngôn ngữ và cấp độ, phân trang.
- Trang chi tiết chủ đề hiển thị mô tả và danh sách bài học thật từ DB.
- Admin CRUD ngôn ngữ/chủ đề với validation.
- Không cho xóa ngôn ngữ đã có chủ đề hoặc chủ đề đã có bài học.
- `Topic.Level` dùng Unicode để lưu đúng “Cơ bản”, “Trung cấp”, “Nâng cao”.

### 3.4. Bài học và từ vựng

Các file chính:

- `Services/LessonService.cs`
- `Controllers/LessonController.cs`
- `Views/Lesson/Index.cshtml`
- `Views/Lesson/Components/_Vocabulary.cshtml`
- `Views/Lesson/Components/_Grammar.cshtml`
- `Views/Lesson/Components/_Listening.cshtml`
- `Views/Lesson/Components/_Speaking.cshtml`
- `Views/Lesson/Index.cshtml` (JavaScript tab và audio được viết trong `@section Scripts`)
- `Views/Admin/Lesson/*`

Luồng xử lý:

1. Khi mở bài học, service kiểm tra user và lesson có tồn tại.
2. Nếu chưa có `LearningProgress`, hệ thống tạo tiến độ `INPROGRESS` ở mức 10%.
3. Service xác định bài trước/bài sau trong cùng chủ đề.
4. Từ vựng được ánh xạ sang thẻ nhớ và ví dụ.
5. Ngữ pháp dùng nội dung `Lesson.Content`.
6. Luyện nghe lấy các câu `ListeningChoice`/`ListeningFill` thuộc test của lesson.
7. Tab luyện nói chỉ cho nghe audio mẫu từ Free Dictionary rồi tự luyện theo.
8. Nút hoàn thành cập nhật `CompletionPercent = 100` và trạng thái `COMPLETED`.

Lưu ý bảo mật câu luyện nghe:

- Câu `ListeningChoice` được gửi danh sách lựa chọn nhưng không gửi cờ `IsCorrect`.
- Câu `ListeningFill` không gửi danh sách đáp án đúng xuống trình duyệt.
- Truy vấn luyện nghe được tải thành entity trước rồi ánh xạ trong bộ nhớ để EF Core không dịch phép so sánh enum sai sang SQL.

CRUD quản trị:

- Admin thêm/sửa/xóa Lesson và Vocabulary.
- Không xóa lesson đã có test hoặc tiến độ học.
- Không xóa vocabulary đã có dữ liệu lịch sử liên quan.

### 3.5. Free Dictionary

Các file chính:

- `Services/FreeDictionaryService.cs`
- `Services/Interfaces/IFreeDictionaryService.cs`
- `Controllers/DictionaryController.cs`
- `Dtos/DictionaryPronunciationDto.cs`

Luồng xử lý:

```text
Nút loa
  -> GET /dictionary/pronunciation?word=...
  -> FreeDictionaryService
  -> GET https://api.dictionaryapi.dev/api/v2/entries/en/{word}
  -> chỉ trả Word, Phonetic, AudioUrl
  -> trình duyệt tạo thẻ audio và phát
```

Quy tắc:

- Timeout 10 giây.
- Encode từ trước khi ghép URL.
- Xử lý riêng 404, response rỗng, response lỗi, timeout, lỗi mạng và JSON lỗi.
- URL audio bắt đầu bằng `//` được chuẩn hóa thành `https://`.
- Không dùng Web Speech API, browser TTS, ghi âm, microphone, MediaRecorder, STT hoặc chấm điểm phát âm.
- Nếu API không có audio, giao diện báo lỗi nhẹ và vẫn dùng được các chức năng khác.

### 3.6. Bài kiểm tra TOEIC-style và IELTS-style

Các file chính:

- `Models/enums/TestFormat.cs`
- `Models/enums/QuestionType.cs`
- `Models/Test.cs`
- `Models/Question.cs`
- `Models/Answer.cs`
- `Models/TestResult.cs`
- `Models/UserAnswer.cs`
- `Services/TestService.cs`
- `Controllers/TestController.cs`
- `Views/Test/Index.cshtml`
- `Views/Test/Take.cshtml`
- `Views/Test/Result.cshtml`
- `Views/Test/History.cshtml`

Metadata hỗ trợ:

- `Test.Format`: `ToeicStyle` hoặc `IeltsStyle`.
- `Test.Mode`: `Practice` hoặc `FullMock`.
- `Test.IsActive`: quyết định đề có xuất hiện và có nhận bài nộp hay không.
- `Test.CreatedAt`: thời điểm tạo đề theo UTC.
- `Question.QuestionType`: `MultipleChoice`, `FillBlank`, `ListeningChoice`, `ListeningFill`, `TrueFalseNotGiven`, `YesNoNotGiven`.
- `SectionName`: `Listening` hoặc `Reading`.
- `PartNumber`: TOEIC-style Part 1–7; IELTS-style Listening Section 1–4 hoặc Reading Passage 1–3.
- `Order`: thứ tự câu toàn bài.
- `AudioUrl`, `ImageUrl`, `ContextText`, `GroupCode`: dữ liệu media/ngữ cảnh/nhóm câu.
- `Instruction`, `MaxWords`: hướng dẫn và giới hạn từ của dạng completion.
- `TestResult.TotalQuestions`: snapshot tổng số câu tại thời điểm nộp, giúp lịch sử không đổi khi đề được cập nhật hợp lệ.

Luồng làm bài:

1. GET `/tests/{id}` chỉ lấy nội dung câu hỏi và nội dung lựa chọn.
2. View không nhận `Answer.IsCorrect`.
3. Service gom câu theo `Section + Part + GroupCode`; View render audio/passage một lần cho cả group.
4. JavaScript nội tuyến chỉ chuyển group, cập nhật số câu đã trả lời, điều khiển audio FullMock và đếm ngược.
5. Form gửi `QuestionId`, `AnswerId` hoặc `TextAnswer` cùng anti-forgery token.
6. Controller lấy `TestId` đáng tin cậy từ route.
7. Service tải lại toàn bộ test, question và answer từ DB.
8. Service từ chối câu trùng, question không thuộc test và answer không thuộc question.
9. Service chấm từng câu rồi lưu `TestResult` và `UserAnswer` trong một transaction.
10. Trang kết quả chỉ mở nếu `TestResult.UserId` trùng user hiện tại.

Công thức điểm:

```text
Score = round(CorrectCount * 100 / TotalQuestions, 2)
```

So khớp câu điền:

- Bỏ khoảng trắng đầu/cuối.
- Gộp nhiều khoảng trắng thành một.
- Không phân biệt chữ hoa/chữ thường.
- Một câu có thể có nhiều đáp án đúng chấp nhận được.
- Nếu số token tách theo khoảng trắng lớn hơn `MaxWords`, câu luôn sai dù chuỗi còn lại có chứa đáp án đúng.
- Câu text lưu `AnswerId = null`; câu lựa chọn lưu `TextAnswer = null`.

Đây là điểm nội bộ phần trăm của SKDJK, không quy đổi band IELTS hoặc thang TOEIC chính thức.

### 3.7. Quản trị bài kiểm tra và câu hỏi

Các view nằm trong `Views/Admin/Test/*`.

Validation chính:

- TOEIC-style chỉ nhận ListeningChoice ở Part 1–4 và MultipleChoice ở Part 5–7.
- IELTS-style Listening chỉ nhận Section 1–4.
- IELTS-style Reading chỉ nhận Passage 1–3.
- Câu Listening bắt buộc `AudioUrl`.
- TOEIC-style Listening Part 1 bắt buộc thêm `ImageUrl`.
- Câu lựa chọn cần ít nhất hai lựa chọn và đúng một đáp án đúng.
- Câu điền cần ít nhất một đáp án được đánh dấu đúng.
- Thứ tự câu không được trùng trong cùng test.
- `FullMock` TOEIC yêu cầu đúng 200 câu, 120 phút, số câu Part 1–7 là `6/25/39/30/30/16/54`, Part 3 có 13 group x 3 câu và Part 4 có 10 group x 3 câu.
- `FullMock` IELTS trong hệ thống dùng timer chung 90 phút, 40 Listening và 40 Reading; Listening có 10 câu mỗi Part 1–4, Reading có đủ Passage 1–3.
- Sau khi có lượt làm, Test chỉ cho sửa `Title`, `Description`, `IsActive`; câu hỏi và đáp án bị khóa để bảo toàn lịch sử.

### 3.8. Tiến độ

Các file chính:

- `Services/ProgressService.cs`
- `Controllers/ProgressController.cs`
- `Views/Progress/Index.cshtml`

Trang tiến độ hiển thị:

- Số bài hoàn thành trên tổng bài.
- Số test đã làm trên tổng test.
- Tiến độ theo chủ đề.
- Các bài học hoàn thành gần đây.
- Lịch sử test gần đây và liên kết xem kết quả chi tiết.

## 4. Dữ liệu demo

`Data/DemoData.cs` seed dữ liệu Practice tối thiểu nhưng đủ kiểm tra nghiệp vụ:

- Ngôn ngữ demo.
- Chủ đề và bài học demo.
- Từ vựng có phiên âm/ví dụ.
- Một TOEIC Practice 22 câu: Part 1–7 lần lượt `2/2/3/3/3/4/5`; Part 3, 4, 6, 7 dùng group chung.
- Một IELTS Practice 11 câu: Listening Part 1–4 có `1/1/2/1` câu và Reading Passage 1–3 có `2/2/2` câu.
- Các loại câu hỏi lựa chọn, điền từ, TRUE/FALSE/NOT GIVEN và YES/NO/NOT GIVEN.
- Có trường hợp completion `MaxWords = 1` để kiểm thử giới hạn từ.
- Ảnh SVG nội bộ trong `wwwroot/images` để không phụ thuộc dịch vụ ảnh ngoài.

Tài khoản admin có sẵn từ migration gốc:

```text
Email: Bach1994@gmail.com
Password: Bach123
```

## 5. Migration cơ sở dữ liệu

Ba migration chính hoàn thiện đặc tả hiện tại:

- `20260820102913_CompleteCoreFeatures`: thêm format, metadata câu hỏi, kiểu câu mới, nới độ chính xác điểm, làm `UserAnswer.AnswerId` nullable và seed dữ liệu demo.
- `20260820104854_MakeTopicLevelUnicode`: chuyển `Topic.Level` sang Unicode và sửa lại dữ liệu tiếng Việt.
- `20260820114301_FixTestModesGroupsAndWordLimits`: thêm Mode/IsActive/CreatedAt, Instruction/MaxWords, TotalQuestions, index unique Order, chuẩn hóa UserAnswer và bổ sung seed theo group.

Các migration trước đó của dự án được giữ nguyên, không viết lại lịch sử migration.

Lệnh áp dụng:

```powershell
dotnet ef database update --project SKDJK.csproj
```

## 6. Cấu hình và chạy ứng dụng

Connection string hiện dùng SQL Server Express cục bộ và `Encrypt=False` để phù hợp môi trường phát triển:

```text
Server=LAPTOP-N6493SNQ\SQLEXPRESS;Database=HeThongHocNgoaiNguTrucTuyen;Trusted_Connection=true;Encrypt=False;TrustServerCertificate=true;
```

Chạy ứng dụng:

```powershell
dotnet restore SKDJK.csproj
dotnet ef database update --project SKDJK.csproj
dotnet run --project SKDJK.csproj
```

## 7. Kiểm thử HTTP thủ công

`app.http` có sẵn các ví dụ:

- Lấy trang đăng nhập và anti-forgery token.
- Đăng nhập đúng/sai.
- Mở trang chủ, chủ đề, chi tiết chủ đề và bài học.
- Gọi Free Dictionary proxy.
- Mở `/tests`, lọc Format/Mode, mở TOEIC và IELTS Practice.
- Ví dụ POST đủ 22 câu TOEIC, đủ 11 câu IELTS, một ca vượt `MaxWords`, AnswerId sai Question, QuestionId sai Test và câu trùng.
- Kiểm tra các dashboard quản trị.

Các biến `authCookie` và `antiForgeryToken` phải được lấy từ phản hồi đăng nhập thật; không hard-code token bảo mật.

## 8. Kết quả kiểm tra đã thực hiện

- `dotnet build SKDJK.csproj --no-restore`: thành công, 0 warning, 0 error.
- Migration đã áp dụng thành công vào SQL Server Express.
- Trang Home, Topic, Topic Details, Lesson, Test, Progress render dữ liệu thật.
- Dashboard Language, Topic, Lesson/Vocabulary, Test và Question render thành công.
- Luồng TOEIC Practice 22 câu được nộp thật; server chấm `22/22 = 100%`, breakdown Listening `10/10`, Reading `12/12`.
- Luồng IELTS Practice được nộp thật; câu `five pm` vượt `MaxWords = 1` bị chấm sai, kết quả `10/11 = 90.91%`.
- SQL xác nhận cả hai kết quả không có dòng text chứa AnswerId và không có dòng choice chứa TextAnswer.
- HTML trang làm bài không chứa chuỗi `IsCorrect`; Part 3/4 chỉ có một audio cho ba câu, Part 6/7 chỉ có một context cho cả group.
- Trang kết quả hiển thị lịch sử đáp án sau khi nộp.
- Người đăng xuất truy cập `/Language` bị chuyển đến Login.
- Không có lỗi JavaScript trong console trên các luồng đã kiểm tra.
- Tab luyện nói chỉ có nút audio mẫu, không có thành phần ghi âm/chấm giọng nói.

## 9. Các điểm cố ý giữ đơn giản

- Service dùng EF Core trực tiếp, không thêm repository/unit-of-work trung gian.
- Tiến độ dùng tỉ lệ hoàn thành đơn giản, không thêm engine phân tích học tập.
- UI dùng Razor, CSS/JavaScript viết thẳng trong `.cshtml`; `wwwroot` chỉ giữ favicon và ảnh tĩnh.
- Không upload media cho nội dung test trong lần này; admin nhập URL theo đúng đặc tả tối thiểu.
- Không quy đổi điểm sang chuẩn thi chính thức.
- Không triển khai ghi âm hoặc AI chấm phát âm.
