using Serilog;

namespace Blog.Api.Extensions
{
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder AddSerilogApi(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((ctx, lc) => lc
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
            );

            return builder;
        }

        public static void ConfigureBootstrapLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateBootstrapLogger();
        }
    }
}