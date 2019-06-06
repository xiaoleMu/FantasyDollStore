using UnityEngine;
using System.Collections;

namespace TabTale 
{
	public interface IGeneralDialogService : IService
	{
		IModalHandle Show (GeneralDialogData data,bool isOverriding=true,bool isCloseOnOutSideTap=false, bool stacking=false );
	}
}
