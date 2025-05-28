using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model.QaViolation
{
    [Table("qa_violation_categories")]
    public class QaViolationCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
