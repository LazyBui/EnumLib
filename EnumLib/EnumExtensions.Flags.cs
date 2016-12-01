using System;
using System.Linq;

namespace System {
	public static partial class EnumExtensions {
		/// <summary>
		/// Determines whether a <see cref="System.FlagsAttribute"/> enum has any flags.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <returns>true if the enum value contains any flags.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="@this"/> contains a value not defined by the enum.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="@this"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="@this"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool HasAnyFlags(this Enum @this) {
			@this.ThrowIfInvalid(nameof(@this));
			CheckType(@this, true);
			return Convert.ToUInt64(@this) != 0;
		}

		/// <summary>
		/// Determines whether a <see cref="System.FlagsAttribute"/> enum has any of the specified flags set.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <param name="flag">The flag to test.</param>
		/// <param name="flags">Any additional flags to test.</param>
		/// <returns>true if the enum value contains any of the specified flags.</returns>
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="@this"/> contains a value not defined by the enum.
		/// 	<paramref name="flag"/> contains a value not defined by the enum or is not of a consistent type with <paramref name="@this"/>.
		/// 	<paramref name="flags"/> contains any value that is null or not defined by the enum or are not of a consistent type with <paramref name="@this"/>.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// 	<paramref name="@this"/> is null.
		/// 	<paramref name="flag"/> is null.
		/// 	<paramref name="flags"/> is null.
		/// </exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="@this"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool HasAnyFlags(this Enum @this, Enum flag, params Enum[] flags) {
			@this.ThrowIfInvalid(nameof(@this));
			flag.ThrowIfInvalid(nameof(flag));
			if (object.ReferenceEquals(flags, null)) throw new ArgumentNullException(nameof(flags));
			if (flags.Any(f => object.ReferenceEquals(f, null) || !f.HasValidValue())) throw new ArgumentException("Must have no nulls or invalid values", nameof(flags));
			CheckType(@this, true);
			CheckTypeConsistent(@this, flag, nameof(flag));
			CheckTypeConsistent(@this, flags, nameof(flags));

