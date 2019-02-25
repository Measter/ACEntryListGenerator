using System.Diagnostics;
using Newtonsoft.Json;

namespace ACEntryListGenerator
{

	public class RawClientRaceData
	{
		public string track
		{
			get; set;
		}
		public int number_of_sessions
		{
			get; set;
		}
		public RawClientPlayer[] players
		{
			get; set;
		}
		public RawClientSession[] sessions
		{
			get; set;
		}
		public RawClientExtra[] extras
		{
			get; set;
		}
	}

	[DebuggerDisplay( "{name} - {car}" )]
    [JsonObject( "player" )]
    public class RawClientPlayer
	{
		public string name
		{
			get; set;
		}
		public string car
		{
			get; set;
		}
		public string skin
		{
			get; set;
		}
	}

	[DebuggerDisplay( "Session: {name}" )]
    [JsonObject( "session" )]
    public class RawClientSession
	{
		public int _event
		{
			get; set;
		}
		public string name
		{
			get; set;
		}
		public int type
		{
			get; set;
		}
		public int lapsCount
		{
			get; set;
		}
		public int duration
		{
			get; set;
		}
		public RawClientLap[] laps
		{
			get; set;
		}
		public int[] lapstotal
		{
			get; set;
		}
		public RawClientBestlap[] bestLaps
		{
			get; set;
		}
		public int[] raceResult
		{
			get; set;
		}
	}

    [JsonObject( "lap" )]
    public class RawClientLap
	{
		public int lap
		{
			get; set;
		}
		public int car
		{
			get; set;
		}
		public object[] sectors
		{
			get; set;
		}
		public int time
		{
			get; set;
		}
	}

    [JsonObject( "bestlap" )]
    public class RawClientBestlap
	{
		public int car
		{
			get; set;
		}
		public int time
		{
			get; set;
		}
		public int lap
		{
			get; set;
		}
	}

    [JsonObject( "extra" )]
    public class RawClientExtra
	{
		public string name
		{
			get; set;
		}
		public int time
		{
			get; set;
		}
	}
}
