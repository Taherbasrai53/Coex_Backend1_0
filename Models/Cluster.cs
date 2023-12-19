namespace COeX_India1._0.Models
{
    public class Cluster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClusterCode { get; set; }
        public string Passcode { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public int AvailableRakes { get; set; }
        public int TentativelyAvailable { get; set; }

        public int LiveRequests { get; set; }
    }
}
