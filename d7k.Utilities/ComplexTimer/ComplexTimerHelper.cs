using System;
using System.Text;

namespace d7k.Utilities.Tasks
{
	public static class TimerHelper
	{
		public static string ToString(this IComplexTimer timer)
		{
			var values = timer.AllValues();

			var accum = new StringBuilder("");

			foreach (var t in values)
			{
				if (t.Value is TimeSpan)
					accum.AppendFormat($"{t.Key}: {TimeString((TimeSpan)t.Value)}; ");
				else if (t.Value is long)
					accum.AppendFormat($"{t.Key}: {LongString((long)t.Value)}; ");
				else
					accum.AppendFormat($"{t.Key}: {t.Value}; ");
			}

			return accum.ToString();
		}

		private static string TimeString(TimeSpan value)
		{
			var res = "";

			if (value.TotalDays > 14)
				res = ">2w";
			else if (value.TotalDays > 1)
			{
				var days = Math.Floor(value.TotalDays);
				var hours = value.TotalHours - TimeSpan.FromDays(days).TotalHours;
				res = ((int)days).ToString("D") + ":" + ((int)hours).ToString("D2") + "d";
			}
			else if (value.TotalHours > 1)
			{
				var hours = Math.Floor(value.TotalHours);
				var minutes = value.TotalMinutes - TimeSpan.FromHours(hours).TotalMinutes;
				res = ((int)hours).ToString("D") + ":" + ((int)minutes).ToString("D2") + "h";
			}
			else if (value.TotalMinutes > 1)
			{
				var minutes = Math.Floor(value.TotalMinutes);
				var seconds = value.TotalSeconds - TimeSpan.FromMinutes(minutes).TotalSeconds;
				res = ((int)minutes).ToString("D") + ":" + ((int)seconds).ToString("D2") + "m";
			}
			else
				res = value.TotalSeconds.ToString("F2") + "s";

			return res;
		}

		private static string LongString(long value)
		{
			var res = "";

			if (value < 1000)
				res = value.ToString();
			else if (value < 1000 * 1000)
				res = (value / 1000) + "K";
			else if (value < 1000 * 1000 * 1000)
				res = (value / (1000 * 1000)) + "M";
			else if (value < (long)1000 * 1000 * 1000 * 1000)
				res = (value / (1000 * 1000 * 1000)) + "G";
			else
				res = ">T";

			return res;
		}
	}
}