using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class TranslatedPosition
	{
		Vector3 _position;
		Vector3 _translation;

		public Vector3 Position
		{
			get { return _position; }
		}

		public Vector3 Translation
		{
			get { return _translation; }
		}

		public Vector3 FinalPosition
		{
			get { return _position + _translation; }
		}

		public TranslatedPosition(Vector3 position, Vector3 translation)
		{
			_translation = translation;
			_position = position;
		}
	}
}
