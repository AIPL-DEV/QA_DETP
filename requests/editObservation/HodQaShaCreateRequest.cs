using FluentValidation;
using OfficeOpenXml.FormulaParsing.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DETP.requests.editObservation
{
    public class HodQaShaCreateRequest : ObservationFlowRequest
    {

        public string Decision { get; set; }
        public string Remarks { get; set; }

        public FluentValidation.Results.ValidationResult Validate()
        {
            var validator = new HodQaShaCreateRequestValidator();
            return validator.Validate(this);
        }


    }

    public class HodQaShaCreateRequestValidator : AbstractValidator<HodQaShaCreateRequest>
    {
        public HodQaShaCreateRequestValidator()
        {
            RuleFor(x => x.FromId).NotNull();
            RuleFor(x => x.ToId).NotNull();
            RuleFor(x => x.ObservationId).NotNull();
            RuleFor(x => x.FlowId).NotNull();


        }
    }
}
