using System;

namespace TabTale
{
	public class SetupPsdkCommand : GameCommand
	{
		[Inject]
		public IPsdkCoreInitializer psdkCoreInitializer { get; set; }

		public override void Execute()
		{
			logger.Log(Tag,"Execute");

			// We init psdk before the rest of the services, since after moving them to psdk they are now dependent on it
			psdkCoreInitializer.Init().Then(Release);

			Retain();


		}
	}
}

