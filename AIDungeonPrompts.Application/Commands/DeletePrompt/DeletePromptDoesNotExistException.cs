using System;
using System.Runtime.Serialization;

namespace AIDungeonPrompts.Application.Commands.DeletePrompt
{
	[Serializable]
	public class DeletePromptDoesNotExistException : Exception
	{
		public DeletePromptDoesNotExistException()
		{
		}

		public DeletePromptDoesNotExistException(string message) : base(message)
		{
		}

		public DeletePromptDoesNotExistException(string message, Exception innerException) : base(message,
			innerException)
		{
		}

		protected DeletePromptDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info,
			context)
		{
		}
	}
}
