using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class UserTeam
{
    public int UserId { get; set; }

    public int TeamId { get; set; }

    public bool RegisterTeam { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
