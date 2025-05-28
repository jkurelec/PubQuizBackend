using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class Team
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public string Name { get; set; } = null!;

    public int Rating { get; set; }

    public int CategoryId { get; set; }

    public int QuizId { get; set; }

    public virtual QuizCategory Category { get; set; } = null!;

    public virtual User Owner { get; set; } = null!;

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual ICollection<QuizEditionApplication> QuizEditionApplications { get; set; } = new List<QuizEditionApplication>();

    public virtual ICollection<QuizEditionResult> QuizEditionResults { get; set; } = new List<QuizEditionResult>();

    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}
