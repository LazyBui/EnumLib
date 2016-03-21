using System;
using System.ComponentModel;

namespace EnumLib.Tests {
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
	public sealed class CustomEnumAttribute : Attribute {
		public string Data { get; private set; }

		public CustomEnumAttribute(string data) {
			Data = data;
		}
	}
}