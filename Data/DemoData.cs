using Microsoft.EntityFrameworkCore;
using SKDJK.Models;
using SKDJK.Models.enums;

namespace SKDJK.Data
{
    // Seed dữ liệu tự viết để chứng minh cấu trúc, không sao chép câu hỏi thi có bản quyền.
    public static class DemoData
    {
        // Audio mẫu của Test được cấu hình ở Question, hoàn toàn tách Free Dictionary.
        private const string DemoAudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3";

        // Thời điểm cố định giúp migration seed có dữ liệu xác định.
        private static readonly DateTime SeedCreatedAt = new(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc);

        // Đăng ký toàn bộ dữ liệu demo bằng HasData để migration quản lý lịch sử.
        public static void Configure(ModelBuilder modelBuilder)
        {
            // Seed hai ngôn ngữ phục vụ các màn hình quản trị và người học.
            modelBuilder.Entity<Language>().HasData(
                new Language { Id = 10001, Name = "Tiếng Anh", Code = "en-demo", Description = "Ngôn ngữ dùng cho dữ liệu demo TOEIC/IELTS-style." },
                new Language { Id = 10002, Name = "Tiếng Nhật", Code = "ja-demo", Description = "Ngôn ngữ mẫu bổ sung." });

            // Seed chủ đề có đủ cấp độ và ảnh nội bộ.
            modelBuilder.Entity<Topic>().HasData(
                new Topic { Id = 10001, LanguageId = 10001, Name = "Giao tiếp và du lịch", Level = "Trung cấp", Description = "Từ vựng và kỹ năng giao tiếp trong các tình huống du lịch.", ImageUrl = "/images/topic-travel.svg" },
                new Topic { Id = 10002, LanguageId = 10001, Name = "Học tập và công việc", Level = "Cơ bản", Description = "Mẫu câu thường gặp trong học tập và công việc.", ImageUrl = "/images/topic-work.svg" },
                new Topic { Id = 10003, LanguageId = 10002, Name = "Chào hỏi tiếng Nhật", Level = "Cơ bản", Description = "Các mẫu chào hỏi đơn giản.", ImageUrl = "/images/topic-japanese.svg" });

            // Seed bài học để Test tiếp tục suy ra Topic và Level qua quan hệ hiện có.
            modelBuilder.Entity<Lesson>().HasData(
                new Lesson { Id = 10001, TopicId = 10001, Title = "Đặt phòng khách sạn", Description = "Học từ vựng và mẫu câu đặt phòng.", Content = "Thì hiện tại đơn được dùng để nói về thói quen và sự thật. Cấu trúc: S + V(s/es). Ví dụ: She books a room online." },
                new Lesson { Id = 10002, TopicId = 10001, Title = "Hỏi đường và phương tiện", Description = "Hỏi đường, vị trí và phương tiện đi lại.", Content = "Dùng Can you tell me...? hoặc How can I get to...? để hỏi đường một cách lịch sự." },
                new Lesson { Id = 10003, TopicId = 10002, Title = "Giao tiếp tại nơi làm việc", Description = "Từ vựng văn phòng cơ bản.", Content = "Dùng hiện tại đơn để mô tả công việc thường ngày và lịch làm việc." },
                new Lesson { Id = 10004, TopicId = 10003, Title = "Lời chào cơ bản", Description = "Các lời chào thông dụng.", Content = "こんにちは được dùng để chào hỏi vào ban ngày." });

            // Seed từ vựng phục vụ riêng module học và Free Dictionary.
            modelBuilder.Entity<Vocabulary>().HasData(
                new Vocabulary { Id = 10001, LessonId = 10001, Word = "journey", Meaning = "hành trình", Pronunciation = "/ˈdʒɜːni/", Example = "It was a long journey across the country." },
                new Vocabulary { Id = 10002, LessonId = 10001, Word = "reservation", Meaning = "đặt chỗ", Pronunciation = "/ˌrezəˈveɪʃn/", Example = "I have a reservation for two nights." },
                new Vocabulary { Id = 10003, LessonId = 10001, Word = "luggage", Meaning = "hành lý", Pronunciation = "/ˈlʌɡɪdʒ/", Example = "Your luggage is beside the desk." },
                new Vocabulary { Id = 10004, LessonId = 10002, Word = "station", Meaning = "nhà ga", Pronunciation = "/ˈsteɪʃn/", Example = "The station is near the hotel." },
                new Vocabulary { Id = 10005, LessonId = 10002, Word = "direction", Meaning = "phương hướng", Pronunciation = "/dəˈrekʃn/", Example = "Could you give me directions?" },
                new Vocabulary { Id = 10006, LessonId = 10003, Word = "meeting", Meaning = "cuộc họp", Pronunciation = "/ˈmiːtɪŋ/", Example = "The meeting starts at nine." },
                new Vocabulary { Id = 10007, LessonId = 10003, Word = "deadline", Meaning = "hạn chót", Pronunciation = "/ˈdedlaɪn/", Example = "The deadline is Friday." },
                new Vocabulary { Id = 10008, LessonId = 10004, Word = "hello", Meaning = "xin chào", Pronunciation = "/həˈləʊ/", Example = "Hello, nice to meet you." });

            // Seed hai đề Practice; FullMock được hỗ trợ bằng validation nhưng không seed hàng trăm câu.
            modelBuilder.Entity<Test>().HasData(
                new Test { Id = 10001, LessonId = 10001, Title = "TOEIC-style Listening & Reading Practice", Description = "Bài luyện tập nội bộ có đủ Part và group dùng chung audio/passage.", DurationMinutes = 40, Format = TestFormat.ToeicStyle, Mode = TestMode.Practice, IsActive = true, CreatedAt = SeedCreatedAt },
                new Test { Id = 10002, LessonId = 10003, Title = "IELTS Academic Listening & Reading Practice", Description = "Bài luyện tập nội bộ có đủ Listening Part 1-4 và Reading Passage 1-3.", DurationMinutes = 40, Format = TestFormat.IeltsStyle, Mode = TestMode.Practice, IsActive = true, CreatedAt = SeedCreatedAt });

            // Seed riêng từng format để mã dễ đọc và dễ đếm acceptance criteria.
            ConfigureToeicQuestions(modelBuilder);
            ConfigureIeltsQuestions(modelBuilder);
        }

