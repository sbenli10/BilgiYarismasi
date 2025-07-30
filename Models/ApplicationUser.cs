using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BilgiYarismasiMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
    }
}
