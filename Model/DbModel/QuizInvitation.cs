using System;
using System.Collections.Generic;

namespace PubQuizBackend.Model.DbModel;

public partial class QuizInvitation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int QuizId { get; set; }

    public bool? Response { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
