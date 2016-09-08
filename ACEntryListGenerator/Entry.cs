using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACEntryListGenerator.Annotations;

namespace ACEntryListGenerator
{
    [DebuggerDisplay( "{Name} - {CarModel}" )]
	public class Entry : INotifyPropertyChanged
	{
		#region NameEqualityComparer

		private sealed class NameEqualityComparer : IEqualityComparer<Entry>
		{
			public bool Equals( Entry x, Entry y )
			{
				if ( ReferenceEquals( x, y ) ) return true;
				if ( ReferenceEquals( x, null ) ) return false;
				if ( ReferenceEquals( y, null ) ) return false;
				if ( x.GetType() != y.GetType() ) return false;
				return string.Equals( x.m_name, y.m_name );
			}

			public int GetHashCode( Entry obj )
			{
				return obj.m_name?.GetHashCode() ?? 0;
			}
		}

		public static IEqualityComparer<Entry> NameComparer { get; } = new NameEqualityComparer();

		#endregion

		private string m_name;
		private string m_team;
		private string m_carModel;
		private string m_carSkin;
		private string m_guid;
		private string m_ballast;
		private bool m_hadBallastTag;
		private bool m_isSpectator;
		private string m_fixedSetup;
		private bool m_hadFixedTag;
		private int m_racePosition;
		private TimeSpan m_lapTime;
        private Region m_region;

        public string Name
		{
			get { return m_name; }
			set
			{
				if ( value == m_name ) return;
				m_name = value;
				OnPropertyChanged();
			}
		}

		public string Team
		{
			get { return m_team; }
			set
			{
				if ( value == m_team ) return;
				m_team = value;
				OnPropertyChanged();
			}
		}

		public string CarModel
		{
			get { return m_carModel; }
			set
			{
				if ( value == m_carModel ) return;
				m_carModel = value;
				OnPropertyChanged();
			}
		}

		public string CarSkin
		{
			get { return m_carSkin; }
			set
			{
				if ( value == m_carSkin ) return;
				m_carSkin = value;
				OnPropertyChanged();
			}
		}

		public string GUID
		{
			get { return m_guid; }
			set
			{
				if ( value == m_guid ) return;
				m_guid = value;
				OnPropertyChanged();
			}
		}

		public string Ballast
		{
			get { return m_ballast; }
			set
			{
				if ( value == m_ballast ) return;
				m_ballast = value;
				OnPropertyChanged();
			}
		}

		public bool HadBallastTag
		{
			get { return m_hadBallastTag; }
			set
			{
				if ( value == m_hadBallastTag ) return;
				m_hadBallastTag = value;
				OnPropertyChanged();
			}
		}

		public bool IsSpectator
		{
			get { return m_isSpectator; }
			set
			{
				if ( value == m_isSpectator ) return;
				m_isSpectator = value;
				OnPropertyChanged();
			}
		}

        public Region Region
        {
            get { return m_region; }
            set
            {
                if (value == m_region) return;
                m_region = value;
                OnPropertyChanged();
            }
        }


        public string FixedSetup
		{
			get { return m_fixedSetup; }
			set
			{
				if ( value == m_fixedSetup ) return;
				m_fixedSetup = value;
				OnPropertyChanged();
			}
		}

		public bool HadFixedTag
		{
			get { return m_hadFixedTag; }
			set
			{
				if ( value == m_hadFixedTag ) return;
				m_hadFixedTag = value;
				OnPropertyChanged();
			}
		}

		public int RacePosition
		{
			get { return m_racePosition; }
			set
			{
				if ( value == m_racePosition ) return;
				m_racePosition = value;
				OnPropertyChanged();
			}
		}

