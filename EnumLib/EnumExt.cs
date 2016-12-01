using System;
using System.Collections.Generic;

namespace System {
	public static class EnumExt {
		/// <summary>
		/// Throws if the specified value of the specified enum is not a valid one for the type.
		/// </summary>
		/// <param name="value">The enum value to test.</param>
		/// <param name="name">The formal name of the parameter to propagate in the exception.</param>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static void ThrowIfInvalid<TValue>(TValue value, string name = null) where TValue : struct, IComparable {
			EnumExt<TValue>.ThrowIfInvalid(value, name: name);
		}

		/// <summary>
		/// Converts the specified value of a specified enum to its equivalent string representation according to the specified format.
		/// </summary>
		/// <param name="value">The enum value to format.</param>
		/// <param name="format">The format string to use.</param>
		/// <returns>String representation of the specified value based on the specified format.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="format"/> is null.</exception>
		/// <exception cref="System.FormatException">The <paramref name="format" /> parameter contains an invalid value.</exception>
		public static string Format<TValue>(TValue value, string format) where TValue : struct, IComparable {
			return EnumExt<TValue>.Format(value, format);
		}

		/// <summary>
		/// Returns information relating to a specific enum value.
		/// </summary>
		/// <param name="value">The value to retrieve information for.</param>
		/// <returns>Information relating to a specific enum value.</returns>
		/// <exception cref="System.AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static EnumMemberInfo GetInfo<TValue>(TValue value) where TValue : struct, IComparable {
			return EnumExt<TValue>.GetInfo(value);
		}

		/// <summary>
		/// Returns the <see cref="System.ComponentModel.DescriptionAttribute"/> text associated to an enum member if applicable.
		/// </summary>
		/// <param name="value">The enum value to retrieve a <see cref="System.ComponentModel.DescriptionAttribute"/> for.</param>
		/// <returns>null if no <see cref="System.ComponentModel.DescriptionAttribute"/> is present on the enum member, the string value associated to it otherwise.</returns>
		/// <exception cref="System.AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static string GetDescription<TValue>(TValue value) where TValue : struct, IComparable {
			return EnumExt<TValue>.GetDescription(value);
		}

		/// <summary>
		/// Returns the <see cref="System.ComponentModel.DescriptionAttribute"/> text associated to an enum member if applicable, otherwise, the enum member name as a string.
		/// </summary>
		/// <param name="value">The enum value to retrieve text for.</param>
		/// <returns>The enum member name if no <see cref="System.ComponentModel.DescriptionAttribute"/> is present on the enum member, the string value associated to it otherwise.</returns>
		/// <exception cref="System.AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static string GetText<TValue>(TValue value) where TValue : struct, IComparable {
			return EnumExt<TValue>.GetText(value);
		}

		/// <summary>
		/// Returns the enum member name as a string.
		/// </summary>
		/// <param name="value">The enum value to retrieve the name of.</param>
		/// <returns>The enum member name as a string.</returns>
		/// <exception cref="System.AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static string GetName<TValue>(TValue value) where TValue : struct, IComparable {
			return EnumExt<TValue>.GetName(value);
		}

		/// <summary>
		/// Retrieves custom attributes on the enum type.
		/// </summary>
		/// <returns>All of the custom attributes on the enum type.</returns>
		public static IEnumerable<Attribute> GetTypeAttributes<TValue>() where TValue : struct, IComparable {
			return EnumExt<TValue>.GetTypeAttributes();
		}

		/// <summary>
		/// Retrieves custom attributes of the specific attribute type requested on the enum type.
		/// </summary>
		/// <returns>All of the custom attributes that match the specified attribute type on the enum type.</returns>
		public static IEnumerable<TAttribute> GetTypeAttributes<TValue, TAttribute>()
			where TValue : struct, IComparable
			where TAttribute : Attribute
		{
			return EnumExt<TValue>.GetTypeAttributes<TAttribute>();
		}

		/// <summary>
		/// Retrieves a custom attribute of the specific attribute type requested on the enum type.
		/// </summary>
		/// <returns>null if the specified attribute type is not found, the attribute value otherwise.</returns>
		/// <exception cref="System.Reflection.AmbiguousMatchException">There are multiple attributes of type <typeparamref name="TAttribute"/> associated to the enum type.</exception>
		public static TAttribute GetTypeAttribute<TValue, TAttribute>()
			where TValue : struct, IComparable
			where TAttribute : Attribute
		{
			return EnumExt<TValue>.GetTypeAttribute<TAttribute>();
		}

		/// <summary>
		/// Retrieves custom attributes from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>All of the custom attributes on the enum member.</returns>
		/// <exception cref="System.AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static IEnumerable<Attribute> GetAttributes<TValue>(TValue value) where TValue : struct, IComparable {
			return EnumExt<TValue>.GetAttributes(value);
		}

		/// <summary>
		/// Retrieves custom attributes of the specified type from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>All of the custom attributes of the specified type on the enum member.</returns>
		/// <exception cref="System.AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static IEnumerable<TAttribute> GetAttributes<TValue, TAttribute>(TValue value)
			where TValue : struct, IComparable
			where TAttribute : Attribute
		{
			return EnumExt<TValue>.GetAttributes<TAttribute>(value);
		}

		/// <summary>
		/// Retrieves a custom attribute of the specified type from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>null if a custom attribute of the specified type is not found, the attribute value otherwise.</returns>
		/// <exception cref="System.AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		/// <exception cref="System.Reflection.AmbiguousMatchException">There are multiple attributes of type <typeparamref name="TAttribute"/> associated to <paramref name="value"/>.</exception>
		public static TAttribute GetAttribute<TValue, TAttribute>(TValue value)
			where TValue : struct, IComparable
			where TAttribute : Attribute
		{
			return EnumExt<TValue>.GetAttribute<TAttribute>(value);
		}

		/// <summary>
		/// Determines whether an enum value contains an invalid value.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue<TValue>(TValue value) where TValue : struct, IComparable {
			return EnumExt<TValue>.IsValidValue(value);
		}

		/// <summary>
		/// Determines whether an enum value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined<TValue>(TValue value) where TValue : struct, IComparable {
			return EnumExt<TValue>.IsDefined(value);
		}

		/// <summary>
		/// Extracts all of the applicable flags in a flags enum value.
		/// If there are flags with multiple bits set and all of the conditions are satisfied, any that match will go in.
		/// For example, if you had bit 1 as a specific value, bit 2 as a specific value, and bits 1 and 2 as another value, all of these will exist in the return if both bits are set.
		/// </summary>
		/// <param name="value">The value to extract flags from.</param>
		/// <returns>All of the applicable flags in a flags enum value.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> contains a value not defined by the enum.</exception>
		/// <exception cref="System.InvalidOperationException"><typeparamref name="TValue"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static IEnumerable<TValue> ExtractFlags<TValue>(TValue value) where TValue : struct, IComparable {
			return EnumExt<TValue>.ExtractFlags(value);
		}

		/// <summary>
		/// Converts a string representation into an enum value or indicates that it cannot.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the parse rules.</param>
		/// <returns>true if the enum value is successfully parsed, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is blank or whitespace-only.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool TryParse<TValue>(string value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryParse(value, out result);
		}

		/// <summary>
		/// Converts a string representation into an enum value or indicates that it cannot.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the parse rules.</param>
		/// <returns>true if the enum value is successfully parsed, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> is blank or whitespace-only.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		/// </exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool TryParse<TValue>(string value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryParse(value, policy, out result);
		}

		/// <summary>
		/// Converts a string representation into an enum value or indicates that it cannot.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="ignoreCase">Indicates whether case should be ignored in conversion.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the parse rules.</param>
		/// <returns>true if the enum value is successfully parsed, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is blank or whitespace-only.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool TryParse<TValue>(string value, bool ignoreCase, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryParse(value, ignoreCase, out result);
		}

		/// <summary>
		/// Converts a string representation into an enum value or indicates that it cannot.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="ignoreCase">Indicates whether case should be ignored in conversion.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the parse rules.</param>
		/// <returns>true if the enum value is successfully parsed, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> is blank or whitespace-only.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		/// </exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool TryParse<TValue>(string value, bool ignoreCase, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryParse(value, ignoreCase, policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast<TValue>(sbyte value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast<TValue>(short value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast<TValue>(int value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast<TValue>(long value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast<TValue>(byte value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast<TValue>(ushort value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast<TValue>(uint value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, out result);
		}
		
		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast<TValue>(ulong value, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> exceeds the bounds of the underlying type.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast<TValue>(sbyte value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> exceeds the bounds of the underlying type.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast<TValue>(short value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> exceeds the bounds of the underlying type.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast<TValue>(int value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> exceeds the bounds of the underlying type.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast<TValue>(long value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> exceeds the bounds of the underlying type.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast<TValue>(byte value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> exceeds the bounds of the underlying type.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast<TValue>(ushort value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> exceeds the bounds of the underlying type.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast<TValue>(uint value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		///.	<paramref name="value"/> exceeds the bounds of the underlying type.
		///.	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast<TValue>(ulong value, InvalidEnumPolicy policy, out TValue result) where TValue : struct, IComparable {
			return EnumExt<TValue>.TryCast(value, policy, out result);
		}
	}
}