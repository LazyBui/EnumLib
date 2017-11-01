using System;
using System.Runtime.Serialization;

namespace System {
	/// <summary>
	/// The exception that is thrown when two or more enum values match some criteria.
	/// </summary>
	[Serializable]
	public sealed class AmbiguousEnumException : Exception {
		internal AmbiguousEnumException() : this("Value resolves to multiple enum members, must be manually resolved") { }

		internal AmbiguousEnumException(string message) : base(message) { }

		private AmbiguousEnumException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}