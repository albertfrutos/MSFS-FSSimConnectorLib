using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    public class ReferenceSpeeds
    {

        public ReferenceSpeeds(IniFile ini)
        {
            
            this.FullFlapsStallSpeed = ini.Read("full_flaps_stall_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.FlapsUpStallSpeed = ini.Read("flaps_up_stall_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.CruiseSpeed = ini.Read("cruise_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.MaxMach = ini.Read("max_mach", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.MaxIndicatedSpeed = ini.Read("max_indicated_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.MaxFlapsExtended = ini.Read("max_flaps_extended", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.NormalOperatingSpeed = ini.Read("normal_operating_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.AirspeedIndicatorMax = ini.Read("airspeed_indicator_max", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.RotationSpeedMin = ini.Read("rotation_speed_min", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.ClimbSpeed = ini.Read("climb_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.CruiseAlt = ini.Read("cruise_alt", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.TakeoffSpeed = ini.Read("takeoff_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.SpawnCruiseAltitude = ini.Read("spawn_cruise_altitude", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.SpawnDescentAltitude = ini.Read("spawn_descent_altitude", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.BestAngleClimbSpeed = ini.Read("best_angle_climb_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.ApproachSpeed = ini.Read("approach_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.BestGlide = ini.Read("best_glide", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.BestSingleEngineRateOfClimbSpeed = ini.Read("best_single_engine_rate_of_climb_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();
            this.MinimumControlSpeed = ini.Read("minimum_control_speed", "REFERENCE SPEEDS").Split(';')[0].Trim();

        }

        public string FullFlapsStallSpeed { get; set; }
        public string FlapsUpStallSpeed { get; set; }
        public string CruiseSpeed { get; set; }
        public string MaxMach { get; set; }
        public string MaxIndicatedSpeed { get; set; }
        public string MaxFlapsExtended { get; set; }
        public string NormalOperatingSpeed { get; set; }
        public string AirspeedIndicatorMax { get; set; }
        public string RotationSpeedMin { get; set; }
        public string ClimbSpeed { get; set; }
        public string CruiseAlt { get; set; }
        public string TakeoffSpeed { get; set; }
        public string SpawnCruiseAltitude { get; set; }
        public string SpawnDescentAltitude { get; set; }
        public string BestAngleClimbSpeed { get; set; }
        public string ApproachSpeed { get; set; }
        public string BestGlide { get; set; }
        public string BestSingleEngineRateOfClimbSpeed { get; set; }
        public string MinimumControlSpeed { get; set; }
    }
}
