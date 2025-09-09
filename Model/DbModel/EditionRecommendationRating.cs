using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class EditionRecommendationRating
{
    public int Id { get; set; }

    public int EditionId { get; set; }

    public int UserId { get; set; }

    public Dictionary<string, decimal> Ratings { get; set; } = new();

    public virtual QuizEdition Edition { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
