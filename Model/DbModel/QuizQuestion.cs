using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizQuestion
{
    public int Id { get; set; }

    public int SegmentId { get; set; }

    public int Type { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public decimal Points { get; set; }

    public decimal? BonusPoints { get; set; }

    public string? MediaUrl { get; set; }

    public virtual ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();

    public virtual QuizSegment Segment { get; set; } = null!;
}
