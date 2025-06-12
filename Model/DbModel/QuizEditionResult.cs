using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizEditionResult
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public int EditionId { get; set; }

    public int? Rank { get; set; }

    public decimal TotalPoints { get; set; }

    public int Rating { get; set; }

    public virtual QuizEdition Edition { get; set; } = null!;

    public virtual ICollection<QuizRoundResult> QuizRoundResults { get; set; } = new List<QuizRoundResult>();

    public virtual Team Team { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
