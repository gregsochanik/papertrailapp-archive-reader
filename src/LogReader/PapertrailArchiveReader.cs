using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace LogReader
{
	public class PapertrailArchiveReader
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		private const string URL_FORMAT = "https://papertrailapp.com/api/v1/archives/{0}/download";

		public PapertrailArchiveReader(HttpClient httpClient, string apiKey)
		{
			_httpClient = httpClient;
			_apiKey = apiKey;

			if (string.IsNullOrEmpty(_apiKey))
			{
				throw new ArgumentException("apiKey must not be empty");
			}
		}

		public async Task<Stream> DownloadAndUnzip(DateTime date)
		{
			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, string.Format(URL_FORMAT, ToArchiveDate(date)));
			httpRequestMessage.Headers.Add("X-Papertrail-Token", _apiKey);
			var response = await _httpClient.SendAsync(httpRequestMessage);
			var readAsStreamAsync = await response.Content.ReadAsStreamAsync();
			return new GZipStream(readAsStreamAsync, CompressionMode.Decompress);
		}

		public async Task<IEnumerable<string>> DownloadAndFilter(DateTime date, string filter)
		{
			var stream = await DownloadAndUnzip(date);
			var response = new List<string>();

			using (var sr = new StreamReader(stream))
			{
				while (!sr.EndOfStream)
				{
					var line = await sr.ReadLineAsync();
					if (line.Contains(filter))
					{
						response.Add(line);
					}
				}
			}
			return response;
		}

		public async Task<FileStream> DownloadAndFilterAndSave(DateTime date, string filter, string outputFilePath)
		{
			var lines = await DownloadAndFilter(date, filter);

			File.WriteAllLines(outputFilePath, lines);

			return File.OpenRead(outputFilePath);
		}

		public static string ToArchiveDate(DateTime date)
		{
			return date.ToString("yyyy-MM-dd");
		}
	}
}