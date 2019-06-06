using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public partial class CategoryConfigModel : ConfigModel<CategoryConfigData>
	{
		#region Getters
		public string GetName(string id)
		{
			return _configs.FirstOrDefault(x => x.GetId() == id).name;
		}

		public int GetIndex(string id)
		{
			return _configs.FirstOrDefault(x => x.GetId() == id).index;
		}

		public List<GameElementData> GetRestrictions(string id)
		{
			return _configs.FirstOrDefault(x => x.GetId() == id).restrictions;
		}
		
		public List<AssetData> GetAssets(string id)
		{
			return _configs.FirstOrDefault(x => x.GetId() == id).assets;
		}

		public List<CategoryConfigData> GetAllCategories()
		{
			return _configs.OrderBy(c => c.index).ToList();
		}

		#endregion
	}
}
