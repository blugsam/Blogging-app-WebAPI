namespace Blog.Application.Exceptions
{
    public class SlugGenerationException : BlogApplicationException
    {
        public SlugGenerationException(string message) : base(message) { }

        public SlugGenerationException(string title, int attempts)
            : base($"Could not generate a unique slug for title '{title}' after {attempts} attempts.") { }
    }
}
