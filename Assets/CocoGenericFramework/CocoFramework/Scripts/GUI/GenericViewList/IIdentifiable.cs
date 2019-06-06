using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TabTale.AssetManagement;
using TabTale;
using System.Linq;

namespace TabTale
{

	public interface IIdentifiable<TDataId>{
		TDataId GetId();
		bool Identify(TDataId other);
	}

}
