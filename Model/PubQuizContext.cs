using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model;

public partial class PubQuizContext : DbContext
{
    public PubQuizContext()
    {
    }

    public PubQuizContext(DbContextOptions<PubQuizContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EditionPrize> EditionPrizes { get; set; }

    public virtual DbSet<HostOrganizationQuiz> HostOrganizationQuizzes { get; set; }

    public virtual DbSet<LeaguePrize> LeaguePrizes { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<PostalCode> PostalCodes { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<QuizAnswer> QuizAnswers { get; set; }

    public virtual DbSet<QuizCategory> QuizCategories { get; set; }

    public virtual DbSet<QuizEdition> QuizEditions { get; set; }

    public virtual DbSet<QuizEditionApplication> QuizEditionApplications { get; set; }

    public virtual DbSet<QuizEditionResult> QuizEditionResults { get; set; }

    public virtual DbSet<QuizLeague> QuizLeagues { get; set; }

    public virtual DbSet<QuizQuestion> QuizQuestions { get; set; }

    public virtual DbSet<QuizRound> QuizRounds { get; set; }

    public virtual DbSet<QuizSegment> QuizSegments { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserTeam> UserTeams { get; set; }

    public virtual DbSet<UserTeamEdition> UserTeamEditions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=backend;Password=Pasvord123;Database=pub_quiz");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("city_country_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("country_pkey");

            entity.ToTable("country");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .HasColumnName("country_code");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<EditionPrize>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prize_pkey");

            entity.ToTable("edition_prize");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.EditionId).HasColumnName("edition_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Position).HasColumnName("position");

            entity.HasOne(d => d.Edition).WithMany(p => p.EditionPrizes)
                .HasForeignKey(d => d.EditionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prize_edition_id_fkey");
        });

        modelBuilder.Entity<HostOrganizationQuiz>(entity =>
        {
            entity.HasKey(e => new { e.HostId, e.OrganizationId, e.QuizId }).HasName("host_organizer_quiz_pkey");

            entity.ToTable("host_organization_quiz");

            entity.Property(e => e.HostId).HasColumnName("host_id");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.QuizId).HasColumnName("quiz_id");
            entity.Property(e => e.CreateEdition)
                .HasDefaultValue(false)
                .HasColumnName("create_edition");
            entity.Property(e => e.CrudQuestion)
                .HasDefaultValue(false)
                .HasColumnName("crud_question");
            entity.Property(e => e.DeleteEdition)
                .HasDefaultValue(false)
                .HasColumnName("delete_edition");
            entity.Property(e => e.EditEdition)
                .HasDefaultValue(false)
                .HasColumnName("edit_edition");
            entity.Property(e => e.ManageApplication)
                .HasDefaultValue(false)
                .HasColumnName("manage_application");

            entity.HasOne(d => d.Host).WithMany(p => p.HostOrganizationQuizzes)
                .HasForeignKey(d => d.HostId)
                .HasConstraintName("host_organizer_quiz_host_id_fkey");

            entity.HasOne(d => d.Organization).WithMany(p => p.HostOrganizationQuizzes)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("host_organizer_quiz_organizer_id_fkey");

            entity.HasOne(d => d.Quiz).WithMany(p => p.HostOrganizationQuizzes)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("host_organizer_quiz_quiz_id_fkey");
        });

        modelBuilder.Entity<LeaguePrize>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("league_prize_pkey");

            entity.ToTable("league_prize");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Position).HasColumnName("position");

            entity.HasOne(d => d.League).WithMany(p => p.LeaguePrizes)
                .HasForeignKey(d => d.LeagueId)
                .HasConstraintName("league_prize_league_id_fkey");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("location_pkey");

            entity.ToTable("location");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.GmapsLink).HasColumnName("gmaps_link");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lon).HasColumnName("lon");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PostalCodeId).HasColumnName("postal_code_id");

            entity.HasOne(d => d.City).WithMany(p => p.Locations)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("location_city_id_fkey");

            entity.HasOne(d => d.PostalCode).WithMany(p => p.Locations)
                .HasForeignKey(d => d.PostalCodeId)
                .HasConstraintName("fk_postal_code");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("organizer_pkey");

            entity.ToTable("organization");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.EditionsHosted)
                .HasDefaultValue(0)
                .HasColumnName("editions_hosted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");

            entity.HasOne(d => d.Owner).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("fk_owner_id");
        });

        modelBuilder.Entity<PostalCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("postal_code_pkey");

            entity.ToTable("postal_code");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");

            entity.HasOne(d => d.City).WithMany(p => p.PostalCodes)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("postal_code_city_id_fkey");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quiz_pkey");

            entity.ToTable("quiz");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.EditionsHosted)
                .HasDefaultValue(0)
                .HasColumnName("editions_hosted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.Rating)
                .HasDefaultValue(1000)
                .HasColumnName("rating");

            entity.HasOne(d => d.Organization).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("quiz_owner_id_fkey");

            entity.HasMany(d => d.Locations).WithMany(p => p.Quizzes)
                .UsingEntity<Dictionary<string, object>>(
                    "QuizLocation",
                    r => r.HasOne<Location>().WithMany()
                        .HasForeignKey("LocationId")
                        .HasConstraintName("quiz_location_location_id_fkey"),
                    l => l.HasOne<Quiz>().WithMany()
                        .HasForeignKey("QuizId")
                        .HasConstraintName("quiz_location_quiz_id_fkey"),
                    j =>
                    {
                        j.HasKey("QuizId", "LocationId").HasName("quiz_location_pkey");
                        j.ToTable("quiz_location");
                        j.IndexerProperty<int>("QuizId").HasColumnName("quiz_id");
                        j.IndexerProperty<int>("LocationId").HasColumnName("location_id");
                    });

            entity.HasMany(d => d.QuizCategories).WithMany(p => p.Quizzes)
                .UsingEntity<Dictionary<string, object>>(
                    "QuizQuizCategory",
                    r => r.HasOne<QuizCategory>().WithMany()
                        .HasForeignKey("QuizCategoryId")
                        .HasConstraintName("quiz_quiz_category_quiz_category_id_fkey"),
                    l => l.HasOne<Quiz>().WithMany()
                        .HasForeignKey("QuizId")
                        .HasConstraintName("quiz_quiz_category_quiz_id_fkey"),
                    j =>
                    {
                        j.HasKey("QuizId", "QuizCategoryId").HasName("quiz_quiz_category_pkey");
                        j.ToTable("quiz_quiz_category");
                        j.IndexerProperty<int>("QuizId").HasColumnName("quiz_id");
                        j.IndexerProperty<int>("QuizCategoryId").HasColumnName("quiz_category_id");
                    });
        });

        modelBuilder.Entity<QuizAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quiz_answer_pkey");

            entity.ToTable("quiz_answer");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Answer)
                .HasMaxLength(512)
                .HasColumnName("answer");
            entity.Property(e => e.Points)
                .HasPrecision(10, 2)
                .HasColumnName("points");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Result)
                .HasDefaultValue(0)
                .HasColumnName("result");
            entity.Property(e => e.TeamId).HasColumnName("team_id");

            entity.HasOne(d => d.Question).WithMany(p => p.QuizAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("fk_quiz_answer_question");

            entity.HasOne(d => d.Team).WithMany(p => p.QuizAnswers)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("quiz_answer_team_id_fkey");
        });

        modelBuilder.Entity<QuizCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pkey");

            entity.ToTable("quiz_category");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SuperCategoryId).HasColumnName("super_category_id");

            entity.HasOne(d => d.SuperCategory).WithMany(p => p.InverseSuperCategory)
                .HasForeignKey(d => d.SuperCategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("category_super_category_fkey");
        });

        modelBuilder.Entity<QuizEdition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quiz_edition_pkey");

            entity.ToTable("quiz_edition");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Fee).HasColumnName("fee");
            entity.Property(e => e.FeeType).HasColumnName("fee_type");
            entity.Property(e => e.HostId).HasColumnName("host_id");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.MaxTeamSize).HasColumnName("max_team_size");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.QuizId).HasColumnName("quiz_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RegistrationEnd).HasColumnName("registration_end");
            entity.Property(e => e.RegistrationStart).HasColumnName("registration_start");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.TotalPoints)
                .HasPrecision(10, 2)
                .HasColumnName("total_points");
            entity.Property(e => e.Visibility)
                .HasDefaultValue(0)
                .HasColumnName("visibility");

            entity.HasOne(d => d.Category).WithMany(p => p.QuizEditions)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("quiz_edition_category_id_fkey");

            entity.HasOne(d => d.Host).WithMany(p => p.QuizEditions)
                .HasForeignKey(d => d.HostId)
                .HasConstraintName("quiz_edition_host_fkey");

            entity.HasOne(d => d.League).WithMany(p => p.QuizEditions)
                .HasForeignKey(d => d.LeagueId)
                .HasConstraintName("fk_quiz_edition_league");

            entity.HasOne(d => d.Location).WithMany(p => p.QuizEditions)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("quiz_edition_location_id_fkey");

            entity.HasOne(d => d.Quiz).WithMany(p => p.QuizEditions)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("quiz_edition_quiz_id_fkey");
        });

        modelBuilder.Entity<QuizEditionApplication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quiz_edition_application_pkey");

            entity.ToTable("quiz_edition_application");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Accepted).HasColumnName("accepted");
            entity.Property(e => e.QuizEditionId).HasColumnName("quiz_edition_id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");

            entity.HasOne(d => d.QuizEdition).WithMany(p => p.QuizEditionApplications)
                .HasForeignKey(d => d.QuizEditionId)
                .HasConstraintName("quiz_edition_application_quiz_edition_id_fkey");

            entity.HasOne(d => d.Team).WithMany(p => p.QuizEditionApplications)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("quiz_edition_application_team_id_fkey");

            entity.HasMany(d => d.Users).WithMany(p => p.Applications)
                .UsingEntity<Dictionary<string, object>>(
                    "QuizEditionApplicationUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("quiz_edition_application_user_user_id_fkey"),
                    l => l.HasOne<QuizEditionApplication>().WithMany()
                        .HasForeignKey("ApplicationId")
                        .HasConstraintName("quiz_edition_application_user_application_id_fkey"),
                    j =>
                    {
                        j.HasKey("ApplicationId", "UserId").HasName("quiz_edition_application_user_pkey");
                        j.ToTable("quiz_edition_application_user");
                        j.IndexerProperty<int>("ApplicationId").HasColumnName("application_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<QuizEditionResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quiz_edition_results_pkey");

            entity.ToTable("quiz_edition_results");

            entity.HasIndex(e => new { e.TeamId, e.EditionId }, "unique_team_quiz_edition").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EditionId).HasColumnName("edition_id");
            entity.Property(e => e.Rank).HasColumnName("rank");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.TotalPoints)
                .HasPrecision(10, 2)
                .HasColumnName("total_points");

            entity.HasOne(d => d.Edition).WithMany(p => p.QuizEditionResults)
                .HasForeignKey(d => d.EditionId)
                .HasConstraintName("quiz_edition_results_edition_id_fkey");

            entity.HasOne(d => d.Team).WithMany(p => p.QuizEditionResults)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("quiz_edition_results_team_id_fkey");
        });

        modelBuilder.Entity<QuizLeague>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quiz_league_pkey");

            entity.ToTable("quiz_league");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.QuizId).HasColumnName("quiz_id");

            entity.HasOne(d => d.Quiz).WithMany(p => p.QuizLeagues)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("quiz_league_quiz_id_fkey");
        });

        modelBuilder.Entity<QuizQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quiz_question_pkey");

            entity.ToTable("quiz_question");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Answer)
                .HasMaxLength(512)
                .HasColumnName("answer");
            entity.Property(e => e.BonusPoints)
                .HasPrecision(10, 2)
                .HasColumnName("bonus_points");
            entity.Property(e => e.MediaUrl)
                .HasColumnType("character varying")
                .HasColumnName("media_url");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.Points)
                .HasPrecision(10, 2)
                .HasColumnName("points");
            entity.Property(e => e.Question).HasColumnName("question");
            entity.Property(e => e.SegmentId).HasColumnName("segment_id");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Segment).WithMany(p => p.QuizQuestions)
                .HasForeignKey(d => d.SegmentId)
                .HasConstraintName("quiz_question_segment_id_fkey");
        });

        modelBuilder.Entity<QuizRound>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("round_pkey");

            entity.ToTable("quiz_round");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.EditionId).HasColumnName("edition_id");
            entity.Property(e => e.Number).HasColumnName("number");

            entity.HasOne(d => d.Edition).WithMany(p => p.QuizRounds)
                .HasForeignKey(d => d.EditionId)
                .HasConstraintName("round_edition_id_fkey");
        });

        modelBuilder.Entity<QuizSegment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("segment_pkey");

            entity.ToTable("quiz_segment");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.BonusPoints)
                .HasPrecision(10, 2)
                .HasColumnName("bonus_points");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Round).WithMany(p => p.QuizSegments)
                .HasForeignKey(d => d.RoundId)
                .HasConstraintName("segment_round_id_fkey");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_token_pkey");

            entity.ToTable("refresh_token");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.App).HasColumnName("app");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expires_at");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_refresh");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attendee_team_pkey");

            entity.ToTable("team");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.QuizId).HasColumnName("quiz_id");
            entity.Property(e => e.Rating)
                .HasDefaultValue(1000)
                .HasColumnName("rating");

            entity.HasOne(d => d.Category).WithMany(p => p.Teams)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("attendee_team_category_id_fkey");

            entity.HasOne(d => d.Owner).WithMany(p => p.Teams)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("attendee_team_owner_id_fkey");

            entity.HasOne(d => d.Quiz).WithMany(p => p.Teams)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("attendee_team_quiz_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("host_pkey");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(255)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(255)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Rating)
                .HasDefaultValue(1000)
                .HasColumnName("rating");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserTeam>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TeamId }).HasName("user_team_pkey");

            entity.ToTable("user_team");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.RegisterTeam)
                .HasDefaultValue(false)
                .HasColumnName("register_team");

            entity.HasOne(d => d.Team).WithMany(p => p.UserTeams)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("user_team_team_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserTeams)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_team_user_id_fkey");
        });

        modelBuilder.Entity<UserTeamEdition>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TeamId, e.QuizEditionResultId }).HasName("user_team_edition_pkey");

            entity.ToTable("user_team_edition");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.QuizEditionResultId).HasColumnName("quiz_edition_result_id");

            entity.HasOne(d => d.QuizEditionResult).WithMany(p => p.UserTeamEditions)
                .HasForeignKey(d => d.QuizEditionResultId)
                .HasConstraintName("user_team_edition_quiz_edition_result_id_fkey");

            entity.HasOne(d => d.Team).WithMany(p => p.UserTeamEditions)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("user_team_edition_team_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserTeamEditions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_team_edition_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
