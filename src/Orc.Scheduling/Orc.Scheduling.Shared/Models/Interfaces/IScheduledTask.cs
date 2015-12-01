// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunningTask.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Threading.Tasks;

    public interface IScheduledTask : ICloneable
    {
        string Name { get; set; }
        DateTime Start { get; set; }
        TimeSpan? Recurring { get; set; }
        TimeSpan MaximumDuration { get; set; }

        Task InvokeAsync();
    }
}