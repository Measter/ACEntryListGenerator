using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ACEntryListGenerator
{
    public class RawServerRaceData
    {
        [JsonProperty( "TrackName" )]
        public string TrackName
        {
            get; set;
        }
        [JsonProperty( "TrackConfig" )]
        public string TrackConfig
        {
            get; set;
        }
        [JsonProperty( "Type" )]
        public string Type
        {
            get; set;
        }
        [JsonProperty( "DurationSecs" )]
        public int DurationSecs
        {
            get; set;
        }
        [JsonProperty( "RaceLaps" )]
        public int RaceLaps
        {
            get; set;
        }
        [JsonProperty( "Cars" )]
        public List<RawServerCar> Cars
        {
            get; set;
        }
        [JsonProperty( "Result" )]
        public List<RawServerResult> Result
        {
            get; set;
        }
        [JsonProperty( "Laps" )]
        public List<RawServerLap> Laps
        {
            get; set;
        }
        [JsonProperty( "Events" )]
        public List<RawServerEvent> Events
        {
            get; set;
        }
    }

    [DebuggerDisplay( "{Driver.Name} - {Model}" )]
    [JsonObject( "Car" )]
    public class RawServerCar
    {
        [JsonProperty( "CarId" )]
        public int CarId
        {
            get; set;
        }
        [JsonProperty( "Driver" )]
        public RawServerDriver Driver
        {
            get; set;
        }
        [JsonProperty( "Model" )]
        public string Model
        {
            get; set;
        }
        [JsonProperty( "Skin" )]
        public string Skin
        {
            get; set;
        }
        [JsonProperty( "BallastKG" )]
        public int BallastKG
        {
            get; set;
        }
    }

    [JsonObject( "RawServerDriver" )]
    public class RawServerDriver
    {
        [JsonProperty( "Name" )]
        public string Name
        {
            get; set;
        }
        [JsonProperty( "Team" )]
        public string Team
        {
            get; set;
        }
        [JsonProperty( "Guid" )]
        public string Guid
        {
            get; set;
        }
    }

    [JsonObject( "Result" )]
    public class RawServerResult
    {
        [JsonProperty( "DriverName" )]
        public string DriverName
        {
            get; set;
        }
        [JsonProperty( "DriverGuid" )]
        public string DriverGuid
        {
            get; set;
        }
        [JsonProperty( "CarId" )]
        public int CarId
        {
            get; set;
        }
        [JsonProperty( "CarModel" )]
        public string CarModel
        {
            get; set;
        }
        [JsonProperty( "BestLap" )]
        public int BestLap
        {
            get; set;
        }
        [JsonProperty( "TotalTime" )]
        public int TotalTime
        {
            get; set;
        }
        [JsonProperty( "BallastKG" )]
        public int BallastKG
        {
            get; set;
        }
    }

    [JsonObject( "Lap" )]
    public class RawServerLap
    {
        [JsonProperty( "DriverName" )]
        public string DriverName
        {
            get; set;
        }
        [JsonProperty( "DriverGuid" )]
        public string DriverGuid
        {
            get; set;
        }
        [JsonProperty( "CarId" )]
        public int CarId
        {
            get; set;
        }
        [JsonProperty( "CarModel" )]
        public string CarModel
        {
            get; set;
        }
        [JsonProperty( "Timestamp" )]
        public int Timestamp
        {
            get; set;
        }
        [JsonProperty( "LapTime" )]
        public int LapTime
        {
            get; set;
        }
        [JsonProperty( "Sectors" )]
        public List<int> Sectors
        {
            get; set;
        }
        [JsonProperty( "Cuts" )]
        public int Cuts
        {
            get; set;
        }
        [JsonProperty( "BallastKG" )]
        public int BallastKG
        {
            get; set;
        }
    }

    [JsonObject( "Event" )]
    public class RawServerEvent
    {
        [JsonProperty( "Type" )]
        public string Type
        {
            get; set;
        }
        [JsonProperty( "CarId" )]
        public int CarId
        {
            get; set;
        }
        [JsonProperty( "RawServerDriver" )]
        public Driver Driver
        {
            get; set;
        }
        [JsonProperty( "OtherCarId" )]
        public int OtherCarId
        {
            get; set;
        }
        [JsonProperty( "OtherDriver" )]
        public RawServerOtherDriver OtherDriver
        {
            get; set;
        }
        [JsonProperty( "ImpactSpeed" )]
        public double ImpactSpeed
        {
            get; set;
        }
        [JsonProperty( "WorldPosition" )]
        public RawServerWorldPosition WorldPosition
        {
            get; set;
        }
        [JsonProperty( "RelPosition" )]
        public RawServerRelPosition RelPosition
        {
            get; set;
        }
    }

    [JsonObject( "OtherDriver" )]
    public class RawServerOtherDriver
    {
        [JsonProperty( "Name" )]
        public string Name
        {
            get; set;
        }
        [JsonProperty( "Team" )]
        public string Team
        {
            get; set;
        }
        [JsonProperty( "Guid" )]
        public string Guid
        {
            get; set;
        }
    }

    [JsonObject( "WorldPosition" )]
    public class RawServerWorldPosition
    {
        [JsonProperty( "X" )]
        public double X
        {
            get; set;
        }
        [JsonProperty( "Y" )]
        public double Y
        {
            get; set;
        }
        [JsonProperty( "Z" )]
        public double Z
        {
            get; set;
        }
    }

    [JsonObject( "RelPosition" )]
    public class RawServerRelPosition
    {
        [JsonProperty( "X" )]
        public double X
        {
            get; set;
        }
        [JsonProperty( "Y" )]
        public double Y
        {
            get; set;
        }
        [JsonProperty( "Z" )]
        public double Z
        {
            get; set;
        }
    }

}
