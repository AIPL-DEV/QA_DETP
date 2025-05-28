using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model.QaViolation
{
    [Table("qa_violation_sub_categories")]
    public class QaViolationSubCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long QaViolationCategoryId { get; set; }
        public virtual QaViolationCategory Category { get; set; }
        public PenaltyDetail PenaltyDetail { get; set; }
    }
}
