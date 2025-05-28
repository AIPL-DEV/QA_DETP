using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DETP.model
{
    [Table(name:"department")]
    public class Department
    {
        [Key]
        [Column(name: "department_id")]
        public int Id { get; set; }
        [Column(name: "department_abbr")]
        public string? Abbr { get; set; }
        [Column(name: "department_name")]
        public string? Name { get; set; }

        [Column(name: "updated_on")]
        public DateTime? UpdatedOn { get; set; }
        [ForeignKey("DivisionId")]
        public Division? Division { get; set; }
        [Column(name: "division_id")]
        public long? DivisionId { get; set; }
    }
}
