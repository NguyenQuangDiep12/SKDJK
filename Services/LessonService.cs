using Microsoft.EntityFrameworkCore;
using SKDJK.Data;
using SKDJK.Dtos;
using SKDJK.Models;
using SKDJK.Models.commons;
using SKDJK.Models.enums;
using SKDJK.Services.Interfaces;

namespace SKDJK.Services
{
    // Xử lý trang học, tiến độ và CRUD Lesson/Vocabulary bằng các truy vấn đơn giản.
    public sealed class LessonService : ILessonService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IFreeDictionaryService _freeDictionaryService;

        public LessonService(ApplicationDbContext dbContext, IFreeDictionaryService freeDictionaryService)
        {
            _dbContext = dbContext;
            _freeDictionaryService = freeDictionaryService;
        }

        public async Task<Result<MyLessonPageDto>> GetMyLessonsAsync(int userId, CancellationToken cancellationToken = default)
        {
            // Xác nhận UserId lấy từ claim vẫn tồn tại trước khi truy vấn tiến độ.
            bool userExists = await _dbContext.Users.AsNoTracking().AnyAsync(x => x.Id == userId, cancellationToken);

            // Không trả dữ liệu của tài khoản không tồn tại.
            if (!userExists)
            {
                return Result<MyLessonPageDto>.Failure(new Error("User.NotFound", "Không tìm thấy người học."));
            }

            // Chỉ lấy một bài có lần học gần nhất, bất kể đang dở hay đã hoàn thành.
            MyLessonItemDto? latestLesson = await _dbContext.LearningProgress
                .AsNoTracking()
                .Where(progress => progress.UserId == userId && progress.Status != LearningStatus.NOTSTARTED)
                .OrderByDescending(progress => progress.LastStudyAt)
                .ThenByDescending(progress => progress.Id)
                .Select(progress => new MyLessonItemDto
                {
                    LessonId = progress.LessonId,
                    LessonTitle = progress.Lesson.Title,
                    Description = progress.Lesson.Description,
                    TopicName = progress.Lesson.Topic.Name,
                    LanguageName = progress.Lesson.Topic.Language.Name,
                    Status = progress.Status,
                    CompletionPercent = progress.CompletionPercent,
                    LastStudyAt = progress.LastStudyAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            // DTO vẫn hợp lệ khi LatestLesson null để View hiển thị trạng thái chưa có bài học.
            MyLessonPageDto dto = new()
            {
                LatestLesson = latestLesson
            };

            // Trả duy nhất bài gần nhất hoặc null cho Controller.
            return Result<MyLessonPageDto>.Success(dto);
        }

        public async Task<Result<LessonStudyDto>> GetStudyAsync(int lessonId, int userId, CancellationToken cancellationToken = default)
        {
            bool userExists = await _dbContext.Users.AsNoTracking().AnyAsync(x => x.Id == userId, cancellationToken);
            if (!userExists)
            {
                return Result<LessonStudyDto>.Failure(new Error("User.NotFound", "Không tìm thấy người học."));
            }

            Lesson? lesson = await _dbContext.Lessons
                .AsNoTracking()
                .Include(x => x.Topic)
                .Include(x => x.Vocabularies)
                .FirstOrDefaultAsync(x => x.Id == lessonId, cancellationToken);

            if (lesson is null)
            {
                return Result<LessonStudyDto>.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
            }

            LearningProgress? progress = await _dbContext.LearningProgress
                .FirstOrDefaultAsync(x => x.UserId == userId && x.LessonId == lessonId, cancellationToken);

            if (progress is null)
            {
                progress = new LearningProgress
                {
                    UserId = userId,
                    LessonId = lessonId,
                    Status = LearningStatus.INPROGRESS,
                    CompletionPercent = 10,
                    LastStudyAt = DateTime.UtcNow
                };
                _dbContext.LearningProgress.Add(progress);
            }
            else if (progress.Status != LearningStatus.COMPLETED)
            {
                progress.Status = LearningStatus.INPROGRESS;
                progress.CompletionPercent = Math.Max(progress.CompletionPercent, 10);
                progress.LastStudyAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            List<int> lessonIds = await _dbContext.Lessons
                .AsNoTracking()
                .Where(x => x.TopicId == lesson.TopicId)
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
            int currentIndex = lessonIds.IndexOf(lessonId);

            // Ánh xạ Vocabulary ngay trong GetStudyAsync để Service không cần hàm private.
            VocabularyLearningDto vocabulary = new()
            {
                LessonId = lesson.Id,
                LessonTitle = lesson.Title,
                Vocabularies = lesson.Vocabularies
                    .OrderBy(x => x.Id)
                    .Select(x => new VocabularyItemDto
                    {
                        VocabularyId = x.Id,
                        Word = x.Word,
                        Meaning = x.Meaning ?? string.Empty,
                        Pronunciation = x.Pronunciation,
                        Example = x.Example,
                        AudioUrl = x.AudioUrl
                    })
                    .ToList()
            };

            // Tải câu Listening ngay trong GetStudyAsync thay vì gọi QueryListeningAsync private.
            List<Question> listeningQuestions = await _dbContext.Questions
                .AsNoTracking()
                .Include(question => question.Answers)
                .Where(question => question.Test.LessonId == lessonId
                    && (question.QuestionType == QuestionType.ListeningChoice
                        || question.QuestionType == QuestionType.ListeningFill))
                .OrderBy(question => question.Order)
                .ToListAsync(cancellationToken);

            // DTO Listening không chứa IsCorrect và câu text không chứa đáp án đúng.
            ListeningLearningDto listening = new()
            {
                LessonId = lessonId,
                LessonTitle = lesson.Title,
                Questions = listeningQuestions.Select(question => new ListeningQuestionDto
                {
                    QuestionId = question.Id,
                    Content = question.Content,
                    AudioUrl = question.AudioUrl,
                    ImageUrl = question.ImageUrl,
                    PartNumber = question.PartNumber,
                    Answers = question.QuestionType == QuestionType.ListeningChoice
                        ? question.Answers
                            .OrderBy(answer => answer.Id)
                            .Select(answer => new ListeningAnswerDto { AnswerId = answer.Id, Content = answer.Content })
                            .ToList()
                        : []
                }).ToList()
            };

            LessonStudyDto dto = new()
            {
                LessonId = lesson.Id,
                LessonTitle = lesson.Title,
                Description = lesson.Description,
                TopicName = lesson.Topic.Name,
                CompletionPercent = progress.CompletionPercent,
                LearningStatus = progress.Status,
                PreviousLessonId = currentIndex > 0 ? lessonIds[currentIndex - 1] : null,
                NextLessonId = currentIndex >= 0 && currentIndex < lessonIds.Count - 1 ? lessonIds[currentIndex + 1] : null,
                Vocabulary = vocabulary,
                Grammar = new GrammarLearningDto { LessonId = lesson.Id, LessonTitle = lesson.Title, Content = lesson.Content },
                Listening = listening
            };

            return Result<LessonStudyDto>.Success(dto);
        }

        public async Task<Result<VocabularyLearningDto>> GetVocabularyAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            Lesson? lesson = await _dbContext.Lessons
                .AsNoTracking()
                .Include(x => x.Vocabularies)
                .FirstOrDefaultAsync(x => x.Id == lessonId, cancellationToken);

            if (lesson is null)
            {
                return Result<VocabularyLearningDto>.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
            }

            // Ánh xạ Entity thành DTO trực tiếp trong hàm công khai.
            VocabularyLearningDto dto = new()
            {
                LessonId = lesson.Id,
                LessonTitle = lesson.Title,
                Vocabularies = lesson.Vocabularies
                    .OrderBy(x => x.Id)
                    .Select(x => new VocabularyItemDto
                    {
                        VocabularyId = x.Id,
                        Word = x.Word,
                        Meaning = x.Meaning ?? string.Empty,
                        Pronunciation = x.Pronunciation,
                        Example = x.Example,
                        AudioUrl = x.AudioUrl
                    })
                    .ToList()
            };

            return Result<VocabularyLearningDto>.Success(dto);
        }

        public async Task<Result<GrammarLearningDto>> GetGrammarAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            GrammarLearningDto? dto = await _dbContext.Lessons
                .AsNoTracking()
                .Where(x => x.Id == lessonId)
                .Select(x => new GrammarLearningDto { LessonId = x.Id, LessonTitle = x.Title, Content = x.Content })
                .FirstOrDefaultAsync(cancellationToken);

            return dto is null
                ? Result<GrammarLearningDto>.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."))
                : Result<GrammarLearningDto>.Success(dto);
        }

        public async Task<Result<ListeningLearningDto>> GetListeningAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            string? title = await _dbContext.Lessons.AsNoTracking().Where(x => x.Id == lessonId).Select(x => x.Title).FirstOrDefaultAsync(cancellationToken);
            if (title is null)
            {
                return Result<ListeningLearningDto>.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
            }

            // Truy vấn Listening được viết thẳng trong GetListeningAsync.
            List<Question> questions = await _dbContext.Questions
                .AsNoTracking()
                .Include(question => question.Answers)
                .Where(question => question.Test.LessonId == lessonId
                    && (question.QuestionType == QuestionType.ListeningChoice
                        || question.QuestionType == QuestionType.ListeningFill))
                .OrderBy(question => question.Order)
                .ToListAsync(cancellationToken);

            // Tạo DTO an toàn, không gửi IsCorrect xuống Controller/View.
            ListeningLearningDto dto = new()
            {
                LessonId = lessonId,
                LessonTitle = title,
                Questions = questions.Select(question => new ListeningQuestionDto
                {
                    QuestionId = question.Id,
                    Content = question.Content,
                    AudioUrl = question.AudioUrl,
                    ImageUrl = question.ImageUrl,
                    PartNumber = question.PartNumber,
                    Answers = question.QuestionType == QuestionType.ListeningChoice
                        ? question.Answers
                            .OrderBy(answer => answer.Id)
                            .Select(answer => new ListeningAnswerDto { AnswerId = answer.Id, Content = answer.Content })
                            .ToList()
                        : []
                }).ToList()
            };

            return Result<ListeningLearningDto>.Success(dto);
        }

