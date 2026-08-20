# Báo cáo hoàn thành module Test TOEIC-style / IELTS-style

## 1. Phạm vi đã thực hiện

Module Test đã được sửa theo đặc tả mới trên kiến trúc ASP.NET Core MVC + EF Core hiện có. Logic nằm trực tiếp trong `TestService`, không thêm repository hoặc framework giao diện mới. Các nội dung đề chỉ là dữ liệu tự viết để chứng minh cấu trúc; màn hình luôn ghi rõ đây là điểm nội bộ SKDJK và không phải bài thi chính thức.

Nguồn đối chiếu cấu trúc:

- ETS TOEIC Listening & Reading: https://www.ets.org/toeic/about/listening-reading.html
- ETS TOEIC Examinee Handbook: https://www.ets.org/content/dam/ets-org/pdfs/toeic/toeic-listening-reading-test-examinee-handbook.pdf
- IELTS Academic test format: https://ielts.org/organisations/ielts-for-organisations/test-types/ielts-academic-test/academic-test-format-in-detail

## 2. Thay đổi cơ sở dữ liệu

Migration `20260820114301_FixTestModesGroupsAndWordLimits` bổ sung:

- `tests.Mode`, `tests.IsActive`, `tests.CreatedAt`.
- `questions.Instruction`, `questions.MaxWords`.
- `testresults.TotalQuestions` để lưu tổng câu tại thời điểm nộp.
- `useranswers.TextAnswer` nullable.
- Unique index `(TestId, Order)`.

Migration cũng backfill `TotalQuestions` cho lịch sử cũ, mặc định Test cũ là `Practice`, và chuẩn hóa dữ liệu cũ theo hai quy tắc:

- Câu `FillBlank`/`ListeningFill`: `AnswerId = null`.
- Câu lựa chọn: `TextAnswer = null`.

Migration đã áp thành công lên database `HeThongHocNgoaiNguTrucTuyen`.

## 3. Quy tắc Format và Mode

`TestMode` có hai giá trị:

- `Practice`: đề rút gọn nhưng vẫn giữ đúng cách chia Part/Section và group.
- `FullMock`: service kiểm tra toàn bộ cấu trúc trước khi cho phép lưu.

TOEIC FullMock:

- Timer chung 120 phút.
- Tổng 200 câu.
- Part 1–7 lần lượt `6/25/39/30/30/16/54`.
- Part 1–4 là `ListeningChoice`; Part 5–7 là `MultipleChoice`.
- Part 3 có 13 group x 3 câu; Part 4 có 10 group x 3 câu.
- Listening phải có audio; Part 1 phải có ảnh; Part 6–7 phải có passage và GroupCode.

IELTS FullMock của web app:

- Timer chung 90 phút, tương ứng 30 phút Listening + 60 phút Reading trong phạm vi ứng dụng.
- Listening 40 câu, 10 câu ở mỗi Part 1–4.
- Reading 40 câu, tổ chức đủ Passage 1–3.
- Listening dùng `ListeningChoice`/`ListeningFill`, có audio và GroupCode.
- Reading dùng dạng đọc, có ContextText và GroupCode.

## 4. Dữ liệu Practice đã seed

TOEIC Practice, Test ID `10001`, tổng 22 câu:

| Part | Số câu | Cấu trúc |
|---|---:|---|
| 1 | 2 | Mỗi câu có ảnh + audio |
| 2 | 2 | Question-response |
| 3 | 3 | Một conversation/audio chung |
| 4 | 3 | Một talk/audio chung |
| 5 | 3 | MultipleChoice độc lập |
| 6 | 4 | Một email/passage chung |
| 7 | 5 | Một notice/passage chung |

IELTS Practice, Test ID `10002`, tổng 11 câu:

| Section | Part/Passage | Số câu | Dạng chính |
|---|---:|---:|---|
| Listening | 1 | 1 | Completion, MaxWords=1 |
| Listening | 2 | 1 | Choice |
| Listening | 3 | 2 | Matching biểu diễn bằng choice |
| Listening | 4 | 1 | Completion, MaxWords=1 |
| Reading | 1 | 2 | MultipleChoice |
| Reading | 2 | 2 | TFNG và YNNG |
| Reading | 3 | 2 | Completion và matching choice |

## 5. Luồng chấm điểm server-side

`TestService.SubmitAsync` hoạt động theo thứ tự:

1. Kiểm tra User và Test đang hoạt động.
2. Tải lại Questions, Answers và `IsCorrect` từ database.
3. Từ chối QuestionId trùng, QuestionId không thuộc Test hoặc AnswerId không thuộc Question.
4. Với câu text, chuẩn hóa khoảng trắng và chữ hoa/thường, sau đó kiểm tra `MaxWords`.
5. Với câu choice, chỉ chấp nhận Answer thuộc đúng Question.
6. Tạo `TestResult` với snapshot `TotalQuestions`.
7. Lưu `TestResult` và toàn bộ `UserAnswer` trong một transaction.
8. Tính `Score = round(CorrectCount * 100 / TotalQuestions, 2)`.

