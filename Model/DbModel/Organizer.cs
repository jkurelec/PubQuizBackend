using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class Organizer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int EditionsHosted { get; set; }

    public int OwnerId { get; set; }

    public virtual ICollection<HostOrganizer> HostOrganizers { get; set; } = new List<HostOrganizer>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
