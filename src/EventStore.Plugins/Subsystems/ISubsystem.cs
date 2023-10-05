using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins.Subsystems;

public interface ISubsystem {
	IApplicationBuilder Configure(IApplicationBuilder builder);
	IServiceCollection ConfigureServices(IServiceCollection services);
	IEnumerable<Task> Start();
	void Stop();
}
