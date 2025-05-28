using FluentValidation;
using OfficeOpenXml.FormulaParsing.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DETP.requests.editObservation
{
    public class HeadDetpCreateRequest : ObservationFlowRequest
    {

        public string Decision { get; set; }
        public string Remarks { get; set; }
        public string Job { get; internal set; }
        public string Qaaa { get; internal set; }

        public FluentValidation.Results.ValidationResult Validate()
        {
            var validator = new HeadDetpCreateRequestValidator();
            return validator.Validate(this);
        }


    }

    public class HeadDetpCreateRequestValidator : AbstractValidator<HeadDetpCreateRequest>
    {
        public HeadDetpCreateRequestValidator()
        {
            RuleFor(x => x.FromId).NotNull();
            RuleFor(x => x.ToId).NotNull();
            RuleFor(x => x.ObservationId).NotNull();
            RuleFor(x => x.FlowId).NotNull();


        }
    }
}
