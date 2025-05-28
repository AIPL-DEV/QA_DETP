using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model.QaViolation
{
    [Table("qa_violation_step")]
    public class QaViolationStep
    {
        public long Id{ get; set; }
        public int RoleId { get; set; }
        public int Step { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
