using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizSegment
{
    public int Id { get; set; }

    public int RoundId { get; set; }

    public int? BonusPoints { get; set; }

    public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();

    public virtual QuizRound Round { get; set; } = null!;
}
