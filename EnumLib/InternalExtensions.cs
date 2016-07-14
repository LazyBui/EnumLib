using System;
using System.Collections.Generic;
using System.Reflection;

namespace System {
	internal static class InternalExtensions {
		public static ulong BitwiseCastUnsigned(this long @this) {
			unsafe {
				return *(ulong *)&@this;
			}
		}

		public static long BitwiseCastSigned(this ulong @this) {
			unsafe {
				return *(long *)&@this;
			}
		}

		#if NETFX_40_OR_LOWER
		public static TAttribute GetCustomAttribute<TAttribute>(this Type @this) where TAttribute : Attribute {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			var attrs = @this.GetCustomAttributes(typeof(TAttribute), false);
			if (attrs.Length == 0) return null;
			if (attrs.Length > 1) throw new AmbiguousMatchException();
			return (TAttribute)attrs[0];
		}

		public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo @this) where TAttribute : Attribute {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			var attrs = @this.GetCustomAttributes(typeof(TAttribute), false);
			if (attrs.Length == 0) return null;
			if (attrs.Length > 1) throw new AmbiguousMatchException();
			return (TAttribute)attrs[0];
		}

		public static IEnumerable<Attribute> GetCustomAttributes(this Type @this) {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			var attrs = @this.GetCustomAttributes(false);
			var result = new Attribute[attrs.Length];
			int i = 0;
			foreach (object attr in attrs) {
				result[i++] = (Attribute)attr;
			}
			return result;
		}

		public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo @this) where TAttribute : Attribute {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			var attrs = @this.GetCustomAttributes(typeof(TAttribute), false);
			var result = new TAttribute[attrs.Length];
			int i = 0;
			foreach (object attr in attrs) {
				result[i++] = (TAttribute)attr;
			}
			return result;
		}
		#endif
	}
}