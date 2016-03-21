using System;
using System.Runtime.Serialization;

namespace System {
	[Serializable]
	public class AmbiguousEnumException : Exception {
		public AmbiguousEnumException() : this("Value resolves to multiple enum members, must be manually resolved") { }
		public AmbiguousEnumException(string message) : base(message) { }
		public AmbiguousEnumException(string message, Exception inner) : base(message, inner) { }
		protected AmbiguousEnumException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}