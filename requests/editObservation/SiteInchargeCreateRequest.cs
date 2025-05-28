using FluentValidation;
using OfficeOpenXml.FormulaParsing.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DETP.requests.editObservation
{
    public class SiteInchargeCreateRequest: ObservationFlowRequest
    {
        [Required]
        public string Remarks { get; set; }
        [Required]
        public string ValueRec { get; set; }
        [Required]
        public string TimeLoss { get; set; }
        [Required]
        public string TimeVal { get; set; }

        public FluentValidation.Results.ValidationResult Validate()
        {
            var validator = new SiteInchargeCreateRequestValidator();
            return validator.Validate(this);
        }


    }

    public class SiteInchargeCreateRequestValidator : AbstractValidator<SiteInchargeCreateRequest>
    {
        public SiteInchargeCreateRequestValidator()
        {
            RuleFor(x => x.FromId).NotNull();
            RuleFor(x => x.ToId).NotNull();
            RuleFor(x => x.ObservationId).NotNull();
            RuleFor(x => x.FlowId).NotNull();

            RuleFor(x => x.Remarks)
                .NotNull();
            RuleFor(x => x.ValueRec).NotNull().Must((x, remarks) => double.TryParse(x.ValueRec, out _));
            RuleFor(x => x.TimeVal).NotNull().Must((x, remarks) => double.TryParse(x.ValueRec, out _)); ;
            RuleFor(x => x.TimeLoss).NotNull().Must((x, remarks) => double.TryParse(x.ValueRec, out _)); ;
            RuleFor(x => x.Att);
        }
    }
}
