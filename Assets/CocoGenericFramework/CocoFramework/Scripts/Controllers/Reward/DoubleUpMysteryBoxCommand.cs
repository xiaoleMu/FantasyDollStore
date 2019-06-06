using UnityEngine;
using System.Collections;
using TabTale;
using System.Collections.Generic;

public class DoubleUpMysteryBoxCommand : GameCommand 
{
	[Inject]
	public MysteryBoxData mbData { get; set; }

	[Inject]
	public CollectGameElementsSignal collectGameElementsSignal { get; set; }

	public override void Execute()
	{
		logger.Log(Tag,"Doubling up mystery box prizes");

		collectGameElementsSignal.Dispatch(mbData.rewards);

	}
}
