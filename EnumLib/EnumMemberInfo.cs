using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System {
	/// <summary>
	/// Represents information about an enum value.
	/// </summary>
	public sealed class EnumMemberInfo {
		/// <summary>
		/// The name of the enum value.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// The <see cref="DescriptionAttribute" /> text of the enum value.
		/// </summary>
		public string Description { get; private set; }
		/// <summary>
		/// A list of <see cref="Attribute" /> values associated to the enum value.
		/// </summary>
		public IEnumerable<Attribute> Attributes { get; private set; }

		internal EnumMemberInfo(string name, string description, IEnumerable<Attribute> attributes) {
			Name = name;
			Description = description;
			Attributes = attributes;
		}
	}
}