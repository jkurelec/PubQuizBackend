using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class LeaguePrize
{
    public int Id { get; set; }

    public int LeagueId { get; set; }

    public string Name { get; set; } = null!;

    public int? Position { get; set; }

    public virtual QuizLeague League { get; set; } = null!;
}
