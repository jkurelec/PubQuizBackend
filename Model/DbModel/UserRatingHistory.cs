using Microsoft.EntityFrameworkCore.Migrations;
using PubQuizBackend.Util.Interfaces;
using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class UserRatingHistory : IRatingHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime Date { get; set; }

    public int RatingChange { get; set; }

    public int? EditionId { get; set; }

    public int? OldRating { get; set; }

    public int? NewRating { get; set; }

    public virtual QuizEdition? Edition { get; set; }

    public virtual User User { get; set; } = null!;
}
