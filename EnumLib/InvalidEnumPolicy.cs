using System;

namespace System {
	/// <summary>
	/// Indicates whether invalid enum values should be allowed or disallowed.
	/// </summary>
	public enum InvalidEnumPolicy {
		/// <summary>
		/// Indicates that invalid enum values should be disallowed which will result in exceptions for invalid values.
		/// </summary>
		Disallow,
		/// <summary>
		/// Indicates that invalid enum values should be allowed.
		/// </summary>
		Allow,
	}
}