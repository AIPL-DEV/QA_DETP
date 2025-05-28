using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    public class BaseModel
    {
        [NotMapped]
        public List<QaAtt> Atts { get; set; }
    }
}
