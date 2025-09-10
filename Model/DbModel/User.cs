using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Rating { get; set; }

    public int Role { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string? ProfileImage { get; set; }

    public virtual ICollection<HostOrganizationQuiz> HostOrganizationQuizzes { get; set; } = new List<HostOrganizationQuiz>();

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    public virtual ICollection<QuizEditionRecommendationParam> QuizEditionRecommendationParams { get; set; } = new List<QuizEditionRecommendationParam>();

    public virtual ICollection<QuizEdition> QuizEditions { get; set; } = new List<QuizEdition>();

    public virtual ICollection<QuizInvitation> QuizInvitations { get; set; } = new List<QuizInvitation>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<TeamApplication> TeamApplications { get; set; } = new List<TeamApplication>();

    public virtual ICollection<TeamInvitation> TeamInvitations { get; set; } = new List<TeamInvitation>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<UserRatingHistory> UserRatingHistories { get; set; } = new List<UserRatingHistory>();

    public virtual UserRecommendationParam? UserRecommendationParam { get; set; }

    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();

    public virtual ICollection<QuizEditionApplication> Applications { get; set; } = new List<QuizEditionApplication>();

    public virtual ICollection<QuizEditionResult> EditionResults { get; set; } = new List<QuizEditionResult>();
}
