using System;
using System.Runtime.Serialization;

namespace AIDungeonPrompts.Application.Queries.GetReports
{
	[Serializable]
	public class GetReportUnauthorizedUserException : Exception
	{
		public GetReportUnauthorizedUserException()
		{
		}

		public GetReportUnauthorizedUserException(string message) : base(message)
		{
		}

		public GetReportUnauthorizedUserException(string message, Exception innerException) : base(message,
			innerException)
		{
		}

		protected GetReportUnauthorizedUserException(SerializationInfo info, StreamingContext context) : base(info,
			context)
		{
		}
	}
}
