using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizEditionResult
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public int EditionId { get; set; }

    public int Rank { get; set; }

    public int TotalPoints { get; set; }

    public virtual QuizEdition Edition { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
