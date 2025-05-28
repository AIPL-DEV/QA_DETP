using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("sha_request_image")]
    public class ShaRequestImage
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("path")]
        public string Path { get; set; }
        [Column("sha_request_id")]
        public int ShaRequestId { get; set; }
    }
}
