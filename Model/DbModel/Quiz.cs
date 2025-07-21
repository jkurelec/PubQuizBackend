using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class Quiz
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int OrganizationId { get; set; }

    public int Rating { get; set; }

    public int EditionsHosted { get; set; }

    public string? ProfileImage { get; set; }

    public virtual ICollection<HostOrganizationQuiz> HostOrganizationQuizzes { get; set; } = new List<HostOrganizationQuiz>();

    public virtual Organization Organization { get; set; } = null!;

    public virtual ICollection<QuizEdition> QuizEditions { get; set; } = new List<QuizEdition>();

    public virtual ICollection<QuizInvitation> QuizInvitations { get; set; } = new List<QuizInvitation>();

    public virtual ICollection<QuizLeague> QuizLeagues { get; set; } = new List<QuizLeague>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual ICollection<QuizCategory> QuizCategories { get; set; } = new List<QuizCategory>();
}
