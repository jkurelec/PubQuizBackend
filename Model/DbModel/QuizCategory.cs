using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? SuperCategoryId { get; set; }

    public virtual ICollection<QuizCategory> InverseSuperCategory { get; set; } = new List<QuizCategory>();

    public virtual ICollection<QuizEdition> QuizEditions { get; set; } = new List<QuizEdition>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

    public virtual QuizCategory? SuperCategory { get; set; }

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
