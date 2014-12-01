using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Args;

namespace LogReader
{
	class Program
	{
		static void Main(string[] args)
		{
			var modelBindingDefinition = Configuration.Configure<PapertrailArchiveReaderArguments>();
			var config = modelBindingDefinition.CreateAndBind(args);
			
			var papertrailArchiveReader = new PapertrailArchiveReader(new HttpClient(), Environment.GetEnvironmentVariable("PAPERTRAILAPP_API_KEY"));

			var dateRange = new DateRange(config.From.Value, TimeSpan.FromDays(config.Days));

			var weeks = dateRange.AsWeeks();
			var output = config.OutputFile;
			
			using (var fileStream = File.OpenWrite(output))
			{
				using (var sw = new StreamWriter(fileStream))
				{
					foreach (var week in weeks)
					{
						foreach (var day in week)
						{
							Console.WriteLine(PapertrailArchiveReader.ToArchiveDate(day));
							var downloadAndFilter = papertrailArchiveReader.DownloadAndFilter(day, config.Query).Result.ToArray();
							Console.WriteLine(downloadAndFilter.Length);
							sw.WriteLine(PapertrailArchiveReader.ToArchiveDate(day) + "\t" + downloadAndFilter.Length);
						}
					}
				}
			}
		}
	}
}
