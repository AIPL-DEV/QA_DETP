using FluentValidation;
using OfficeOpenXml.FormulaParsing.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages;

namespace DETP.requests.editObservation
{
    public class DeptHodCreateRequest: ObservationFlowRequest
    {
        
        public string Remarks { get; set; }
        public string ValueRec { get; set; }
        public string TimeLoss { get; set; }
        public string TimeVal { get; set; }
        public string Decision { get; set; }
        public int? AssignedToId { get; set; }
        public int FormType { get; set; }

        public FluentValidation.Results.ValidationResult Validate()
        {
            var validator = new DeptHodCreateRequestValidator();
            return validator.Validate(this);
        }


    }

    public class DeptHodCreateRequestValidator : AbstractValidator<DeptHodCreateRequest>
    {
        public DeptHodCreateRequestValidator()
        {
            RuleFor(x => x.FromId).NotNull();
            RuleFor(x => x.ToId).NotNull();
            RuleFor(x => x.ObservationId).NotNull();
            RuleFor(x => x.FlowId).NotNull();
            
            RuleFor(x => x.Remarks).NotNull();
            RuleFor(x => x.ValueRec).Must((x, val) => ValidateValueRec(x, val));
            RuleFor(x => x.TimeVal).Must((x, val) => ValidateTimeVal(x, val)); ;
            RuleFor(x => x.TimeLoss).Must((x, val) => ValidateTimeLoss(x, val));
            RuleFor(x => x.Att);
            RuleFor(x => x.AssignedToId).Must((x, val) => ValidateAssignToId(x, val));
            RuleFor(x => x.Decision).Must((x, decision) => ValidateDecision(x,decision));
        }
        private static bool ValidateDecision(DeptHodCreateRequest x, string decision)
        {
            if(x.FormType == 2)
            {
                if(decision.IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }
        private static bool ValidateValueRec(DeptHodCreateRequest x, string remarks)
        {
            if(x.FormType == 1)
            {
                if (x.ValueRec.IsEmpty())
                {
                    return false;
                }
                if(!double.TryParse(x.ValueRec, out _))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidateTimeVal(DeptHodCreateRequest x, string remarks)
        {
            if(x.FormType == 1)
            {
                if(x.TimeVal.IsEmpty())
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateTimeLoss(DeptHodCreateRequest x, string remarks)
        {
            if(x.FormType == 1)
            {
                if(x.TimeLoss.IsEmpty())
                {
                    return false;
                }
                if(!double.TryParse(x.TimeLoss, out _))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidateAssignToId(DeptHodCreateRequest x, int? value)
        {
            if(x.FormType == 3)
            {
                if(x.AssignedToId == null)
                {
                    return false;
                }
            }   
            return true;
        }
    }


}
