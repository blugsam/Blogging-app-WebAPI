namespace Blog.Application.Exceptions
{
    public class NotFoundException : BlogApplicationException
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key was not found. [Id={key}].") { }
    }
}