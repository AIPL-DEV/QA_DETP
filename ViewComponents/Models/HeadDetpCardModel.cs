using DETP.model;

namespace DETP.ViewComponents.Models
{
    public class HeadDetpCardModel
    {
        public HeadDetp HeadDetp { get; set; }
        public int Index { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsCritical { get; set; }
    }
}
