using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins.Subsystems;

public interface ISubsystem {
	string Name { get; }
	IApplicationBuilder Configure(IApplicationBuilder builder);
	IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration);
	Task Start();
	Task Stop();
}