        public async Task<Result> CompleteAsync(int lessonId, int userId, CancellationToken cancellationToken = default)
        {
            bool lessonExists = await _dbContext.Lessons.AsNoTracking().AnyAsync(x => x.Id == lessonId, cancellationToken);
            if (!lessonExists)
            {
                return Result.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
            }

            LearningProgress? progress = await _dbContext.LearningProgress
                .FirstOrDefaultAsync(x => x.UserId == userId && x.LessonId == lessonId, cancellationToken);

            if (progress is null)
            {
                progress = new LearningProgress { UserId = userId, LessonId = lessonId };
                _dbContext.LearningProgress.Add(progress);
            }

            progress.Status = LearningStatus.COMPLETED;
            progress.CompletionPercent = 100;
            progress.LastStudyAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<AdminLessonPageDto>> GetAdminPageAsync(int? selectedLessonId, CancellationToken cancellationToken = default)
        {
            List<AdminLessonItemDto> lessons = await _dbContext.Lessons
                .AsNoTracking()
                .OrderBy(x => x.Topic.Name)
                .ThenBy(x => x.Title)
                .Select(x => new AdminLessonItemDto { Id = x.Id, Title = x.Title, TopicName = x.Topic.Name })
                .ToListAsync(cancellationToken);

            int? actualId = selectedLessonId ?? lessons.FirstOrDefault()?.Id;
            AdminLessonItemDto? selected = lessons.FirstOrDefault(x => x.Id == actualId);
            List<AdminVocabularyItemDto> vocabularies = selected is null
                ? []
                : await _dbContext.Vocabularies
                    .AsNoTracking()
                    .Where(x => x.LessonId == selected.Id)
                    .OrderBy(x => x.Word)
                    .Select(x => new AdminVocabularyItemDto { Id = x.Id, LessonId = x.LessonId, Word = x.Word, Meaning = x.Meaning, Pronunciation = x.Pronunciation })
                    .ToListAsync(cancellationToken);

            return Result<AdminLessonPageDto>.Success(new AdminLessonPageDto
            {
                SelectedLessonId = actualId,
                SelectedLesson = selected,
                Lessons = lessons,
                Vocabularies = vocabularies
            });
        }

        public async Task<Result<AdminLessonFormDto>> GetLessonFormAsync(int? id, CancellationToken cancellationToken = default)
        {
            AdminLessonFormDto dto;
            if (id.HasValue)
            {
                dto = await _dbContext.Lessons
                    .AsNoTracking()
                    .Where(x => x.Id == id.Value)
                    .Select(x => new AdminLessonFormDto { Id = x.Id, TopicId = x.TopicId, Title = x.Title, Description = x.Description, Content = x.Content })
                    .FirstOrDefaultAsync(cancellationToken) ?? null!;

                if (dto is null)
                {
                    return Result<AdminLessonFormDto>.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
                }
            }
            else
            {
                dto = new AdminLessonFormDto();
            }

            // Options Topic được tải trực tiếp trong hàm, không gọi helper private.
            dto.Topics = await _dbContext.Topics
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new AdminTopicOptionDto { Id = x.Id, Name = $"{x.Name} - {x.Language.Name}" })
                .ToListAsync(cancellationToken);

            return Result<AdminLessonFormDto>.Success(dto);
        }

