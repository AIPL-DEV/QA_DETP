using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("hod_qa_sha")]
    public class HodQaSha : BaseModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("observation_id")]
        public int? ObservationId { get; set; }
        [Column("flow_id")]
        public int? FlowId { get; set; }
        [Column("decision")]
        public string? Decision { get; set; }
        [Column("job")]
        public string? Job { get; set; }
        [Column("non_conformance")]
        public string? NonConformance { get; set; }
        [Column("remarks")]
        public string? Remarks { get; set; }
        [Column("decision_by")]
        public int? DecisionById { get; set; }
        [ForeignKey(nameof(DecisionById))]
        public User? DecisionBy { get; set; }
        [Column("decision_date")]
        public DateTime? DecisionDate { get; set; }
    }
}