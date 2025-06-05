using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizSegmentResult
{
    public int Id { get; set; }

    public int SegmentId { get; set; }

    public decimal BonusPoints { get; set; }

    public int RoundResultId { get; set; }

    public virtual ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();

    public virtual QuizRoundResult RoundResult { get; set; } = null!;

    public virtual QuizSegment Segment { get; set; } = null!;
}
