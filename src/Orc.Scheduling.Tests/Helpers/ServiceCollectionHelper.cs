namespace Orc.Scheduling.Tests
{
    using Catel;
    using Microsoft.Extensions.DependencyInjection;
    using Orc.Scheduling;

    internal static class ServiceCollectionHelper
    {
        public static IServiceCollection CreateServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging();
            serviceCollection.AddCatelCore();
            serviceCollection.AddOrcScheduling();

            return serviceCollection;
        }
    }
}
