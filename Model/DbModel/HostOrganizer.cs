using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class HostOrganizer
{
    public int HostId { get; set; }

    public int OrganizerId { get; set; }

    public bool CreateEdition { get; set; }

    public bool EditEdition { get; set; }

    public bool DeleteEdition { get; set; }

    public bool CreateQuiz { get; set; }

    public bool EditQuiz { get; set; }

    public bool DeleteQuiz { get; set; }

    public virtual User Host { get; set; } = null!;

    public virtual Organizer Organizer { get; set; } = null!;
}
