using System;
using System.Runtime.Serialization;

namespace AIDungeonPrompts.Application.Commands.UpdateUser
{
	[Serializable]
	public class UpdateUserNotFoundException : Exception
	{
		public UpdateUserNotFoundException()
		{
		}

		public UpdateUserNotFoundException(string message) : base(message)
		{
		}

		public UpdateUserNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UpdateUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
