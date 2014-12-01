using System;

namespace LogReader
{
	public class PapertrailArchiveReaderArguments
	{
		public string Query { get; set; }
		public string OutputFile { get; set; }
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }
		public int Days { get; set; }
		public bool Verbose { get; set; }
	}
}