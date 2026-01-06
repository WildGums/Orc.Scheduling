namespace Orc
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Orc.Scheduling;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcSchedulingModule
    {
        public static IServiceCollection AddOrcScheduling(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ITimeService, TimeService>();
            serviceCollection.TryAddSingleton<ISchedulingService, SchedulingService>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.Scheduling", "Orc.Scheduling.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.Scheduling", "https://github.com/wildgums/orc.scheduling"));

            return serviceCollection;
        }
    }
}
