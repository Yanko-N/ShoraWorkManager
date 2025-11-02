namespace Application.Contracts.Response
{
    public class WorkedHoursDto
    {
        public int WorkerId { get; set; }
        public string WorkerName { get; set; }
        public float TotalHours { get; set; }
        public float UnpaidHours { get; set; }
        public float PaidHours { get; set; }
    }
}
