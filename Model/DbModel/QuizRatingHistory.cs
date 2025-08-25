using PubQuizBackend.Util.Interfaces;
using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizRatingHistory : IRatingHistory
{
    public int Id { get; set; }

    public int QuizId { get; set; }

    public DateTime Date { get; set; }

    public int Rating { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;
}
