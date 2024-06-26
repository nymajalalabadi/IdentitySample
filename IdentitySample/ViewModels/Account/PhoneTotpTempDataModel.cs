﻿using System;

namespace IdentitySample.ViewModels.Account
{
    public class PhoneTotpTempDataModel
    {
        public byte[] SecretKey { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime ExpirationTime { get; set; }
    }

}