			ulong mask = new[] { flag }.Concat(flags).
				Select(v => Convert.ToUInt64(v)).
				Aggregate(0UL, (s, v) => s |= v);
			return (Convert.ToUInt64(@this) & mask) != 0;
		}

		/// <summary>
		/// Determines whether a <see cref="System.FlagsAttribute"/> enum has no flags.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <returns>true if the enum value contains no flags.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="@this"/> contains a value not defined by the enum.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="@this"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="@this"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool HasNoFlags(this Enum @this) {
			@this.ThrowIfInvalid(nameof(@this));
			CheckType(@this, true);
			return Convert.ToUInt64(@this) == 0;
		}

		/// <summary>
		/// Determines whether a <see cref="System.FlagsAttribute"/> enum has none of the specified flags set.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <param name="flag">The flag to test.</param>
		/// <param name="flags">Any additional flags to test.</param>
		/// <returns>true if the enum value contains none of the specified flags.</returns>
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="@this"/> contains a value not defined by the enum.
		/// 	<paramref name="flag"/> contains a value not defined by the enum or is not of a consistent type with <paramref name="@this"/>.
		/// 	<paramref name="flags"/> contains any value that is null or not defined by the enum or are not of a consistent type with <paramref name="@this"/>.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// 	<paramref name="@this"/> is null.
		/// 	<paramref name="flag"/> is null.
		/// 	<paramref name="flags"/> is null.
		/// </exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="@this"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool HasNoFlags(this Enum @this, Enum flag, params Enum[] flags) {
			@this.ThrowIfInvalid(nameof(@this));
			flag.ThrowIfInvalid(nameof(flag));
			if (object.ReferenceEquals(flags, null)) throw new ArgumentNullException(nameof(flags));
			if (flags.Any(f => object.ReferenceEquals(f, null) || !f.HasValidValue())) throw new ArgumentException("Must have no nulls or invalid values", nameof(flags));
			CheckType(@this, true);
			CheckTypeConsistent(@this, flag, nameof(flag));
			CheckTypeConsistent(@this, flags, nameof(flags));

			ulong mask = new[] { flag }.Concat(flags).
				Select(v => Convert.ToUInt64(v)).
				Aggregate(0UL, (s, v) => s |= v);
			return (Convert.ToUInt64(@this) & mask) == 0;
		}

		/// <summary>
		/// Determines whether a <see cref="System.FlagsAttribute"/> enum has all flags set.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <returns>true if the enum value contains all flags.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="@this"/> contains a value not defined by the enum.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="@this"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="@this"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool HasAllFlags(this Enum @this) {
			@this.ThrowIfInvalid(nameof(@this));
			Type type = CheckType(@this, true);

			ulong mask = GetCachedValues(type).
				Aggregate(0UL, (s, v) => s |= v);
			return (Convert.ToUInt64(@this) & mask) == mask;
		}

		/// <summary>
		/// Determines whether a <see cref="System.FlagsAttribute"/> enum has all of the specified flags set.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <param name="flag">The flag to test.</param>
		/// <param name="flags">Any additional flags to test.</param>
		/// <returns>true if the enum value contains all of the specified flags.</returns>
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="@this"/> contains a value not defined by the enum.
		/// 	<paramref name="flag"/> contains a value not defined by the enum or is not of a consistent type with <paramref name="@this"/>.
		/// 	<paramref name="flags"/> contains any value that is null or not defined by the enum or are not of a consistent type with <paramref name="@this"/>.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// 	<paramref name="@this"/> is null.
		/// 	<paramref name="flag"/> is null.
		/// 	<paramref name="flags"/> is null.
		/// </exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="@this"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool HasAllFlags(this Enum @this, Enum flag, params Enum[] flags) {
			@this.ThrowIfInvalid(nameof(@this));
			flag.ThrowIfInvalid(nameof(flag));
			if (object.ReferenceEquals(flags, null)) throw new ArgumentNullException(nameof(flags));
			if (flags.Any(f => object.ReferenceEquals(f, null) || !f.HasValidValue())) throw new ArgumentException("Must have no nulls or invalid values", nameof(flags));
			CheckType(@this, true);
			CheckTypeConsistent(@this, flag, nameof(flag));
			CheckTypeConsistent(@this, flags, nameof(flags));

			ulong mask = new[] { flag }.Concat(flags).
				Select(v => Convert.ToUInt64(v)).
				Aggregate(0UL, (s, v) => s |= v);
			return (Convert.ToUInt64(@this) & mask) == mask;
		}

		/// <summary>
		/// Determines whether a <see cref="System.FlagsAttribute"/> enum has no flags set other than potentially the flags specified.
		/// Does not require any of the flags specified to be set in the value.
		/// If you are interested in whether any or all of the flags are also set, use <see cref="HasAnyFlags" /> or <see cref="HasAllFlags" /> in addition.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <param name="flag">The flag to test.</param>
		/// <param name="flags">Any additional flags to test.</param>
		/// <returns>true if the enum value has no flags other than the ones specified (but does not imply that any of the specified bits are set).</returns>
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="@this"/> contains a value not defined by the enum.
		/// 	<paramref name="flag"/> contains a value not defined by the enum or is not of a consistent type with <paramref name="@this"/>.
		/// 	<paramref name="flags"/> contains any value that is null or not defined by the enum or are not of a consistent type with <paramref name="@this"/>.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// 	<paramref name="@this"/> is null.
		/// 	<paramref name="flag"/> is null.
		/// 	<paramref name="flags"/> is null.
		/// </exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="@this"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool HasOnlyFlags(this Enum @this, Enum flag, params Enum[] flags) {
			@this.ThrowIfInvalid(nameof(@this));
			flag.ThrowIfInvalid(nameof(flag));
			if (object.ReferenceEquals(flags, null)) throw new ArgumentNullException(nameof(flags));
			if (flags.Any(f => object.ReferenceEquals(f, null) || !f.HasValidValue())) throw new ArgumentException("Must have no nulls or invalid values", nameof(flags));
			CheckType(@this, true);
			CheckTypeConsistent(@this, flag, nameof(flag));
			CheckTypeConsistent(@this, flags, nameof(flags));

			ulong value = Convert.ToUInt64(@this);
			ulong mask = new[] { flag }.Concat(flags).
				Select(v => Convert.ToUInt64(v)).
				Aggregate(0UL, (s, v) => s |= v);
			if (value == 0 && mask == 0) return true;
			if (value == 0) return false;
			return (value & ~mask) == 0;
		}

		/// <summary>
		/// Determines whether a <see cref="System.FlagsAttribute"/> enum has the flags that are specified and no other flags.
		/// </summary>
		/// <param name="this">The enum value.</param>
		/// <param name="flag">The flag to test.</param>
		/// <param name="flags">Any additional flags to test.</param>
		/// <returns>true if the enum value has all of the specified flags and no other flags.</returns>
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="@this"/> contains a value not defined by the enum.
		/// 	<paramref name="flag"/> contains a value not defined by the enum or is not of a consistent type with <paramref name="@this"/>.
		/// 	<paramref name="flags"/> contains any value that is null or not defined by the enum or are not of a consistent type with <paramref name="@this"/>.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// 	<paramref name="@this"/> is null.
		/// 	<paramref name="flag"/> is null.
		/// 	<paramref name="flags"/> is null.
		/// </exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="@this"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool HasExactlyFlags(this Enum @this, Enum flag, params Enum[] flags) {
			@this.ThrowIfInvalid(nameof(@this));
			flag.ThrowIfInvalid(nameof(flag));
			if (object.ReferenceEquals(flags, null)) throw new ArgumentNullException(nameof(flags));
			if (flags.Any(f => object.ReferenceEquals(f, null) || !f.HasValidValue())) throw new ArgumentException("Must have no nulls or invalid values", nameof(flags));
			CheckType(@this, true);
			CheckTypeConsistent(@this, flag, nameof(flag));
			CheckTypeConsistent(@this, flags, nameof(flags));

			ulong value = Convert.ToUInt64(@this);
			if (value == 0) return false;

			ulong mask = new[] { flag }.Concat(flags).
				Select(v => Convert.ToUInt64(v)).
				Aggregate(0UL, (s, v) => s |= v);
			return (value & mask) == mask && (value & ~mask) == 0;
		}
	}
}