        public async Task<Result<int>> SaveLessonAsync(AdminLessonFormDto request, CancellationToken cancellationToken = default)
        {
            bool topicExists = await _dbContext.Topics.AsNoTracking().AnyAsync(x => x.Id == request.TopicId, cancellationToken);
            if (!topicExists)
            {
                return Result<int>.Failure(new Error("Lesson.InvalidTopic", "Chủ đề được chọn không tồn tại."));
            }

            Lesson lesson;
            if (request.Id.HasValue)
            {
                lesson = await _dbContext.Lessons.FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken) ?? null!;
                if (lesson is null)
                {
                    return Result<int>.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
                }
            }
            else
            {
                lesson = new Lesson();
                _dbContext.Lessons.Add(lesson);
            }

            lesson.TopicId = request.TopicId;
            lesson.Title = request.Title.Trim();
            lesson.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            lesson.Content = request.Content.Trim();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(lesson.Id);
        }

        public async Task<Result> DeleteLessonAsync(int id, CancellationToken cancellationToken = default)
        {
            Lesson? lesson = await _dbContext.Lessons
                .Include(x => x.Tests)
                .Include(x => x.LearningProgresses)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (lesson is null)
            {
                return Result.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
            }

            if (lesson.Tests.Count > 0 || lesson.LearningProgresses.Count > 0)
            {
                return Result.Failure(new Error("Lesson.InUse", "Không thể xóa bài học đã có bài kiểm tra hoặc tiến độ."));
            }

            List<Vocabulary> vocabularies = await _dbContext.Vocabularies.Where(x => x.LessonId == id).ToListAsync(cancellationToken);
            _dbContext.Vocabularies.RemoveRange(vocabularies);
            _dbContext.Lessons.Remove(lesson);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<AdminVocabularyFormDto>> GetVocabularyFormAsync(int lessonId, int? id, CancellationToken cancellationToken = default)
        {
            bool lessonExists = await _dbContext.Lessons.AsNoTracking().AnyAsync(x => x.Id == lessonId, cancellationToken);
            if (!lessonExists)
            {
                return Result<AdminVocabularyFormDto>.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
            }

            if (!id.HasValue)
            {
                return Result<AdminVocabularyFormDto>.Success(new AdminVocabularyFormDto { LessonId = lessonId });
            }

            AdminVocabularyFormDto? dto = await _dbContext.Vocabularies
                .AsNoTracking()
                .Where(x => x.Id == id.Value && x.LessonId == lessonId)
                .Select(x => new AdminVocabularyFormDto
                {
                    Id = x.Id,
                    LessonId = x.LessonId,
                    Word = x.Word,
                    Meaning = x.Meaning,
                    Pronunciation = x.Pronunciation,
                    Example = x.Example,
                    AudioUrl = x.AudioUrl
                })
                .FirstOrDefaultAsync(cancellationToken);

            return dto is null
                ? Result<AdminVocabularyFormDto>.Failure(new Error("Vocabulary.NotFound", "Không tìm thấy từ vựng."))
                : Result<AdminVocabularyFormDto>.Success(dto);
        }

        public async Task<Result> SaveVocabularyAsync(AdminVocabularyFormDto request, CancellationToken cancellationToken = default)
        {
            bool lessonExists = await _dbContext.Lessons.AsNoTracking().AnyAsync(x => x.Id == request.LessonId, cancellationToken);
            if (!lessonExists)
            {
                return Result.Failure(new Error("Lesson.NotFound", "Không tìm thấy bài học."));
            }

            // Tra đúng từ admin nhập để AudioUrl luôn có nguồn từ Free Dictionary, không lấy URL từ form.
            string normalizedWord = request.Word.Trim();
            var pronunciationResult = await _freeDictionaryService.GetPronunciationAsync(normalizedWord, cancellationToken);
            if (!pronunciationResult.IsSuccess)
            {
                return Result.Failure(pronunciationResult.Error);
            }

            Vocabulary vocabulary;
            if (request.Id.HasValue)
            {
                vocabulary = await _dbContext.Vocabularies.FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.LessonId == request.LessonId, cancellationToken) ?? null!;
                if (vocabulary is null)
                {
                    return Result.Failure(new Error("Vocabulary.NotFound", "Không tìm thấy từ vựng."));
                }
            }
            else
            {
                vocabulary = new Vocabulary { LessonId = request.LessonId };
                _dbContext.Vocabularies.Add(vocabulary);
            }

            vocabulary.Word = normalizedWord;
            vocabulary.Meaning = string.IsNullOrWhiteSpace(request.Meaning) ? null : request.Meaning.Trim();

            // Ưu tiên phiên âm Free Dictionary; chỉ dùng giá trị admin nhập khi API không có phiên âm.
            vocabulary.Pronunciation = string.IsNullOrWhiteSpace(pronunciationResult.Value.Phonetic)
                ? (string.IsNullOrWhiteSpace(request.Pronunciation) ? null : request.Pronunciation.Trim())
                : pronunciationResult.Value.Phonetic.Trim();
            vocabulary.Example = string.IsNullOrWhiteSpace(request.Example) ? null : request.Example.Trim();

            // Chỉ lưu URL audio HTTPS do Free Dictionary trả về.
            vocabulary.AudioUrl = pronunciationResult.Value.AudioUrl;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteVocabularyAsync(int id, CancellationToken cancellationToken = default)
        {
            Vocabulary? vocabulary = await _dbContext.Vocabularies.Include(x => x.PronunciationResults).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (vocabulary is null)
            {
                return Result.Failure(new Error("Vocabulary.NotFound", "Không tìm thấy từ vựng."));
            }

            if (vocabulary.PronunciationResults.Count > 0)
            {
                return Result.Failure(new Error("Vocabulary.InUse", "Không thể xóa từ vựng đang có dữ liệu lịch sử."));
            }

            _dbContext.Vocabularies.Remove(vocabulary);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

    }
}
