using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace TabTale
{
	public class GeneralDialogData
	{
		public string title = "Title";
		public string message = "Message";
		public string imagePrefabPath = "";
		public bool hasCloseButton = true;
		public List<BasicDialogButtonData> buttons = new List<BasicDialogButtonData> ();
	}

	public class BasicDialogButtonData
	{
		public string caption = "Caption";

		public BasicDialogButtonData ()
		{
		}

		public BasicDialogButtonData (string caption)
		{
			this.caption = caption;
		}

		public virtual void Dispatch ()
		{

		}
	}

	public class DialogButtonData : BasicDialogButtonData
	{
		public Action dispatchAction = () => {};

		public DialogButtonData ()
		{
		}

		public DialogButtonData (string caption, Action action):base(caption)
		{
			dispatchAction = action;
		}

		public override void Dispatch ()
		{
			dispatchAction ();
		}
	}
}