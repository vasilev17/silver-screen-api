namespace SilverScreen.Models
{
    public class UserStat
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int FakeReports { get; set; }
        public int Reports { get; set; }
        public int Warnings { get; set; }
    }
}
