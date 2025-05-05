using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class Quiz
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int OrganizerId { get; set; }

    public int CategoryId { get; set; }

    public int Rating { get; set; }

    public int LocationId { get; set; }

    public DateTime Time { get; set; }

    public int EditionsHosted { get; set; }

    public virtual QuizCategory Category { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual Organizer Organizer { get; set; } = null!;

    public virtual ICollection<QuizEdition> QuizEditions { get; set; } = new List<QuizEdition>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
