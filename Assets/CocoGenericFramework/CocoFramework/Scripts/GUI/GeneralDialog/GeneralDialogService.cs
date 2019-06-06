using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class GeneralDialogService : MonoService, IGeneralDialogService
	{
		public GameObject prefab;
		IGeneralDialog _instance;

		IModalityManager _modalityManager;

		protected override IEnumerator InitService (IServiceResolver resolver)
		{
			_modalityManager = resolver.Get<IModalityManager>();

			return base.InitService (resolver);
		}

		public IModalHandle Show (GeneralDialogData data,bool isOverriding=true,bool isCloseOnOutSideTap=false, bool stacking=false)
		{
			// IModalHandle handle = new AppModalHandle<GeneralDialogData>(prefab,data);
			IModalHandle handle = new AppModalHandle<GeneralDialogData>("GamePopups/GenericDialog",data);
            handle.MaskType = isCloseOnOutSideTap?IModalMaskType.CloseOnOutSideTap : IModalMaskType.Masked;
			return _modalityManager.Add(handle,isOverriding,stacking);
		}
	}
}