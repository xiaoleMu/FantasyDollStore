using System.Collections.Generic;
using TabTale;

namespace CocoPlay
{
	public abstract class CocoNumericalStateData<T> : IStateData where T : CocoNumericalData, new()
	{
		#region IStateData implementation

		public abstract string GetStateName ();

		public virtual string ToLogString ()
		{
			return string.Format ("{0}: data cout [{1}]", GetStateName (), dataDic.Count);
		}

		public IStateData Clone ()
		{
			CocoNumericalStateData<T> clone = Create ();

			clone.dataDic = new Dictionary<string, T> (dataDic.Count);
			dataDic.ForEach ((key, data) => {
				clone.dataDic.Add (key, (T)data.Clone ());
			});

			return clone;
		}

		#endregion


		#region Currency Datas

		protected abstract CocoNumericalStateData<T> Create ();

		public Dictionary<string, T> dataDic = new Dictionary<string, T> ();

		#endregion

	}
}
