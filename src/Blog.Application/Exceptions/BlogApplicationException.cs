namespace Blog.Application.Exceptions
{
    public abstract class BlogApplicationException : Exception
    {
        protected BlogApplicationException(string message) : base(message) { }
    }
}