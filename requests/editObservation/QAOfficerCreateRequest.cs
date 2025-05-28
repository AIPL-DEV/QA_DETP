using FluentValidation;

namespace DETP.requests.editObservation
{
    public class QAOfficerCreateRequest: ObservationFlowRequest
    {
        public string ComplianceSatisfactory { get; set; }
        public string Remarks { get; set; }
        public string WithinSlg { get; set; }
        public bool IsValid
        {
            get
            {
                return Validate().IsValid;
            }
            private set { }
        }

        public FluentValidation.Results.ValidationResult Validate()
        {
            var validator = new QAOfficerCreateRequestValidator();
            return validator.Validate(this);
        }
    }

    public class QAOfficerCreateRequestValidator : AbstractValidator<QAOfficerCreateRequest>
    {
        public QAOfficerCreateRequestValidator()
        {
            RuleFor(x => x.FromId).NotNull();
            RuleFor(x => x.ToId).NotNull();
            RuleFor(x => x.ObservationId).NotNull();
            RuleFor(x => x.FlowId).NotNull();

            RuleFor(x => x.Remarks)
                .NotNull();
            RuleFor(x => x.ComplianceSatisfactory).NotNull();
            RuleFor(x => x.WithinSlg).NotNull().Must((x,y) => x.WithinSlg == "Yes" || x.WithinSlg == "No");
            RuleFor(x => x.Att);
        }
    }
}
