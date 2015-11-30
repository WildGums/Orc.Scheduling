﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimeService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling
{
    using System;
    using System.Threading.Tasks;

    public interface ITimeService
    {
        #region Properties
        TimeSpan MinuteDuration { get; }
        DateTime CurrentDateTime { get; }
        #endregion

        #region Methods
        Task WaitAsync(TimeSpan timeSpan);
        #endregion
    }
}