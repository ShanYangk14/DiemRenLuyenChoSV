using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PMQLSV.Models
{
    public class Role : IdentityRole<int>
    {
        [Key]
        public new int Id { get; set; }
        public string rolename { get; set; }
        public User User { get; set; }
    }
}
