using strange.extensions.signal.impl;
using System.Collections.Generic;

namespace TabTale
{
	public interface ISocialSync 
	{
		void RefreshRelationsScores (string levelConfigId);
		void RefreshRelationships ();
	}

	public class RelationshipsScoresUpdateSignal : Signal<HashSet<string>> {}
}
