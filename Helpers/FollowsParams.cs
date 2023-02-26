namespace API.Helpers
{
    public class FollowsParams : PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}
