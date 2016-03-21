using System;
using System.ComponentModel;

namespace EnumLib.Tests {
	public enum EnumVanillaDuplicate {
		None = 0,
		[Description("Eleven")]
		One = 1,
		[Description("Twelve")]
		Two = 2,
		[Description("Eleven")]
		AlsoOne = 1,
	}
}