using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PubQuizBackend.Model.DbModel;

public partial class UserRecommendationParam
{
    public int UserId { get; set; }

    public float Duration { get; set; }

    public float NumberOfTeams { get; set; }

    public float TeamSize { get; set; }

    public int EditionCount { get; set; }

    public string Hosts { get; set; } = null!;

    public string Categories { get; set; } = null!;

    public float TimeOfEdition { get; set; }

    public List<int> DayOfWeek { get; set; } = null!;

    public virtual User User { get; set; } = null!;


    public void AddHost(int host)
    {
        var hosts = JsonSerializer.Deserialize<Dictionary<int, int>>(Hosts) ?? new Dictionary<int, int>();

        if (hosts.TryGetValue(host, out int value))
        {
            hosts[host] = ++value;
        }
        else
        {
            hosts[host] = 1;
        }

        Hosts = JsonSerializer.Serialize(hosts);
    }

    public void AddCategories(List<int> categoryList)
    {
        var categories = JsonSerializer.Deserialize<Dictionary<int, int>>(Categories) ?? new Dictionary<int, int>();

        foreach (var category in categoryList)
        {
            if (categories.TryGetValue(category, out int value))
            {
                categories[category] = ++value;
            }
            else
            {
                categories[category] = 1;
            }
        }
    }

    public void AddDayOfWeek(int dayOfWeek)
    {
        DayOfWeek[dayOfWeek]++;
    }
}
