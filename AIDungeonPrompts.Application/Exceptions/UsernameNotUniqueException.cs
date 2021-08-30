using System;
using System.Runtime.Serialization;

namespace AIDungeonPrompts.Application.Exceptions
{
	[Serializable]
	public class UsernameNotUniqueException : Exception
	{
		public UsernameNotUniqueException()
		{
		}

		public UsernameNotUniqueException(string message) : base(message)
		{
		}

		public UsernameNotUniqueException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UsernameNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
