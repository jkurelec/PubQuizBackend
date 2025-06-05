using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizAnswerDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizAnswerService
    {
        Task<QuizRoundResultDetailedDto> GradeTeamAnswers(NewQuizRoundResultDto roundDto, int hostId);
        Task<QuizAnswerDetailedDto> UpdateAnswer(QuizAnswerDetailedDto answerDto, int hostId);
        Task<QuizSegmentResultDetailedDto> UpdateSegment(QuizSegmentResultDetailedDto segmentResultDto, int hostId);
        Task<QuizRoundResultMinimalDto> AddTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId);
        Task<QuizRoundResultMinimalDto> UpdateTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId);
        Task<IEnumerable<QuizRoundResultDetailedDto>> GetTeamAnswers(int editionResultId, int hostId);
        Task<IEnumerable<QuizEditionResultBriefDto>> GetEditionResults(int editionId, int hostId);
        Task<IEnumerable<QuizEditionResultBriefDto>> RankTeamsOnEdition(int editionId, int hostId);
        Task<IEnumerable<QuizEditionResultBriefDto>> BreakTie(int promotedId, int editionId, int hostId);
    }
}
