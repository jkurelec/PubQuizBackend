using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class City
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual ICollection<PostalCode> PostalCodes { get; set; } = new List<PostalCode>();
}
