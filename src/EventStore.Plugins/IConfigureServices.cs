using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins;

public interface IConfigureServices {
	IApplicationBuilder Configure(IApplicationBuilder builder);
	IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration);
}
