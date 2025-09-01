using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Specific;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Service.Implementation
{
    // ZRIHVAT SVE OVE EDITION => HOST => UnauthorizedHostOrEditionHappened
    public class UpcomingQuizQuestionService : IUpcomingQuizQuestionService
    {
        private readonly IUpcomingQuizQuestionRepository _repository;
        private readonly MediaServerClient _mediaServerClient;

        public UpcomingQuizQuestionService(IUpcomingQuizQuestionRepository repository, MediaServerClient mediaServerClient)
        {
            _repository = repository;
            _mediaServerClient = mediaServerClient;
        }

        public async Task<QuizQuestionDto> AddQuestion(QuizQuestionDto questionDto, int userId, IFormFile? file)
        {
            var edition = await _repository.EditionFromSegment(questionDto.SegmentId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            if (questionDto.Type is QuestionType.IMAGE or QuestionType.AUDIO or QuestionType.VIDEO)
            {
                if (file == null || file.Length == 0)
                    throw new BadRequestException("A media file is required for this question type.");

                var maxSizeBytes = questionDto.Type switch
                {
                    QuestionType.IMAGE => 10 * 1024 * 1024,
                    QuestionType.AUDIO => 20 * 1024 * 1024,
                    QuestionType.VIDEO => 100 * 1024 * 1024,
                    _ => throw new BadRequestException("Unsupported media type.")
                };

                if (file.Length > maxSizeBytes)
                    throw new BadRequestException($"File too large. Max allowed for {questionDto.Type} is {maxSizeBytes / (1024 * 1024)} MB.");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var contentType = file.ContentType.ToLowerInvariant();

                bool isValid = questionDto.Type switch
                {
                    QuestionType.IMAGE => contentType.StartsWith("image/")
                                          && (extension is ".jpg" or ".jpeg" or ".png"),
                    QuestionType.AUDIO => contentType == "audio/mpeg"
                                          && extension == ".mp3",
                    QuestionType.VIDEO => contentType == "video/mp4"
                                          && extension == ".mp4",
                    _ => false
                };

                if (!isValid)
                    throw new BadRequestException($"Invalid file type or extension for {questionDto.Type}. Only .jpg, .jpeg, .png, .mp3, and .mp4 are allowed.");

                string relativeUrl = $"/question/{questionDto.Type.ToString().ToLower()}/{edition.Id}";

                await _repository.SetQuestionNumberAndId(questionDto);
                var question = await _repository.AddQuestion(questionDto);

                try
                {
                    question.MediaUrl = $"{questionDto.Type.ToString().ToLower()}/{edition.Id}/" +
                        questionDto.Type switch
                        {
                            QuestionType.IMAGE => await _mediaServerClient.PostImage(relativeUrl, file, $"{question.Id}{Path.GetExtension(file.FileName)}"),
                            QuestionType.AUDIO => await _mediaServerClient.PostAudio(relativeUrl, file, $"{question.Id}{Path.GetExtension(file.FileName)}"),
                            QuestionType.VIDEO => await _mediaServerClient.PostVideo(relativeUrl, file, $"{question.Id}{Path.GetExtension(file.FileName)}"),
                            _ => null
                        };

                    question = await _repository.EditQuestion(new(question));
                }
                catch
                {
                    await _repository.DeleteQuestion(question.Id);

                    throw new BadRequestException("Media could not be saved!");
                }

                return new(question);
            }

            await _repository.SetQuestionNumberAndId(questionDto);
            return new (await _repository.AddQuestion(questionDto));
        }


        public async Task<QuizRoundDto> AddRound(QuizRoundDto roundDto, int userId)
        {
            var edition = await _repository.GetEdition(roundDto.EditionId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.SetRoundNumberAndId(roundDto);

            return new(await _repository.AddRound(roundDto));
        }

        public async Task<QuizSegmentDto> AddSegment(QuizSegmentDto segmentDto, int userId)
        {
            var edition = await _repository.EditionFromRound(segmentDto.RoundId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.SetSegmentNumberAndId(segmentDto);

            return new(await _repository.AddSegment(segmentDto));
        }

        public async Task DeleteQuestion(int questionId, int userId)
        {
            var edition = await _repository.EditionFromQuestion(questionId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.DeleteQuestion(questionId);
        }

        public async Task DeleteRound(int roundId, int userId)
        {
            var edition = await _repository.EditionFromRound(roundId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.DeleteRound(roundId, edition);
        }

        public async Task DeleteSegment(int segmentId, int userId)
        {
            var edition = await _repository.EditionFromSegment(segmentId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.DeleteSegment(segmentId);
        }

        public async Task<bool> DoesEditionHaveQuestions(int editionId, int userId)
        {
            var edition = await _repository.GetEditionWithQuestions(editionId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            foreach (var round in edition.QuizRounds)
            {
                if (round.QuizSegments.Count == 0)
                    return false;

                foreach (var segment in round.QuizSegments)
                    if (segment.QuizQuestions.Count == 0)
                        return false;
            }

            return true;
        }

        public async Task<QuizQuestionDto> EditQuestion(QuizQuestionDto questionDto, int userId, IFormFile? file)
        {
            var edition = await _repository.EditionFromSegment(questionDto.SegmentId);
            var host = await _repository.GetHost(edition.QuizId, userId);
            var question = await _repository.GetQuestion(questionDto.Id);

            UnauthorizedHostOrEditionHappened(host, edition);

            if ((questionDto.Type is QuestionType.IMAGE or QuestionType.AUDIO or QuestionType.VIDEO) && question.Type != (int)questionDto.Type)
            {
                if (file == null || file.Length == 0)
                    throw new BadRequestException("A media file is required for this question type.");

                var maxSizeBytes = questionDto.Type switch
                {
                    QuestionType.IMAGE => 10 * 1024 * 1024,
                    QuestionType.AUDIO => 20 * 1024 * 1024,
                    QuestionType.VIDEO => 100 * 1024 * 1024,
                    _ => throw new BadRequestException("Unsupported media type.")
                };

                if (file.Length > maxSizeBytes)
                    throw new BadRequestException($"File too large. Max allowed for {questionDto.Type} is {maxSizeBytes / (1024 * 1024)} MB.");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var contentType = file.ContentType.ToLowerInvariant();

                bool isValid = questionDto.Type switch
                {
                    QuestionType.IMAGE => contentType.StartsWith("image/")
                                          && (extension is ".jpg" or ".jpeg" or ".png"),
                    QuestionType.AUDIO => contentType == "audio/mpeg"
                                          && extension == ".mp3",
                    QuestionType.VIDEO => contentType == "video/mp4"
                                          && extension == ".mp4",
                    _ => false
                };

                if (!isValid)
                    throw new BadRequestException($"Invalid file type or extension for {questionDto.Type}. Only .jpg, .jpeg, .png, .mp3, and .mp4 are allowed.");

                string relativeUrl = $"/question/{questionDto.Type.ToString().ToLower()}/{edition.Id}";

                try
                {
                    questionDto.MediaUrl = $"{questionDto.Type.ToString().ToLower()}/{edition.Id}/" +
                        questionDto.Type switch
                        {
                            QuestionType.IMAGE => await _mediaServerClient.PostImage(relativeUrl, file, $"{questionDto.Id}{Path.GetExtension(file.FileName)}"),
                            QuestionType.AUDIO => await _mediaServerClient.PostAudio(relativeUrl, file, $"{questionDto.Id}{Path.GetExtension(file.FileName)}"),
                            QuestionType.VIDEO => await _mediaServerClient.PostVideo(relativeUrl, file, $"{questionDto.Id}{Path.GetExtension(file.FileName)}"),
                            _ => null
                        };

                    _ = await _repository.EditQuestion(questionDto);
                }
                catch
                {
                    throw new BadRequestException("Media could not be saved!");
                }

                return questionDto;
            }

            return new(await _repository.EditQuestion(questionDto));
        }

        public async Task<QuizRoundBriefDto> EditRound(QuizRoundBriefDto roundDto, int userId)
        {
            var edition = await _repository.EditionFromRound(roundDto.Id);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.EditRound(roundDto));
        }

        public async Task<QuizSegmentDto> EditSegment(QuizSegmentDto segmentDto, int userId)
        {
            var edition = await _repository.EditionFromRound(segmentDto.RoundId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.EditSegment(segmentDto));
        }

        public async Task<QuizQuestionDto> GetQuestion(int questionId, int userId)
        {
            var edition = await _repository.EditionFromQuestion(questionId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.GetQuestion(questionId));
        }

        public async Task<QuizRoundDto> GetRound(int roundId, int userId)
        {
            var edition = await _repository.EditionFromRound(roundId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.GetRound(roundId));
        }

        public async Task<IEnumerable<QuizRoundDto>> GetRounds(int editionId, int userId, bool detailed = false)
        {
            var edition = await _repository.GetEdition(editionId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            var rounds = await _repository.GetRounds(edition.Id, detailed);

            return rounds.Select(x => new QuizRoundDto(x)).ToList();
        }

        public async Task<QuizSegmentDto> GetSegment(int segmentId, int userId)
        {
            var edition = await _repository.EditionFromSegment(segmentId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.GetSegment(segmentId));
        }

        public async Task<QuizSegmentDto> UpdateQuestionOrder(UpdateOrderDto orderDto, int userId)
        {
            var edition = await _repository.EditionFromQuestion(orderDto.Id);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.UpdateQuestionOrder(orderDto, edition));
        }

        public async Task<List<QuizRoundDto>> UpdateRoundOrder(UpdateOrderDto orderDto, int userId)
        {
            var edition = await _repository.EditionFromRound(orderDto.Id);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            var rounds = await _repository.UpdateRoundOrder(orderDto);

            return rounds.Select(x => new QuizRoundDto(x)).ToList();
        }

        public async Task<QuizRoundDto> UpdateSegmentOrder(UpdateOrderDto orderDto, int userId)
        {
            var edition = await _repository.EditionFromSegment(orderDto.Id);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.UpdateSegmentOrder(orderDto, edition));
        }

        private void UnauthorizedHostOrEditionHappened(HostOrganizationQuiz host, QuizEdition edition)
        {
            if (!host.CrudQuestion)
                throw new ForbiddenException();

            //moze editat i nakon sto se desi!?
            //_repository.EditionHappened(edition);
        }
    }
}
