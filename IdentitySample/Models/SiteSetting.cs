using System.ComponentModel.DataAnnotations;
using System;

namespace IdentitySample.Models
{
    public class SiteSetting
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? LastTimeChanged { get; set; }
    }
}
