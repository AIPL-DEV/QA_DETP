using DETP.model;

namespace DETP.ViewComponents.Models
{
    public class DeptHodCardModel
    {
        public DeptHod DeptHod { get; set; }
        public int Index { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsCritical { get; set; }
    }
}
