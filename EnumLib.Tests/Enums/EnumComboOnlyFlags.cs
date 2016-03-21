using System;
using System.ComponentModel;

namespace EnumLib.Tests {
	[Flags]
	public enum EnumComboOnlyFlags {
		None = 0,
		[Description("Indicates ...")]
		BitOne = 1 << 0,
		[Description("Indicates ...")]
		BitTwo = 1 << 1,
		[Description("Indicates ...")]
		BitThree = 1 << 2,
		[Description("Combines ...")]
		BitsOneFour = BitOne | (1 << 3),
		[Description("Combines ...")]
		BitsOneTwoFour = BitOne | BitTwo | (1 << 3),
	}
}