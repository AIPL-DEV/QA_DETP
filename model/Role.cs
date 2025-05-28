using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("Role")]
    public class Role
    {
        [Column("role_id")]
        [Key]
        public int Id { get; set; }
        [Column("role")]
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }
}
