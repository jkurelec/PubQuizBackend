using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class Organization
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int EditionsHosted { get; set; }

    public int OwnerId { get; set; }

    public string? ProfileImage { get; set; }

    public virtual ICollection<HostOrganizationQuiz> HostOrganizationQuizzes { get; set; } = new List<HostOrganizationQuiz>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
