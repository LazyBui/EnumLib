using System;
using System.ComponentModel;

namespace EnumLib.Tests {
	[CustomEnum("Hello")]
	[CustomEnum("Hello again")]
	[Description("Testing")]
	public enum EnumVanilla {
		None = 0,
		[CustomEnum("Hello")]
		[CustomEnum("Hello again")]
		[Description("Eleven")]
		One,
		[Description("Twelve")]
		Two,
		[Description("Thirteen")]
		Three,
		[Description("Fourteen")]
		Four,
	}
}