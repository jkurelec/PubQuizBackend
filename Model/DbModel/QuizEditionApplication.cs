using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizEditionApplication
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public int EditionId { get; set; }

    public bool? Accepted { get; set; }

    public virtual QuizEdition Edition { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
