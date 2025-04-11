namespace backend.DTOs.User
{
    public class UserResDto
    {
        public string ?name { get; set; } 
        public string ?phone { get; set; }
        public string ?email { get; set; }
        public string ?address { get; set; }
        public IEnumerable<string> roles { get; set; }
    }
}
