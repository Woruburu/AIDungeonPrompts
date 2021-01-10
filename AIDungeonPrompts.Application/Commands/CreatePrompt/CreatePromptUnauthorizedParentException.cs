using System;
using System.Runtime.Serialization;

namespace AIDungeonPrompts.Application.Commands.CreatePrompt
{
	[Serializable]
	public class CreatePromptUnauthorizedParentException : Exception
	{
		public CreatePromptUnauthorizedParentException()
		{
		}

		public CreatePromptUnauthorizedParentException(string message) : base(message)
		{
		}

		public CreatePromptUnauthorizedParentException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CreatePromptUnauthorizedParentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
