namespace LogglyAPI.Configuration
{
    public class LogglyConfig
    {
        public LogglyConfig(string account, string username, string password)
        {
            this.Account = account;
            this.Username = username;
            this.Password = password;
        }

        public LogglyConfig()
        {

        }

        public string Account { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
