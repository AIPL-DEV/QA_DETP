using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("assignee")]
    public class Assignee: BaseModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("observation_id")]
        public int ObservationId { get; set; }
        [Column("flow_id")]
        public int FlowId { get; set; }
        [Column("ovservation_details")]
        public string ObservationDetails { get; set; }
        [Column("root_cause_analysis")]
        public string RootCauseAnalysis { get; set; }
        [Column("corrective_action")]
        public string CorrectiveAction { get; set; }
        [Column("preventive_action")]
        public string PreventiveAction { get; set; }
        [Column("value_of_rectification")]
        public string ValueOfRectification { get; set; }
        [Column("time_loss")]
        public string TimeLoss { get; set; }
        [Column("time_value")]
        public string TimeValue { get; set; }
        [Column("decision_by")]
        public int? DecisionById { get; set; }
        [ForeignKey(nameof(DecisionById))]
        public User DecisionBy { get; set; }
        [Column("decision_date")]
        public DateTime? DecisionDate { get; set; }
        
    }
}
