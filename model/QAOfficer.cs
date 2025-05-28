using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("qa_officer")]
    public class QAOfficer : BaseModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("observation_id")]
        public int ObservationId { get; set; }
        [Column("flow_id")]
        public int FlowId { get; set; }
        [Column("compliance_satisfactory")]
        public string ComplianceSatisfactory { get; set; }
        [Column("remarks")]
        public string Remarks { get; set; }
        [Column("decision_by")]
        public int? DecisionById { get; set; }
        [ForeignKey(nameof(DecisionById))]
        public User? DecisionBy { get; set; }
        [Column("decision_date")]
        public DateTime? DecisionDate { get; set; }
        [Column("within_slg")]
        public string WithinSlg { get; set; }
    }
}
