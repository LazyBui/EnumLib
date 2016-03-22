using System;
using System.Collections.Generic;

namespace System {
	public sealed class EnumMemberInfo {
		public string Name { get; private set; }
		public string Description { get; private set; }
		public IEnumerable<Attribute> Attributes { get; private set; }

		internal EnumMemberInfo(string name, string description, IEnumerable<Attribute> attributes) {
			Name = name;
			Description = description;
			Attributes = attributes;
		}
	}
}