using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    public class Division
    {
        public long id { get; set; }
        public string name { get; set; }
        public int? DivisionHeadId { get; set; }
        public User? DivisionHead { get; set; }
    }
}
