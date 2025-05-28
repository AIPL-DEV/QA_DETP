using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("qa_flow")]
    public class QAFlow
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("observation_id")]
        public int? ObservationId { get; set; }
        [Column("table_name")]
        public string? TableName { get; set; }
        [Column("from_id")]
        [ForeignKey("From")]
        public int? FromId { get; set; }
        [Column("to_id")]
        [ForeignKey("To")]
        public int? ToId { get; set; }
        [Column("date")]
        public DateTime? Date { get; set; }
        [Column("completed")]
        public string? Completed { get; set; }

        [ForeignKey("ObservationId")]
        public QAObservation? QAObservation { get; set; }
        public User? To { get; set; }
        public User? From { get; set; }
    }
}