        // TOEIC Practice: P1=2, P2=2, P3=3, P4=3, P5=3, P6=4, P7=5; tổng 22 câu.
        private static void ConfigureToeicQuestions(ModelBuilder modelBuilder)
        {
            // Danh sách câu được tạo theo đúng Order hiển thị.
            List<Question> questions =
            [
                // Part 1 có hai ảnh và audio.
                ToeicQuestion(10101, 1, 1, "What is the person doing?", QuestionType.ListeningChoice, "Listening", "TOEIC-P1-Q01", imageUrl: "/images/toeic-part1.svg"),
                ToeicQuestion(10108, 2, 1, "Where is the suitcase?", QuestionType.ListeningChoice, "Listening", "TOEIC-P1-Q02", imageUrl: "/images/toeic-part1.svg"),

                // Part 2 có hai câu question-response.
                ToeicQuestion(10102, 3, 2, "Choose the best response to the recorded question.", QuestionType.ListeningChoice, "Listening", "TOEIC-P2-Q01"),
                ToeicQuestion(10109, 4, 2, "Choose the best response about the reservation.", QuestionType.ListeningChoice, "Listening", "TOEIC-P2-Q02"),

                // Part 3 có ba câu dùng chung một conversation/audio.
                ToeicQuestion(10103, 5, 3, "Where will the speakers meet?", QuestionType.ListeningChoice, "Listening", "TOEIC-P3-G01"),
                ToeicQuestion(10110, 6, 3, "What time will they meet?", QuestionType.ListeningChoice, "Listening", "TOEIC-P3-G01"),
                ToeicQuestion(10111, 7, 3, "What will the woman bring?", QuestionType.ListeningChoice, "Listening", "TOEIC-P3-G01"),

                // Part 4 có ba câu dùng chung một talk/audio.
                ToeicQuestion(10104, 8, 4, "What is the announcement mainly about?", QuestionType.ListeningChoice, "Listening", "TOEIC-P4-G01"),
                ToeicQuestion(10112, 9, 4, "When will the change begin?", QuestionType.ListeningChoice, "Listening", "TOEIC-P4-G01"),
                ToeicQuestion(10113, 10, 4, "What are listeners asked to do?", QuestionType.ListeningChoice, "Listening", "TOEIC-P4-G01"),

                // Part 5 luôn là MultipleChoice dù câu có dấu trống.
                ToeicQuestion(10105, 11, 5, "The manager _____ the report every Monday.", QuestionType.MultipleChoice, "Reading"),
                ToeicQuestion(10114, 12, 5, "Guests should _____ their keys at reception.", QuestionType.MultipleChoice, "Reading"),
                ToeicQuestion(10115, 13, 5, "The meeting was _____ until Friday.", QuestionType.MultipleChoice, "Reading"),

                // Part 6 có bốn câu dùng chung một email.
                ToeicQuestion(10106, 14, 6, "Choose the word that best completes blank 1.", QuestionType.MultipleChoice, "Reading", "TOEIC-P6-G01", contextText: "Thank you for your interest. Please complete the attached form before Friday. Our team will review it and reply next week."),
                ToeicQuestion(10116, 15, 6, "Choose the word that best completes blank 2.", QuestionType.MultipleChoice, "Reading", "TOEIC-P6-G01", contextText: "Thank you for your interest. Please complete the attached form before Friday. Our team will review it and reply next week."),
                ToeicQuestion(10117, 16, 6, "When should the form be returned?", QuestionType.MultipleChoice, "Reading", "TOEIC-P6-G01", contextText: "Thank you for your interest. Please complete the attached form before Friday. Our team will review it and reply next week."),
                ToeicQuestion(10118, 17, 6, "What will the team do next?", QuestionType.MultipleChoice, "Reading", "TOEIC-P6-G01", contextText: "Thank you for your interest. Please complete the attached form before Friday. Our team will review it and reply next week."),

                // Part 7 có năm câu dùng chung một notice.
                ToeicQuestion(10107, 18, 7, "When does the hotel serve breakfast?", QuestionType.MultipleChoice, "Reading", "TOEIC-P7-G01", contextText: "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception."),
                ToeicQuestion(10119, 19, 7, "Where is breakfast served?", QuestionType.MultipleChoice, "Reading", "TOEIC-P7-G01", contextText: "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception."),
                ToeicQuestion(10120, 20, 7, "What closes at 9:00 p.m.?", QuestionType.MultipleChoice, "Reading", "TOEIC-P7-G01", contextText: "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception."),
                ToeicQuestion(10121, 21, 7, "Where can guests request late checkout?", QuestionType.MultipleChoice, "Reading", "TOEIC-P7-G01", contextText: "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception."),
                ToeicQuestion(10122, 22, 7, "What type of text is this?", QuestionType.MultipleChoice, "Reading", "TOEIC-P7-G01", contextText: "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception.")
            ];

            // Đăng ký 22 câu TOEIC vào model seed.
            modelBuilder.Entity<Question>().HasData(questions);

            // Giữ ID đáp án cũ cho bảy câu ban đầu để migration không phá lịch sử hiện có.
            List<Answer> answers = ExistingToeicAnswers();

            // ID mới bắt đầu ở vùng riêng để không xung đột seed cũ.
            int nextAnswerId = 20301;

            // Thêm đáp án cho các câu TOEIC mới.
            AddChoiceAnswers(answers, ref nextAnswerId, 10108, 0, "Beside the reception desk", "Inside a taxi", "Under the bed");
            AddChoiceAnswers(answers, ref nextAnswerId, 10109, 0, "It is under your name.", "At the bus stop.", "Three copies, please.");
            AddChoiceAnswers(answers, ref nextAnswerId, 10110, 0, "At 9:00 a.m.", "At noon", "After dinner");
            AddChoiceAnswers(answers, ref nextAnswerId, 10111, 0, "The booking documents", "A bicycle", "A lunch menu");
            AddChoiceAnswers(answers, ref nextAnswerId, 10112, 0, "Next Monday", "Last month", "Tomorrow evening");
            AddChoiceAnswers(answers, ref nextAnswerId, 10113, 0, "Check the updated timetable", "Buy a new ticket", "Leave the station");
            AddChoiceAnswers(answers, ref nextAnswerId, 10114, 0, "return", "returns", "returned");
            AddChoiceAnswers(answers, ref nextAnswerId, 10115, 0, "postponed", "postpone", "postponing");
            AddChoiceAnswers(answers, ref nextAnswerId, 10116, 0, "review", "reviewed", "reviewing");
            AddChoiceAnswers(answers, ref nextAnswerId, 10117, 0, "Before Friday", "Next month", "After the meeting");
            AddChoiceAnswers(answers, ref nextAnswerId, 10118, 0, "Reply next week", "Cancel the form", "Call the hotel");
            AddChoiceAnswers(answers, ref nextAnswerId, 10119, 0, "In the first-floor restaurant", "Beside the pool", "At reception");
            AddChoiceAnswers(answers, ref nextAnswerId, 10120, 0, "The pool", "The restaurant", "The front desk");
            AddChoiceAnswers(answers, ref nextAnswerId, 10121, 0, "At reception", "In the restaurant", "Online only");
            AddChoiceAnswers(answers, ref nextAnswerId, 10122, 0, "A hotel notice", "A train ticket", "A job advertisement");

            // Đăng ký toàn bộ đáp án TOEIC.
            modelBuilder.Entity<Answer>().HasData(answers);
        }

