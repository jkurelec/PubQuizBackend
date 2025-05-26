using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizEdition
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int QuizId { get; set; }

    public int HostId { get; set; }

    public int CategoryId { get; set; }

    public int LocationId { get; set; }

    public DateTime Time { get; set; }

    public int Rating { get; set; }

    public decimal TotalPoints { get; set; }

    public int? FeeType { get; set; }

    public int? Fee { get; set; }

    public int? Duration { get; set; }

    public int? MaxTeamSize { get; set; }

    public string? Description { get; set; }

    public DateTime RegistrationStart { get; set; }

    public DateTime RegistrationEnd { get; set; }

    public int? LeagueId { get; set; }

    public int Visibility { get; set; }

    public virtual QuizCategory Category { get; set; } = null!;

    public virtual ICollection<EditionPrize> EditionPrizes { get; set; } = new List<EditionPrize>();

    public virtual User Host { get; set; } = null!;

    public virtual QuizLeague? League { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual ICollection<QuizEditionResult> QuizEditionResults { get; set; } = new List<QuizEditionResult>();

    public virtual ICollection<QuizRound> QuizRounds { get; set; } = new List<QuizRound>();
}
