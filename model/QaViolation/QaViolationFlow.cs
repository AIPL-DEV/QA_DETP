using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model.QaViolation
{
    [Table("qa_violation_flow")]
    public class QaViolationFlow
    {
        public long Id { get; set; }
        public long QaViolationId { get; set; }
        public int? FromId { get; set; }
        public int? ToId { get; set; }
        public int? ToRoleId { get; set; }
        public Role ToRole { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TableName { get; set; }
        public bool Completed { get; set; }
    }
}
