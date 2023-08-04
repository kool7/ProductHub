namespace ProductHub.Domain.Exceptions
{
    public class ValidationErrorResponse
    {
        public int StatusCode { get; set; }
        public string Title { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
    }

}
