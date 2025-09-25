using System;
using System.Collections.Generic;

namespace TheSocialCebu_Capstone.Models;

public partial class Feedback
{
    public string FeedbackId { get; set; } = null!;

    public int Rating { get; set; }

    public DateTime DateCreated { get; set; }
}
