using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System {
	public static class EnumExt<TValue> where TValue : struct, IComparable {
		private class EnumValue {
			public TValue Value { get; private set; }
			public string Description { get; private set; }
			public string Name { get; private set; }
			public ulong Underlying { get; private set; }
			public List<Attribute> Attributes { get; private set; }

			public EnumValue(FieldInfo field) {
				Value = (TValue)field.GetRawConstantValue();
				DescriptionAttribute attr = field.GetCustomAttribute<DescriptionAttribute>();
				if (attr != null) Description = attr.Description;
				Attributes = field.GetCustomAttributes(false).Cast<Attribute>().ToList();
				Underlying = Convert.ToUInt64(Value);
				Name = field.Name;
			}
		}

		private static Lazy<IEnumerable<EnumValue>> sEnumValueCache =
			new Lazy<IEnumerable<EnumValue>>(
				() => {
					Type enumType = typeof(TValue);
					FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

					var result = new List<EnumValue>();
					for (int i = 0; i < fields.Length; i++) {
						var field = fields[i];
						result.Add(new EnumValue(field));
					}

					return result.AsReadOnly();
				});

		static EnumExt() {
			Type type = typeof(TValue);
			if (!type.IsEnum) throw new InvalidOperationException("Must be used with an enum type");
			sIsFlagsType = EnumExtensions.IsFlagsType(type);
			sUnderlying = Enum.GetUnderlyingType(type);
		}

		private static Type sUnderlying = null;
		private static bool sIsFlagsType = false;
		/// <summary>
		/// Determines whether the generic type is an enum type with the <see cref="System.FlagsAttribute"/>.
		/// </summary>
		/// <returns>true if the <see cref="System.Enum"/> has <see cref="FlagsAttribute"/>, false otherwise.</returns>
		public static bool IsFlagsType() {
			return sIsFlagsType;
		}

		/// <summary>
		/// Retrieves the values of the constants in the specified enum.
		/// </summary>
		/// <returns>Values of the constants in the specified enum.</returns>
		public static IEnumerable<TValue> GetValues() {
			return sEnumValueCache.Value.Select(v => v.Value);
		}

		/// <summary>
		/// Retrieves the names of the constants in the specified enum.
		/// </summary>
		/// <returns>Names of the constants in the specified enum.</returns>
		public static IEnumerable<string> GetNames() {
			return sEnumValueCache.Value.Select(v => v.Name);
		}

		/// <summary>
		/// Converts the specified value of a specified enum to its equivalent string representation according to the specified format.
		/// </summary>
		/// <param name="value">The enum value to format.</param>
		/// <param name="format">The format string to use.</param>
		/// <returns>String representation of the specified value based on the specified format.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.FormatException"></exception>
		public static string Format(TValue value, string format) {
			if (format == null) throw new ArgumentNullException(nameof(format));
			EnumExtensions.ThrowIfInvalid((Enum)(object)value);
			return Enum.Format(typeof(TValue), value, format);
		}

		/// <summary>
		/// Returns the underlying type of the specified enum.
		/// </summary>
		/// <returns>The underlying type of the specified enum.</returns>
		public static Type GetUnderlyingType() {
			return sUnderlying;
		}

		/// <summary>
		/// Returns the <see cref="System.ComponentModel.DescriptionAttribute"/> associated to an enum member if applicable.
		/// </summary>
		/// <param name="value">The enum value to retrieve a <see cref="System.ComponentModel.DescriptionAttribute"/> for.</param>
		/// <returns>null if no <see cref="System.ComponentModel.DescriptionAttribute"/> is present on the enum member, the string value associated to it otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="AmbiguousEnumException"></exception>
		public static string GetDescription(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.CompareTo(value) == 0);
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			return result.First().Description;
		}

		/// <summary>
		/// Returns the <see cref="System.ComponentModel.DescriptionAttribute"/> associated to an enum member if applicable, otherwise, the enum member name as a string.
		/// </summary>
		/// <param name="value">The enum value to retrieve text for.</param>
		/// <returns>The enum member name if no <see cref="System.ComponentModel.DescriptionAttribute"/> is present on the enum member, the string value associated to it otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="AmbiguousEnumException"></exception>
		public static string GetText(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.CompareTo(value) == 0);
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			var member = result.First();
			return member.Description ?? member.Name;
		}

		/// <summary>
		/// Returns the enum member name as a string.
		/// </summary>
		/// <param name="value">The enum value to retrieve the name of.</param>
		/// <returns>The enum member name as a string.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="AmbiguousEnumException"></exception>
		public static string GetName(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.CompareTo(value) == 0);
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			return result.First().Name;
		}

		/// <summary>
		/// Retrieves custom attributes on the enum type.
		/// </summary>
		/// <returns>All of the custom attributes on the enum type.</returns>
		public static IEnumerable<Attribute> GetTypeAttributes() {
			return typeof(TValue).GetCustomAttributes();
		}

		/// <summary>
		/// Retrieves custom attributes of the specific attribute type requested on the enum type.
		/// </summary>
		/// <returns>All of the custom attributes that match the specified attribute type on the enum type.</returns>
		public static IEnumerable<TAttribute> GetTypeAttributes<TAttribute>() where TAttribute : Attribute {
			return typeof(TValue).GetCustomAttributes<TAttribute>();
		}

		/// <summary>
		/// Retrieves a custom attribute of the specific attribute type requested on the enum type.
		/// </summary>
		/// <returns>null if the specified attribute type is not found, the attribute value otherwise.</returns>
		/// <exception cref="System.Reflection.AmbiguousMatchException"></exception>
		public static TAttribute GetTypeAttribute<TAttribute>() where TAttribute : Attribute {
			return typeof(TValue).GetCustomAttribute<TAttribute>();
		}

		/// <summary>
		/// Retrieves custom attributes from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>All of the custom attributes on the enum member.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.AmbiguousEnumException"></exception>
		public static IEnumerable<Attribute> GetAttributes(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.CompareTo(value) == 0);
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			return result.First().Attributes.AsReadOnly();
		}

		/// <summary>
		/// Retrieves custom attributes of the specified type from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>All of the custom attributes of the specified type on the enum member.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.AmbiguousEnumException"></exception>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(TValue value) where TAttribute : Attribute {
			var result = sEnumValueCache.Value.Where(v => v.Value.CompareTo(value) == 0);
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			return result.First().Attributes.OfType<TAttribute>();
		}

		/// <summary>
		/// Retrieves a custom attribute of the specified type from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>null if a custom attribute of the specified type is not found, the attribute value otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.AmbiguousEnumException"></exception>
		/// <exception cref="System.Reflection.AmbiguousMatchException"></exception>
		public static TAttribute GetAttribute<TAttribute>(TValue value) where TAttribute : Attribute {
			var result = sEnumValueCache.Value.Where(v => v.Value.CompareTo(value) == 0);
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			var attrs = result.First().Attributes.OfType<TAttribute>();
			if (attrs.Count() > 1) throw new AmbiguousMatchException("Multiple custom attributes of the same type found.");
			return attrs.FirstOrDefault();
		}

		/// <summary>
		/// Determines whether an enum value contains an invalid value.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(TValue value) {
			return EnumExtensions.HasValidValue((Enum)(object)value, typeof(TValue));
		}

		/// <summary>
		/// Determines whether an enum value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(TValue value) {
			return Enum.IsDefined(typeof(TValue), value);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(sbyte value) {
			ulong converted = (ulong)value;
			return sEnumValueCache.Value.Any(v => v.Underlying == converted);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(short value) {
			ulong converted = (ulong)value;
			return sEnumValueCache.Value.Any(v => v.Underlying == converted);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(int value) {
			ulong converted = (ulong)value;
			return sEnumValueCache.Value.Any(v => v.Underlying == converted);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(long value) {
			ulong converted = (ulong)value;
			return sEnumValueCache.Value.Any(v => v.Underlying == converted);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(byte value) {
			return sEnumValueCache.Value.Any(v => v.Underlying == value);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(ushort value) {
			return sEnumValueCache.Value.Any(v => v.Underlying == value);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(uint value) {
			return sEnumValueCache.Value.Any(v => v.Underlying == value);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(ulong value) {
			return sEnumValueCache.Value.Any(v => v.Underlying == value);
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(sbyte value) {
			return EnumExtensions.HasValidValue(value, typeof(TValue));
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(short value) {
			return EnumExtensions.HasValidValue(value, typeof(TValue));
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(int value) {
			return EnumExtensions.HasValidValue(value, typeof(TValue));
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(long value) {
			return EnumExtensions.HasValidValue(value, typeof(TValue));
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(byte value) {
			return EnumExtensions.HasValidValue(value, typeof(TValue));
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(ushort value) {
			return EnumExtensions.HasValidValue(value, typeof(TValue));
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(uint value) {
			return EnumExtensions.HasValidValue(value, typeof(TValue));
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(ulong value) {
			return EnumExtensions.HasValidValue(value, typeof(TValue));
		}

		/// <summary>
		/// Extracts all of the applicable flags in a flags enum value.
		/// </summary>
		/// <param name="value">The value to extract flags from.</param>
		/// <returns>All of the applicable flags in a flags enum value.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static IEnumerable<TValue> ExtractFlags(TValue value) {
			EnumExtensions.ThrowIfInvalid((Enum)(object)value);
			EnsureFlagsType();

			var result = new List<TValue>();
			ulong test = Convert.ToUInt64(value);
			foreach (var check in sEnumValueCache.Value) {
				if (check.Underlying == 0) continue;
				if ((test & check.Underlying) == check.Underlying) result.Add(check.Value);
			}
			return result.AsReadOnly();
		}

		/// <summary>
		/// Converts a string representation into an enum value.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value based on the parse rules.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.FormatException"></exception>
		public static TValue Parse(string value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryParse(value, policy, out result)) throw new FormatException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts a string representation into an enum value.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="ignoreCase">Indicates whether case should be ignored in conversion.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value based on the parse rules.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.FormatException"></exception>
		public static TValue Parse(string value, bool ignoreCase, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryParse(value, ignoreCase, policy, out result)) throw new FormatException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts an integral representation into an enum value.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value if successfully casted based on the cast rules.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidCastException"></exception>
		public static TValue Cast(sbyte value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryCast(value, policy, out result)) throw new InvalidCastException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts an integral representation into an enum value.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value if successfully casted based on the cast rules.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidCastException"></exception>
		public static TValue Cast(short value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryCast(value, policy, out result)) throw new InvalidCastException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts an integral representation into an enum value.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value if successfully casted based on the cast rules.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidCastException"></exception>
		public static TValue Cast(int value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryCast(value, policy, out result)) throw new InvalidCastException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts an integral representation into an enum value.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value if successfully casted based on the cast rules.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidCastException"></exception>
		public static TValue Cast(long value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryCast(value, policy, out result)) throw new InvalidCastException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts an integral representation into an enum value.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value if successfully casted based on the cast rules.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidCastException"></exception>
		public static TValue Cast(byte value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryCast(value, policy, out result)) throw new InvalidCastException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts an integral representation into an enum value.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value if successfully casted based on the cast rules.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidCastException"></exception>
		public static TValue Cast(ushort value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryCast(value, policy, out result)) throw new InvalidCastException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts an integral representation into an enum value.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value if successfully casted based on the cast rules.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidCastException"></exception>
		public static TValue Cast(uint value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryCast(value, policy, out result)) throw new InvalidCastException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts an integral representation into an enum value.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <returns>An enum value if successfully casted based on the cast rules.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.InvalidCastException"></exception>
		public static TValue Cast(ulong value, InvalidEnumPolicy policy = InvalidEnumPolicy.Disallow) {
			TValue result;
			if (!TryCast(value, policy, out result)) throw new InvalidCastException("No enum value exists with specified value");
			return result;
		}

		/// <summary>
		/// Converts a string representation into an enum value or indicates that it cannot.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the parse rules.</param>
		/// <returns>true if the enum value is successfully parsed, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryParse(string value, out TValue result) {
			return TryParse(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts a string representation into an enum value or indicates that it cannot.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the parse rules.</param>
		/// <returns>true if the enum value is successfully parsed, false otherwise.</returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryParse(string value, InvalidEnumPolicy policy, out TValue result) {
			return TryParse(value, false, policy, out result);
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
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryParse(string value, bool ignoreCase, out TValue result) {
			return TryParse(value, ignoreCase, InvalidEnumPolicy.Disallow, out result);
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
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryParse(string value, bool ignoreCase, InvalidEnumPolicy policy, out TValue result) {
			policy.ThrowIfInvalid(nameof(policy));
			if (value == null) throw new ArgumentNullException(nameof(value));
			value = value.Trim();
			if (value.Length == 0) throw new ArgumentException("Must be non-blank", nameof(value));

			if (char.IsDigit(value, 0) || value[0] == '-' || value[0] == '+') {
				// We have an integral representation. Probably.
				try {
					object temp = Convert.ChangeType(value, sUnderlying, CultureInfo.InvariantCulture);

					if (policy == InvalidEnumPolicy.Disallow) {
						ulong casted = 0;
						if (sUnderlying == typeof(sbyte)) casted = (ulong)(sbyte)temp;
						else if (sUnderlying == typeof(short)) casted = (ulong)(short)temp;
						else if (sUnderlying == typeof(int)) casted = (ulong)(int)temp;
						else if (sUnderlying == typeof(long)) casted = (ulong)(long)temp;
						else if (sUnderlying == typeof(byte)) casted = (byte)temp;
						else if (sUnderlying == typeof(ushort)) casted = (ushort)temp;
						else if (sUnderlying == typeof(uint)) casted = (uint)temp;
						else if (sUnderlying == typeof(ulong)) casted = (ulong)temp;
						else throw new NotImplementedException();

						if (!IsValidValue(casted)) {
							result = default(TValue);
							return false;
						}
					}

					// We can either be sure that all values are allowed or we have a valid value by this point
					object enumValue = Enum.ToObject(typeof(TValue), temp);

					// It doesn't matter what we have at this point, it's valid based on the rules
					result = (TValue)enumValue;
					return true;
				}
				catch (FormatException) {
					// Attempt to parse it as a string
				}
			}

			string[] values = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (values.Length > 1 && !sIsFlagsType) throw new InvalidOperationException("Must be a flags enum type to parse multiple values");

			ulong resultValue = 0;
			bool foundAny = false;
			for (int i = 0; i < values.Length; i++) {
				string v = values[i].Trim();
				foreach (var entry in sEnumValueCache.Value) {
					if (string.Compare(entry.Name, v, ignoreCase) == 0) {
						if (values.Length == 1) {
							result = entry.Value;
							return true;
						}
						resultValue |= entry.Underlying;
						foundAny = true;
					}
				}
			}

			if (!foundAny) {
				// Our string doesn't match any enum members and we don't have a valid integer representation
				result = default(TValue);
				return false;
			}

			// At this point, we have a flags enumeration and a valid integer
			// Only thing that could be wrong now is an ambiguous cast, which should throw
			result = Cast(resultValue, InvalidEnumPolicy.Allow);
			return true;
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(sbyte value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(short value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(int value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(long value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(byte value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(ushort value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(uint value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}
		
		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(ulong value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(sbyte value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast((ulong)value, typeof(sbyte), policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(short value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast((ulong)value, typeof(short), policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(int value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast((ulong)value, typeof(int), policy, out result);
		}

		public static bool TryCast(long value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast((ulong)value, typeof(long), policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(byte value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast(value, typeof(byte), policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(ushort value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast(value, typeof(ushort), policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(uint value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast(value, typeof(uint), policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool TryCast(ulong value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast(value, typeof(ulong), policy, out result);
		}

		private static ArgumentException BoundaryExceeded {
			get { return new ArgumentException("Provided integer exceeds the bounds of the underlying type", "value"); }
		}
		private static readonly Type[] sSignedTypes = new[] { typeof(sbyte), typeof(short), typeof(int), typeof(long) };
		private static readonly Type[] sUnsignedTypes = new[] { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong) };
		private static void ThrowIfValueExceedsEnumBounds(byte value) {
			bool @throw = false;
			if (sUnderlying == typeof(sbyte) && value > sbyte.MaxValue) @throw = true;

			if (@throw) throw BoundaryExceeded;
		}

		private static void ThrowIfValueExceedsEnumBounds(ushort value) {
			bool @throw = false;
			if (sUnderlying == typeof(byte) && value > byte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(sbyte) && value > sbyte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(short) && value > short.MaxValue) @throw = true;

			if (@throw) throw BoundaryExceeded;
		}

		private static void ThrowIfValueExceedsEnumBounds(uint value) {
			bool @throw = false;
			if (sUnderlying == typeof(byte) && value > byte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(ushort) && value > ushort.MaxValue) @throw = true;
			else if (sUnderlying == typeof(sbyte) && value > (int)sbyte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(short) && value > (int)short.MaxValue) @throw = true;
			else if (sUnderlying == typeof(int) && value > int.MaxValue) @throw = true;

			if (@throw) throw BoundaryExceeded;
		}

		private static void ThrowIfValueExceedsEnumBounds(ulong value) {
			bool @throw = false;
			if (sUnderlying == typeof(byte) && value > byte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(ushort) && value > ushort.MaxValue) @throw = true;
			else if (sUnderlying == typeof(uint) && value > uint.MaxValue) @throw = true;
			else if (sUnderlying == typeof(sbyte) && value > (int)sbyte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(short) && value > (int)short.MaxValue) @throw = true;
			else if (sUnderlying == typeof(int) && value > int.MaxValue) @throw = true;
			else if (sUnderlying == typeof(long) && value > long.MaxValue) @throw = true;

			if (@throw) throw BoundaryExceeded;
		}

		private static void ThrowIfValueExceedsEnumBounds(sbyte value) {
			bool @throw = false;
			if (sUnsignedTypes.Contains(sUnderlying) && value < 0) @throw = true;

			if (@throw) throw BoundaryExceeded;
		}

		private static void ThrowIfValueExceedsEnumBounds(short value) {
			bool @throw = false;
			if (sUnsignedTypes.Contains(sUnderlying) && value < 0) @throw = true;
			else if (sUnderlying == typeof(byte) && value > byte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(sbyte) && (value > sbyte.MaxValue || value < sbyte.MinValue)) @throw = true;

			if (@throw) throw BoundaryExceeded;
		}

		private static void ThrowIfValueExceedsEnumBounds(int value) {
			bool @throw = false;
			if (sUnsignedTypes.Contains(sUnderlying) && value < 0) @throw = true;
			else if (sUnderlying == typeof(byte) && value > byte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(ushort) && value > ushort.MaxValue) @throw = true;
			else if (sUnderlying == typeof(sbyte) && (value > sbyte.MaxValue || value < sbyte.MinValue)) @throw = true;
			else if (sUnderlying == typeof(short) && (value > short.MaxValue || value < short.MinValue)) @throw = true;

			if (@throw) throw BoundaryExceeded;
		}

		private static void ThrowIfValueExceedsEnumBounds(long value) {
			bool @throw = false;
			if (sUnsignedTypes.Contains(sUnderlying) && value < 0) @throw = true;
			else if (sUnderlying == typeof(byte) && value > byte.MaxValue) @throw = true;
			else if (sUnderlying == typeof(ushort) && value > ushort.MaxValue) @throw = true;
			else if (sUnderlying == typeof(uint) && value > uint.MaxValue) @throw = true;
			else if (sUnderlying == typeof(sbyte) && (value > sbyte.MaxValue || value < sbyte.MinValue)) @throw = true;
			else if (sUnderlying == typeof(short) && (value > short.MaxValue || value < short.MinValue)) @throw = true;
			else if (sUnderlying == typeof(int) && (value > int.MaxValue || value < int.MinValue)) @throw = true;

			if (@throw) throw BoundaryExceeded;
		}

		private static bool TryCast(ulong value, Type providedType, InvalidEnumPolicy policy, out TValue result) {
			policy.ThrowIfInvalid(nameof(policy));
			if (!IsFlagsType()) {
				var values = sEnumValueCache.Value.Where(v => v.Underlying == value);
				if (!values.Any()) {
					if (policy == InvalidEnumPolicy.Allow) {
						// At this point, we have to check each underlying type, otherwise casting is going to fail
						// The problem is that we want to be able to specify any type - sbyte, short, etc. and still get consistent behavior
						// So for example if you provide a ulong for a signed enum, there's an issue
						Type underlyingType = sUnderlying;
						object convertResult = null;
						bool underlyingIsSigned = sSignedTypes.Contains(underlyingType);
						if (underlyingIsSigned) {
							convertResult = Convert.ChangeType((long)value, underlyingType);
						}
						else {
							convertResult = Convert.ChangeType(value, underlyingType);
						}

						result = (TValue)Enum.ToObject(typeof(TValue), convertResult);
						return true;
					}

					result = default(TValue);
					return false;
				}

				result = values.First().Value;
				return true;
			}

			if (value == 0) {
				var noneValue = sEnumValueCache.Value.FirstOrDefault(v => v.Underlying == 0);
				if (noneValue == null) {
					if (policy == InvalidEnumPolicy.Disallow) {
						result = default(TValue);
						return false;
					}

					result = (TValue)Enum.ToObject(typeof(TValue), 0);
					return true;
				}

				result = noneValue.Value;
				return true;
			}

			ulong consumed = 0;
			ulong remaining = value;
			foreach (var cached in sEnumValueCache.Value) {
				if (cached.Underlying == 0) continue;
				ulong v = cached.Underlying;

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

			if (policy == InvalidEnumPolicy.Disallow && remaining != 0) {
				result = default(TValue);
				return false;
			}

			result = (TValue)Enum.ToObject(typeof(TValue), value);
			return true;
		}

		private static void EnsureFlagsType() {
			if (!sIsFlagsType) throw new InvalidOperationException("Must be used with a flags enum type");
		}

		private static void EnsureNonFlagsType() {
			if (!sIsFlagsType) throw new InvalidOperationException("Must be used with a non-flags enum type");
		}
	}
}