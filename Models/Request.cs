namespace COeX_India1._0.Models
{
    public class Request
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecieverId { get; set; }
        public EPriority? Priority { get; set; } = EPriority.None;
        public EStatus? Status { get; set; } = EStatus.Waiting;
        public DateTime InsertDate { get; set; }
        public enum EPriority
        {
            None = 0,
            Low = 1,
            Mid = 2,
            High = 3,
        }
        public enum EStatus
        {
            Waiting = 0,
            Rejected = 1,
            Accepted = 2
        }
    }
}
