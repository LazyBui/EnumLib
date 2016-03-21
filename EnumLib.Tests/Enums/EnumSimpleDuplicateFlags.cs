using System;
using System.ComponentModel;

namespace EnumLib.Tests {
	[Flags]
	public enum EnumSimpleDuplicateFlags {
		None = 0,
		[Description("Indicates ...")]
		BitOne = 1 << 0,
		[Description("Indicates ...")]
		BitTwo = 1 << 1,
		[Description("Indicates ...")]
		BitThree = 1 << 2,
		[Description("Indicates ...")]
		BitAlsoThree = 1 << 2,
	}
}