﻿using Catel.IoC;
using Catel.Services;
using Orc.Scheduling;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        serviceLocator.RegisterType<ITimeService, TimeService>();
        serviceLocator.RegisterType<ISchedulingService, SchedulingService>();

        var languageService = serviceLocator.ResolveRequiredType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.Scheduling", "Orc.Scheduling.Properties", "Resources"));
    }
}
