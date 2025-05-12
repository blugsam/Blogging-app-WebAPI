using Blog.Api.ErrorHandling;
using Blog.Api.Extensions;
using Blog.Api.Filters;
using Blog.Application.Interfaces.Persistence;
using Blog.Application.Services;
using Blog.Application.Validators;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

SerilogExtensions.ConfigureBootstrapLogger();

try
{
    Log.Information("Starting Blog API host");

    var builder = WebApplication.CreateBuilder(args);

    builder.AddSerilogApi();

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddAutoMapper(typeof(Blog.Application.Mappings.MappingProfile).Assembly);

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    builder.Services.AddScoped<IPostRepository, PostRepository>();
    builder.Services.AddScoped<ITagRepository, TagRepository>();
    builder.Services.AddScoped<IPostService, PostService>();
    builder.Services.AddScoped<ITagService, TagService>();

    builder.Services.AddValidatorsFromAssemblyContaining<CreatePostDtoValidator>(ServiceLifetime.Scoped);

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilterAttribute>();
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
return 0;