        // IELTS Practice: Listening 5 câu và Reading 6 câu; tổng 11 câu.
        private static void ConfigureIeltsQuestions(ModelBuilder modelBuilder)
        {
            // Passage được lặp trong seed entity nhưng UI sẽ group và chỉ render một lần.
            const string passageOne = "The campus library now offers quiet rooms and online booking. Students can reserve a room for up to two hours.";
            const string passageTwo = "The city library opens Monday to Saturday. In the writer's view, remote access improves study flexibility. The notice gives no information about Sunday opening.";
            const string passageThree = "Solar panels capture sunlight and convert it into electricity. Paragraph A explains collection, while Paragraph B describes storage.";

            // Tạo đủ Listening Part 1-4 và Reading Passage 1-3.
            List<Question> questions =
            [
                // Listening Part 1 có completion và word-limit một từ.
                IeltsQuestion(10202, 1, 1, "The museum closes at ____ p.m.", QuestionType.ListeningFill, "Listening", "IELTS-L1-G01", instruction: "NO MORE THAN ONE WORD AND/OR A NUMBER.", maxWords: 1),

                // Listening Part 2 dùng lựa chọn.
                IeltsQuestion(10201, 2, 2, "What type of room does the caller request?", QuestionType.ListeningChoice, "Listening", "IELTS-L2-G01"),

                // Listening Part 3 biểu diễn matching thành hai câu lựa chọn chung recording.
                IeltsQuestion(10203, 3, 3, "Which subject is assigned to Student A?", QuestionType.ListeningChoice, "Listening", "IELTS-L3-G01", instruction: "Match each student with the correct subject."),
                IeltsQuestion(10208, 4, 3, "Which subject is assigned to Student B?", QuestionType.ListeningChoice, "Listening", "IELTS-L3-G01", instruction: "Match each student with the correct subject."),

                // Listening Part 4 có completion học thuật.
                IeltsQuestion(10204, 5, 4, "The lecture focuses on renewable ____.", QuestionType.ListeningFill, "Listening", "IELTS-L4-G01", instruction: "NO MORE THAN ONE WORD.", maxWords: 1),

                // Reading Passage 1 có hai câu MultipleChoice.
                IeltsQuestion(10209, 6, 1, "How long may a student reserve a quiet room?", QuestionType.MultipleChoice, "Reading", "IELTS-R1-G01", passageOne),
                IeltsQuestion(10210, 7, 1, "How are the rooms booked?", QuestionType.MultipleChoice, "Reading", "IELTS-R1-G01", passageOne),

                // Reading Passage 2 chứng minh cả TFNG và YNNG.
                IeltsQuestion(10205, 8, 2, "The library opens every Sunday.", QuestionType.TrueFalseNotGiven, "Reading", "IELTS-R2-G01", passageTwo, "Do the statements agree with the information in the passage?"),
                IeltsQuestion(10206, 9, 2, "The writer believes remote access improves study flexibility.", QuestionType.YesNoNotGiven, "Reading", "IELTS-R2-G01", passageTwo, "Do the statements agree with the views of the writer?"),

                // Reading Passage 3 có completion và matching dạng lựa chọn.
                IeltsQuestion(10207, 10, 3, "Solar panels convert sunlight into ____.", QuestionType.FillBlank, "Reading", "IELTS-R3-G01", passageThree, "NO MORE THAN ONE WORD.", 1),
                IeltsQuestion(10211, 11, 3, "Which paragraph describes energy storage?", QuestionType.MultipleChoice, "Reading", "IELTS-R3-G01", passageThree, "Choose paragraph A or B.")
            ];

            // Đăng ký 11 câu IELTS.
            modelBuilder.Entity<Question>().HasData(questions);

            // Giữ đáp án cũ và điều chỉnh mapping theo Part mới.
            List<Answer> answers = ExistingIeltsAnswers();

            // ID mới tách khỏi vùng đáp án TOEIC.
            int nextAnswerId = 20401;

            // Thêm đáp án cho matching và Reading MultipleChoice mới.
            AddChoiceAnswers(answers, ref nextAnswerId, 10208, 1, "Environmental science", "Modern history", "Business law");
            AddChoiceAnswers(answers, ref nextAnswerId, 10209, 1, "One hour", "Two hours", "A full day");
            AddChoiceAnswers(answers, ref nextAnswerId, 10210, 0, "Online", "By telephone only", "At the front desk only");
            AddChoiceAnswers(answers, ref nextAnswerId, 10211, 1, "Paragraph A", "Paragraph B", "Neither paragraph");

            // Đăng ký toàn bộ đáp án IELTS.
            modelBuilder.Entity<Answer>().HasData(answers);
        }

