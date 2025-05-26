namespace Blog.Application.Exceptions
{
    public class ForbiddenException : BlogApplicationException
    {
        public ForbiddenException(string message) : base(message) { }

        public ForbiddenException(string entityName, object key) 
            : base($"You don't have access to {entityName}. [Id={key}].") { }
    }
}