using DETP.services;
using FluentValidation;
using System;

namespace DETP.requests.editObservation
{
    public class BusinessHeadCreateRequest: ObservationFlowRequest
    {
        public int? AssignedTo { get; set; }
        public string Decision { get; set; }
        public string Input { get; set; }
        public string TargetDate { get; set; }
        public string Remarks { get; set; }
        public int FormType { get; set; } 

        public FluentValidation.Results.ValidationResult Validate()
        {
            var validator = new BusinessHeadCreateRequestValidator();
            return validator.Validate(this);
        }
    }

    public class BusinessHeadCreateRequestValidator : AbstractValidator<BusinessHeadCreateRequest>
    {
        public BusinessHeadCreateRequestValidator()
        {
            RuleFor(x => x.FromId).NotNull();
            RuleFor(x => x.ToId).NotNull();
            RuleFor(x => x.ObservationId).NotNull();
            RuleFor(x => x.FlowId).NotNull();

            
        }
    }
}
