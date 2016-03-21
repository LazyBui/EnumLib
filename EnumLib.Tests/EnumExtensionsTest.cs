using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = TestLib.Framework.Assert;

namespace EnumLib.Tests {
	[TestClass]
	public class EnumExtensionsTest {
		[TestMethod]
		public void ThrowIfInvalid() {
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.ThrowIfInvalid(null));

			EnumSimpleFlags testSimpleFlags1 = EnumSimpleFlags.BitOne;
			Assert.DoesNotThrow(() => testSimpleFlags1.ThrowIfInvalid());

			EnumSimpleFlags testSimpleFlags2 = ~EnumSimpleFlags.BitOne;
			Assert.ThrowsExact<ArgumentException>(() => testSimpleFlags2.ThrowIfInvalid());

			EnumSimpleFlags testSimpleFlags3 = EnumSimpleFlags.None;
			Assert.DoesNotThrow(() => testSimpleFlags3.ThrowIfInvalid());

			EnumSimpleDuplicateFlags testSimpleDuplicateFlags1 = EnumSimpleDuplicateFlags.BitThree;
			Assert.DoesNotThrow(() => testSimpleDuplicateFlags1.ThrowIfInvalid());

			EnumSimpleDuplicateFlags testSimpleDuplicateFlags2 = ~EnumSimpleDuplicateFlags.BitAlsoThree;
			Assert.ThrowsExact<ArgumentException>(() => testSimpleDuplicateFlags2.ThrowIfInvalid());

			EnumSimpleDuplicateFlags testSimpleDuplicateFlags3 = EnumSimpleDuplicateFlags.None;
			Assert.DoesNotThrow(() => testSimpleDuplicateFlags3.ThrowIfInvalid());

			EnumVanilla testVanilla1 = EnumVanilla.Three;
			Assert.DoesNotThrow(() => testVanilla1.ThrowIfInvalid());

			EnumVanilla testVanilla2 = (EnumVanilla)(-1);
			Assert.ThrowsExact<ArgumentException>(() => testVanilla2.ThrowIfInvalid());

			EnumVanilla testVanilla3 = EnumVanilla.None;
			Assert.DoesNotThrow(() => testVanilla3.ThrowIfInvalid());

			EnumVanillaDuplicate testVanillaDuplicate1 = EnumVanillaDuplicate.One;
			Assert.DoesNotThrow(() => testVanillaDuplicate1.ThrowIfInvalid());

			EnumVanillaDuplicate testVanillaDuplicate2 = EnumVanillaDuplicate.AlsoOne;
			Assert.DoesNotThrow(() => testVanillaDuplicate2.ThrowIfInvalid());

			EnumVanillaDuplicate testVanillaDuplicate3 = (EnumVanillaDuplicate)(-1);
			Assert.ThrowsExact<ArgumentException>(() => testVanillaDuplicate3.ThrowIfInvalid());

			EnumComboFlags testComboFlags1 = EnumComboFlags.BitsOneThree;
			Assert.DoesNotThrow(() => testComboFlags1.ThrowIfInvalid());

			EnumComboFlags testComboFlags2 = EnumComboFlags.BitOne | (EnumComboFlags)(1 << 4);
			Assert.ThrowsExact<ArgumentException>(() => testComboFlags2.ThrowIfInvalid());

			EnumComboFlags testComboFlags3 = EnumComboFlags.BitsOneTwoThree;
			Assert.DoesNotThrow(() => testComboFlags3.ThrowIfInvalid());

			EnumComboFlags testComboFlags4 = EnumComboFlags.BitsOneTwoThree | (EnumComboFlags)(1 << 4);
			Assert.ThrowsExact<ArgumentException>(() => testComboFlags4.ThrowIfInvalid());

			EnumComboFlags testComboFlags5 = 0;
			Assert.ThrowsExact<ArgumentException>(() => testComboFlags5.ThrowIfInvalid());

			EnumComboOnlyFlags testComboOnlyFlags1 = EnumComboOnlyFlags.BitsOneFour;
			Assert.DoesNotThrow(() => testComboOnlyFlags1.ThrowIfInvalid());