        // Tạo Question TOEIC và tự gán AudioUrl cho Listening Part 1-4.
        private static Question ToeicQuestion(int id, int order, int part, string content, QuestionType type, string section, string? groupCode = null, string? contextText = null, string? imageUrl = null)
        {
            // Trả entity seed không gắn navigation property.
            return new Question
            {
                Id = id,
                TestId = 10001,
                Content = content,
                QuestionType = type,
                SectionName = section,
                PartNumber = part,
                Order = order,
                GroupCode = groupCode,
                ContextText = contextText,
                ImageUrl = imageUrl,
                AudioUrl = section == "Listening" ? DemoAudioUrl : null,
                Instruction = "Choose the best answer."
            };
        }

        // Tạo Question IELTS với metadata completion/matching cần thiết.
        private static Question IeltsQuestion(int id, int order, int part, string content, QuestionType type, string section, string groupCode, string? contextText = null, string? instruction = null, int? maxWords = null)
        {
            // Trả entity seed và dùng cùng audio cho các câu trong một recording group.
            return new Question
            {
                Id = id,
                TestId = 10002,
                Content = content,
                QuestionType = type,
                SectionName = section,
                PartNumber = part,
                Order = order,
                GroupCode = groupCode,
                ContextText = contextText,
                AudioUrl = section == "Listening" ? DemoAudioUrl : null,
                Instruction = instruction,
                MaxWords = maxWords
            };
        }

