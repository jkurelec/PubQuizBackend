using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class HostOrganizationQuiz
{
    public int HostId { get; set; }

    public int OrganizationId { get; set; }

    public int QuizId { get; set; }

    public bool CreateEdition { get; set; }

    public bool EditEdition { get; set; }

    public bool DeleteEdition { get; set; }

    public virtual User Host { get; set; } = null!;

    public virtual Organization Organization { get; set; } = null!;

    public virtual Quiz Quiz { get; set; } = null!;
}
