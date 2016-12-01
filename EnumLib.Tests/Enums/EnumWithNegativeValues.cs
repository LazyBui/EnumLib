using System;
using System.ComponentModel;

namespace EnumLib.Tests {
	public enum EnumWithNegativeValues : sbyte {
		Low = -34,
		Middle = -22,
		High = -3,
	}
}