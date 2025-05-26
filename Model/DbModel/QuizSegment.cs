using PubQuizBackend.Util.Interfaces;
using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizSegment : INumbered
{
    public int Id { get; set; }

    public int RoundId { get; set; }

    public decimal BonusPoints { get; set; }

    public int Type { get; set; }

    public int Number { get; set; }

    public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();

    public virtual QuizRound Round { get; set; } = null!;
}
