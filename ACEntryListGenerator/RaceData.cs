using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;		   

namespace ACEntryListGenerator
{
	[DebuggerDisplay( "Track: {TrackName} - Sessions: {AllSessions.Count} - Drivers: {Drivers.Count}" )]
	public class RaceData
	{

		public String TrackName
		{
			get;
			set;
		}

		public List<Driver> Drivers { get; } = new List<Driver>();
		public List<CleanSession> PracticeSessions { get; } = new List<CleanSession>();
		public List<CleanSession> QualifySessions { get; } = new List<CleanSession>();
		public List<CleanSession> RaceSessions { get; } = new List<CleanSession>();
		public List<CleanSession> UnknownSessions { get; } = new List<CleanSession>();

		public List<CleanSession> AllSessions { get; } = new List<CleanSession>();

		public static RaceData ParseRawJSONData( RawRaceData rawData )
		{
			RaceData rd = new RaceData();

			rd.TrackName = rawData.track;

			for( int i = 0; i < rawData.players.Length; i++ )
			{
				Player rawDriver = rawData.players[i];
				Driver cleanDriver = new Driver();
				cleanDriver.Car = rawDriver.car;
				cleanDriver.ID = i;
				cleanDriver.Name = rawDriver.name;
				cleanDriver.Skin = rawDriver.skin;

				rd.Drivers.Add( cleanDriver );
			}

			for( int i = 0; i < rawData.sessions.Length; i++ )
			{
				Session ses = rawData.sessions[i];
				CleanSession cSess = new CleanSession();

				rd.AllSessions.Add( cSess );

				cSess.ID = i;
				cSess.Name = $"{ses.name} {ses._event + 1}";
				cSess.Duration = TimeSpan.FromMinutes( ses.duration );
				cSess.TotalLapCount = ses.laps.Length;

				switch( ses.type )
				{
					case 1:
						cSess.Type = CleanSession.ESessionType.Practice;
						rd.PracticeSessions.Add( cSess );
						break;
					case 2:
						cSess.Type = CleanSession.ESessionType.Qualifying;
						rd.QualifySessions.Add( cSess );
						break;
					case 3:
						cSess.Type = CleanSession.ESessionType.Race;
						rd.RaceSessions.Add( cSess );
						break;
					default:
						cSess.Type = CleanSession.ESessionType.Unknown;
						rd.UnknownSessions.Add( cSess );
						break;

				}	

				foreach( Lap rawLap in ses.laps )
				{
					CleanLap cLap = new CleanLap();
					cLap.LapNumber = rawLap.lap; 
					cLap.Time = TimeSpan.FromMilliseconds(rawLap.time);

					Driver driv = rd.Drivers[rawLap.car];
					driv.AddLap( cSess, cLap );
					cLap.Driver = driv;

					cSess.AddDriver( driv );
				}

				foreach( Bestlap rawLap in ses.bestLaps )
				{
					Driver driv = rd.Drivers[rawLap.car];
					CleanLap cLap = driv.LapsBySession[cSess][rawLap.lap];
					driv.BestLapBySession[cSess] = cLap;
				}

				if ( ses.raceResult != null )
				{
					for ( int pos = 0; pos < ses.raceResult.Length; pos++ )
					{
						int id = ses.raceResult[pos];
						Driver driv = rd.Drivers[id];
						cSess.Result[driv] = pos + 1;
					}
				}
			}

			return rd;
		}

	    public RaceData ParseRawTSVData(string[] tsvLines)
	    {
	        RaceData data = new RaceData();


	        return data;
	    }
	}

	[DebuggerDisplay( "{Name}" )]
	public class CleanSession
	{
		public Int32 ID
		{
			get; set;
		}
		public ESessionType Type
		{
			get; set;
		}
		public Int32 TotalLapCount
		{
			get; set;
		}
		public TimeSpan Duration
		{
			get; set;
		}
		public String Name
		{
			get; set;
		}

		public List<Driver> DriversInSession { get; } = new List<Driver>();

		public Dictionary<Driver,Int32> Result { get; } = new Dictionary<Driver, int>();

		public enum ESessionType
		{
			Unknown,
			Practice,
			Qualifying,
			Race
		}

		public void AddDriver( Driver driv )
		{
			if ( !DriversInSession.Contains( driv ) )
				DriversInSession.Add( driv );
		}

		public override string ToString()
		{
			return $"{Name}";
		}
	}

	[DebuggerDisplay( "{Name} - {Car}" )]
	public class Driver
	{
		public String Name
		{
			get; set;
		}
		public Int32 ID
		{
			get; set;
		}
		public String Car
		{
			get; set;
		}
		public String Skin
		{
			get; set;
		}
		public Dictionary<CleanSession, List<CleanLap>> LapsBySession { get; } = new Dictionary<CleanSession, List<CleanLap>>();
		public Dictionary<CleanSession, CleanLap> BestLapBySession { get; } = new Dictionary<CleanSession, CleanLap>();

		public void AddLap( CleanSession cSess, CleanLap cLap )
		{
			if( !LapsBySession.ContainsKey( cSess ) )
				LapsBySession[cSess] = new List<CleanLap>();

			LapsBySession[cSess].Add( cLap );
		}
	}

	[DebuggerDisplay( "{Driver.Name} - {Time}" )]
	public class CleanLap
	{
		public Int32 LapNumber
		{
			get; set;
		}
		public TimeSpan Time
		{
			get; set;
		}

		public Driver Driver
		{
			get; set;
		}
	}
}
