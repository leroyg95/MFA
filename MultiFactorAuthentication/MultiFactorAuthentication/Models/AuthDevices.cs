using System;
using System.ComponentModel.DataAnnotations;

namespace MultiFactorAuthentication.Models
{
    public class AuthDevices
    {
        [Key]
        public int Id { get; set; }
        public string DeviceToken { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
