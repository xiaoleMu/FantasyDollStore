using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class NullGeneralDialogService : IGeneralDialogService
	{
		#region IGeneralDialogService implementation

		public IModalHandle Show (GeneralDialogData data,bool isOverriding=true,bool isCloseOnOutSideTap=false, bool stacking=false)
		{
			return null;
		}


		#endregion

		#region IService Implementation
		
		public ITask GetInitializer(IServiceResolver resolver)
		{
			return resolver.TaskFactory.FromEnumerableAction(Init);
		}
		
		IEnumerator Init()
		{
			yield break;
		}
		
		#endregion


	}
}
