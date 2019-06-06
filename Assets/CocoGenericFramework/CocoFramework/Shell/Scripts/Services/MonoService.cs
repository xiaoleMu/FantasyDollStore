using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class MonoService : MonoBehaviour, IService
	{
		protected ITaskFactory _taskFactory;

		#region IService implementation

		public ITask GetInitializer (IServiceResolver serviceResolver)
		{
			_taskFactory = serviceResolver.TaskFactory;

			return _taskFactory.FromEnumerableAction(() => InitService(serviceResolver));
		}

		protected virtual IEnumerator InitService(IServiceResolver resolver)
		{
			yield break;
		}

		#endregion


	}
}
