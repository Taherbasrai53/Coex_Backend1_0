namespace COeX_India1._0.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public int ClusterId { get; set; }
        public double RemainingDays { get; set; }
        public double Dist { get; set; }
        public double CostFunc { get; set; }
        public int RequesteeClusterId { get; set; }
        
    }
    public class Overflow
    {
        public int OverflowVal { get; set; }
    }
}
