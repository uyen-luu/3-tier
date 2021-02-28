namespace ApplicationTier.Domain.Models
{
	public class ErrorViewModel
    {
        public string Key { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }

        public ErrorViewModel()
        {

        }

        public ErrorViewModel(string message)
        {
            Key = message;
            Message = message;
        }

        public ErrorViewModel(string message, string detail) : this(message)
        {
            Detail = detail;
        }
    }
}
