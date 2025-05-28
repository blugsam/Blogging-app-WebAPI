using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;

namespace Blog.Api.Filters
{
    public class ValidationFilterAttribute : IAsyncActionFilter
    {
        private static readonly ConcurrentDictionary<Type, IValidator?> _validatorCache = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ValidationFilterAttribute> _logger;

        public ValidationFilterAttribute(IServiceProvider serviceProvider, ILogger<ValidationFilterAttribute> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var validationFailures = new List<ValidationFailure>();

            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value == null) continue;

                var modelToValidate = argument.Value;
                var modelType = modelToValidate.GetType();

                if (!_validatorCache.TryGetValue(modelType, out var validator))
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
                    validator = _serviceProvider.GetService(validatorType) as IValidator;
                    _validatorCache.TryAdd(modelType, validator);
                }

                if (validator == null)
                {
                    continue;
                }

                var validationContext = new ValidationContext<object>(modelToValidate);
                var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                if (!validationResult.IsValid)
                {
                    validationFailures.AddRange(validationResult.Errors);
                }
            }

            if (validationFailures.Any())
            {
                var errorsDictionary = validationFailures
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        grouping => ToCamelCase(grouping.Key),
                        grouping => grouping.Select(e => e.ErrorMessage).ToArray()
                    );

                _logger.LogWarning("Validation failed for action {ActionName} in controller {ControllerName}. Errors: {@ValidationErrors}",
                    actionDescriptor?.ActionName,
                    actionDescriptor?.ControllerName,
                    errorsDictionary);

                context.Result = new ObjectResult(new ValidationProblemDetails(errorsDictionary)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
                return;
            }

            await next();
        }

        private static string ToCamelCase(string? str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
                return str ?? string.Empty;

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}