using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("users")]
    public class User
    {
        [Column("user_id")]
        [Key]
        public int UserId { get; set; }

        [Column("pno")]
        public string PNo { get; set; }
        [Column("password")]
        
        public string? Password { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [ForeignKey("department")]
        public Department? Department { get; set; }
        [Column("app")]
        public string? App { get; set; }
        [Column("updated_on")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updated_by")]

        public int? UpdatedBy { get; set; }
        [Column("force_reset")]
        public string? ForceReset { get; set; }
        [Column("noted_mail")]
        public string? NotedMail { get; set; }
        public List<Role> Roles { get; set; }
        [NotMapped]
        public int? RoleId { get; set; }

    }
}
