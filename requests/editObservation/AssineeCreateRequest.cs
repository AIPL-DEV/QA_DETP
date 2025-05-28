using FluentValidation;
using OfficeOpenXml.FormulaParsing.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DETP.requests.editObservation
{
    public class AssigneeCreateRequest : ObservationFlowRequest
    {

        public string ObservationDetails { get; set; }
        public string RootCauseAnalysis { get; set; }
        public string CorrectiveAction { get; set; }
        public string PreventiveAction { get; set; }
        public string ValueOfRectification { get; set; }
        public string TimeLoss { get; set; }
        public string TimeValue { get; set; }

        public FluentValidation.Results.ValidationResult Validate()
        {
            var validator = new AssigneeCreateRequestValidator();
            return validator.Validate(this);
        }


    }

    public class AssigneeCreateRequestValidator : AbstractValidator<AssigneeCreateRequest>
    {
        public AssigneeCreateRequestValidator()
        {
            RuleFor(x => x.FromId).NotNull();
            RuleFor(x => x.ToId).NotNull();
            RuleFor(x => x.ObservationId).NotNull();
            RuleFor(x => x.FlowId).NotNull();


        }
    }
}
