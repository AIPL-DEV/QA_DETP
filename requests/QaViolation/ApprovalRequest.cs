using Microsoft.AspNetCore.Http;

namespace DETP.requests.QaViolation
{
    public class ApprovalRequest
    {
        public long FlowId { get; set; }
        public bool PenaltyClauseCorrect { get; set; }
        public bool PenaltyAmountCorrect { get; set; }
        public string? Decision { get; set; }
        public string Comment { get; set; }
        public IFormFile Attachment { get; set; }
    }
}
