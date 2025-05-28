using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DETP.requests.QaViolation
{
    public class QaViolationCreateRequest
    {
        [EmailAddress]
        [Required(ErrorMessage = "Vendor email is required.")]
        public string VendorEmail { get; set; }
        [Required(ErrorMessage = "Observation Detail is required.")]
        public string ObservationDetails { get; set; }
        public IFormFile Attachment { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        public long ViolationCategory { get; set; }
        [Required(ErrorMessage = "Sub Category is required.")]
        public long SubCategory { get; set; }
        [Required(ErrorMessage = "Amount is required.")]
        public int Amount { get; set; }
        [Required(ErrorMessage = "Observation Id is required.")]
        public int ObservationId { get; set; }
        
    }
}
