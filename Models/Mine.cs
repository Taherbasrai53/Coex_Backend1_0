using System.Diagnostics.Eventing.Reader;

namespace COeX_India1._0.Models
{
    public class Mine
    {
        public int Id { get; set; }
        public int ClusterId { get; set; }
        public string MineCode { get; set; }
        public string Name { get; set; }
        public string Passcode { get; set; }
        public double YieldPerDay { get; set; }
        public double CurrYield { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public double? TriggerYield { get; set; }
        public EAllocation? AllocationStatus { get; set; } = EAllocation.Pending;
        public enum EAllocation
        {
            Pending=0,
            Alloted=1
        }
    }
}
