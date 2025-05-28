using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model.QaViolation
{
    [Table("qa_violation_cfo_review")]
    public class QaViolationCFOReview
    {
        public long Id { get; set; }
        public long QaViolationId { get; set; }
        public long QaViolationFlowId { get; set; }
        public string Comments { get; set; }
        public DateTime DeducationDate { get; set; }
        public string DebitNote { get; set; }

        public int ApprovedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual User ApprovedBy { get; set; }
    }
}
