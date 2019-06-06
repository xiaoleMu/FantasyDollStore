using UnityEngine;
using System.Collections;

namespace TabTale {

	public class LocalGameNotification : ILocalGameNotifiation
	{
		#region ILocalGameNotifiation implementation

		public string Title
		{
			get { return _title; }
		}

		public string Image
		{
			get { return _image; }
		}

		public string Body
		{
			get { return _body; }
		}

		public System.DateTime Time
		{
			get { return _time; }
		}

		#endregion

		private string _body;
		private string _title;
		private string _image;
		private System.DateTime _time;

		public LocalGameNotification(string title, string body, string image, System.DateTime time)
		{
			_title = title;
			_body = body;
			_time = time;
			_image = image;
		}
	}
}
