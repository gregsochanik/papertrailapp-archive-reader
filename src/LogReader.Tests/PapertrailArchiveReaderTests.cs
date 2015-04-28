using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using NUnit.Framework;

namespace LogReader.Tests
{
	[TestFixture]
	public class PapertrailArchiveReaderTests
	{
		private HttpClient _httpClient;
		private PapertrailArchiveReader _papertrailArchiveReader;

		[SetUp]
		public void SetUp()
		{
			_httpClient = new HttpClient();
			var apiKey = Environment.GetEnvironmentVariable("PAPERTRAILAPP_API_KEY");
			_papertrailArchiveReader = new PapertrailArchiveReader(_httpClient, apiKey);
		}

		[TearDown]
		public void TearDown()
		{
			_httpClient.Dispose();
		}

		[Test]
		public void Creates_correct_archive_date()
		{
			var date = new DateTime(2014, 11, 23);
			Assert.That(PapertrailArchiveReader.ToArchiveDate(date), Is.EqualTo("2014-11-23"));
		}

		[Test]
		public void Not_found_file_throws_correct_error()
		{
			var archiveReaderException = Assert.Throws<ArchiveReaderException>(async () => { await _papertrailArchiveReader.DownloadAndUnzip(DateTime.MinValue); });

			Assert.That(archiveReaderException.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
			Assert.That(archiveReaderException.RequestUri.ToString(), Is.EqualTo("https://papertrailapp.com/api/v1/archives/0001-01-01/download"));
			Assert.That(archiveReaderException.ResponseBody, Is.Not.Empty);
		}

		[Test]
		public async void Can_download_file_as_stream()
		{
			var fileName = Path.GetTempFileName();
			using (var downloadAndUnzip = await _papertrailArchiveReader.DownloadAndUnzip(new DateTime(2014, 11, 23)))
			{
				using (var fileStream = File.OpenWrite(fileName))
				{
					downloadAndUnzip.CopyTo(fileStream);
				}
			}

			Assert.That(File.Exists(fileName));

			File.Delete(fileName);
		}

		[Test]
		public async void Can_download__filter_and_save()
		{
			var fileName = Path.GetTempFileName();
			using (
				var downloadAndUnzip =
					await _papertrailArchiveReader.DownloadAndFilterAndSave(new DateTime(2014, 11, 23), "LastFmLookup", fileName))
			{
				Assert.That(File.Exists(fileName));
				using (var sr = new StreamReader(downloadAndUnzip))
				{
					var readLineAsync = await sr.ReadLineAsync();
					Console.WriteLine(readLineAsync);
					Assert.That(readLineAsync, Is.Not.Empty);
				}
			}

			File.Delete(fileName);
		}

		[Test]
		public async void Can_read_as_text()
		{
			var downloadAndUnzip = await _papertrailArchiveReader.DownloadAndUnzip(new DateTime(2014, 11, 23));
			using(var sr = new StreamReader(downloadAndUnzip))
			{
				var readLineAsync = await sr.ReadLineAsync();
				Console.WriteLine(readLineAsync);
				Assert.That(readLineAsync, Is.Not.Empty);
			}
		}

		[Test]
		public async void Can_filter_specific_lines()
		{
			const string filter = "LastFmLookup";

			var downloadAndFilter = (await _papertrailArchiveReader.DownloadAndFilter(new DateTime(2014, 11, 23), filter)).ToArray();

			Assert.That(downloadAndFilter, Is.Not.Empty);
			Console.WriteLine(downloadAndFilter.First());

			Assert.That(downloadAndFilter.First(), Is.StringContaining(filter));
		}
	}
}
