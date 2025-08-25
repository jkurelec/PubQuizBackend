using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class TeamDeviation
{
    public int Id { get; set; }

    public double Deviation { get; set; }

    public int EditionResultId { get; set; }

    public virtual QuizEditionResult EditionResult { get; set; } = null!;
}
