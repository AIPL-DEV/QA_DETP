using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("login_logs")]
    public class LoginLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("datetime")]
        public DateTime Datetime { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("ip")]
        public string Ip { get; set; }
    }
}
