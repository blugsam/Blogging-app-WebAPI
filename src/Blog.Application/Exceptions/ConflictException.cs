namespace Blog.Application.Exceptions
{
    public class ConflictException : BlogApplicationException
    {
        public ConflictException(string message) : base(message) { }

        public ConflictException(string entityName, object key)
            : base($"Conflict on {entityName}. [Id={key}].") { }
    }
}