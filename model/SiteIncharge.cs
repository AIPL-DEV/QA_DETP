using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("site_incharge")]
    public class SiteIncharge : BaseModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("observation_id")]
        public int ObservationId { get; set; }
        [Column("flow_id")]
        public int FlowId { get; set; }
        [Column("valuerec")]
        public string ValueRec { get; set; }
        [Column("timeloss")]
        public string TimeLoss { get; set; }
        [Column("timeval")]
        public string TimeVal { get; set; }
        [Column("within_target_date")]
        public string WithinTargetDate { get; set; }
        [Column("remarks")]
        public string Remarks { get; set; }
        [Column("decision_by")]
        public int? DecisionById { get; set; }
        [ForeignKey(nameof(DecisionById))]
        public User? DecisionBy { get; set; }
        [Column("decision_date")]
        public DateTime? DecisionDate { get; set; }
    }
}
