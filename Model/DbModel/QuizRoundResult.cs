using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizRoundResult
{
    public int Id { get; set; }

    public int RoundId { get; set; }

    public int EditionResultId { get; set; }

    public decimal Points { get; set; }

    public virtual QuizEditionResult EditionResult { get; set; } = null!;

    public virtual ICollection<QuizSegmentResult> QuizSegmentResults { get; set; } = new List<QuizSegmentResult>();

    public virtual QuizRound Round { get; set; } = null!;
}
