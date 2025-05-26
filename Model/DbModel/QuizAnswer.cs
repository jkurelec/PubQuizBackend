using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizAnswer
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public string? Answer { get; set; }

    public decimal Points { get; set; }

    public int QuestionId { get; set; }

    public int Result { get; set; }

    public virtual QuizQuestion Question { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
