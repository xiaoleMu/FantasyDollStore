using UnityEngine;
using System.Collections;
using System.Linq;

namespace TabTale
{
	public partial class LevelStateModel : SharedStateModel<LevelStateData>
	{
		public LevelStateData GetLevel(string levelConfigId)
		{
			LevelStateData data = _sharedStateItems.FirstOrDefault(l => l.id == levelConfigId);
			if(data == null)
			{
				data = new LevelStateData();
				data.id = levelConfigId;
			}
			return data.Clone() as LevelStateData;
		}

		public bool SetLevel(LevelStateData level)
		{
			return Save(level);
		}

		public GameElementData GetLevelProgress(string levelConfigId, System.Enum key)
		{
			LevelStateData data = _sharedStateItems.FirstOrDefault(l => l.id == levelConfigId);

			if(data != null)
			{
				int index = data.progress.FirstIndex(p => p.key == key.ToString());
				if (  index != -1)
				{
					return data.progress[index].Clone() as GameElementData;
				}
			}

			return GameElementData.CreateState(key.ToString(), 0);

		}

	}
}
