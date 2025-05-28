using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DETP.requests.QaViolation
{
    public class HeadProcurementSubmitRequest
    {
        public long FlowId { get; set; }
        public string Comment { get; set; }
        public IFormFile Attachment { get; set; }
        public string DebitNote { get; set; }
    }
}
