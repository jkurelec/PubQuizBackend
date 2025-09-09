using PubQuizBackend.Util.Interfaces;
using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizRatingHistory : IRatingHistory
{
    public int Id { get; set; }

    public int QuizId { get; set; }

    public DateTime Date { get; set; }

    public int RatingChange { get; set; }

    public int? EditionId { get; set; }

    public int? OldRating { get; set; }

    public int? NewRating { get; set; }

    public virtual QuizEdition? Edition { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;
}
