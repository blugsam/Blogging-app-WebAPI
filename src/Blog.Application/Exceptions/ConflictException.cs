namespace Blog.Application.Exceptions
{
    public class ConflictException : BlogApplicationException
    {
        public ConflictException(string message) : base(message) { }
    }
}