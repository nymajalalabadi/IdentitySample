﻿using Kaktos.UserImmediateActions;
using System.ComponentModel.DataAnnotations;
using System;

namespace IdentitySample.Models
{
    public class ImmediateAction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ActionKey { get; set; }

        public DateTime ExpirationTime { get; set; }

        public DateTime AddedDate { get; set; }

        public AddPurpose Purpose { get; set; }
    }

}
