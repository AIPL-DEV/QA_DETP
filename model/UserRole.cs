using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("user_role")]
    [Keyless]
    public class UserRole
    {
        [Column("user_id")]
        public int? UserId { get; set; }
        [Column("role_id")]
        public int? RoleId { get; set; }
    }
}
