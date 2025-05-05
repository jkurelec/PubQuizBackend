using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? CountryCode { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
