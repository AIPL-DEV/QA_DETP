using DETP.model;

namespace DETP.ViewComponents.Models
{
    public class EicDetpCardModel
    {
        public EicDetp EicDetp { get; set; }
        public int Index { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsHoDDETP { get; set; }
    }
}
