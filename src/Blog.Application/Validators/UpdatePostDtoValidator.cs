using Blog.Application.Contracts.PostDtos;
using FluentValidation;

namespace Blog.Application.Validators
{
    public class UpdatePostDtoValidator : AbstractValidator<UpdatePostDto>
    {
        public UpdatePostDtoValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(p => p.Content)
                .NotEmpty().WithMessage("Content is required.");

            RuleFor(p => p.Excerpt)
                .MaximumLength(500).WithMessage("Excerpt must not exceed 500 characters.");


            RuleFor(p => p.TagNames)
                .NotNull().WithMessage("TagNames list cannot be null.")
                .Must(tags => tags == null || tags.All(tag => !string.IsNullOrWhiteSpace(tag)))
                    .WithMessage("All tags in TagNames list must be non-empty strings.")
                .Must(tags => tags == null || tags.Count == tags.Distinct(StringComparer.OrdinalIgnoreCase).Count())
                    .WithMessage("Tag names must be unique (case-insensitive).");
        }
    }
}