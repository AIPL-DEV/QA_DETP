using System.Collections.Generic;

namespace DETP.requests.editObservation
{
    public class ObservationFlowRequest
    {
        public List<string> Att { get; set; }
        public int ToId { get; set; }
        public int FromId { get; set; }
        public int ObservationId { get; set; }
        public int FlowId { get; set; }
    }
}
