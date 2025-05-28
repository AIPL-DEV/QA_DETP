using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("qa_att")]
    public class QaAtt: BaseModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("data")]
        public string Data { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("type_id")]
        public int TypeId { get; set; }
    }
}
