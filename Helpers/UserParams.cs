namespace API.Helpers
{
    public class UserParams : PaginationParams
    {
        public string CurrentUsername { get; set; }=string.Empty;
        public string OrderBy { get; set; } = "lastActive";
    }
}
