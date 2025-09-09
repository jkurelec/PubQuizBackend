using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class EditionRecommendationParameter
{
    public int Id { get; set; }

    public int EditionId { get; set; }

    public int UserId { get; set; }

    public string Ratings { get; set; } = null!;

    public virtual QuizEdition Edition { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
