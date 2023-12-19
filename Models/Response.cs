namespace COeX_India1._0.Models
{
    public class Response
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public Response(bool success, string msg)
        {
            this.success = success;
            this.msg = msg;
        }
    }
    public class LoginResponse
    {
        public bool success { get; set; }
        public string token { get; set; }
        public LoginResponse(bool success, string token)
        {
            this.success = success;
            this.token = token;
        }
    }
}
