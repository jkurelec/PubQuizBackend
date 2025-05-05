using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizEdition
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int QuizId { get; set; }

    public int Host { get; set; }

    public int CategoryId { get; set; }

    public int LocationId { get; set; }

    public DateTime Time { get; set; }

    public int Rating { get; set; }

    public int TotalPoints { get; set; }

    public virtual QuizCategory Category { get; set; } = null!;

    public virtual User HostNavigation { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual ICollection<QuizEditionResult> QuizEditionResults { get; set; } = new List<QuizEditionResult>();

    public virtual ICollection<QuizRound> QuizRounds { get; set; } = new List<QuizRound>();
}
