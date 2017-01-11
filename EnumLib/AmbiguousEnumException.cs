using System;
using System.Runtime.Serialization;

namespace System {
	/// <summary>
	/// The exception that is thrown when two or more enum values match some criteria.
	/// </summary>
	[Serializable]
	public sealed class AmbiguousEnumException : Exception {
		/// <summary>
		/// Creates a new instance of the <see cref="AmbiguousEnumException" /> class.
		/// </summary>
		public AmbiguousEnumException() : this("Value resolves to multiple enum members, must be manually resolved") { }
		/// <summary>
		/// Creates a new instance of the <see cref="AmbiguousEnumException" /> class with the specified message.
		/// </summary>
		/// <param name="message">The error message.</param>
		public AmbiguousEnumException(string message) : base(message) { }

		private AmbiguousEnumException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}