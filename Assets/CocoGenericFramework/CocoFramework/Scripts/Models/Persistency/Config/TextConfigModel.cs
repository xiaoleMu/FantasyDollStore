using System.Linq;
using System.Collections.Generic;

namespace TabTale
{
	public class TextConfigModel : ConfigModel<TextConfigData>
	{

		[Inject]
		public SettingsStateModel settings {get;set;}

		/// <summary>
		/// Returns the text associated to key and the language set in the SettingsStateModel.
		/// </summary>
		/// <returns>The text with key Key and the lang thats defined in the settings.</returns>
		/// <param name="key">Key.</param>
		public string GetText(string key)
		{
			return _configs.FirstOrDefault (text => text.key == key && settings.Language() == text.lang).value;
		} 
	}
}