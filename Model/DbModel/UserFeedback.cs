using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class UserFeedback
{
    public int UserId { get; set; }

    public int EditionId { get; set; }

    public float GeneralRating { get; set; }

    public float HostRating { get; set; }

    public int DifficultyRating { get; set; }

    public int DurationRating { get; set; }

    public int NumberOfPeopleRating { get; set; }

    public DateTime Timestamp { get; set; }
}
