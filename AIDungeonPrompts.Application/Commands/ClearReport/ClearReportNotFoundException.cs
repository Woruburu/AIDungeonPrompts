using System;
using System.Runtime.Serialization;

namespace AIDungeonPrompts.Application.Commands.ClearReport
{
	[Serializable]
	internal class ClearReportNotFoundException : Exception
	{
		public ClearReportNotFoundException()
		{
		}

		public ClearReportNotFoundException(string message) : base(message)
		{
		}

		public ClearReportNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ClearReportNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}