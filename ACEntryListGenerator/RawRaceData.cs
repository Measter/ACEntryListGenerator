using System.Diagnostics;

namespace ACEntryListGenerator
{

	public class RawRaceData
	{
		public string track
		{
			get; set;
		}
		public int number_of_sessions
		{
			get; set;
		}
		public Player[] players
		{
			get; set;
		}
		public Session[] sessions
		{
			get; set;
		}
		public Extra[] extras
		{
			get; set;
		}
	}

	[DebuggerDisplay( "{name} - {car}" )]
	public class Player
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
	public class Session
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
		public Lap[] laps
		{
			get; set;
		}
		public int[] lapstotal
		{
			get; set;
		}
		public Bestlap[] bestLaps
		{
			get; set;
		}
		public int[] raceResult
		{
			get; set;
		}
	}

	public class Lap
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

	public class Bestlap
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

	public class Extra
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
