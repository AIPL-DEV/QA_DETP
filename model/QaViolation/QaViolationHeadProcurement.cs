using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace DETP.model.QaViolation
{
    [Table("qa_violation_head_procurement")]
    public class QaViolationHeadProcurement
    {
        public long Id { get; set; }
        public long QaViolationId { get; set; }
        public long QaViolationFlowId { get; set; }
        public string Comments { get; set; }
        public string Attachment { get; set; }

        public int ApprovedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual User ApprovedBy { get; set; }

        [NotMapped]
        public string AttachmentFullPath =>

    Attachment == null ? null : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ViolationAttachments", Attachment);
    }
}
