using System;
using System.Collections.Generic;
using System.Linq;

namespace System {
	public static class IEnumerableExtensionsUniqueName {
		public static void ThrowIfAnyInvalidEnums<TValue>(this IEnumerable<TValue> @this, string name = null) where TValue : struct, IComparable {
			EnumExt<TValue>.CheckType();
			if (@this.Any(v => !EnumExt<TValue>.IsValidValue(v))) throw new ArgumentException("Must not have any invalid enum values", name);
		}
	}
}