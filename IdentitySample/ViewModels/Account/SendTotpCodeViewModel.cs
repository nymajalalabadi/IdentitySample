﻿using System.ComponentModel.DataAnnotations;

namespace IdentitySample.ViewModels.Account
{
    public class SendTotpCodeViewModel
    {
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [MaxLength(11, ErrorMessage = "حداکثر طول مجاز {0} {1} کاراکتر است.")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
