using SKDJK.Models.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDJK.Models
{
    public class Role
    {
        public int Id { get; set; }
        public UserRole RoleName { get; set; }
        public string? Description { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}