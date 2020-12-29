using System;
using System.Runtime.Serialization;

namespace AIDungeonPrompts.Application.Queries.GetReports
{
	[Serializable]
	internal class UnauthorizedUserReportException : Exception
	{
		public UnauthorizedUserReportException()
		{
		}

		public UnauthorizedUserReportException(string message) : base(message)
		{
		}

		public UnauthorizedUserReportException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UnauthorizedUserReportException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
