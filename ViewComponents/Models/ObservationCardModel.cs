using DETP.model;

namespace DETP.ViewComponents.Models
{
    public class ObservationCardModel
    {
        public QAObservation Observation { get; set; }
        public int Index { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string RoleName { get; set; }
    }
}
