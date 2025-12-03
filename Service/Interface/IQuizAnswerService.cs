using PubQuizBackend.Model.Dto.QuizAnswerDto;
using static PubQuizBackend.Controllers.QuizAnswerController;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizAnswerService
    {
        Task<QuizRoundResultDetailedDto> GradeTeamAnswers(NewQuizRoundResultDto roundDto, int hostId);
        Task<QuizRoundResultDetailedDto> GradeExistingTeamAnswers(QuizRoundResultDetailedDto roundDto, int hostId);
        Task<QuizAnswerDetailedDto> UpdateAnswer(QuizAnswerDetailedDto answerDto, int hostId);
        Task<QuizSegmentResultDetailedDto> UpdateSegment(QuizSegmentResultDetailedDto segmentResultDto, int hostId);
        Task<QuizRoundResultMinimalDto> AddTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId);
        Task<QuizRoundResultDetailedDto> AddTeamRoundPointsDetailed(NewQuizRoundResultDto roundResultDto, int hostId);
        Task<QuizRoundResultMinimalDto> UpdateTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId);
        Task<QuizRoundResultDetailedDto> UpdateTeamRoundPointsDetailed(QuizRoundResultDetailedDto roundResultDto, int hostId);
        Task<IEnumerable<QuizRoundResultDetailedDto>> GetTeamAnswers(int editionResultId, int hostId);
        Task<IEnumerable<QuizEditionResultBriefDto>> GetEditionResults(int editionId, int hostId);
        Task<IEnumerable<QuizEditionResultDetailedDto>> GetEditionResultsDetailed(int editionId, int hostId);
        Task<IEnumerable<QuizEditionResultBriefDto>> RankTeamsOnEdition(int editionId, int hostId);
        Task<IEnumerable<QuizEditionResultBriefDto>> BreakTie(int promotedId, int editionId, int hostId);
        Task<QuizRoundResultDetailedDto> AutogradeAnswers(QuizRoundResultDetailedDto roundResult, List<PredictedAnswers> answers);
        Task<bool> IsDetailedResult(int roundResultId);
        Task DeleteRoundResultSegments(int roundResultId, int hostId);
    }
}
