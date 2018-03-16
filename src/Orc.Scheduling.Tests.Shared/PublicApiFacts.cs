// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Scheduling.Tests
{
    using ApiApprover;
    using NUnit.Framework;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test]
        public void Orc_Scheduling_HasNoBreakingChanges()
        {
            var assembly = typeof(SchedulingService).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}