		public TimeSpan LapTime
		{
			get { return m_lapTime; }
			set
			{
				if ( value.Equals( m_lapTime ) ) return;
				m_lapTime = value;
				OnPropertyChanged();
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}


		public static List<Entry> ParseEntryList( string[] lines )
		{
			List<Entry> drivers = new List<Entry>();

			Entry curDriver = null;

			foreach( string l in lines )
			{
				string lLower = l.ToLower();

				if( lLower.StartsWith( "[car_" ) )
				{
					curDriver = new Entry();
                    curDriver.Region = Region.None;
					drivers.Add( curDriver );
				} else if( lLower.StartsWith( "drivername=" ) )
				{
					curDriver.Name = l.Split( '=' )[1].Trim();
				} else if( lLower.StartsWith( "team=" ) )
				{
					curDriver.Team = l.Split( '=' )[1].Trim();
				} else if( lLower.StartsWith( "model=" ) )
				{
					curDriver.CarModel = l.Split( '=' )[1].Trim();
				} else if( lLower.StartsWith( "skin=" ) )
				{
					curDriver.CarSkin = l.Split( '=' )[1].Trim();
				} else if( lLower.StartsWith( "guid=" ) )
				{
					curDriver.GUID = l.Split( '=' )[1].Trim();
				} else if( lLower.StartsWith( "ballast=" ) )
				{
					curDriver.Ballast = l.Split( '=' )[1].Trim();
					curDriver.HadBallastTag = true;
				} else if( lLower.StartsWith( "spectator_mode=" ) )
				{
					curDriver.IsSpectator = l.Split( '=' )[1].Trim() == "1";
				} else if( lLower.StartsWith( "fixed_setup=" ) )
				{
					curDriver.FixedSetup = l.Split( '=' )[1].Trim();
					curDriver.HadFixedTag = true;
				}
			}


			return drivers;
		}

		public static void SaveEntryList( List<Entry> reversedGrid, string fileName )
		{
			using( StreamWriter sw = new StreamWriter( fileName, false, new UTF8Encoding(false) ) )
			{
				for( int i = 0; i < reversedGrid.Count; i++ )
				{
					Entry d = reversedGrid[i];

					sw.WriteLine( $"[CAR_{i}]" );
					sw.WriteLine( $"DRIVERNAME={d.Name}" );
					sw.WriteLine( $"TEAM={d.Team}" );
					sw.WriteLine( $"MODEL={d.CarModel}" );
					sw.WriteLine( $"SKIN={d.CarSkin}" );
					sw.WriteLine( $"GUID={d.GUID}" );

					sw.WriteLine( "SPECTATOR_MODE={0}", d.IsSpectator ? "1" : "0" );
					if( d.HadBallastTag )
						sw.WriteLine( $"BALLAST={d.Ballast}" );

					if( d.HadFixedTag )
						sw.WriteLine( $"FIXED_SETUP={d.FixedSetup}" );

					sw.WriteLine();
				}
			}
		}

        public static void SaveEntryListSimpleSplit(List<Entry> entries, int maxPerServer, int minCarsPerClass, Dictionary<string, string> classes, string folderPath)
        {
            Dictionary<string, List<Entry>> entriesByClass = GetEntryListByClass(entries, classes);

            List<List<Entry>> splitList = new List<List<Entry>>();
            int serverCount = (int)Math.Ceiling(entries.Count/(float) maxPerServer);

            for (int i = 0; i < serverCount; i++)
                splitList.Add(new List<Entry>());

            foreach (KeyValuePair<string, List<Entry>> pair in entriesByClass)
            {
                List<List<Entry>> classSplit = SplitClassByServerCount(pair.Value, serverCount, minCarsPerClass);

                for (int i = 0; i < classSplit.Count; i++)
                    splitList[i].AddRange(classSplit[i]);
            }

            foreach (string key in entriesByClass.Keys)
            {
                List<Entry> classList = entriesByClass[key];

                while (classList.Count > 0)
                {
                    List<Entry> slowest = splitList.Last();

                    slowest.Add( classList.First() );
                    classList = classList.Skip( 1 ).ToList();
                }
            }

            for (int i = 0; i < splitList.Count; i++)
            {
                string filename = Path.Combine(folderPath, $"entry_list_{i}.ini");
                SaveEntryList(splitList[i], filename);
            }
        }

        private static List<List<Entry>> SplitClassByServerCount(List<Entry> entries, int serverCount, int minCarsPerClass)
        {
            List<Entry> localList = new List<Entry>(entries);

            List<List<Entry>> splitList = new List<List<Entry>>();
            for( int i = 0; i < serverCount; i++ )
                splitList.Add( new List<Entry>() );

            while ( entries.Count()/serverCount < minCarsPerClass)
                serverCount--;

            int carsPerSplit = entries.Count()/serverCount;

            for (int i = 0; i < serverCount; i++)
            {
                splitList[i].AddRange( localList.Take(carsPerSplit));
                localList = localList.Skip(carsPerSplit).ToList();
            }

            // Modify the outside list so the parent can better balance the leftovers.
            entries.Clear();
            entries.AddRange(localList);

            return splitList;
        }

        private static Dictionary<string, List<Entry>> GetEntryListByClass(List<Entry> entries, Dictionary<string, string> classes)
        {
            return entries.GroupBy(e => classes.ContainsKey(e.CarModel) ? classes[e.CarModel] : "other").ToDictionary(e => e.Key, v => v.ToList());
        }
	}
}
