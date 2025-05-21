using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizLeague
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int QuizId { get; set; }

    public string Points { get; set; } = null!;

    public virtual ICollection<LeaguePrize> LeaguePrizes { get; set; } = new List<LeaguePrize>();

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual ICollection<QuizEdition> QuizEditions { get; set; } = new List<QuizEdition>();
}
