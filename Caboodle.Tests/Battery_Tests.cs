﻿using Microsoft.Caboodle;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Caboodle.Tests
{
    public class Battery_Tests
    {
        [Fact]
        public void Charge_Level_On_NetStandard() =>
            Assert.Throws<NotImplentedInReferenceAssembly>(() => Battery.ChargeLevel);

        [Fact]
        public void Charge_State_On_NetStandard() =>
            Assert.Throws<NotImplentedInReferenceAssembly>(() => Battery.State);

        [Fact]
        public void Charge_Power_Source_On_NetStandard() =>
            Assert.Throws<NotImplentedInReferenceAssembly>(() => Battery.PowerSource);
    }
}
