using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace ACEntryListGenerator
{
	class EntryList
	{
		public ObservableCollection<Entry> Entries
		{
			get;
		}
        
	    public event EventHandler<int> EntryListChanged;

		private Random m_rand = new Random();

		public EntryList()
		{
			Entries = new ObservableCollection<Entry>();
		}

		private void UpdateEntryList( List<Entry> newList )
		{
		    var oldLen = Entries.Count;

		    Entries.Clear();

			foreach( Entry entry in newList )
				Entries.Add( entry );

		    if( oldLen != Entries.Count )
		        OnEntryListLengthChanged();
		}

		public void AddEntries( IEnumerable<Entry> newEntries )
		{
		    var oldLen = Entries.Count;
		    var uniqueEntries = newEntries.Except( Entries, Entry.NameComparer ).ToList();

			// Observable collection has no way to add a list of items to it.
			foreach( Entry e in uniqueEntries )
				Entries.Add( e );

		    if( oldLen != Entries.Count )
		        OnEntryListLengthChanged();
		}

		public void DeleteEntries( IEnumerable<Entry> deletedEntries )
		{
			List<Entry> localEntries = Entries.Except( deletedEntries ).ToList();
			UpdateEntryList( localEntries );
		}

		public void UpdateSessionData( CleanSession ses, bool clearMissingDriverData )
		{
			Dictionary<Entry, Driver> entryListForSession = new Dictionary<Entry, Driver>();

			foreach ( Driver sD in ses.DriversInSession )
			{																								   
				Entry curEntry = Entries.FirstOrDefault( eD => eD.Name == sD.Name );
				if ( curEntry != null )
					entryListForSession[curEntry] = sD;
			}

			List<Entry> missingFromSession = Entries.Except( entryListForSession.Keys ).ToList();

			foreach( var driver in entryListForSession )
			{
				driver.Key.LapTime = driver.Value.BestLapBySession[ses].Time;

				if( ses.Result.ContainsKey( driver.Value ) )
					driver.Key.RacePosition = ses.Result[driver.Value];
				else if( clearMissingDriverData )
					driver.Key.RacePosition = 99;
			}

			if( clearMissingDriverData )
			{
				foreach( Entry driver in missingFromSession )
				{
					driver.LapTime = TimeSpan.FromMinutes( 59 );
					driver.RacePosition = 99;
				}
			}
		}

	    private void OnEntryListLengthChanged()
	    {
	        var handler = EntryListChanged;
	        handler?.Invoke( this, Entries.Count );
	    }


		private List<Entry> ReverseEntries( List<Entry> entries, int reverseUpTo )
		{
			if( reverseUpTo == 1 )
				return entries;

			List<Entry> reversedGrid = entries.Take( reverseUpTo ).ToList();
			reversedGrid.Reverse();
			reversedGrid.AddRange( entries.Skip( reverseUpTo ) );

			return reversedGrid;
		}

		public void SortEntryListByPosition( int reverseUpTo )
		{
			var sortedEntries = Entries.OrderBy( e => e.RacePosition ).ToList();
			sortedEntries = ReverseEntries( sortedEntries, reverseUpTo );

			UpdateEntryList( sortedEntries );
		}

		public void SortEntryListByLapTime( int reverseUpTo )
		{
			var sortedEntries = Entries.OrderBy( e => e.LapTime ).ToList();
			sortedEntries = ReverseEntries( sortedEntries, reverseUpTo );

			UpdateEntryList( sortedEntries );
		}

		public void SortEntryListByRandom()
		{
			var sortedEntries = new List<Entry>( Entries );

			for( int i = sortedEntries.Count - 1; i > 0; i-- )
			{
				int selected = m_rand.Next( i );

				Entry temp = sortedEntries[selected];
				sortedEntries[selected] = sortedEntries[i];
				sortedEntries[i] = temp;
			}

			UpdateEntryList( sortedEntries );
		}
	}
}