ViewModel trang làm bài không có `IsCorrect`, vì vậy đáp án đúng không bị render xuống HTML trước khi nộp.

## 6. Group audio và passage

`TestService.GetTakeAsync` gom câu trực tiếp bằng khóa `SectionName + PartNumber + GroupCode`. Nếu câu không có GroupCode, QuestionId được dùng làm group riêng. Logic này nằm ngay trong hàm public, không tách thành hàm private.

Kết quả:

- TOEIC Part 3: 3 câu nhưng chỉ 1 audio.
- TOEIC Part 4: 3 câu nhưng chỉ 1 audio.
- TOEIC Part 6: 4 câu nhưng chỉ 1 passage.
- TOEIC Part 7: 5 câu nhưng chỉ 1 passage.
- IELTS Listening/Reading cũng render media/context một lần theo recording/passage.

Practice hiển thị audio controls. FullMock chỉ có nút phát một lần, không render thanh tua; trạng thái phát lại được giữ bằng JavaScript trong `Take.cshtml`.

## 7. Khóa dữ liệu sau lượt làm

Sau khi Test đã có `TestResult`:

- Test chỉ được sửa `Title`, `Description`, `IsActive`.
- Không được đổi Lesson, Format, Mode hoặc Duration.
- Không được sửa, thêm hoặc xóa Question/Answer.
- Không được xóa Test.

Kiểm thử form admin trả đúng thông báo: `Không thể thay đổi câu hỏi của bài kiểm tra đã có kết quả.`

## 8. Giao diện và file tĩnh

Toàn bộ file sau đã bị xóa theo yêu cầu:

- `wwwroot/css/site.css`
- `wwwroot/css/user-pages.css`
- `wwwroot/js/site.js`
- `wwwroot/js/lesson-tabs.js`

CSS hiện nằm trong `Views/Shared/_InlineStyles.cshtml`. JavaScript nằm trong `_Layout.cshtml`, `Lesson/Index.cshtml` và `Test/Take.cshtml`. Style chỉ dùng nền trắng/xám, một màu nhấn, border đơn giản, không gradient và không box-shadow. `wwwroot` chỉ còn favicon và ảnh SVG tĩnh.

## 9. Chú thích code

Các lớp, hàm và khối nghiệp vụ mới đều có chú thích ngay phía trên, đặc biệt ở:

- `Models/enums/TestMode.cs`.
- `Services/TestService.cs`.
- `Data/DemoData.cs`.
- `Views/Test/Take.cshtml`.
- `Views/Shared/_InlineStyles.cshtml`.
- `Views/Lesson/Index.cshtml`.

## 10. Ranh giới DTO và ViewModel

- DTO chỉ đi giữa Service và Controller.
- ViewModel chỉ đi giữa Controller và Razor View.
- Service và interface service không tham chiếu `SKDJK.ViewModels`.
- Controller tạo DTO từ ViewModel trước khi gọi hàm lưu/nộp bài.
- Controller tạo ViewModel từ DTO trước khi render danh sách, form, trang làm bài và trang kết quả.
- Service không có hàm private; validation FullMock, validation Question, chuẩn hóa câu trả lời và gom group được viết trực tiếp trong hàm public tương ứng.
- `Views/Shared/_Layout.cshtml`.

Chú thích giải thích mục đích của câu lệnh hoặc khối lệnh; tài liệu này giải thích luồng tổng thể để người đọc không phải suy đoán quan hệ giữa Controller, Service, EF và Razor.

## 10. Kết quả kiểm thử

- `dotnet build SKDJK.csproj --no-restore`: 0 warning, 0 error.
- `dotnet ef migrations has-pending-model-changes`: không có thay đổi model chưa migrate.
- `dotnet ef database update`: thành công.
- TOEIC browser submit: `22/22`, `100%`; Listening `10/10`, Reading `12/12`.
- IELTS browser submit: `10/11`, `90.91%`; câu `five pm` với `MaxWords=1` bị chấm sai.
- SQL xác nhận result TOEIC có `TotalQuestions=22`, IELTS có `TotalQuestions=11`.
- SQL xác nhận `InvalidTextRows=0`, `InvalidChoiceRows=0`.
- DOM xác nhận trang TOEIC có 22 câu, IELTS có 11 câu và không chứa `IsCorrect`.
- Browser console: không có error/warning.

## 11. Bộ request thủ công

`app.http` có các request cho:

- Login và lấy anti-forgery token.
- Lọc `/tests` theo Format/Mode.
- Mở TOEIC/IELTS Practice.
- Submit đủ 22 câu TOEIC và đủ 11 câu IELTS.
- Kiểm tra vượt MaxWords.
- Kiểm tra AnswerId sai Question, QuestionId sai Test và QuestionId trùng.
- Mở lịch sử và dashboard admin.

## 12. Cách chạy

```powershell
dotnet restore SKDJK.csproj
dotnet ef database update --project SKDJK.csproj
dotnet run --project SKDJK.csproj
```
