using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = TestLib.Framework.Assert;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace EnumLib.Tests {
	[TestClass]
	public class IEnumerableExtensionsTest {
		[TestMethod]
		public void GenericEnum() {
			Assert.ThrowsExact<TypeInitializationException>(() => new int[0].ThrowIfAnyInvalidEnums());
			Assert.ThrowsExact<TypeInitializationException>(() => new float[0].ThrowIfAnyInvalidEnums());
			Assert.ThrowsExact<TypeInitializationException>(() => new Guid[0].ThrowIfAnyInvalidEnums());
			Assert.ThrowsExact<TypeInitializationException>(() => new DateTime[0].ThrowIfAnyInvalidEnums());
			Assert.ThrowsExact<ArgumentNullException>(() => (null as EnumVanilla[]).ThrowIfAnyInvalidEnums());
			Assert.DoesNotThrow(() => new EnumVanilla[0].ThrowIfAnyInvalidEnums());
			Assert.DoesNotThrow(() => new EnumSimpleFlags[0].ThrowIfAnyInvalidEnums());
			Assert.DoesNotThrow(() => new EnumComboFlags[0].ThrowIfAnyInvalidEnums());

			Assert.DoesNotThrow(() => new[] { EnumSimpleFlags.BitOne }.ThrowIfAnyInvalidEnums());
			Assert.ThrowsExact<ArgumentException>(() => new[] { ~EnumSimpleFlags.BitOne }.ThrowIfAnyInvalidEnums());
			Assert.DoesNotThrow(() => new[] { EnumSimpleFlags.None }.ThrowIfAnyInvalidEnums());
			Assert.ThrowsExact<ArgumentException>(() => new[] { EnumSimpleFlags.BitOne, ~EnumSimpleFlags.BitOne, EnumSimpleFlags.None }.ThrowIfAnyInvalidEnums());
		}
	}
}
