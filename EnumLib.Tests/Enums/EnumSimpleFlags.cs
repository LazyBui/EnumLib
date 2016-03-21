using System;
using System.ComponentModel;

namespace EnumLib.Tests {
	[Flags]
	public enum EnumSimpleFlags {
		None = 0,
		[Description("Indicates ...")]
		BitOne = 1 << 0,
		[Description("Indicates ...")]
		BitTwo = 1 << 1,
		[Description("Indicates ...")]
		BitThree = 1 << 2,
		[Description("Indicates ...")]
		BitFour = 1 << 3,
	}
}