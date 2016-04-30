using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = TestLib.Framework.Assert;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace EnumLib.Tests {
	[TestClass]
	public class EnumExtTest {
		[TestMethod]
		public void StaticConstructor() {
			Assert.ThrowsExact<TypeInitializationException>(() => EnumExt<int>.GetNames());
			Assert.ThrowsExact<TypeInitializationException>(() => EnumExt<float>.GetNames());
			Assert.ThrowsExact<TypeInitializationException>(() => EnumExt<Guid>.GetNames());
			Assert.ThrowsExact<TypeInitializationException>(() => EnumExt<DateTime>.GetNames());
			Assert.DoesNotThrow(() => EnumExt<EnumVanilla>.GetNames());
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleFlags>.GetNames());
			Assert.DoesNotThrow(() => EnumExt<EnumComboFlags>.GetNames());
		}

		[TestMethod]
		public void ThrowIfInvalid() {
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleFlags>.ThrowIfInvalid(EnumSimpleFlags.BitOne));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.ThrowIfInvalid(~EnumSimpleFlags.BitOne));
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleFlags>.ThrowIfInvalid(EnumSimpleFlags.None));
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleFlags>.ThrowIfInvalid((EnumSimpleFlags)(1 << 0)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.ThrowIfInvalid((EnumSimpleFlags)byte.MaxValue));
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleFlags>.ThrowIfInvalid(0));

			Assert.DoesNotThrow(() => EnumExt<EnumSimpleDuplicateFlags>.ThrowIfInvalid(EnumSimpleDuplicateFlags.BitThree));
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleDuplicateFlags>.ThrowIfInvalid(EnumSimpleDuplicateFlags.BitAlsoThree));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleDuplicateFlags>.ThrowIfInvalid(~EnumSimpleDuplicateFlags.BitThree));
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleDuplicateFlags>.ThrowIfInvalid(EnumSimpleDuplicateFlags.None));
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleDuplicateFlags>.ThrowIfInvalid((EnumSimpleDuplicateFlags)(1 << 2 | 1 << 1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleDuplicateFlags>.ThrowIfInvalid((EnumSimpleDuplicateFlags)short.MaxValue));
			Assert.DoesNotThrow(() => EnumExt<EnumSimpleDuplicateFlags>.ThrowIfInvalid(0));

			Assert.DoesNotThrow(() => EnumExt<EnumVanilla>.ThrowIfInvalid(EnumVanilla.Three));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.ThrowIfInvalid((EnumVanilla)(-1)));
			Assert.DoesNotThrow(() => EnumExt<EnumVanilla>.ThrowIfInvalid(EnumVanilla.None));
			Assert.DoesNotThrow(() => EnumExt<EnumVanilla>.ThrowIfInvalid((EnumVanilla)1));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.ThrowIfInvalid((EnumVanilla)int.MinValue));
			Assert.DoesNotThrow(() => EnumExt<EnumVanilla>.ThrowIfInvalid(0));

			Assert.DoesNotThrow(() => EnumExt<EnumVanillaDuplicate>.ThrowIfInvalid(EnumVanillaDuplicate.One));
			Assert.DoesNotThrow(() => EnumExt<EnumVanillaDuplicate>.ThrowIfInvalid(EnumVanillaDuplicate.AlsoOne));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanillaDuplicate>.ThrowIfInvalid((EnumVanillaDuplicate)(-1)));
			Assert.DoesNotThrow(() => EnumExt<EnumVanillaDuplicate>.ThrowIfInvalid(EnumVanillaDuplicate.None));
			Assert.DoesNotThrow(() => EnumExt<EnumVanillaDuplicate>.ThrowIfInvalid((EnumVanillaDuplicate)1));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanillaDuplicate>.ThrowIfInvalid((EnumVanillaDuplicate)int.MinValue));
			Assert.DoesNotThrow(() => EnumExt<EnumVanillaDuplicate>.ThrowIfInvalid(0));

			Assert.DoesNotThrow(() => EnumExt<EnumComboFlags>.ThrowIfInvalid(EnumComboFlags.BitsOneThree));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboFlags>.ThrowIfInvalid(EnumComboFlags.BitOne | (EnumComboFlags)(1 << 4)));
			Assert.DoesNotThrow(() => EnumExt<EnumComboFlags>.ThrowIfInvalid(EnumComboFlags.BitsOneTwoThree));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboFlags>.ThrowIfInvalid(EnumComboFlags.BitsOneTwoThree | (EnumComboFlags)(1 << 4)));
			Assert.DoesNotThrow(() => EnumExt<EnumComboFlags>.ThrowIfInvalid((EnumComboFlags)(1 << 0 | 1 << 2)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboFlags>.ThrowIfInvalid((EnumComboFlags)(sbyte)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboFlags>.ThrowIfInvalid(0));

			Assert.DoesNotThrow(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid(EnumComboOnlyFlags.BitsOneFour));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid(EnumComboOnlyFlags.BitOne | (EnumComboOnlyFlags)(1 << 5)));
			Assert.DoesNotThrow(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid(EnumComboOnlyFlags.BitsOneTwoFour));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid(EnumComboOnlyFlags.BitsOneTwoFour | (EnumComboOnlyFlags)(1 << 5)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid((EnumComboOnlyFlags)(1 << 3)));
			Assert.DoesNotThrow(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid((EnumComboOnlyFlags)(1 << 0 | 1 << 3)));
			Assert.DoesNotThrow(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid((EnumComboOnlyFlags)(1 << 0 | 1 << 2)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid((EnumComboOnlyFlags)ushort.MaxValue));
			Assert.DoesNotThrow(() => EnumExt<EnumComboOnlyFlags>.ThrowIfInvalid(0));			
		}

		[TestMethod]
		public void GetValues() {
			IEnumerable<EnumVanilla> resultVanilla = null;
			var expectedVanilla = new[] {
				EnumVanilla.None,
				EnumVanilla.One,
				EnumVanilla.Two,
				EnumVanilla.Three,
				EnumVanilla.Four,
			};
			Assert.DoesNotThrow(() => resultVanilla = EnumExt<EnumVanilla>.GetValues());
			Assert.NotNull(resultVanilla);
			Assert.True(resultVanilla.Count() == expectedVanilla.Count());
			Assert.True(resultVanilla.Intersect(expectedVanilla).Count() == expectedVanilla.Count());

			IEnumerable<EnumSimpleFlags> resultSimpleFlags = null;
			var expectedSimpleFlags = new[] {
				EnumSimpleFlags.None,
				EnumSimpleFlags.BitOne,
				EnumSimpleFlags.BitTwo,
				EnumSimpleFlags.BitThree,
				EnumSimpleFlags.BitFour,
			};
			Assert.DoesNotThrow(() => resultSimpleFlags = EnumExt<EnumSimpleFlags>.GetValues());
			Assert.NotNull(resultSimpleFlags);
			Assert.True(resultSimpleFlags.Count() == expectedSimpleFlags.Count());
			Assert.True(resultSimpleFlags.Intersect(expectedSimpleFlags).Count() == expectedSimpleFlags.Count());

			IEnumerable<EnumComboFlags> resultComboFlags = null;
			var expectedComboFlags = new[] {
				EnumComboFlags.BitOne,
				EnumComboFlags.BitTwo,
				EnumComboFlags.BitThree,
				EnumComboFlags.BitsOneThree,
				EnumComboFlags.BitsOneTwoThree,
			};
			Assert.DoesNotThrow(() => resultComboFlags = EnumExt<EnumComboFlags>.GetValues());
			Assert.NotNull(resultComboFlags);
			Assert.True(resultComboFlags.Count() == expectedComboFlags.Count());
			Assert.True(resultComboFlags.Intersect(expectedComboFlags).Count() == expectedComboFlags.Count());

			IEnumerable<EnumComboOnlyFlags> resultComboOnlyFlags = null;
			var expectedComboOnlyFlags = new[] {
				EnumComboOnlyFlags.None,
				EnumComboOnlyFlags.BitOne,
				EnumComboOnlyFlags.BitTwo,
				EnumComboOnlyFlags.BitThree,
				EnumComboOnlyFlags.BitsOneFour,
				EnumComboOnlyFlags.BitsOneTwoFour,
			};
			Assert.DoesNotThrow(() => resultComboOnlyFlags = EnumExt<EnumComboOnlyFlags>.GetValues());
			Assert.NotNull(resultComboOnlyFlags);
			Assert.True(resultComboOnlyFlags.Count() == expectedComboOnlyFlags.Count());
			Assert.True(resultComboOnlyFlags.Intersect(expectedComboOnlyFlags).Count() == expectedComboOnlyFlags.Count());
		}

		[TestMethod]
		public void GetNames() {
			IEnumerable<string> result = null;
			IEnumerable<string> expected = new[] {
				nameof(EnumVanilla.None),
				nameof(EnumVanilla.One),
				nameof(EnumVanilla.Two),
				nameof(EnumVanilla.Three),
				nameof(EnumVanilla.Four),
			};
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.GetNames());
			Assert.NotNull(result);
			Assert.True(result.Count() == expected.Count());
			Assert.True(result.Intersect(expected).Count() == expected.Count());

			result = null;
			expected = new[] {
				nameof(EnumSimpleFlags.None),
				nameof(EnumSimpleFlags.BitOne),
				nameof(EnumSimpleFlags.BitTwo),
				nameof(EnumSimpleFlags.BitThree),
				nameof(EnumSimpleFlags.BitFour),
			};
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.GetNames());
			Assert.NotNull(result);
			Assert.True(result.Count() == expected.Count());
			Assert.True(result.Intersect(expected).Count() == expected.Count());

			result = null;
			expected = new[] {
				nameof(EnumComboFlags.BitOne),
				nameof(EnumComboFlags.BitTwo),
				nameof(EnumComboFlags.BitThree),
				nameof(EnumComboFlags.BitsOneThree),
				nameof(EnumComboFlags.BitsOneTwoThree),
			};
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.GetNames());
			Assert.NotNull(result);
			Assert.True(result.Count() == expected.Count());
			Assert.True(result.Intersect(expected).Count() == expected.Count());

			result = null;
			expected = new[] {
				nameof(EnumComboOnlyFlags.None),
				nameof(EnumComboOnlyFlags.BitOne),
				nameof(EnumComboOnlyFlags.BitTwo),
				nameof(EnumComboOnlyFlags.BitThree),
				nameof(EnumComboOnlyFlags.BitsOneFour),
				nameof(EnumComboOnlyFlags.BitsOneTwoFour),
			};
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.GetNames());
			Assert.NotNull(result);
			Assert.True(result.Count() == expected.Count());
			Assert.True(result.Intersect(expected).Count() == expected.Count());
		}

		[TestMethod]
		public void Format() {
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExt<EnumVanilla>.Format(EnumVanilla.One, null));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Format((EnumVanilla)(-1), "g"));
			Assert.ThrowsExact<FormatException>(() => EnumExt<EnumVanilla>.Format(EnumVanilla.One, "t"));

			string result = null;

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Format(EnumVanilla.One, "d"));
			Assert.NotNull(result);
			Assert.Equal(result, "1");

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Format(EnumVanilla.One, "x"));
			Assert.NotNull(result);
			Assert.Equal(result, "00000001");

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Format(EnumVanilla.Three, "f"));
			Assert.NotNull(result);
			Assert.Equal(result, "Three");

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Format(EnumVanilla.Three, "g"));
			Assert.NotNull(result);
			Assert.Equal(result, "Three");

			/*
			// Assuming that EnumExt's Format function did not throw on invalid values, these two tests would be the only material difference g and f make
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Format(EnumVanilla.Four | EnumVanilla.Two, "f"));
			Assert.NotNull(result);
			Assert.Equal(result, "Two, Four");

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Format(EnumVanilla.Four | EnumVanilla.Two, "g"));
			Assert.NotNull(result);
			Assert.Equal(result, "6");
			*/

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.Format(EnumSimpleFlags.BitOne | EnumSimpleFlags.BitTwo, "f"));
			Assert.NotNull(result);
			Assert.Equal(result, "BitOne, BitTwo");

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.Format(EnumSimpleFlags.BitOne | EnumSimpleFlags.BitTwo, "g"));
			Assert.NotNull(result);
			Assert.Equal(result, "BitOne, BitTwo");

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.Format(EnumComboOnlyFlags.BitsOneTwoFour, "f"));
			Assert.NotNull(result);
			Assert.Equal(result, "BitsOneTwoFour");

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.Format(EnumComboOnlyFlags.BitsOneTwoFour, "g"));
			Assert.NotNull(result);
			Assert.Equal(result, "BitsOneTwoFour");
		}

		[TestMethod]
		public void GetUnderlyingType() {
			Type result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.GetUnderlyingType());
			Assert.True(result == typeof(int));

			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.GetUnderlyingType());
			Assert.True(result == typeof(int));

			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.GetUnderlyingType());
			Assert.True(result == typeof(int));

			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.GetUnderlyingType());
			Assert.True(result == typeof(int));
		}

		[TestMethod]
		public void IsFlagsType() {
			bool result = false;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsFlagsType());
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsFlagsType());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsFlagsType());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsFlagsType());
			Assert.True(result);
		}

		[TestMethod]
		public void GetDescription() {
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.GetDescription((EnumVanilla)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboFlags>.GetDescription(EnumComboFlags.BitOne | EnumComboFlags.BitTwo));
			Assert.ThrowsExact<AmbiguousEnumException>(() => EnumExt<EnumVanillaDuplicate>.GetDescription(EnumVanillaDuplicate.AlsoOne));

			string result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.GetDescription(EnumVanilla.None));
			Assert.Null(result);

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.GetDescription(EnumVanilla.One));
			Assert.Equal(result, "Eleven");
		}

		[TestMethod]
		public void GetText() {
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.GetText((EnumVanilla)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboFlags>.GetText(EnumComboFlags.BitOne | EnumComboFlags.BitTwo));
			Assert.ThrowsExact<AmbiguousEnumException>(() => EnumExt<EnumVanillaDuplicate>.GetText(EnumVanillaDuplicate.AlsoOne));

			string result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.GetText(EnumVanilla.None));
			Assert.Equal(result, nameof(EnumVanilla.None));

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.GetText(EnumVanilla.One));
			Assert.Equal(result, "Eleven");
		}

		[TestMethod]
		public void GetName() {
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.GetName((EnumVanilla)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumComboFlags>.GetName(EnumComboFlags.BitOne | EnumComboFlags.BitTwo));
			Assert.ThrowsExact<AmbiguousEnumException>(() => EnumExt<EnumVanillaDuplicate>.GetName(EnumVanillaDuplicate.AlsoOne));

			string result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.GetName(EnumVanilla.None));
			Assert.Equal(result, nameof(EnumVanilla.None));

			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.GetName(EnumVanilla.One));
			Assert.Equal(result, nameof(EnumVanilla.One));
		}

		[TestMethod]
		public void GetTypeAttributes() {
			IEnumerable<Attribute> attributes = null;
			Assert.DoesNotThrow(() => attributes = EnumExt<EnumVanilla>.GetTypeAttributes());
			Assert.Count(attributes, 3);
			Assert.Count(attributes.OfType<CustomEnumAttribute>(), 2);
			Assert.Count(attributes.OfType<DescriptionAttribute>(), 1);

			IEnumerable<CustomEnumAttribute> customAttribute = null;
			IEnumerable<DescriptionAttribute> descriptionAttribute = null;
			IEnumerable<AttributeUsageAttribute> nonsenseAttribute = null;
			Assert.DoesNotThrow(() => customAttribute = EnumExt<EnumVanilla>.GetTypeAttributes<CustomEnumAttribute>());
			Assert.DoesNotThrow(() => descriptionAttribute = EnumExt<EnumVanilla>.GetTypeAttributes<DescriptionAttribute>());
			Assert.DoesNotThrow(() => nonsenseAttribute = EnumExt<EnumVanilla>.GetTypeAttributes<AttributeUsageAttribute>());
			Assert.NotNull(customAttribute);
			Assert.NotNull(descriptionAttribute);
			Assert.NotNull(nonsenseAttribute);
			Assert.Count(customAttribute, 2);
			Assert.Count(descriptionAttribute, 1);
			Assert.Empty(nonsenseAttribute);
		}

		[TestMethod]
		public void GetTypeAttribute() {
			Assert.ThrowsExact<AmbiguousMatchException>(() => EnumExt<EnumVanilla>.GetTypeAttribute<CustomEnumAttribute>());

			DescriptionAttribute descriptionAttribute = null;
			AttributeUsageAttribute nonsenseAttribute = null;
			Assert.DoesNotThrow(() => descriptionAttribute = EnumExt<EnumVanilla>.GetTypeAttribute<DescriptionAttribute>());
			Assert.DoesNotThrow(() => nonsenseAttribute = EnumExt<EnumVanilla>.GetTypeAttribute<AttributeUsageAttribute>());
			Assert.NotNull(descriptionAttribute);
			Assert.Null(nonsenseAttribute);
		}

		[TestMethod]
		public void GetAttributes() {
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.GetAttributes((EnumVanilla)(-1)));
			Assert.ThrowsExact<AmbiguousEnumException>(() => EnumExt<EnumVanillaDuplicate>.GetAttributes(EnumVanillaDuplicate.AlsoOne));

			IEnumerable<Attribute> attributes = null;
			Assert.DoesNotThrow(() => attributes = EnumExt<EnumVanilla>.GetAttributes(EnumVanilla.One));
			Assert.Count(attributes, 3);
			Assert.Count(attributes.OfType<CustomEnumAttribute>(), 2);
			Assert.Count(attributes.OfType<DescriptionAttribute>(), 1);

			IEnumerable<CustomEnumAttribute> customAttribute = null;
			IEnumerable<DescriptionAttribute> descriptionAttribute = null;
			IEnumerable<AttributeUsageAttribute> nonsenseAttribute = null;
			Assert.DoesNotThrow(() => customAttribute = EnumExt<EnumVanilla>.GetAttributes<CustomEnumAttribute>(EnumVanilla.One));
			Assert.DoesNotThrow(() => descriptionAttribute = EnumExt<EnumVanilla>.GetAttributes<DescriptionAttribute>(EnumVanilla.One));
			Assert.DoesNotThrow(() => nonsenseAttribute = EnumExt<EnumVanilla>.GetAttributes<AttributeUsageAttribute>(EnumVanilla.One));
			Assert.NotNull(customAttribute);
			Assert.NotNull(descriptionAttribute);
			Assert.NotNull(nonsenseAttribute);
			Assert.Count(customAttribute, 2);
			Assert.Count(descriptionAttribute, 1);
			Assert.Empty(nonsenseAttribute);
		}

		[TestMethod]
		public void GetAttribute() {
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.GetAttribute<DescriptionAttribute>((EnumVanilla)(-1)));
			Assert.ThrowsExact<AmbiguousEnumException>(() => EnumExt<EnumVanillaDuplicate>.GetAttribute<DescriptionAttribute>(EnumVanillaDuplicate.AlsoOne));
			Assert.ThrowsExact<AmbiguousMatchException>(() => EnumExt<EnumVanilla>.GetAttribute<CustomEnumAttribute>(EnumVanilla.One));

			DescriptionAttribute descriptionAttribute = null;
			AttributeUsageAttribute nonsenseAttribute = null;
			Assert.DoesNotThrow(() => descriptionAttribute = EnumExt<EnumVanilla>.GetAttribute<DescriptionAttribute>(EnumVanilla.One));
			Assert.DoesNotThrow(() => nonsenseAttribute = EnumExt<EnumVanilla>.GetAttribute<AttributeUsageAttribute>(EnumVanilla.One));
			Assert.NotNull(descriptionAttribute);
			Assert.Null(nonsenseAttribute);
		}

		[TestMethod]
		public void IsValidValue() {
			bool result = false;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsValidValue(EnumSimpleFlags.BitOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsValidValue(~EnumSimpleFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsValidValue(EnumSimpleFlags.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsValidValue(1 << 0));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsValidValue(0));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsValidValue(EnumSimpleDuplicateFlags.BitThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsValidValue(EnumSimpleDuplicateFlags.BitAlsoThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsValidValue(~EnumSimpleDuplicateFlags.BitThree));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsValidValue(EnumSimpleDuplicateFlags.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsValidValue(1 << 2 | 1 << 1));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsValidValue(0));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsValidValue(EnumVanilla.Three));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsValidValue((EnumVanilla)(-1)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsValidValue(EnumVanilla.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsValidValue(1));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsValidValue(0));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsValidValue(EnumVanillaDuplicate.One));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsValidValue(EnumVanillaDuplicate.AlsoOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (EnumExt<EnumVanillaDuplicate>.IsValidValue((EnumVanillaDuplicate)(-1))));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsValidValue(EnumVanillaDuplicate.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsValidValue(1));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsValidValue(0));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsValidValue(EnumComboFlags.BitsOneThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsValidValue(EnumComboFlags.BitOne | (EnumComboFlags)(1 << 4)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsValidValue(EnumComboFlags.BitsOneTwoThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsValidValue(EnumComboFlags.BitsOneTwoThree | (EnumComboFlags)(1 << 4)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsValidValue(1 << 0 | 1 << 2));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsValidValue((sbyte)(-1)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsValidValue(0));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue(EnumComboOnlyFlags.BitsOneFour));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue(EnumComboOnlyFlags.BitOne | (EnumComboOnlyFlags)(1 << 5)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue(EnumComboOnlyFlags.BitsOneTwoFour));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue(EnumComboOnlyFlags.BitsOneTwoFour | (EnumComboOnlyFlags)(1 << 5)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue((EnumComboOnlyFlags)(1 << 3)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue(1 << 0 | 1 << 3));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue(1 << 0 | 1 << 2));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsValidValue(0));
			Assert.True(result);
		}

		[TestMethod]
		public void IsValidValueBackingTypes() {
			bool result = false;
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsValidValue(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsValidValue(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsValidValue(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsValidValue(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsValidValue(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsValidValue(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsValidValue(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsValidValue(long.MaxValue));
			Assert.False(result);
		}

		[TestMethod]
		public void IsDefined() {
			bool result = false;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsDefined(EnumSimpleFlags.BitOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsDefined(~EnumSimpleFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsDefined(EnumSimpleFlags.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsDefined(1 << 0));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.IsDefined(0));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsDefined(EnumSimpleDuplicateFlags.BitThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsDefined(EnumSimpleDuplicateFlags.BitAlsoThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsDefined(~EnumSimpleDuplicateFlags.BitThree));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsDefined(EnumSimpleDuplicateFlags.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsDefined(1 << 2 | 1 << 1));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.IsDefined(0));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsDefined(EnumVanilla.Three));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsDefined((EnumVanilla)(-1)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsDefined(EnumVanilla.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsDefined(1));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.IsDefined(0));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsDefined(EnumVanillaDuplicate.One));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsDefined(EnumVanillaDuplicate.AlsoOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (EnumExt<EnumVanillaDuplicate>.IsDefined((EnumVanillaDuplicate)(-1))));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsDefined(EnumVanillaDuplicate.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsDefined(1));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.IsDefined(0));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsDefined(EnumComboFlags.BitsOneThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsDefined(EnumComboFlags.BitOne | (EnumComboFlags)(1 << 4)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsDefined(EnumComboFlags.BitsOneTwoThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsDefined(EnumComboFlags.BitsOneTwoThree | (EnumComboFlags)(1 << 4)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsDefined(1 << 0 | 1 << 2));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsDefined((sbyte)(-1)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboFlags>.IsDefined(0));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined(EnumComboOnlyFlags.BitsOneFour));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined(EnumComboOnlyFlags.BitOne | (EnumComboOnlyFlags)(1 << 5)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined(EnumComboOnlyFlags.BitsOneTwoFour));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined(EnumComboOnlyFlags.BitsOneTwoFour | (EnumComboOnlyFlags)(1 << 5)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined((EnumComboOnlyFlags)(1 << 3)));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined(1 << 0 | 1 << 3));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined(1 << 0 | 1 << 2));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumComboOnlyFlags>.IsDefined(0));
			Assert.True(result);
		}

		[TestMethod]
		public void IsDefinedBackingTypes() {
			bool result = false;
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt8>.IsDefined(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt8>.IsDefined(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt16>.IsDefined(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt16>.IsDefined(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt32>.IsDefined(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt32>.IsDefined(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedUInt64>.IsDefined(long.MaxValue));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(byte.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(byte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(ushort.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(ushort.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(uint.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(uint.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(ulong.MinValue));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(ulong.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(sbyte.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(sbyte.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(short.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(short.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(int.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(int.MaxValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(long.MinValue));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumBackedInt64>.IsDefined(long.MaxValue));
			Assert.False(result);
		}

		[TestMethod]
		public void ExtractFlags() {
			Assert.ThrowsExact<InvalidOperationException>(() => EnumExt<EnumVanilla>.ExtractFlags(EnumVanilla.One));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.ExtractFlags((EnumSimpleFlags)(-1)));

			IEnumerable<EnumComboOnlyFlags> flags = null;

			flags = null;
			Assert.DoesNotThrow(() => flags = EnumExt<EnumComboOnlyFlags>.ExtractFlags(EnumComboOnlyFlags.BitOne));
			Assert.NotNull(flags);
			Assert.Count(flags, 1);
			Assert.Contains(flags, EnumComboOnlyFlags.BitOne);

			flags = null;
			Assert.DoesNotThrow(() => flags = EnumExt<EnumComboOnlyFlags>.ExtractFlags(EnumComboOnlyFlags.BitOne | EnumComboOnlyFlags.BitTwo));
			Assert.NotNull(flags);
			Assert.Count(flags, 2);
			Assert.Contains(flags, EnumComboOnlyFlags.BitOne);
			Assert.Contains(flags, EnumComboOnlyFlags.BitTwo);

			flags = null;
			Assert.DoesNotThrow(() => flags = EnumExt<EnumComboOnlyFlags>.ExtractFlags(EnumComboOnlyFlags.BitsOneTwoFour));
			Assert.NotNull(flags);
			Assert.Count(flags, 4);
			Assert.Contains(flags, EnumComboOnlyFlags.BitOne);
			Assert.Contains(flags, EnumComboOnlyFlags.BitTwo);
			Assert.Contains(flags, EnumComboOnlyFlags.BitsOneFour);
			Assert.Contains(flags, EnumComboOnlyFlags.BitsOneTwoFour);

			flags = null;
			Assert.DoesNotThrow(() => flags = EnumExt<EnumComboOnlyFlags>.ExtractFlags(EnumComboOnlyFlags.None));
			Assert.NotNull(flags);
			Assert.Empty(flags);
		}

		[TestMethod]
		public void Parse() {
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExt<EnumVanilla>.Parse(null));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Parse(""));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Parse(nameof(EnumVanilla.None), policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExt<EnumVanilla>.Parse(null, true));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Parse("", true));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Parse(nameof(EnumVanilla.None), true, policy: (InvalidEnumPolicy)(-1)));

			EnumVanilla result = EnumVanilla.None;
			EnumVanillaDuplicate result2 = EnumVanillaDuplicate.None;

			result = EnumVanilla.None;
			Assert.ThrowsExact<InvalidOperationException>(() => EnumExt<EnumVanilla>.Parse($"{nameof(EnumVanilla.One)}, {nameof(EnumVanilla.Two)}"));

			result = EnumVanilla.None;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Parse(nameof(EnumVanilla.One)));
			Assert.True(result == EnumVanilla.One);

			result = EnumVanilla.None;
			Assert.ThrowsExact<FormatException>(() => result = EnumExt<EnumVanilla>.Parse(nameof(EnumVanilla.One).ToLowerInvariant()));
			Assert.ThrowsExact<FormatException>(() => result = EnumExt<EnumVanilla>.Parse(nameof(EnumVanilla.One).ToLowerInvariant(), policy: InvalidEnumPolicy.Allow));

			result = EnumVanilla.None;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Parse(nameof(EnumVanilla.One).ToLowerInvariant(), true));
			Assert.True(result == EnumVanilla.One);
			result = EnumVanilla.None;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Parse(nameof(EnumVanilla.One).ToLowerInvariant(), true, policy: InvalidEnumPolicy.Allow));
			Assert.True(result == EnumVanilla.One);

			result = EnumVanilla.None;
			Assert.ThrowsExact<FormatException>(() => result = EnumExt<EnumVanilla>.Parse("abc"));
			Assert.ThrowsExact<FormatException>(() => result = EnumExt<EnumVanilla>.Parse("abc", true));
			Assert.ThrowsExact<FormatException>(() => result = EnumExt<EnumVanilla>.Parse("abc", policy: InvalidEnumPolicy.Allow));
			Assert.ThrowsExact<FormatException>(() => result = EnumExt<EnumVanilla>.Parse("abc", true, policy: InvalidEnumPolicy.Allow));

			result = EnumVanilla.None;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Parse("1"));
			Assert.True(result == EnumVanilla.One);
			result = EnumVanilla.None;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Parse("1", policy: InvalidEnumPolicy.Allow));
			Assert.True(result == EnumVanilla.One);

			result = EnumVanilla.None;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Parse("+1"));
			Assert.True(result == EnumVanilla.One);
			result = EnumVanilla.None;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Parse("+1", policy: InvalidEnumPolicy.Allow));
			Assert.True(result == EnumVanilla.One);

			result = EnumVanilla.None;
			Assert.ThrowsExact<FormatException>(() => result = EnumExt<EnumVanilla>.Parse("-1"));

			result = EnumVanilla.None;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.Parse("-1", policy: InvalidEnumPolicy.Allow));
			Assert.True(result == (EnumVanilla)(-1));

			result2 = EnumVanillaDuplicate.None;
			Assert.DoesNotThrow(() => result2 = EnumExt<EnumVanillaDuplicate>.Parse("One"));
			Assert.True(result2 == EnumVanillaDuplicate.One);

			result2 = EnumVanillaDuplicate.None;
			Assert.DoesNotThrow(() => result2 = EnumExt<EnumVanillaDuplicate>.Parse("AlsoOne"));
			Assert.True(result2 == EnumVanillaDuplicate.AlsoOne);

			result2 = EnumVanillaDuplicate.None;
			Assert.DoesNotThrow(() => result2 = EnumExt<EnumVanillaDuplicate>.Parse("1"));
			Assert.True(result2 == EnumVanillaDuplicate.One);
		}

		[TestMethod]
		public void TryParse() {
			EnumVanilla value = EnumVanilla.None;
			EnumVanillaDuplicate value2 = EnumVanillaDuplicate.None;
			bool? result = null;
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExt<EnumVanilla>.TryParse(null, out value));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.TryParse("", out value));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.TryParse(nameof(EnumVanilla.None), (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExt<EnumVanilla>.TryParse(null, true, out value));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.TryParse("", true, out value));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.TryParse(nameof(EnumVanilla.None), true, (InvalidEnumPolicy)(-1), out value));

			value = EnumVanilla.None;
			result = null;
			Assert.ThrowsExact<InvalidOperationException>(() => result = EnumExt<EnumVanilla>.TryParse($"{nameof(EnumVanilla.One)}, {nameof(EnumVanilla.Two)}", out value));

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse(nameof(EnumVanilla.One), out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse(nameof(EnumVanilla.One).ToLowerInvariant(), out value));
			Assert.False(result);
			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse(nameof(EnumVanilla.One).ToLowerInvariant(), InvalidEnumPolicy.Allow, out value));
			Assert.False(result);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse(nameof(EnumVanilla.One).ToLowerInvariant(), true, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);
			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse(nameof(EnumVanilla.One).ToLowerInvariant(), true, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("abc", out value));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("abc", true, out value));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("abc", InvalidEnumPolicy.Allow, out value));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("abc", true, InvalidEnumPolicy.Allow, out value));
			Assert.False(result);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("1", out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);
			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("1", InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("+1", out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);
			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("+1", InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("-1", out value));
			Assert.False(result);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryParse("-1", InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == (EnumVanilla)(-1));

			value2 = EnumVanillaDuplicate.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.TryParse("One", out value2));
			Assert.True(result);
			Assert.True(value2 == EnumVanillaDuplicate.One);

			value2 = EnumVanillaDuplicate.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.TryParse("AlsoOne", out value2));
			Assert.True(result);
			Assert.True(value2 == EnumVanillaDuplicate.AlsoOne);

			value2 = EnumVanillaDuplicate.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.TryParse("1", out value2));
			Assert.True(result);
			Assert.True(value2 == EnumVanillaDuplicate.One);
		}

		[TestMethod]
		public void CastNonFlags() {
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast((sbyte)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast((short)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast((int)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast((long)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast((byte)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast((ushort)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast((uint)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast((ulong)0, policy: (InvalidEnumPolicy)(-1)));

			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast(uint.MaxValue));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast(ulong.MaxValue));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast(long.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast(uint.MaxValue, policy: InvalidEnumPolicy.Allow));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast(ulong.MaxValue, policy: InvalidEnumPolicy.Allow));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumVanilla>.Cast(long.MinValue, policy: InvalidEnumPolicy.Allow));

			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumVanilla>.Cast((sbyte)-1));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumVanilla>.Cast((short)-1));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumVanilla>.Cast((int)-1));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumVanilla>.Cast((long)-1));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumVanilla>.Cast(byte.MaxValue));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumVanilla>.Cast(ushort.MaxValue));

			Assert.DoesNotThrow(() => EnumExt<EnumVanillaDuplicate>.Cast(1));

			EnumVanilla value = EnumVanilla.None;
			EnumVanilla invalid = (EnumVanilla)(-1);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((sbyte)1));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((short)1));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((int)1));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((long)1));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((byte)1));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((ushort)1));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((uint)1));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((ulong)1));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((sbyte)-1, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == invalid);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((short)-1, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == invalid);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((int)-1, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == invalid);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast((long)-1, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == invalid);

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast(byte.MaxValue, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == (EnumVanilla)(byte.MaxValue));

			value = EnumVanilla.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumVanilla>.Cast(ushort.MaxValue, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == (EnumVanilla)(ushort.MaxValue));
		}

		[TestMethod]
		public void CastNonFlagsBackingTypes() {
			EnumBackedUInt8 uint8Result = EnumBackedUInt8.None;
			EnumBackedUInt16 uint16Result = EnumBackedUInt16.None;
			EnumBackedUInt32 uint32Result = EnumBackedUInt32.None;
			EnumBackedUInt64 uint64Result = EnumBackedUInt64.None;
			EnumBackedInt8 int8Result = EnumBackedInt8.None;
			EnumBackedInt16 int16Result = EnumBackedInt16.None;
			EnumBackedInt32 int32Result = EnumBackedInt32.None;
			EnumBackedInt64 int64Result = EnumBackedInt64.None;

			Assert.DoesNotThrow(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(byte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(byte.MaxValue));
			Assert.DoesNotThrow(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(ushort.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(ushort.MaxValue));
			Assert.DoesNotThrow(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(uint.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(uint.MaxValue));
			Assert.DoesNotThrow(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(ulong.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(ulong.MaxValue));
			Assert.DoesNotThrow(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast((sbyte)0));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(sbyte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(sbyte.MaxValue));
			Assert.DoesNotThrow(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast((short)0));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(short.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(short.MaxValue));
			Assert.DoesNotThrow(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast((int)0));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(int.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(int.MaxValue));
			Assert.DoesNotThrow(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast((long)0));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(long.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint8Result = EnumExt<EnumBackedUInt8>.Cast(long.MaxValue));

			Assert.DoesNotThrow(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(byte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(byte.MaxValue));
			Assert.DoesNotThrow(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(ushort.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(ushort.MaxValue));
			Assert.DoesNotThrow(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(uint.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(uint.MaxValue));
			Assert.DoesNotThrow(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(ulong.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(ulong.MaxValue));
			Assert.DoesNotThrow(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast((sbyte)0));
			Assert.ThrowsExact<ArgumentException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(sbyte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(sbyte.MaxValue));
			Assert.DoesNotThrow(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast((short)0));
			Assert.ThrowsExact<ArgumentException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(short.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(short.MaxValue));
			Assert.DoesNotThrow(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast((int)0));
			Assert.ThrowsExact<ArgumentException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(int.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(int.MaxValue));
			Assert.DoesNotThrow(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast((long)0));
			Assert.ThrowsExact<ArgumentException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(long.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint16Result = EnumExt<EnumBackedUInt16>.Cast(long.MaxValue));

			Assert.DoesNotThrow(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(byte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(byte.MaxValue));
			Assert.DoesNotThrow(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(ushort.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(ushort.MaxValue));
			Assert.DoesNotThrow(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(uint.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(uint.MaxValue));
			Assert.DoesNotThrow(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(ulong.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(ulong.MaxValue));
			Assert.DoesNotThrow(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast((sbyte)0));
			Assert.ThrowsExact<ArgumentException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(sbyte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(sbyte.MaxValue));
			Assert.DoesNotThrow(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast((short)0));
			Assert.ThrowsExact<ArgumentException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(short.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(short.MaxValue));
			Assert.DoesNotThrow(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast((int)0));
			Assert.ThrowsExact<ArgumentException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(int.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(int.MaxValue));
			Assert.DoesNotThrow(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast((long)0));
			Assert.ThrowsExact<ArgumentException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(long.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => uint32Result = EnumExt<EnumBackedUInt32>.Cast(long.MaxValue));

			Assert.DoesNotThrow(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(byte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(byte.MaxValue));
			Assert.DoesNotThrow(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(ushort.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(ushort.MaxValue));
			Assert.DoesNotThrow(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(uint.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(uint.MaxValue));
			Assert.DoesNotThrow(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(ulong.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(ulong.MaxValue));
			Assert.DoesNotThrow(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast((sbyte)0));
			Assert.ThrowsExact<ArgumentException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(sbyte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(sbyte.MaxValue));
			Assert.DoesNotThrow(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast((short)0));
			Assert.ThrowsExact<ArgumentException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(short.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(short.MaxValue));
			Assert.DoesNotThrow(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast((int)0));
			Assert.ThrowsExact<ArgumentException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(int.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(int.MaxValue));
			Assert.DoesNotThrow(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast((long)0));
			Assert.ThrowsExact<ArgumentException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(long.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => uint64Result = EnumExt<EnumBackedUInt64>.Cast(long.MaxValue));

			Assert.DoesNotThrow(() => int8Result = EnumExt<EnumBackedInt8>.Cast(byte.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(byte.MaxValue));
			Assert.DoesNotThrow(() => int8Result = EnumExt<EnumBackedInt8>.Cast(ushort.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(ushort.MaxValue));
			Assert.DoesNotThrow(() => int8Result = EnumExt<EnumBackedInt8>.Cast(uint.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(uint.MaxValue));
			Assert.DoesNotThrow(() => int8Result = EnumExt<EnumBackedInt8>.Cast(ulong.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(ulong.MaxValue));
			Assert.DoesNotThrow(() => int8Result = EnumExt<EnumBackedInt8>.Cast((sbyte)0));
			Assert.ThrowsExact<InvalidCastException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(sbyte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(sbyte.MaxValue));
			Assert.DoesNotThrow(() => int8Result = EnumExt<EnumBackedInt8>.Cast((short)0));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(short.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(short.MaxValue));
			Assert.DoesNotThrow(() => int8Result = EnumExt<EnumBackedInt8>.Cast((int)0));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(int.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(int.MaxValue));
			Assert.DoesNotThrow(() => int8Result = EnumExt<EnumBackedInt8>.Cast((long)0));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(long.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int8Result = EnumExt<EnumBackedInt8>.Cast(long.MaxValue));

			Assert.DoesNotThrow(() => int16Result = EnumExt<EnumBackedInt16>.Cast(byte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(byte.MaxValue));
			Assert.DoesNotThrow(() => int16Result = EnumExt<EnumBackedInt16>.Cast(ushort.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(ushort.MaxValue));
			Assert.DoesNotThrow(() => int16Result = EnumExt<EnumBackedInt16>.Cast(uint.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(uint.MaxValue));
			Assert.DoesNotThrow(() => int16Result = EnumExt<EnumBackedInt16>.Cast(ulong.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(ulong.MaxValue));
			Assert.DoesNotThrow(() => int16Result = EnumExt<EnumBackedInt16>.Cast((sbyte)0));
			Assert.ThrowsExact<InvalidCastException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(sbyte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(sbyte.MaxValue));
			Assert.DoesNotThrow(() => int16Result = EnumExt<EnumBackedInt16>.Cast((short)0));
			Assert.ThrowsExact<InvalidCastException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(short.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(short.MaxValue));
			Assert.DoesNotThrow(() => int16Result = EnumExt<EnumBackedInt16>.Cast((int)0));
			Assert.ThrowsExact<ArgumentException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(int.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(int.MaxValue));
			Assert.DoesNotThrow(() => int16Result = EnumExt<EnumBackedInt16>.Cast((long)0));
			Assert.ThrowsExact<ArgumentException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(long.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int16Result = EnumExt<EnumBackedInt16>.Cast(long.MaxValue));

			Assert.DoesNotThrow(() => int32Result = EnumExt<EnumBackedInt32>.Cast(byte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(byte.MaxValue));
			Assert.DoesNotThrow(() => int32Result = EnumExt<EnumBackedInt32>.Cast(ushort.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(ushort.MaxValue));
			Assert.DoesNotThrow(() => int32Result = EnumExt<EnumBackedInt32>.Cast(uint.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(uint.MaxValue));
			Assert.DoesNotThrow(() => int32Result = EnumExt<EnumBackedInt32>.Cast(ulong.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(ulong.MaxValue));
			Assert.DoesNotThrow(() => int32Result = EnumExt<EnumBackedInt32>.Cast((sbyte)0));
			Assert.ThrowsExact<InvalidCastException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(sbyte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(sbyte.MaxValue));
			Assert.DoesNotThrow(() => int32Result = EnumExt<EnumBackedInt32>.Cast((short)0));
			Assert.ThrowsExact<InvalidCastException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(short.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(short.MaxValue));
			Assert.DoesNotThrow(() => int32Result = EnumExt<EnumBackedInt32>.Cast((int)0));
			Assert.ThrowsExact<InvalidCastException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(int.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(int.MaxValue));
			Assert.DoesNotThrow(() => int32Result = EnumExt<EnumBackedInt32>.Cast((long)0));
			Assert.ThrowsExact<ArgumentException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(long.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int32Result = EnumExt<EnumBackedInt32>.Cast(long.MaxValue));

			Assert.DoesNotThrow(() => int64Result = EnumExt<EnumBackedInt64>.Cast(byte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(byte.MaxValue));
			Assert.DoesNotThrow(() => int64Result = EnumExt<EnumBackedInt64>.Cast(ushort.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(ushort.MaxValue));
			Assert.DoesNotThrow(() => int64Result = EnumExt<EnumBackedInt64>.Cast(uint.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(uint.MaxValue));
			Assert.DoesNotThrow(() => int64Result = EnumExt<EnumBackedInt64>.Cast(ulong.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(ulong.MaxValue));
			Assert.DoesNotThrow(() => int64Result = EnumExt<EnumBackedInt64>.Cast((sbyte)0));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(sbyte.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(sbyte.MaxValue));
			Assert.DoesNotThrow(() => int64Result = EnumExt<EnumBackedInt64>.Cast((short)0));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(short.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(short.MaxValue));
			Assert.DoesNotThrow(() => int64Result = EnumExt<EnumBackedInt64>.Cast((int)0));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(int.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(int.MaxValue));
			Assert.DoesNotThrow(() => int64Result = EnumExt<EnumBackedInt64>.Cast((long)0));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(long.MinValue));
			Assert.ThrowsExact<InvalidCastException>(() => int64Result = EnumExt<EnumBackedInt64>.Cast(long.MaxValue));
		}

		[TestMethod]
		public void TryCastNonFlags() {
			EnumVanilla value;
			EnumVanillaDuplicate duplicateValue;
			bool? result = null;
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast((sbyte)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast((short)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast((int)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast((long)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast((byte)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast((ushort)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast((uint)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast((ulong)0, (InvalidEnumPolicy)(-1), out value));

			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast(uint.MaxValue, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast(ulong.MaxValue, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast(long.MinValue, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast(uint.MaxValue, InvalidEnumPolicy.Allow, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast(ulong.MaxValue, InvalidEnumPolicy.Allow, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumVanilla>.TryCast(long.MinValue, InvalidEnumPolicy.Allow, out value));

			Assert.DoesNotThrow(() => result = EnumExt<EnumVanillaDuplicate>.TryCast(1, out duplicateValue));

			EnumVanilla invalid = (EnumVanilla)(-1);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((sbyte)1, out value));
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((short)1, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((int)1, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((long)1, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((byte)1, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((ushort)1, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((uint)1, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((ulong)1, out value));
			Assert.True(result);
			Assert.True(value == EnumVanilla.One);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((sbyte)-1, out value));
			Assert.False(result);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((sbyte)-1, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == invalid);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((short)-1, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == invalid);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((int)-1, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == invalid);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast((long)-1, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == invalid);

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast(byte.MaxValue, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == (EnumVanilla)(byte.MaxValue));

			value = EnumVanilla.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumVanilla>.TryCast(ushort.MaxValue, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == (EnumVanilla)(ushort.MaxValue));
		}

		[TestMethod]
		public void TryCastNonFlagsBackingTypeUInt8() {
			EnumBackedUInt8 uint8Result = EnumBackedUInt8.None;
			bool? castResult = null;

			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(byte.MinValue, out uint8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(byte.MaxValue, out uint8Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(ushort.MinValue, out uint8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(ushort.MaxValue, out uint8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(uint.MinValue, out uint8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(uint.MaxValue, out uint8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(ulong.MinValue, out uint8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(ulong.MaxValue, out uint8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast((sbyte)0, out uint8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(sbyte.MinValue, out uint8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(sbyte.MaxValue, out uint8Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast((short)0, out uint8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(short.MinValue, out uint8Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(short.MaxValue, out uint8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast((int)0, out uint8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(int.MinValue, out uint8Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(int.MaxValue, out uint8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt8>.TryCast((long)0, out uint8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(long.MinValue, out uint8Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt8>.TryCast(long.MaxValue, out uint8Result));
		}

		[TestMethod]
		public void TryCastNonFlagsBackingTypeUInt16() {
			EnumBackedUInt16 uint16Result = EnumBackedUInt16.None;
			bool? castResult = null;

			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(byte.MinValue, out uint16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(byte.MaxValue, out uint16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(ushort.MinValue, out uint16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(ushort.MaxValue, out uint16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(uint.MinValue, out uint16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(uint.MaxValue, out uint16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(ulong.MinValue, out uint16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(ulong.MaxValue, out uint16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast((sbyte)0, out uint16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(sbyte.MinValue, out uint16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(sbyte.MaxValue, out uint16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast((short)0, out uint16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(short.MinValue, out uint16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(short.MaxValue, out uint16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast((int)0, out uint16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(int.MinValue, out uint16Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(int.MaxValue, out uint16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt16>.TryCast((long)0, out uint16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(long.MinValue, out uint16Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt16>.TryCast(long.MaxValue, out uint16Result));
		}

		[TestMethod]
		public void TryCastNonFlagsBackingTypeUInt32() {
			EnumBackedUInt32 uint32Result = EnumBackedUInt32.None;
			bool? castResult = null;

			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(byte.MinValue, out uint32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(byte.MaxValue, out uint32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(ushort.MinValue, out uint32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(ushort.MaxValue, out uint32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(uint.MinValue, out uint32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(uint.MaxValue, out uint32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(ulong.MinValue, out uint32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(ulong.MaxValue, out uint32Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast((sbyte)0, out uint32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(sbyte.MinValue, out uint32Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(sbyte.MaxValue, out uint32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast((short)0, out uint32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(short.MinValue, out uint32Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(short.MaxValue, out uint32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast((int)0, out uint32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(int.MinValue, out uint32Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(int.MaxValue, out uint32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt32>.TryCast((long)0, out uint32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(long.MinValue, out uint32Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt32>.TryCast(long.MaxValue, out uint32Result));
		}

		[TestMethod]
		public void TryCastNonFlagsBackingTypeUInt64() {
			EnumBackedUInt64 uint64Result = EnumBackedUInt64.None;
			bool? castResult = null;

			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(byte.MinValue, out uint64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(byte.MaxValue, out uint64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(ushort.MinValue, out uint64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(ushort.MaxValue, out uint64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(uint.MinValue, out uint64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(uint.MaxValue, out uint64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(ulong.MinValue, out uint64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(ulong.MaxValue, out uint64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast((sbyte)0, out uint64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(sbyte.MinValue, out uint64Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(sbyte.MaxValue, out uint64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast((short)0, out uint64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(short.MinValue, out uint64Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(short.MaxValue, out uint64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast((int)0, out uint64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(int.MinValue, out uint64Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(int.MaxValue, out uint64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast((long)0, out uint64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(long.MinValue, out uint64Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedUInt64>.TryCast(long.MaxValue, out uint64Result));
			Assert.False(castResult);
		}

		[TestMethod]
		public void TryCastNonFlagsBackingTypeInt8() {
			EnumBackedInt8 int8Result = EnumBackedInt8.None;
			bool? castResult = null;

			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast(byte.MinValue, out int8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(byte.MaxValue, out int8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast(ushort.MinValue, out int8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(ushort.MaxValue, out int8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast(uint.MinValue, out int8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(uint.MaxValue, out int8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast(ulong.MinValue, out int8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(ulong.MaxValue, out int8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast((sbyte)0, out int8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast(sbyte.MinValue, out int8Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast(sbyte.MaxValue, out int8Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast((short)0, out int8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(short.MinValue, out int8Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(short.MaxValue, out int8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast((int)0, out int8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(int.MinValue, out int8Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(int.MaxValue, out int8Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt8>.TryCast((long)0, out int8Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(long.MinValue, out int8Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt8>.TryCast(long.MaxValue, out int8Result));
		}

		[TestMethod]
		public void TryCastNonFlagsBackingTypeInt16() {
			EnumBackedInt16 int16Result = EnumBackedInt16.None;
			bool? castResult = null;

			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(byte.MinValue, out int16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(byte.MaxValue, out int16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(ushort.MinValue, out int16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt16>.TryCast(ushort.MaxValue, out int16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(uint.MinValue, out int16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt16>.TryCast(uint.MaxValue, out int16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(ulong.MinValue, out int16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt16>.TryCast(ulong.MaxValue, out int16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast((sbyte)0, out int16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(sbyte.MinValue, out int16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(sbyte.MaxValue, out int16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast((short)0, out int16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(short.MinValue, out int16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast(short.MaxValue, out int16Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast((int)0, out int16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt16>.TryCast(int.MinValue, out int16Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt16>.TryCast(int.MaxValue, out int16Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt16>.TryCast((long)0, out int16Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt16>.TryCast(long.MinValue, out int16Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt16>.TryCast(long.MaxValue, out int16Result));
		}

		[TestMethod]
		public void TryCastNonFlagsBackingTypeInt32() {
			EnumBackedInt32 int32Result = EnumBackedInt32.None;
			bool? castResult = null;

			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(byte.MinValue, out int32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(byte.MaxValue, out int32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(ushort.MinValue, out int32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(ushort.MaxValue, out int32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(uint.MinValue, out int32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt32>.TryCast(uint.MaxValue, out int32Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(ulong.MinValue, out int32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt32>.TryCast(ulong.MaxValue, out int32Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast((sbyte)0, out int32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(sbyte.MinValue, out int32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(sbyte.MaxValue, out int32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast((short)0, out int32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(short.MinValue, out int32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(short.MaxValue, out int32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast((int)0, out int32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(int.MinValue, out int32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast(int.MaxValue, out int32Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt32>.TryCast((long)0, out int32Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt32>.TryCast(long.MinValue, out int32Result));
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt32>.TryCast(long.MaxValue, out int32Result));
		}

		[TestMethod]
		public void TryCastNonFlagsBackingTypeInt64() {
			EnumBackedInt64 int64Result = EnumBackedInt64.None;
			bool? castResult = null;

			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(byte.MinValue, out int64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(byte.MaxValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(ushort.MinValue, out int64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(ushort.MaxValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(uint.MinValue, out int64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(uint.MaxValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(ulong.MinValue, out int64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.ThrowsExact<ArgumentException>(() => castResult = EnumExt<EnumBackedInt64>.TryCast(ulong.MaxValue, out int64Result));
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast((sbyte)0, out int64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(sbyte.MinValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(sbyte.MaxValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast((short)0, out int64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(short.MinValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(short.MaxValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast((int)0, out int64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(int.MinValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(int.MaxValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast((long)0, out int64Result));
			Assert.True(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(long.MinValue, out int64Result));
			Assert.False(castResult);
			castResult = null;
			Assert.DoesNotThrow(() => castResult = EnumExt<EnumBackedInt64>.TryCast(long.MaxValue, out int64Result));
			Assert.False(castResult);
		}

		[TestMethod]
		public void CastFlags() {
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast((sbyte)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast((short)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast((int)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast((long)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast((byte)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast((ushort)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast((uint)0, policy: (InvalidEnumPolicy)(-1)));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast((ulong)0, policy: (InvalidEnumPolicy)(-1)));

			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast(uint.MaxValue));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast(ulong.MaxValue));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast(long.MinValue));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast(uint.MaxValue, policy: InvalidEnumPolicy.Allow));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast(ulong.MaxValue, policy: InvalidEnumPolicy.Allow));
			Assert.ThrowsExact<ArgumentException>(() => EnumExt<EnumSimpleFlags>.Cast(long.MinValue, policy: InvalidEnumPolicy.Allow));

			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumSimpleFlags>.Cast((sbyte)-1));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumSimpleFlags>.Cast((short)-1));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumSimpleFlags>.Cast((int)-1));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumSimpleFlags>.Cast((long)-1));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumSimpleFlags>.Cast(byte.MaxValue));
			Assert.ThrowsExact<InvalidCastException>(() => EnumExt<EnumSimpleFlags>.Cast(ushort.MaxValue));

			Assert.DoesNotThrow(() => EnumExt<EnumSimpleDuplicateFlags>.Cast(1 << 2));

			EnumSimpleFlags value = EnumSimpleFlags.None;
			EnumSimpleFlags invalid = (EnumSimpleFlags)(-1);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((sbyte)1));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((short)1));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((int)1));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((long)1));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((byte)1));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((ushort)1));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((uint)1));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((ulong)1));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((sbyte)-1, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == invalid);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((short)-1, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == invalid);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((int)-1, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == invalid);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast((long)-1, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == invalid);

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast(byte.MaxValue, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == (EnumSimpleFlags)(byte.MaxValue));

			value = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => value = EnumExt<EnumSimpleFlags>.Cast(ushort.MaxValue, policy: InvalidEnumPolicy.Allow));
			Assert.True(value == (EnumSimpleFlags)(ushort.MaxValue));
		}

		[TestMethod]
		public void TryCastFlags() {
			EnumSimpleFlags value;
			EnumSimpleDuplicateFlags duplicateValue;
			bool? result = null;
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast((sbyte)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast((short)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast((int)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast((long)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast((byte)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast((ushort)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast((uint)0, (InvalidEnumPolicy)(-1), out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast((ulong)0, (InvalidEnumPolicy)(-1), out value));

			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast(uint.MaxValue, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast(ulong.MaxValue, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast(long.MinValue, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast(uint.MaxValue, InvalidEnumPolicy.Allow, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast(ulong.MaxValue, InvalidEnumPolicy.Allow, out value));
			Assert.ThrowsExact<ArgumentException>(() => result = EnumExt<EnumSimpleFlags>.TryCast(long.MinValue, InvalidEnumPolicy.Allow, out value));

			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleDuplicateFlags>.TryCast(1, out duplicateValue));

			EnumSimpleFlags invalid = (EnumSimpleFlags)(-1);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((sbyte)1, out value));
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((short)1, out value));
			Assert.True(result);
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((int)1, out value));
			Assert.True(result);
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((long)1, out value));
			Assert.True(result);
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((byte)1, out value));
			Assert.True(result);
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((ushort)1, out value));
			Assert.True(result);
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((uint)1, out value));
			Assert.True(result);
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((ulong)1, out value));
			Assert.True(result);
			Assert.True(value == EnumSimpleFlags.BitOne);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((sbyte)-1, out value));
			Assert.False(result);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((sbyte)-1, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == invalid);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((short)-1, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == invalid);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((int)-1, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == invalid);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast((long)-1, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == invalid);

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast(byte.MaxValue, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == (EnumSimpleFlags)(byte.MaxValue));

			value = EnumSimpleFlags.None;
			result = null;
			Assert.DoesNotThrow(() => result = EnumExt<EnumSimpleFlags>.TryCast(ushort.MaxValue, InvalidEnumPolicy.Allow, out value));
			Assert.True(result);
			Assert.True(value == (EnumSimpleFlags)(ushort.MaxValue));
		}
	}
}
