using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using System.Collections.Generic;

namespace TabTale
{
	public interface ISyncService {
		int GetMaxProgress();
		void ReportMaxProgress(int maxProgress);
		void SyncData();
		void GetServerTime();
	}

	public class ConnectionDisconnectedSignal:Signal<ModelSyncService.FlowDisconnectReason>
	{
		
	}
	
	public class SyncStatesSignal:Signal<ICollection<string>>{
		
	}
	
	public class SyncConfigsSignal:Signal<ICollection<string>>{
		
	}
	
	public class SyncSharedStateSignal:Signal<ICollection<string>>{
		
	}
	
	public class ModelSyncStartSignal:Signal
	{
		
	}
	
	public class ModelSyncCompletedSignal:Signal
	{
		
	}
}
