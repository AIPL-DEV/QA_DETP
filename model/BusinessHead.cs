using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("business_head")]
    public class BusinessHead : BaseModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("observation_id")]
        public int ObservationId { get; set; }
        [Column("flow_id")]
        public int FlowId { get; set; }
        [Column("assign_to")]
        public int? AssignToId { get; set; }
        [ForeignKey(nameof(AssignToId))]
        public User? AssignTo { get; set; }
        [Column("decision")]
        public string Decision { get; set; }
        [Column("input")]
        public string Input { get; set; }
        [Column("target_date")]
        public string TargetDate { get; set; }
        [Column("remarks")]
        public string Remarks { get; set; }
        [Column("decision_by")]
        public int DecisionById { get; set; }
        [ForeignKey(nameof(DecisionById))]
        public User DecisionBy { get; set; }
        [Column("decision_date")]
        public DateTime? DecisionDate { get; set; }
    }
}
