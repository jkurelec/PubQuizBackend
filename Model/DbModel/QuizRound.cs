using PubQuizBackend.Util.Interfaces;
using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizRound : INumbered
{
    public int Id { get; set; }

    public int Number { get; set; }

    public int EditionId { get; set; }

    public virtual QuizEdition Edition { get; set; } = null!;

    public virtual ICollection<QuizSegment> QuizSegments { get; set; } = new List<QuizSegment>();
}
