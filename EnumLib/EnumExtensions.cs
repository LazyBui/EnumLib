using System;
using System.Collections.Generic;
using System.Linq;

namespace System {
	public static partial class EnumExtensions {
		private static Dictionary<Type, List<ulong>> sValuesCache = new Dictionary<Type, List<ulong>>();

		/// <summary>
		/// Safely casts an enumeration value as any particular integer type.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <typeparam name="TInteger">Desired integer type.</typeparam>
		/// <returns>A casted integer value based on the enumeration value.</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="this"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="this"/> is not an integral enum type.</exception>
		/// <exception cref="System.OverflowException"><paramref name="this"/> cannot be safely casted to the specified type.</exception>
		public static TInteger As<TInteger>(this Enum @this) where TInteger : struct {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			Type integerType = typeof(TInteger);
			if (integerType == typeof(sbyte)) return (TInteger)(object)Convert.ToSByte(@this);
			if (integerType == typeof(short)) return (TInteger)(object)Convert.ToInt16(@this);
			if (integerType == typeof(int)) return (TInteger)(object)Convert.ToInt32(@this);
			if (integerType == typeof(long)) return (TInteger)(object)Convert.ToInt64(@this);
			if (integerType == typeof(byte)) return (TInteger)(object)Convert.ToByte(@this);
			if (integerType == typeof(ushort)) return (TInteger)(object)Convert.ToUInt16(@this);
			if (integerType == typeof(uint)) return (TInteger)(object)Convert.ToUInt32(@this);
			if (integerType == typeof(ulong)) return (TInteger)(object)Convert.ToUInt64(@this);
			throw new InvalidOperationException("Must use an integer type");
		}

		private static void CheckTypeConsistent(Enum value, Enum matching, string name) {
			if (value.GetType() != matching.GetType()) throw new ArgumentException("Flag types must be consistent", name);
		}

		private static void CheckTypeConsistent(Enum value, IEnumerable<Enum> matching, string name) {
			foreach (var @enum in matching) CheckTypeConsistent(value, @enum, name);
		}

		private static Type CheckType(Enum value, bool shouldBeFlags) {
			return CheckType(value.GetType(), shouldBeFlags);
		}

		private static Type CheckType(Type enumType, bool shouldBeFlags) {
			bool isFlags = enumType.GetCustomAttribute<FlagsAttribute>() != null;
			if (isFlags != shouldBeFlags) {
				if (shouldBeFlags) throw new InvalidOperationException("Must be used with a flags enum type");
				else throw new InvalidOperationException("Must be used with a non-flags enum type");
			}
			return enumType;
		}

		private static bool HasFlags(Type enumType) {
			return enumType.GetCustomAttribute<FlagsAttribute>() != null;
		}

		private static List<ulong> GetCachedValues(Type enumType) {
			List<ulong> values = null;
			if (!sValuesCache.TryGetValue(enumType, out values)) {
				values = new List<ulong>();
				foreach (Enum v in Enum.GetValues(enumType)) {
					values.Add(Convert.ToUInt64(v));
				}
				sValuesCache[enumType] = values;
			}
			return values;
		}

		private static bool IsDefined(ulong value, Type enumType) {
			List<ulong> values = GetCachedValues(enumType);
			return values.Any(v => v == value);
		}

		private static bool AllFlagsValuesDefined(Enum value, Type enumType) {
			try {
				return AllFlagsValuesDefined(Convert.ToUInt64(value), enumType);
			}
			catch (OverflowException) { }
			return false;
		}

		private static bool AllFlagsValuesDefined(ulong value, Type enumType) {
			List<ulong> values = GetCachedValues(enumType);
			if (value == 0) {
				return values.Any(v => v == 0);
			}

			ulong consumed = 0;
			ulong remaining = value;
			foreach (var v in values) {
				if (v == 0) continue;

				if ((remaining & v) == v) {
					remaining &= ~v;
					consumed |= v;
				}
				else if ((consumed & v) != 0) {
					// At least one bit of the current combination flag has been applied to other flags
					// We reconstruct the original to see if all flags from the current apply
					// If so, we mask all bits off the value and add them to consumed
					if (((consumed | remaining) & v) == v) {
						remaining &= ~v;
						consumed |= v;
					}
				}
			}

			return remaining == 0;
		}
	}
}