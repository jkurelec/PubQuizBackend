using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class UserTeamEdition
{
    public int UserId { get; set; }

    public int TeamId { get; set; }

    public int QuizEditionResultId { get; set; }

    public virtual QuizEditionResult QuizEditionResult { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
