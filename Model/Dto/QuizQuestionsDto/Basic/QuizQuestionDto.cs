using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using System.ComponentModel.DataAnnotations;

namespace PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic
{
    public class QuizQuestionDto
    {
        public QuizQuestionDto() { }

        public QuizQuestionDto(QuizQuestion question)
        {
            Id = question.Id;
            SegmentId = question.SegmentId;
            Type = Enum.IsDefined(typeof(QuestionType), question.Type)
                ? (QuestionType)question.Type
                : throw new BadRequestException("Invalid question type!");
            Question = question.Question;
            Answer = question.Answer;
            Points = question.Points;
            BonusPoints = question.BonusPoints;
            MediaUrl = question.MediaUrl;
            Number = question.Number;
        }

        public int Id { get; set; }
        public int SegmentId { get; set; }
        public QuestionType Type { get; set; } = QuestionType.REGULAR;
        public string Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
        [Range(0.5, 100, ErrorMessage = "TotalPoints must be at least 0.5")]
        public decimal Points { get; set; }
        public decimal BonusPoints { get; set; } = 0;
        public string? MediaUrl { get; set; }
        public int Number { get; set; }

        public QuizQuestion ToObject()
        {
            return new()
            {
                Id = Id,
                SegmentId = SegmentId,
                Type = Enum.IsDefined(Type)
                    ? (int)Type
                    : throw new BadRequestException("Invalid question type!"),
                Question = Question,
                Answer = Answer,
                Points = Points,
                BonusPoints = BonusPoints,
                MediaUrl = MediaUrl,
                Number = Number
            };
        } 
    }
}
