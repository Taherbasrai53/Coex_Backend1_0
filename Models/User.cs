namespace COeX_India1._0.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int ClusterId { get; set; }
        public int? MineId { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public EUserType? UserType { get; set; }
        public enum EUserType
        {
            ClusterManager=0,
            MineManager=1,
            Admin=2,
        }
    }
    public class ClusterLoginModel
    {
        public int ClusterId { get; set; }
        public string passcode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }    
    }
    public class MineLoginModel
    {
        public int MineId { get; set; }
        public string passcode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
}
