using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class TeamKappa
{
    public long Id { get; set; }

    public int LeaderElo { get; set; }

    public int TeammateElo { get; set; }

    public DateTime CreatedAt { get; set; }

    public float Kappa { get; set; }
}
