using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model.QaViolation
{
    [Table("penalty_details")]
    public class PenaltyDetail
    {
        public long Id { get; set; }
        public string FinancialPenalty { get; set; }
        public string Administrative { get; set; }

        public long QaViolationCategoryId { get; set; }
        public long QaViolationSubCategoryId { get; set; }
		public double? Max_amount { get; set; }
		public virtual QaViolationCategory Category { get; set; }
        public virtual QaViolationSubCategory SubCategory { get; set; }

    }
}
