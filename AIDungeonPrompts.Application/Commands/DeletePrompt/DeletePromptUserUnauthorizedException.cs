using System;
using System.Runtime.Serialization;

namespace AIDungeonPrompts.Application.Commands.DeletePrompt
{
	[Serializable]
	public class DeletePromptUserUnauthorizedException : Exception
	{
		public DeletePromptUserUnauthorizedException()
		{
		}

		public DeletePromptUserUnauthorizedException(string message) : base(message)
		{
		}

		public DeletePromptUserUnauthorizedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DeletePromptUserUnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
