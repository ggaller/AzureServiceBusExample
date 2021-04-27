using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace MessageSender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<HostService>();
                    services.AddHttpClient<AbcFunctionClient>();
                    services.AddOptions<FunctionSettings>()
                        .BindConfiguration(nameof(FunctionSettings));
                })
                .RunConsoleAsync();
        }      
    }
}
