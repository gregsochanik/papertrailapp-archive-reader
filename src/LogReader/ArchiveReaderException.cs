using System;
using System.Net;

namespace LogReader
{
	public class ArchiveReaderException : Exception
	{
		public Uri RequestUri { get; private set; }
		public HttpStatusCode StatusCode { get; private set; }
		public string ResponseBody { get; private set; }

		public ArchiveReaderException(Uri requestUri, HttpStatusCode statusCode, string responseBody)
			: base(string.Format("Request Failed: {0} : {1} : {2}", requestUri, statusCode, responseBody))
		{
			RequestUri = requestUri;
			StatusCode = statusCode;
			ResponseBody = responseBody;
		}
	}
}