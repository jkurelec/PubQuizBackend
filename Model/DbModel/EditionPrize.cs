using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class EditionPrize
{
    public int Id { get; set; }

    public int EditionId { get; set; }

    public string Name { get; set; } = null!;

    public int? Position { get; set; }

    public virtual QuizEdition Edition { get; set; } = null!;
}
