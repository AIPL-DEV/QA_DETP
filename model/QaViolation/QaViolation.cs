using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace DETP.model.QaViolation
{
    [Table("qa_violations")]
    public class QaViolation
    {
        public long Id { get; set; }
        public string VendorEmail { get; set; }
        public string ObservationDetails { get; set; }
        public string? Attachment { get; set; }
        public int Amount { get; set; }
        public int ObservationId { get; set; }
        public long QaViolationCategoryId { get; set; }
        public long QaViolationSubCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? CreatedById { get; set; }
        [Column("Status")]
        public int? QAStatus { get; set; }
        public int? Count { get; set; }
        [NotMapped]
        public QaViolationStatus Status
        {
            get => QAStatus == null ? QaViolationStatus.FINISHED : (QaViolationStatus)QAStatus;
            set => QAStatus = (int)value;
        }

        [ForeignKey(nameof(ObservationId))]
        public QAObservation? Observation { get; set; }
        [ForeignKey(nameof(QaViolationCategoryId))]
        public QaViolationCategory Category { get; set; }
        [ForeignKey(nameof(QaViolationSubCategoryId))]
        public QaViolationSubCategory SubCategory { get; set; }
        public User CreatedBy { get; set; }

        public string AttachmentFullPath =>
    Attachment == null ? null : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ViolationAttachments", Attachment);



    }

    public enum QaViolationStatus
    {
        WITH_HEAD_QA_SHA,
        WITH_GM_TECHNICAL_SERVICES,
        WITH_CFO,
        WITH_HEAD_PROCUREMENT,
        REJECTED,
        FINISHED
    }
}
