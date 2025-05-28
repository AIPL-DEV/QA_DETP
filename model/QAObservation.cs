using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table("qa_observation")]
    public class QAObservation : BaseModel
    {
        [Column("serial_no")]
        [Key]
        public int SerialNo { get; set; }
        [Column("visit_no")]
        public string? VisitNo { get; set; }
        [Column("logged_date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LoggedDate { get; set; } = DateTime.Now;
        [NotMapped]
        public virtual Department? Dept { get; set; }

        [Column(name: "department")]
        public string? DepartmentId { get; set; }

        [Column("status")]
        public string? Status { get; set; }
        [Column("site")]
        public string? Site { get; set; }
        [Column("location")]
        public string? Location { get; set; }
        [Column("nature_of_work")]
        public string? NatureOfWork { get; set; }
        [Column("type_of_observation")]
        public string? TypeOfObservation { get; set; }
        [Column("log_non_confirmance")]
        public string? LogNonConfirmance { get; set; }
        [Column("log_confirmance")]
        public string? LogConfirmance { get; set; }
        [Column("compliance_target_date")]
        public DateTime? ComplianceTargetDate { get; set; }
        [Column("type_of_confirmance")]
        public string? TypeOfConfirmance { get; set; }
        [Column("nature_of_confirmance")]
        public string? NatureOfConfirmance { get; set; }
        [Column("standard")]
        public string? Standard { get; set; }
        [Column("basics")]
        public string? Basics { get; set; }
        [Column("job")]
        public string? Job { get; set; }
        [Column("vendor_code")]
        public string? VendorCode { get; set; }
        [Column("vendor_name")]
        public string? VendorName { get; set; }
        [Column("p_no")]
        public string? Pno { get; set; }
        [Column("site_incharge")]
        [ForeignKey("SiteIncharge")]
        public int? SiteInchargeId { get; set; }
        public User? SiteIncharge { get; set; }

        [Column("head_detp")]
        [ForeignKey("HeadDetp")]
        public int? HeadDetpId { get; set; }
        public User? HeadDetp { get; set; }
        [Column("project_incharge")]
        [ForeignKey("ProjectIncharge")]
        public int? ProjectInchargeId { get; set; }
        public User? ProjectIncharge { get; set; }
        [Column("dept_hod")]
        [ForeignKey("DeptHod")]
        public int? DeptHodId { get; set; }
        public User? DeptHod { get; set; }
        [Column("business_head")]
        [ForeignKey("BusinessHead")]
        public int? BusinessHeadId { get; set; }
        public User? BusinessHead { get; set; }
        [Column("qa_officer")]
        [ForeignKey("QaOfficer")]
        public int? QaOfficerId { get; set; }
        public User? QaOfficer { get; set; }
        [Column("observation_by")]
        [ForeignKey("ObservatioinBy")]
        public int? ObservationById { get; set; }
        public User? ObservatioinBy { get; set; }
        [Column("observation_date")]
        public DateTime? ObservationDate { get; set; }
        [ForeignKey("DivisionId")]
        public Division? Division { get; set; }
        [Column("division_id")]
        public long? DivisionId { get; set; }
        [Column("number_of_observation")]
        public int? NumberOfObservation { get; set; }
        [Column("area_of_concern")]
        public string? AreaOfConcern { get; set; }
        public ICollection<QAFlow> QaFlows { get; set; }
        [Column("hod_qa_sha")]
        public int? HodQaShaId { get; set; }
        [ForeignKey("HodQaShaId")]
        public HodQaSha? HodQaSha { get; set; }

        [NotMapped]
        public bool IsCritical 
        { 
            get { return NatureOfConfirmance == "Critical"; }
        }

        [NotMapped]
        public bool IsJobStopped
        {
            get { return Job == "Yes"; }
        }
        
        
    }
}
