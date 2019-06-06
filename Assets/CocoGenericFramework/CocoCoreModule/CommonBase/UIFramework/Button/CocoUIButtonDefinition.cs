using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace CocoPlay
{
	public class CocoUIButtonClickSignal : Signal<CocoUINormalButton>
	{
	}

	public class CocoUIButtonTriggerClickSignal : Signal<CocoUIButtonIDProperty>
	{
	}

}