			EnumComboOnlyFlags testComboOnlyFlags2 = EnumComboOnlyFlags.BitOne | (EnumComboOnlyFlags)(1 << 5);
			Assert.ThrowsExact<ArgumentException>(() => testComboOnlyFlags2.ThrowIfInvalid());

			EnumComboOnlyFlags testComboOnlyFlags3 = EnumComboOnlyFlags.BitsOneTwoFour;
			Assert.DoesNotThrow(() => testComboOnlyFlags3.ThrowIfInvalid());

			EnumComboOnlyFlags testComboOnlyFlags4 = EnumComboOnlyFlags.BitsOneTwoFour | (EnumComboOnlyFlags)(1 << 5);
			Assert.ThrowsExact<ArgumentException>(() => testComboOnlyFlags4.ThrowIfInvalid());

			EnumComboOnlyFlags testComboOnlyFlags5 = (EnumComboOnlyFlags)(1 << 3);
			Assert.ThrowsExact<ArgumentException>(() => testComboOnlyFlags5.ThrowIfInvalid());
		}

		[TestMethod]
		public void HasValidValue() {
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasValidValue(null));

			bool result = false;
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.BitOne.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (~EnumSimpleFlags.BitOne).HasValidValue());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.None.HasValidValue());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumSimpleDuplicateFlags.BitThree.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumSimpleDuplicateFlags.BitAlsoThree.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (~EnumSimpleDuplicateFlags.BitThree).HasValidValue());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumSimpleDuplicateFlags.None.HasValidValue());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumVanilla.Three.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = ((EnumVanilla)(-1)).HasValidValue());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumVanilla.None.HasValidValue());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumVanillaDuplicate.One.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumVanillaDuplicate.AlsoOne.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = ((EnumVanillaDuplicate)(-1)).HasValidValue());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumVanillaDuplicate.None.HasValidValue());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (EnumComboFlags.BitOne | (EnumComboFlags)(1 << 4)).HasValidValue());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneTwoThree.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (EnumComboFlags.BitsOneTwoThree | (EnumComboFlags)(1 << 4)).HasValidValue());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = ((EnumComboFlags)(0)).HasValidValue());
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (EnumComboOnlyFlags.BitOne | (EnumComboOnlyFlags)(1 << 5)).HasValidValue());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneTwoFour.HasValidValue());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (EnumComboOnlyFlags.BitsOneTwoFour | (EnumComboOnlyFlags)(1 << 5)).HasValidValue());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = ((EnumComboOnlyFlags)(1 << 3)).HasValidValue());
			Assert.False(result);
		}

		[TestMethod]
		public void IsFlagsType() {
			Type enumType = null;
			Assert.ThrowsExact<ArgumentNullException>(() => enumType.IsFlagsType());
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.IsFlagsType(null as Enum));

			bool result = false;
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.BitOne.IsFlagsType());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumVanilla.Three.IsFlagsType());
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.IsFlagsType());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.IsFlagsType());
			Assert.True(result);
		}

		[TestMethod]
		public void HasAnyFlags() {
			EnumSimpleFlags invalid = (EnumSimpleFlags)(-1);
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasAnyFlags(null));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasAnyFlags(null, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasAnyFlags(EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasAnyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAnyFlags(invalid, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAnyFlags(EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAnyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null as Enum));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAnyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAnyFlags(EnumComboFlags.BitOne, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAnyFlags(EnumSimpleFlags.None, EnumComboFlags.BitOne));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAnyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, EnumComboFlags.BitOne));

			bool result = false;
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.BitOne.HasAnyFlags());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.None.HasAnyFlags());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = (EnumSimpleFlags.BitOne | EnumSimpleFlags.BitTwo | EnumSimpleFlags.BitThree | EnumSimpleFlags.BitFour).HasAnyFlags());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumComboFlags.BitOne.HasAnyFlags(EnumComboFlags.BitsOneThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasAnyFlags(EnumComboFlags.BitOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasAnyFlags(EnumComboFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasAnyFlags(EnumComboFlags.BitOne, EnumComboFlags.BitTwo));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasAnyFlags(EnumComboFlags.BitOne, EnumComboFlags.BitThree));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitOne.HasAnyFlags(EnumComboOnlyFlags.BitsOneFour));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasAnyFlags(EnumComboOnlyFlags.BitOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasAnyFlags(EnumComboOnlyFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasAnyFlags(EnumComboOnlyFlags.None));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.None.HasAnyFlags(EnumComboOnlyFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasAnyFlags(EnumComboOnlyFlags.BitOne, EnumComboOnlyFlags.BitTwo));
			Assert.True(result);
		}

		[TestMethod]
		public void HasNoFlags() {
			EnumSimpleFlags invalid = (EnumSimpleFlags)(-1);
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasNoFlags(null));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasNoFlags(null, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasNoFlags(EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasNoFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasNoFlags(invalid, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasNoFlags(EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasNoFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null as Enum));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasNoFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasNoFlags(EnumComboFlags.BitOne, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasNoFlags(EnumSimpleFlags.None, EnumComboFlags.BitOne));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasNoFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, EnumComboFlags.BitOne));

			bool result = false;
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.BitOne.HasNoFlags());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.None.HasNoFlags());
			Assert.True(result);
			Assert.DoesNotThrow(() => result = (EnumSimpleFlags.BitOne | EnumSimpleFlags.BitTwo | EnumSimpleFlags.BitThree | EnumSimpleFlags.BitFour).HasNoFlags());
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumComboFlags.BitOne.HasNoFlags(EnumComboFlags.BitsOneThree));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasNoFlags(EnumComboFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasNoFlags(EnumComboFlags.BitTwo));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasNoFlags(EnumComboFlags.BitOne, EnumComboFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasNoFlags(EnumComboFlags.BitOne, EnumComboFlags.BitThree));
			Assert.False(result);

			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitOne.HasNoFlags(EnumComboOnlyFlags.BitsOneFour));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasNoFlags(EnumComboOnlyFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasNoFlags(EnumComboOnlyFlags.BitTwo));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasNoFlags(EnumComboOnlyFlags.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.None.HasNoFlags(EnumComboOnlyFlags.BitOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasNoFlags(EnumComboOnlyFlags.BitOne, EnumComboOnlyFlags.BitTwo));
			Assert.False(result);
		}

		[TestMethod]
		public void HasAllFlags() {
			EnumSimpleFlags invalid = (EnumSimpleFlags)(-1);
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasAllFlags(null, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasAllFlags(EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasAllFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAllFlags(invalid, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAllFlags(EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAllFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null as Enum));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAllFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAllFlags(EnumComboFlags.BitOne, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAllFlags(EnumSimpleFlags.None, EnumComboFlags.BitOne));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasAllFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, EnumComboFlags.BitOne));

			bool result = false;
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.BitOne.HasAllFlags());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumSimpleFlags.None.HasAllFlags());
			Assert.False(result);
			Assert.DoesNotThrow(() => result = (EnumSimpleFlags.BitOne | EnumSimpleFlags.BitTwo | EnumSimpleFlags.BitThree | EnumSimpleFlags.BitFour).HasAllFlags());
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumComboFlags.BitOne.HasAllFlags(EnumComboFlags.BitsOneThree));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasAllFlags(EnumComboFlags.BitOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasAllFlags(EnumComboFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasAllFlags(EnumComboFlags.BitOne, EnumComboFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasAllFlags(EnumComboFlags.BitOne, EnumComboFlags.BitThree));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitOne.HasAllFlags(EnumComboOnlyFlags.BitsOneFour));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasAllFlags(EnumComboOnlyFlags.BitOne));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasAllFlags(EnumComboOnlyFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasAllFlags(EnumComboOnlyFlags.None));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.None.HasAllFlags(EnumComboOnlyFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasAllFlags(EnumComboOnlyFlags.BitOne, EnumComboOnlyFlags.BitTwo));
			Assert.False(result);
		}

		[TestMethod]
		public void HasOnlyFlags() {
			EnumSimpleFlags invalid = (EnumSimpleFlags)(-1);
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasOnlyFlags(null, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasOnlyFlags(EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasOnlyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasOnlyFlags(invalid, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasOnlyFlags(EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasOnlyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null as Enum));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasOnlyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasOnlyFlags(EnumComboFlags.BitOne, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasOnlyFlags(EnumSimpleFlags.None, EnumComboFlags.BitOne));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasOnlyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, EnumComboFlags.BitOne));

			bool result = false;
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitOne.HasOnlyFlags(EnumComboFlags.BitsOneThree));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasOnlyFlags(EnumComboFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasOnlyFlags(EnumComboFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasOnlyFlags(EnumComboFlags.BitOne, EnumComboFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasOnlyFlags(EnumComboFlags.BitOne, EnumComboFlags.BitThree));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitOne.HasOnlyFlags(EnumComboOnlyFlags.BitsOneFour));
			Assert.True(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasOnlyFlags(EnumComboOnlyFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasOnlyFlags(EnumComboOnlyFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasOnlyFlags(EnumComboOnlyFlags.None));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.None.HasOnlyFlags(EnumComboOnlyFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasOnlyFlags(EnumComboOnlyFlags.BitOne, EnumComboOnlyFlags.BitTwo));
			Assert.False(result);
		}

		[TestMethod]
		public void HasExactlyFlags() {
			EnumSimpleFlags invalid = (EnumSimpleFlags)(-1);
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasExactlyFlags(null, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasExactlyFlags(EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.HasExactlyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasExactlyFlags(invalid, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasExactlyFlags(EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasExactlyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, null as Enum));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasExactlyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, invalid));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasExactlyFlags(EnumComboFlags.BitOne, EnumSimpleFlags.None));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasExactlyFlags(EnumSimpleFlags.None, EnumComboFlags.BitOne));
			Assert.ThrowsExact<ArgumentException>(() => EnumExtensions.HasExactlyFlags(EnumSimpleFlags.None, EnumSimpleFlags.None, EnumComboFlags.BitOne));

			bool result = false;
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitOne.HasExactlyFlags(EnumComboFlags.BitsOneThree));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasExactlyFlags(EnumComboFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasExactlyFlags(EnumComboFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasExactlyFlags(EnumComboFlags.BitOne, EnumComboFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboFlags.BitsOneThree.HasExactlyFlags(EnumComboFlags.BitOne, EnumComboFlags.BitThree));
			Assert.True(result);

			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitOne.HasExactlyFlags(EnumComboOnlyFlags.BitsOneFour));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasExactlyFlags(EnumComboOnlyFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasExactlyFlags(EnumComboOnlyFlags.BitTwo));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasExactlyFlags(EnumComboOnlyFlags.None));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.None.HasExactlyFlags(EnumComboOnlyFlags.BitOne));
			Assert.False(result);
			Assert.DoesNotThrow(() => result = EnumComboOnlyFlags.BitsOneFour.HasExactlyFlags(EnumComboOnlyFlags.BitOne, EnumComboOnlyFlags.BitTwo));
			Assert.False(result);
		}

		[TestMethod]
		public void As() {
			Assert.ThrowsExact<ArgumentNullException>(() => EnumExtensions.As<int>(null));
			Assert.ThrowsExact<InvalidOperationException>(() => EnumExtensions.As<double>(EnumVanilla.None));
			Assert.ThrowsExact<InvalidOperationException>(() => EnumExtensions.As<Guid>(EnumVanilla.None));
			Assert.ThrowsExact<InvalidOperationException>(() => EnumExtensions.As<DateTime>(EnumVanilla.None));

			int value = 0;

			value = 0;
			Assert.DoesNotThrow(() => value = EnumVanilla.One.As<sbyte>());
			Assert.Equal(value, 1);

			value = 0;
			Assert.DoesNotThrow(() => value = EnumVanilla.One.As<short>());
			Assert.Equal(value, 1);

			value = 0;
			Assert.DoesNotThrow(() => value = EnumVanilla.One.As<int>());
			Assert.Equal(value, 1);

			value = 0;
			Assert.DoesNotThrow(() => value = (int)EnumVanilla.One.As<long>());
			Assert.Equal(value, 1);

			value = 0;
			Assert.DoesNotThrow(() => value = EnumVanilla.One.As<byte>());
			Assert.Equal(value, 1);

			value = 0;
			Assert.DoesNotThrow(() => value = EnumVanilla.One.As<ushort>());
			Assert.Equal(value, 1);

			value = 0;
			Assert.DoesNotThrow(() => value = (int)EnumVanilla.One.As<uint>());
			Assert.Equal(value, 1);

			value = 0;
			Assert.DoesNotThrow(() => value = (int)EnumVanilla.One.As<ulong>());
			Assert.Equal(value, 1);
		}
	}
}
