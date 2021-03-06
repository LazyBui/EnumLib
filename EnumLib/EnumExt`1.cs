﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System {
	/// <summary>
	/// Provides utility methods on enum types.
	/// </summary>
	/// <typeparam name="TValue">The type of the enum.</typeparam>
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
				Description = attr?.Description;
				Attributes = field.GetCustomAttributes(false).Cast<Attribute>().ToList();
				Name = field.Name;

				// At this point, we have to check each underlying type, otherwise casting is going to fail
				// The problem is that we want to be able to specify any type - sbyte, short, etc. and still get consistent behavior
				// So for example if you provide a ulong for a signed enum, there's an issue
				Type underlyingType = sUnderlying;

				object convertResult = Convert.ChangeType(Value, underlyingType);
				if (sIsSignedUnderlying) {
					Underlying = (ulong)Convert.ToInt64(convertResult);
				}
				else {
					Underlying = Convert.ToUInt64(convertResult);
				}
			}
		}

		#if NETFX_35_OR_LOWER
		private sealed class EnumValueHolder {
			public ReadOnlyCollection<EnumValue> Value { get; private set; }

			public EnumValueHolder(Func<ReadOnlyCollection<EnumValue>> func) {
				if (object.ReferenceEquals(func, null)) throw new ArgumentNullException(nameof(func));
				Value = func();
			}
		}

		private static EnumValueHolder sEnumValueCache = null;
		#else
		private static Lazy<ReadOnlyCollection<EnumValue>> sEnumValueCache = new Lazy<ReadOnlyCollection<EnumValue>>(ProduceValues);
		#endif

		static EnumExt() {
			Type type = typeof(TValue);
			if (!type.IsEnum) throw new InvalidOperationException("Must be used with an enum type");
			sEnumType = type;
			sIsFlagsType = EnumExtensions.IsFlagsType(type);
			sUnderlying = Enum.GetUnderlyingType(type);
			sIsSignedUnderlying = new[] {
				typeof(sbyte),
				typeof(short),
				typeof(int),
				typeof(long),
			}.Contains(sUnderlying);

			#if NETFX_35_OR_LOWER
			sEnumValueCache = new EnumValueHolder(ProduceValues);
			#endif
			ulong typeMaxValue = 0;
			if (sIsSignedUnderlying) {
				if (sUnderlying == typeof(sbyte)) typeMaxValue = (ulong)sbyte.MaxValue;
				else if (sUnderlying == typeof(short)) typeMaxValue = (ulong)short.MaxValue;
				else if (sUnderlying == typeof(int)) typeMaxValue = int.MaxValue;
				else if (sUnderlying == typeof(long)) typeMaxValue = long.MaxValue;
				else throw new NotImplementedException();
			}
			else {
				typeMaxValue = ulong.MaxValue;
			}

			if (sEnumValueCache.Value.Count == 0) {
				sMaxValue = default(TValue);
				sMinValue = default(TValue);
				sMaxFlagsValue = default(TValue);
			}
			else if (sEnumValueCache.Value.Any(v => v.Underlying > typeMaxValue)) {
				// Special care needs to be taken for sign
				ulong maxPositiveValue = ulong.MinValue;
				ulong minPositiveValue = ulong.MaxValue;
				ulong maxNegativeValue = ulong.MinValue;
				ulong minNegativeValue = ulong.MaxValue;
				bool anyPositive = false;
				bool anyNonZero = false;
				foreach (var value in sEnumValueCache.Value) {
					if (value.Underlying == 0) {
						sZeroValue = value.Value;
					}
					else if (value.Underlying < 0x8000000000000000UL) {
						anyPositive = true;
						anyNonZero = true;
						if (value.Underlying > maxPositiveValue) {
							maxPositiveValue = value.Underlying;
						}
						if (value.Underlying < minPositiveValue) {
							minPositiveValue = value.Underlying;
						}
					}
					else {
						anyNonZero = true;
						if (value.Underlying > maxNegativeValue) {
							maxNegativeValue = value.Underlying;
						}
						if (value.Underlying < minNegativeValue) {
							minNegativeValue = value.Underlying;
						}
					}
				}

				if (anyNonZero) {
					if (!anyPositive) {
						sMinValue = sEnumValueCache.Value.First(v => v.Underlying == minNegativeValue).Value;
						sMaxValue = sEnumValueCache.Value.First(v => v.Underlying == maxNegativeValue).Value;
					}
					else {
						sMinValue = sEnumValueCache.Value.First(v => v.Underlying == minNegativeValue).Value;
						sMaxValue = sEnumValueCache.Value.First(v => v.Underlying == maxPositiveValue).Value;
					}
				}

				if (sIsFlagsType) {
					ulong underlying = 0;
					foreach (var value in sEnumValueCache.Value) {
						underlying |= value.Underlying;
					}

					sMaxFlagsValue = (TValue)Enum.ToObject(sEnumType, underlying);
				}
			}
			else {
				ulong maxValue = ulong.MinValue;
				ulong minValue = ulong.MaxValue;
				bool anyNonZero = false;
				foreach (var value in sEnumValueCache.Value) {
					if (value.Underlying == 0) {
						sZeroValue = value.Value;
					}
					else {
						anyNonZero = true;
						if (value.Underlying > maxValue) {
							maxValue = value.Underlying;
						}
						if (value.Underlying < minValue) {
							minValue = value.Underlying;
						}
					}
				}

				if (anyNonZero) {
					sMinValue = sEnumValueCache.Value.First(v => v.Underlying == minValue).Value;
					sMaxValue = sEnumValueCache.Value.First(v => v.Underlying == maxValue).Value;
				}

				if (sIsFlagsType) {
					maxValue = 0;
					foreach (var value in sEnumValueCache.Value) {
						maxValue |= value.Underlying;
					}

					sMaxFlagsValue = (TValue)Enum.ToObject(sEnumType, maxValue);
				}
			}

			var strings = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (var value in sEnumValueCache.Value) {
				if (!strings.Add(value.Name)) {
					sHasDuplicateCaseInsensitiveStrings = true;
					break;
				}
			}
		}

		private static ReadOnlyCollection<EnumValue> ProduceValues() {
			FieldInfo[] fields = sEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);

			var result = new List<EnumValue>();
			for (int i = 0; i < fields.Length; i++) {
				var field = fields[i];
				result.Add(new EnumValue(field));
			}

			return result.AsReadOnly();
		}

		private static Type sEnumType = null;
		private static Type sUnderlying = null;
		private static TValue? sZeroValue = null;
		private static TValue? sMaxValue = null;
		private static TValue? sMinValue = null;
		private static TValue sMaxFlagsValue;
		private static bool sIsFlagsType = false;
		private static bool sIsSignedUnderlying = false;
		private static bool sHasDuplicateCaseInsensitiveStrings = false;

		/// <summary>
		/// Determines whether the generic type is an enum type with the <see cref="System.FlagsAttribute"/>.
		/// </summary>
		/// <returns>true if the <see cref="System.Enum"/> has <see cref="FlagsAttribute"/>, false otherwise.</returns>
		public static bool IsFlagsType => sIsFlagsType;

		/// <summary>
		/// Returns the count of enum members.
		/// </summary>
		public static int Count => sEnumValueCache.Value.Count;

		/// <summary>
		/// If present, will get the explicit zero enum member.
		/// </summary>
		public static TValue? ZeroValue => sZeroValue;

		/// <summary>
		/// Indicates whether an explicit zero enum member exists.
		/// </summary>
		public static bool HasZeroValue => sZeroValue.HasValue;

		/// <summary>
		/// Gets either the explicit zero enum member or the 0 value for the enum.
		/// </summary>
		public static TValue DefaultValue => sZeroValue ?? default(TValue);

		/// <summary>
		/// Gets the largest value non-0 value in the enum.
		/// In the case where the enum has no values, the value will be 0.
		/// </summary>
		public static TValue MaxValue => sMaxValue ?? default(TValue);

		/// <summary>
		/// Gets the smallest non-0 value in the enum.
		/// In the case where the enum has no values, the value will be 0.
		/// </summary>
		public static TValue MinValue => sMinValue ?? default(TValue);

		/// <summary>
		/// Indicates whether non-zero enum members exist.
		/// </summary>
		public static bool HasNonZeroValues => sMinValue.HasValue;

		/// <summary>
		/// Gets a value with all flags in the enum set.
		/// </summary>
		/// <exception cref="System.InvalidOperationException"><typeparamref name="TValue"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static TValue MaxFlagsValue {
			get {
				EnsureFlagsType();
				return sMaxFlagsValue;
			}
		}

		/// <summary>
		/// Throws if the specified value of the specified enum is not a valid one for the type.
		/// </summary>
		/// <param name="value">The enum value to test.</param>
		/// <param name="name">The formal name of the parameter to propagate in the exception.</param>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static void ThrowIfInvalid(TValue value, string name = null) {
			if (!IsValidValue(value)) throw new ArgumentException("Must have a valid value", name);
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
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="format"/> is null.</exception>
		/// <exception cref="System.FormatException">The <paramref name="format" /> parameter contains an invalid value.</exception>
		public static string Format(TValue value, string format) {
			if (object.ReferenceEquals(format, null)) throw new ArgumentNullException(nameof(format));
			ThrowIfInvalid(value, nameof(value));
			return Enum.Format(sEnumType, value, format);
		}

		/// <summary>
		/// Returns the underlying type of the specified enum.
		/// </summary>
		/// <returns>The underlying type of the specified enum.</returns>
		public static Type GetUnderlyingType() {
			return sUnderlying;
		}

		/// <summary>
		/// Returns information relating to a specific enum value.
		/// </summary>
		/// <param name="value">The value to retrieve information for.</param>
		/// <returns>Information relating to a specific enum value.</returns>
		/// <exception cref="AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static EnumMemberInfo GetInfo(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.Equals(value));
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			var member = result.First();
			return new EnumMemberInfo(member.Name, member.Description, member.Attributes);
		}

		/// <summary>
		/// Returns the <see cref="System.ComponentModel.DescriptionAttribute"/> text associated to an enum member if applicable.
		/// </summary>
		/// <param name="value">The enum value to retrieve a <see cref="System.ComponentModel.DescriptionAttribute"/> for.</param>
		/// <returns>null if no <see cref="System.ComponentModel.DescriptionAttribute"/> is present on the enum member, the string value associated to it otherwise.</returns>
		/// <exception cref="AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static string GetDescription(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.Equals(value));
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			return result.First().Description;
		}

		/// <summary>
		/// Returns the <see cref="System.ComponentModel.DescriptionAttribute"/> text associated to an enum member if applicable, otherwise, the enum member name as a string.
		/// </summary>
		/// <param name="value">The enum value to retrieve text for.</param>
		/// <returns>The enum member name if no <see cref="System.ComponentModel.DescriptionAttribute"/> is present on the enum member, the string value associated to it otherwise.</returns>
		/// <exception cref="AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static string GetText(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.Equals(value));
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
		/// <exception cref="AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static string GetName(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.Equals(value));
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			return result.First().Name;
		}

		/// <summary>
		/// Retrieves custom attributes on the enum type.
		/// </summary>
		/// <returns>All of the custom attributes on the enum type.</returns>
		public static IEnumerable<Attribute> GetTypeAttributes() {
			return sEnumType.GetCustomAttributes();
		}

		/// <summary>
		/// Retrieves custom attributes of the specific attribute type requested on the enum type.
		/// </summary>
		/// <returns>All of the custom attributes that match the specified attribute type on the enum type.</returns>
		public static IEnumerable<TAttribute> GetTypeAttributes<TAttribute>() where TAttribute : Attribute {
			return sEnumType.GetCustomAttributes<TAttribute>();
		}

		/// <summary>
		/// Retrieves a custom attribute of the specific attribute type requested on the enum type.
		/// </summary>
		/// <returns>null if the specified attribute type is not found, the attribute value otherwise.</returns>
		/// <exception cref="System.Reflection.AmbiguousMatchException">There are multiple attributes of type <typeparamref name="TAttribute"/> associated to the enum type.</exception>
		public static TAttribute GetTypeAttribute<TAttribute>() where TAttribute : Attribute {
			return sEnumType.GetCustomAttribute<TAttribute>();
		}

		/// <summary>
		/// Retrieves custom attributes from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>All of the custom attributes on the enum member.</returns>
		/// <exception cref="AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static IEnumerable<Attribute> GetAttributes(TValue value) {
			var result = sEnumValueCache.Value.Where(v => v.Value.Equals(value));
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			return result.First().Attributes.AsReadOnly();
		}

		/// <summary>
		/// Retrieves custom attributes of the specified type from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>All of the custom attributes of the specified type on the enum member.</returns>
		/// <exception cref="AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(TValue value) where TAttribute : Attribute {
			var result = sEnumValueCache.Value.Where(v => v.Value.Equals(value));
			if (!result.Any()) throw new ArgumentException("Must have a valid value that resolves to a single enum member", nameof(value));
			if (result.Count() > 1) throw new AmbiguousEnumException();
			return result.First().Attributes.OfType<TAttribute>();
		}

		/// <summary>
		/// Retrieves a custom attribute of the specified type from the specified enum member.
		/// </summary>
		/// <param name="value">The enum member to retrieve the attributes from.</param>
		/// <returns>null if a custom attribute of the specified type is not found, the attribute value otherwise.</returns>
		/// <exception cref="AmbiguousEnumException"><paramref name="value"/> refers to more than one enum member by value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		/// <exception cref="System.Reflection.AmbiguousMatchException">There are multiple attributes of type <typeparamref name="TAttribute"/> associated to <paramref name="value"/>.</exception>
		public static TAttribute GetAttribute<TAttribute>(TValue value) where TAttribute : Attribute {
			var result = sEnumValueCache.Value.Where(v => v.Value.Equals(value));
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
			try {
				ulong casted = sIsSignedUnderlying ?
					(ulong)Convert.ToInt64(value) :
					Convert.ToUInt64(value);
				if (IsFlagsType) return AllFlagsValuesDefined(casted);
				return IsDefined(casted);
			}
			catch (OverflowException) { }
			return false;
		}

		/// <summary>
		/// Determines whether an enum value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(TValue value) {
			return sEnumValueCache.Value.Any(v => v.Value.Equals(value));
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(sbyte value) {
			return IsDefined((long)value);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(short value) {
			return IsDefined((long)value);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(int value) {
			return IsDefined((long)value);
		}

		/// <summary>
		/// Determines whether an underlying value is defined in the enum.
		/// For both types of enums, this means that the value resolves to one or more enum members.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the value is present in the enum's definition, false otherwise.</returns>
		public static bool IsDefined(long value) {
			ulong casted = value.BitwiseCastUnsigned();
			return sEnumValueCache.Value.Any(v => v.Underlying == casted);
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
			return IsValidValue((long)value);
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(short value) {
			return IsValidValue((long)value);
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(int value) {
			return IsValidValue((long)value);
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(long value) {
			return IsValidValue(value.BitwiseCastUnsigned());
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(byte value) {
			return IsValidValue((ulong)value);
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(ushort value) {
			return IsValidValue((ulong)value);
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(uint value) {
			return IsValidValue((ulong)value);
		}

		/// <summary>
		/// Determines whether a value would be a valid expression in terms of the enum.
		/// For a non-<see cref="System.FlagsAttribute"/> enum, an invalid value is one that is not explicitly defined.
		/// For a <see cref="System.FlagsAttribute"/> enum, an invalid value is one in which there is no way to express the value in terms of the flags.
		/// </summary>
		/// <param name="value">The value to test.</param>
		/// <returns>true if the enum value is consistent with the enum's definition, false otherwise.</returns>
		public static bool IsValidValue(ulong value) {
			if (IsFlagsType) return AllFlagsValuesDefined(value);
			return IsDefined(value);
		}

		/// <summary>
		/// Extracts all of the applicable flags in a <see cref="System.FlagsAttribute"/> enum value.
		/// If there are flags with multiple bits set and all of the conditions are satisfied, any that match will go in.
		/// For example, if you had bit 1 as a specific value, bit 2 as a specific value, and bits 1 and 2 as another value, all of these will exist in the return if both bits are set.
		/// </summary>
		/// <param name="value">The value to extract flags from.</param>
		/// <returns>All of the applicable flags in a flags enum value.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> contains a value not defined by the enum.</exception>
		/// <exception cref="System.InvalidOperationException"><typeparamref name="TValue"/> is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static IEnumerable<TValue> ExtractFlags(TValue value) {
			EnsureFlagsType();
			ThrowIfInvalid(value, nameof(value));

			var result = new List<TValue>();
			ulong test = Convert.ToUInt64(value);
			if (test != 0) {
				foreach (var check in sEnumValueCache.Value) {
					if (check.Underlying == 0) continue;
					if ((test & check.Underlying) == check.Underlying) result.Add(check.Value);
				}
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> is blank or whitespace-only.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		/// </exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> was not successfully parsed.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
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
		/// <exception cref="AmbiguousEnumException"><paramref name="ignoreCase"/> is specified and multiple enum members match the specified value.</exception>
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> is blank or whitespace-only.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		/// </exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.FormatException"><paramref name="value"/> was not successfully parsed.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		/// <exception cref="System.InvalidCastException"><paramref name="value"/> was not successfully casted to an enum value.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		/// <exception cref="System.InvalidCastException"><paramref name="value"/> was not successfully casted to an enum value.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		/// <exception cref="System.InvalidCastException"><paramref name="value"/> was not successfully casted to an enum value.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		/// <exception cref="System.InvalidCastException"><paramref name="value"/> was not successfully casted to an enum value.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		/// <exception cref="System.InvalidCastException"><paramref name="value"/> was not successfully casted to an enum value.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		/// <exception cref="System.InvalidCastException"><paramref name="value"/> was not successfully casted to an enum value.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		/// <exception cref="System.InvalidCastException"><paramref name="value"/> was not successfully casted to an enum value.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		/// <exception cref="System.InvalidCastException"><paramref name="value"/> was not successfully casted to an enum value.</exception>
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
		/// <param name="result">The enum value to initialize based on the parse rules.</param>
		/// <returns>true if the enum value is successfully parsed, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is blank or whitespace-only.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> is blank or whitespace-only.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		/// </exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool TryParse(string value, InvalidEnumPolicy policy, out TValue result) {
			return TryParse(value, false, policy, out result);
		}

		/// <summary>
		/// Converts a string representation into an enum value or indicates that it cannot.
		/// Accepts both integer formats in string form ([-+][digits]) or names.
		/// </summary>
		/// <param name="value">The string representation to convert.</param>
		/// <param name="ignoreCase">Indicates whether case should be ignored in conversion.</param>
		/// <param name="result">The enum value to initialize based on the parse rules.</param>
		/// <returns>true if the enum value is successfully parsed, false otherwise.</returns>
		/// <exception cref="AmbiguousEnumException"><paramref name="ignoreCase"/> is specified and multiple enum members match the specified value.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> is blank or whitespace-only.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
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
		/// <exception cref="AmbiguousEnumException"><paramref name="ignoreCase"/> is specified and multiple enum members match the specified value.</exception>
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> is blank or whitespace-only.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		/// </exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="System.InvalidOperationException"><paramref name="value"/> contains flags but the enum is not a <see cref="System.FlagsAttribute"/> enum.</exception>
		public static bool TryParse(string value, bool ignoreCase, InvalidEnumPolicy policy, out TValue result) {
			policy.ThrowIfInvalid(nameof(policy));
			if (object.ReferenceEquals(value, null)) throw new ArgumentNullException(nameof(value));
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
					object enumValue = Enum.ToObject(sEnumType, temp);

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

			var found = new List<EnumValue>(Count);
			for (int i = 0; i < values.Length; i++) {
				string v = values[i].Trim();
				foreach (var entry in sEnumValueCache.Value) {
					if (string.Compare(entry.Name, v, ignoreCase) == 0) {
						found.Add(entry);
						if (values.Length == 1 && (!sHasDuplicateCaseInsensitiveStrings || !ignoreCase)) {
							// If we're only looking for one value (which may be a flags value or a non-flags value)
							// We can exit early assuming that case is irrelevant to the result or we are factoring case, since there is no possibility for dupes
							result = entry.Value;
							return true;
						}
					}
				}
			}

			if (!found.Any()) {
				// Our string doesn't match any enum members and we don't have a valid integer representation
				result = default(TValue);
				return false;
			}

			if (sIsFlagsType) {
				// At this point, we have a flags enumeration and a valid set of flags

				// We may need to check ambiguity
				if (ignoreCase && sHasDuplicateCaseInsensitiveStrings) {
					var hashes = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
					foreach (var entry in found) {
						if (!hashes.Add(entry.Name)) {
							// We have a collision, the only thing we can do in this case is throw
							throw new AmbiguousEnumException();
						}
					}
				}

				// If we have no ambiguity of the member names, we can straightforwardly cast here
				result = (TValue)Enum.ToObject(
					sEnumType,
					found.Aggregate(0UL, (seed, v) => seed |= v.Underlying));
				return true;
			}

			// At this point, we have have a string that may or may not represent more than one enum member
			// A single result can get down here because it you may have an enum with (A, B, b) for example and be parsing for "a" case-insensitively
			if (found.Count == 1) {
				result = found[0].Value;
				return true;
			}

			// We have multiple results that match and cannot disambiguate case-insensitively
			// We must throw
			throw new AmbiguousEnumException();
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast(sbyte value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast(short value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast(int value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast(long value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast(byte value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast(ushort value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
		public static bool TryCast(uint value, out TValue result) {
			return TryCast(value, InvalidEnumPolicy.Disallow, out result);
		}
		
		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="value"/> exceeds the bounds of the underlying type.</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast(int value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast((ulong)value, typeof(int), policy, out result);
		}

		/// <summary>
		/// Converts an integral representation into an enum value or indicates that it cannot.
		/// </summary>
		/// <param name="value">The integral representation to convert.</param>
		/// <param name="policy">Indicates whether invalid integral values should be returned in enum form or should be an error.</param>
		/// <param name="result">The enum value to initialize based on the casting rules.</param>
		/// <returns>true if the enum value is successfully casted, false otherwise.</returns>
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
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
		/// <exception cref="System.ArgumentException">
		/// 	<paramref name="value"/> exceeds the bounds of the underlying type.
		/// 	<paramref name="policy"/> contains a value not defined by the enum.
		///</exception>
		public static bool TryCast(ulong value, InvalidEnumPolicy policy, out TValue result) {
			ThrowIfValueExceedsEnumBounds(value);
			return TryCast(value, typeof(ulong), policy, out result);
		}

		private static ArgumentException BoundaryExceeded {
			get { return new ArgumentException("Provided integer exceeds the bounds of the underlying type", "value"); }
		}
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
			if (!IsFlagsType) {
				var values = sEnumValueCache.Value.Where(v => v.Underlying == value);
				if (!values.Any()) {
					if (policy == InvalidEnumPolicy.Allow) {
						// At this point, we have to check each underlying type, otherwise casting is going to fail
						// The problem is that we want to be able to specify any type - sbyte, short, etc. and still get consistent behavior
						// So for example if you provide a ulong for a signed enum, there's an issue
						Type underlyingType = sUnderlying;
						object convertResult = null;
						if (sIsSignedUnderlying) {
							convertResult = Convert.ChangeType((long)value, underlyingType);
						}
						else {
							convertResult = Convert.ChangeType(value, underlyingType);
						}

						result = (TValue)Enum.ToObject(sEnumType, convertResult);
						return true;
					}

					result = default(TValue);
					return false;
				}

				result = values.First().Value;
				return true;
			}

			if (value == 0) {
				if (!HasZeroValue) {
					if (policy == InvalidEnumPolicy.Disallow) {
						result = default(TValue);
						return false;
					}

					result = (TValue)Enum.ToObject(sEnumType, 0);
					return true;
				}

				result = sZeroValue.Value;
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

			result = (TValue)Enum.ToObject(sEnumType, value);
			return true;
		}

		private static void EnsureFlagsType() {
			if (!sIsFlagsType) throw new InvalidOperationException("Must be used with a flags enum type");
		}

		private static void EnsureNonFlagsType() {
			if (!sIsFlagsType) throw new InvalidOperationException("Must be used with a non-flags enum type");
		}

		private static bool AllFlagsValuesDefined(ulong value) {
			if (value == 0) {
				return sEnumValueCache.Value.Any(v => v.Underlying == 0);
			}

			ulong consumed = 0;
			ulong remaining = value;
			foreach (var v in sEnumValueCache.Value) {
				if (v.Underlying == 0) continue;

				if ((remaining & v.Underlying) == v.Underlying) {
					remaining &= ~v.Underlying;
					consumed |= v.Underlying;
				}
				else if ((consumed & v.Underlying) != 0) {
					// At least one bit of the current combination flag has been applied to other flags
					// We reconstruct the original to see if all flags from the current apply
					// If so, we mask all bits off the value and add them to consumed
					if (((consumed | remaining) & v.Underlying) == v.Underlying) {
						remaining &= ~v.Underlying;
						consumed |= v.Underlying;
					}
				}
			}

			return remaining == 0;
		}

		/// <summary>
		/// This is designed to allow things within this library to pre-validate types
		/// For example, this allows an array of ints of size 0 to throw based on the type even though it wouldn't otherwise
		/// </summary>
		internal static void CheckType() {
			// Intentionally blank
		}
	}
}