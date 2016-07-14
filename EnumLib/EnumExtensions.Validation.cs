using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System {
	public static partial class EnumExtensions {
		/// <summary>
		/// Throws an exception if the enum value contains an invalid value.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="this">The enum to test.</param>
		/// <param name="name">The formal name of the parameter to propagate in the exception.</param>
		/// <exception cref="System.ArgumentException"><paramref name="@this"/> contains a value not defined by the enum.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="@this"/> is null.</exception>
		public static void ThrowIfInvalid(this Enum @this, string name = null) {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(name);
			if (!HasValidValue(@this)) throw new ArgumentException("Must have a valid value", name);
		}

		/// <summary>
		/// Determines whether an enum value contains an invalid value.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="this">The enum to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="@this"/> is null.</exception>
		public static bool HasValidValue(this Enum @this) {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			Type enumType = @this.GetType();
			return HasValidValue(@this, enumType);
		}

		internal static bool HasValidValue(this Enum @this, Type enumType) {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			if (object.ReferenceEquals(enumType, null)) throw new ArgumentNullException(nameof(enumType));
			if (HasFlags(enumType)) return AllFlagsValuesDefined(@this, enumType);
			return Enum.IsDefined(enumType, @this);
		}

		/// <summary>
		/// Determines whether a type is an enum type with the <see cref="System.FlagsAttribute"/>.
		/// </summary>
		/// <param name="this">The enum to test.</param>
		/// <returns>true if the <see cref="System.Enum"/> has <see cref="FlagsAttribute"/>.</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="@this"/> is null.</exception>
		public static bool IsFlagsType(this Enum @this) {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			return HasFlags(@this.GetType());
		}

		/// <summary>
		/// Determines whether a type is an enum type with the <see cref="System.FlagsAttribute"/>.
		/// </summary>
		/// <param name="this">The enum type to test.</param>
		/// <returns>true if the <see cref="System.Type"/> has <see cref="FlagsAttribute"/>.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="@this"/> is not an enum type.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="@this"/> is null.</exception>
		public static bool IsFlagsType(this Type @this) {
			if (object.ReferenceEquals(@this, null)) throw new ArgumentNullException(nameof(@this));
			if (!@this.IsEnum) throw new ArgumentException("Must be an enum type", nameof(@this));
			return HasFlags(@this);
		}
	}
}