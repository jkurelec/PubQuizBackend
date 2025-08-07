using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizLeagueEntry
{
    public int Id { get; set; }

    public int QuizLeagueRoundId { get; set; }

    public int TeamId { get; set; }

    public double Points { get; set; }

    public virtual QuizLeagueRound QuizLeagueRound { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
