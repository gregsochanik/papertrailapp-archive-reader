using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace LogReader
{
	public class DateRange
	{
		public DateRange(DateTime fromDate, TimeSpan to)
		{
			From = fromDate;
			To = fromDate.Add(to).AddDays(-1);
		}

		public DateRange(DateTime fromDate, DateTime toDate)
		{
			From = fromDate;
			To = toDate;
		}

		public DateTime From { get; private set; }
		public DateTime To { get; private set; }

		public IEnumerable<DateTime> AsDays()
		{
			return Enumerable.Range(0, To.Subtract(From).Days + 1).Select(d => From.AddDays(d));
		} 

		public IEnumerable<IEnumerable<DateTime>> AsWeeks()
		{
			return AsDays().Batch(7);
		}
	}
}