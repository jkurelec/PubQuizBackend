using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizEditionRecommendationParam
{
    public int EditionId { get; set; }

    public int Rating { get; set; }

    public float Duration { get; set; }

    public List<int> CategoryIds { get; set; } = null!;

    public int HostId { get; set; }

    public float NumberOfTeams { get; set; }

    public float TeamSize { get; set; }

    public int DayOfTheWeek { get; set; }

    public int TimeOfEdition { get; set; }

    public NpgsqlPoint Location { get; set; }

    public virtual QuizEdition Edition { get; set; } = null!;

    public virtual User Host { get; set; } = null!;


    public static float NormalizeDuration(int duration)
    {
        return ((float)Math.Clamp(duration, 30, 180) - 30) / 150;
    }

    public static float NormalizeNumberOfTeams(int numberOfTeams)
    {
        return ((float)Math.Clamp(numberOfTeams, 10, 40) - 10) / 30;
    }

    public static float NormalizeTeamSize(double teamSize)
    {
        return ((float)Math.Clamp(teamSize, 1, 5) - 1) / 4;
    }

    public static float NormalizeTimeOfEdition(int timeOfEdition)
    {
        return (float)timeOfEdition / 23;
    }
}
