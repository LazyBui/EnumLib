using System;
using System.Collections.Generic;
using System.Linq;

namespace System {
	/// <summary>
	/// Provides the ability to assert good enum values on a sequence.
	/// </summary>
	public static class IEnumerableExtensionsUniqueName {
		/// <summary>
		/// Throws an exception if there are any enum values that are invalid. <paramref name="this" /> is expected to be non-null.
		/// </summary>
		/// <param name="this">The sequence to check. Expected to be non-null.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <exception cref="ArgumentException"><paramref name="this" /> contains any invalid enum values.</exception>
		public static void ThrowIfAnyInvalidEnums<TValue>(this IEnumerable<TValue> @this, string name = null) where TValue : struct, IComparable {
			EnumExt<TValue>.CheckType();
			if (@this.Any(v => !EnumExt<TValue>.IsValidValue(v))) throw new ArgumentException("Must not have any invalid enum values", name);
		}
	}
}