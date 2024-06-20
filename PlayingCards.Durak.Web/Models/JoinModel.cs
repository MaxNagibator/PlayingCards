﻿using Microsoft.AspNetCore.Mvc;

namespace PlayingCards.Durak.Web.Models
{
    public class JoinModel : AuthModel
    {
        public string PlayerName { get; set; }

        public Guid TableId { get; set; }
    }
}
