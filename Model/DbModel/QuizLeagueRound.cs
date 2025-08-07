using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizLeagueRound
{
    public int Id { get; set; }

    public int QuizLeagueId { get; set; }

    public int Round { get; set; }

    public virtual QuizLeague QuizLeague { get; set; } = null!;

    public virtual ICollection<QuizLeagueEntry> QuizLeagueEntries { get; set; } = new List<QuizLeagueEntry>();
}