        // Thêm ba option single-answer và cấp ID tăng dần ổn định cho migration.
        private static void AddChoiceAnswers(List<Answer> answers, ref int nextId, int questionId, int correctIndex, params string[] options)
        {
            // Mỗi option trở thành một Answer; chỉ index được chỉ định nhận IsCorrect=true.
            for (int index = 0; index < options.Length; index++)
            {
                answers.Add(new Answer { Id = nextId++, QuestionId = questionId, Content = options[index], IsCorrect = index == correctIndex });
            }
        }

        // Các đáp án ID 20101-20120 được giữ để bảo toàn dữ liệu đã migrate trước đó.
        private static List<Answer> ExistingToeicAnswers()
        {
            // Câu 10106 được chuẩn hóa thành MultipleChoice nên chỉ còn một đáp án đúng.
            return
            [
                new Answer { Id = 20101, QuestionId = 10101, Content = "She is checking in at a hotel.", IsCorrect = true },
                new Answer { Id = 20102, QuestionId = 10101, Content = "She is driving a bus.", IsCorrect = false },
                new Answer { Id = 20103, QuestionId = 10101, Content = "She is cooking dinner.", IsCorrect = false },
                new Answer { Id = 20104, QuestionId = 10102, Content = "At three o'clock.", IsCorrect = true },
                new Answer { Id = 20105, QuestionId = 10102, Content = "Yes, I can see it.", IsCorrect = false },
                new Answer { Id = 20106, QuestionId = 10102, Content = "For two people.", IsCorrect = false },
                new Answer { Id = 20107, QuestionId = 10103, Content = "At the station.", IsCorrect = true },
                new Answer { Id = 20108, QuestionId = 10103, Content = "Next Monday.", IsCorrect = false },
                new Answer { Id = 20109, QuestionId = 10103, Content = "By email.", IsCorrect = false },
                new Answer { Id = 20110, QuestionId = 10104, Content = "A change to the train schedule.", IsCorrect = true },
                new Answer { Id = 20111, QuestionId = 10104, Content = "A restaurant opening.", IsCorrect = false },
                new Answer { Id = 20112, QuestionId = 10104, Content = "A job interview.", IsCorrect = false },
                new Answer { Id = 20113, QuestionId = 10105, Content = "reviews", IsCorrect = true },
                new Answer { Id = 20114, QuestionId = 10105, Content = "review", IsCorrect = false },
                new Answer { Id = 20115, QuestionId = 10105, Content = "reviewing", IsCorrect = false },
                new Answer { Id = 20116, QuestionId = 10106, Content = "complete", IsCorrect = true },
                new Answer { Id = 20117, QuestionId = 10106, Content = "completed", IsCorrect = false },
                new Answer { Id = 20118, QuestionId = 10107, Content = "From 6:30 a.m. to 10:00 a.m.", IsCorrect = true },
                new Answer { Id = 20119, QuestionId = 10107, Content = "At noon.", IsCorrect = false },
                new Answer { Id = 20120, QuestionId = 10107, Content = "Only on weekends.", IsCorrect = false },
                new Answer { Id = 20121, QuestionId = 10106, Content = "completion", IsCorrect = false }
            ];
        }

