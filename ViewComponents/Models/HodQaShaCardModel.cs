using DETP.model;

namespace DETP.ViewComponents.Models
{
    public class HodQaShaCardModel
    {
        public HodQaSha HodQaSha { get; set; }
        public int Index { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsCritical { get; set; }
    }
}
