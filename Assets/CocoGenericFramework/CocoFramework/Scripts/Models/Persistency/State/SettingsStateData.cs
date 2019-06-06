using System.Collections.Generic;

namespace TabTale
{
	public class SettingsStateData : IStateData
	{
		public bool sound = true;
		public bool music = true;
		public bool vibration = true;
		public bool noAds;
		public string language;
		public List<GenericPropertyData> properties = new List<GenericPropertyData>();

		#region IStateData implementation
		
		public string GetStateName ()
		{
			return "settingsState";
		}
		
		public string ToLogString ()
		{
			return string.Format("SettingsStateData: Sound:{0}, Music:{1}, Vibration:{2}, NoAds:{3},\n\tProperties:{4}",sound,music,vibration,noAds,language,properties.ArrayString());
		}
		
		public IStateData Clone ()
		{
			SettingsStateData c = new SettingsStateData();
			c.sound = sound;
			c.music = music;
			c.vibration = vibration;
			c.noAds = noAds;
			c.language = language;
			c.properties = new List<GenericPropertyData>(properties.Clone());
			return c;
		}
		
		#endregion
	}
}
