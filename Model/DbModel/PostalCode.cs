using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class PostalCode
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int CityId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
