using System;
using System.Linq;
using System.Collections.Generic;

namespace TabTale
{
	public static class DoubleExtensions
	{
		public static double StandardDeviation(this IList<double> values)
		{
			if(values.Count == 0) return 0f;

			double avg = values.Average();
			return Math.Sqrt(values.Average(v=>Math.Pow(v-avg,2)));
		}
	}
}

