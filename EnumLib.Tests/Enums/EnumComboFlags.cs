using System;
using System.ComponentModel;

namespace EnumLib.Tests {
	[Flags]
	public enum EnumComboFlags {
		[Description("Indicates ...")]
		BitOne = 1 << 0,
		[Description("Indicates ...")]
		BitTwo = 1 << 1,
		[Description("Indicates ...")]
		BitThree = 1 << 2,
		[Description("Combines ...")]
		BitsOneThree = BitOne | BitThree,
		[Description("Combines ...")]
		BitsOneTwoThree = BitOne | BitTwo | BitThree,
	}
}