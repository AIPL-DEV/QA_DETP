namespace DETP.model
{
    public class CurrentFlow : BaseModel
    {
        public string Department { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int FromId { get; set; }
        public int? SubmitTo { get; set; }
        public string ObservationId { get; set; }
        public int FlowId { get; set; }
        public bool Critical { get; set; }
        public string CurrentState { get; set; }
        public bool JobStopped { get; set; }
    }
}
