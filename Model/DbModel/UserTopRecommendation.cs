using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class UserTopRecommendation
{
    public int UserId { get; set; }

    public int EditionId { get; set; }

    public DateTime EditionTimestamp { get; set; }

    public DateTime? CreatedAt { get; set; }

    public float? Match { get; set; }
}
