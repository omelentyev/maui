﻿using System;
using System.Collections.Generic;
using System.Text;


namespace Microsoft.Caboodle
{
    public static partial class Battery
    {
        static Windows.Devices.Power.Battery DefaultBattery => 
            Windows.Devices.Power.Battery.AggregateBattery;

        public static double ChargeLevel
        {
            get
            {
                var finalReport = DefaultBattery.GetReport();
                var finalPercent = -1.0;

                if (finalReport.RemainingCapacityInMilliwattHours.HasValue && finalReport.FullChargeCapacityInMilliwattHours.HasValue)
                {
                    finalPercent = (double)finalReport.RemainingCapacityInMilliwattHours.Value /
                                     (double)finalReport.FullChargeCapacityInMilliwattHours.Value;
                }
                return finalPercent;
            }
        }
        public static BatteryState State
        {
            get
            {
                var report = DefaultBattery.GetReport();

                switch (report.Status)
                {
                    case Windows.System.Power.BatteryStatus.Charging:
                        return BatteryState.Charging;
                    case Windows.System.Power.BatteryStatus.Discharging:
                        return BatteryState.Discharging;
                    case Windows.System.Power.BatteryStatus.Idle:
                        return BatteryState.NotCharging;
                    case Windows.System.Power.BatteryStatus.NotPresent:
                        return BatteryState.Unknown;
                }
                
                if (ChargeLevel >= 1.0)
                    return BatteryState.Full;

                return BatteryState.Unknown;
            }
        }
        public static BatteryPowerSource PowerSource
        {
            get
            {
                var currentStatus = State;
                if (currentStatus == BatteryState.Full || currentStatus == BatteryState.Charging)
                    return BatteryPowerSource.Ac;

                return BatteryPowerSource.Battery;
            }
        }
    }
}