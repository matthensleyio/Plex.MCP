using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using Plex.MCP.Host.Services;
using Plex.MCP.Host.Mcp;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using Serilog;
using System.Diagnostics;

namespace Plex.MCP.Host
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.SetOut(Console.Error);

            // Ensure logs directory exists
            Directory.CreateDirectory("logs");

            // Configure Serilog for detailed error logging (file only, no console to avoid MCP protocol interference)
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/plex-mcp.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 3,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    shared: true)
                .CreateLogger();

            // Print available tools on startup
            PrintAvailableTools();

            var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

            // Parse command line arguments for Plex credentials
            builder.Configuration.AddCommandLine(args, new Dictionary<string, string>
            {
                ["--plex-server-url"] = "Plex:ServerUrl",
                ["--plex-token"] = "Plex:Token"
            });

            // Use Serilog for logging
            builder.Services.AddSerilog();

            builder.Services
                .AddHttpClient()
                .AddScoped<IPlexApiService, PlexApiService>()
                .AddScoped<McpDispatcher>()
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            await builder.Build().RunAsync();
        }

        private static void PrintAvailableTools()
        {
            Console.Error.WriteLine("=== Available Plex MCP Tools ===");
            Console.Error.WriteLine();

            var assembly = Assembly.GetExecutingAssembly();
            var toolTypes = assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<McpServerToolTypeAttribute>() != null)
                .OrderBy(type => type.Name);

            foreach (var toolType in toolTypes)
            {
                Console.Error.WriteLine($"[{toolType.Name}]");

                var methods = toolType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(method => method.GetCustomAttribute<McpServerToolAttribute>() != null)
                    .OrderBy(method => method.Name);

                foreach (var method in methods)
                {
                    var descriptionAttr = method.GetCustomAttribute<DescriptionAttribute>();
                    var description = descriptionAttr?.Description ?? "No description available";

                    Console.Error.WriteLine($"  - {method.Name}: {description}");
                }

                Console.Error.WriteLine();
            }

            Console.Error.WriteLine("=== End of Available Tools ===");
            Console.Error.WriteLine();
        }
    }
}