        // Các đáp án ID 20201-20216 được giữ, trong đó completion có AnswerId nullable khi user nộp mới.
        private static List<Answer> ExistingIeltsAnswers()
        {
            // TFNG và YNNG giữ đúng ba option cố định.
            return
            [
                new Answer { Id = 20201, QuestionId = 10201, Content = "A single room", IsCorrect = true },
                new Answer { Id = 20202, QuestionId = 10201, Content = "A conference room", IsCorrect = false },
                new Answer { Id = 20203, QuestionId = 10201, Content = "A family suite", IsCorrect = false },
                new Answer { Id = 20204, QuestionId = 10202, Content = "five", IsCorrect = true },
                new Answer { Id = 20205, QuestionId = 10202, Content = "5", IsCorrect = true },
                new Answer { Id = 20206, QuestionId = 10203, Content = "Environmental science", IsCorrect = true },
                new Answer { Id = 20207, QuestionId = 10203, Content = "Modern history", IsCorrect = false },
                new Answer { Id = 20208, QuestionId = 10203, Content = "Business law", IsCorrect = false },
                new Answer { Id = 20209, QuestionId = 10204, Content = "energy", IsCorrect = true },
                new Answer { Id = 20210, QuestionId = 10205, Content = "TRUE", IsCorrect = false },
                new Answer { Id = 20211, QuestionId = 10205, Content = "FALSE", IsCorrect = false },
                new Answer { Id = 20212, QuestionId = 10205, Content = "NOT GIVEN", IsCorrect = true },
                new Answer { Id = 20213, QuestionId = 10206, Content = "YES", IsCorrect = true },
                new Answer { Id = 20214, QuestionId = 10206, Content = "NO", IsCorrect = false },
                new Answer { Id = 20215, QuestionId = 10206, Content = "NOT GIVEN", IsCorrect = false },
                new Answer { Id = 20216, QuestionId = 10207, Content = "electricity", IsCorrect = true }
            ];
        }
    }
}
