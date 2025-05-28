using System;

namespace DETP.requests.QaViolation
{
    public class CFOSubmitRequest
    {
        public long FlowId { get; set; }
        public string Comment { get; set; }
        public DateTime DeducationDate { get; set; }
        public string DebitNote { get; set; }
    }
}
