using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizAnswerDto;
using static PubQuizBackend.Controllers.QuizAnswerController;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizAnswerRepository
    {
        Task AuthorizeHostByRoundId(int hostId, int roundId);
        Task AuthorizeHostByEditionResultId(int hostId, int roundId);
        Task AuthorizeHostByEditionId(int hostId, int roundId);
        Task<QuizRoundResult> GradeTeamRoundAnswers(NewQuizRoundResultDto roundDto);
        Task<QuizRoundResult> GradeExistingTeamRoundAnswers(QuizRoundResultDetailedDto roundDto);
        Task<int> ValidateEditionResultId(int editionId, int teamId);
        Task<QuizAnswer> UpdateAnswer(QuizAnswerDetailedDto answerDto, int hostId);
        Task<QuizSegmentResult> UpdateSegment(QuizSegmentResultDetailedDto segmentResultDto, int hostId);
        Task<QuizRoundResult> AddTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId);
        Task<QuizRoundResult> AddTeamRoundPointsDetailed(NewQuizRoundResultDto roundResultDto, int hostId);
        Task<QuizRoundResult> UpdateTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId);
        Task<QuizRoundResult> UpdateTeamRoundPointsDetailed(QuizRoundResultDetailedDto roundResultDto, int hostId);
        Task<IEnumerable<QuizRoundResult>> GetTeamAnswers(int editionResultId, int hostId);
        Task<IEnumerable<QuizEditionResult>> GetEditionResults(int editionId);
        Task<IEnumerable<QuizEditionResult>> GetEditionResultsDetailed(int editionId);
        Task<IEnumerable<QuizEditionResult>> RankTeamsOnEdition(int editionId);
        Task<IEnumerable<QuizEditionResult>> BreakTie(int promotedId,int editionId);
        Task<bool> IsDetailedResult(int roundResultId);
        Task DeleteRoundResultSegments(int roundResultId, int hostId);
        Task<QuizRoundResultDetailedDto> AutofillRound(QuizRoundResultDetailedDto roundResult, List<PredictedAnswers> answers);
    }
}
