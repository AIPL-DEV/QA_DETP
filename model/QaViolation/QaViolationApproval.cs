using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DETP.model.QaViolation
{
    [Table("qa_violation_approval")]
    public class QaViolationApproval
    {
        public long Id { get; set; }
        public long QaViolationId { get; set; }
        public long QaViolationFlowId { get; set; }
        public int RoleId { get; set; }
        public bool PenaltyClauseCorrect { get; set; }
        public bool PenaltyAmountCorrect { get; set; }
        public string Comments { get; set; }
        public int ApprovedById { get; set; }
        public string Attachment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual Role Role { get; set; }
        public virtual User ApprovedBy { get; set; }

        [NotMapped]
        public string AttachmentFullPath =>

    Attachment == null ? null : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ViolationAttachments", Attachment);

    }
}
