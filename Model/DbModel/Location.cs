using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class Location
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CityId { get; set; }

    public string? GmapsLink { get; set; }

    public int PostalCodeId { get; set; }

    public string Address { get; set; } = null!;

    public double Lat { get; set; }

    public double Lon { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual PostalCode PostalCode { get; set; } = null!;

    public virtual ICollection<QuizEdition> QuizEditions { get; set; } = new List<QuizEdition>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
