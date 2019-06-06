using CocoPlay;
using CocoPlay.Native;

namespace CocoPlay.ResourceManagement
{
	public class ResourceDebug
	{
		public static void Log (string format, params object[] objects)
		{
			if (!CocoDebugSettingsData.Instance.IsResourceLogEnabled) {
				return;
			}

			CocoNative.Log (format, objects);
		}
	}
}