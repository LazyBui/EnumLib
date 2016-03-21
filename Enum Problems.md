This is a full list of the problems I'm aware of with .NET enums, the problems the problems introduce, and how this library does or doesn't address them.

1. Flags are conflated with plain enums, even though they're very distinct concepts.
	This causes many problems since you can use them both precisely the same way even though they should have completely separate APIs. It makes no sense to or a regular enum, for example, but it must be there to support flags. The library cannot address this problem because it's fundamental to .NET, but the library will perform type checking to throw an exception if flags operations are being performed on non-flags enums or non-flags operations are being performed on flags enums.

2. Enums of any kind are not type safe.
	+ They do not force their values to be consistent with the definition of an enum. For example, you can do `(MyEnum)(-2)`. This is invalid on the vast majority of enums. However, .NET will accept this with no issues. This introduces two problems.
		* You must verify that an enum value you accept as a parameter is a valid one just like you must check for non-null values. This library provides a `ThrowIfInvalid` extension method on `Enum` to support this pattern.
		* If you want to keep invalid values out of your program entirely, this means checking at every cast site. `EnumExt` provides parsing and casting functions that reject invalid values if configured to do so (which is the default).
	+ The type system of .NET itself thinks of enum values in terms of their underlying integer. This has a few consequences. First, two enumeration values that are not the same may compare equal. That is, `MyEnum.One == MyEnum.AlsoOne`. Additionally, any function within the .NET Framework to interact with specific enum values (e.g. Enum.GetName, Enum.ToString) will fail for some definitions of fail upon receiving such a duplicate value. The library cannot address this issue and instead throws an AmbiguousEnumException when it encounters such a situation and it makes a difference (e.g. `EnumExt.GetName`, but not `EnumExt.TryParse`). This must be resolved manually (and ideally being explicit about the expected behavior) by the programmer.
	+ Many of the methods on and relating to Enum in the .NET Framework are based on object, meaning that much casting must be done. This is ugly and needless with generics. Additionally, there are even problems such as providing an integer that's different from the underlying type that become apparent through this weakly typed model. The library provides `EnumExt` as a facade to resolve generic types in cases where one is present in the formal parameter list and `EnumExt&lt;MyEnum&gt;` otherwise in order to utilize strong typing.

3. There is not a simple way to interrogate an enum value or type for whether it is a flags type.
	The library provides an `IsFlagsType` extension method on both `Type` and `Enum`, as well as methods in `EnumExt` to interrogate an enum type.

4. `Enum.IsDefined` does not interpret `FlagsAttribute`.
	As such, it is insufficient to check whether a set of flags is a valid one based on the enum definition. The library provides a `HasValidValue` extension method on `Enum` or an `IsValidValue` method on `EnumExt` to interrogate enums. Additionally, `EnumExt` has an `IsDefined` method which behaves more consistently with expectations.

5. `HasFlags` has unclear semantics and is not available at all if older than .NET Framework 4.
	It's unclear what the relationship between the specified flags and the original value must be based on this function alone. It's unclear whether it's an and or or relationship to the original value. For the record, it's equivalent to `HasAnyFlags`. This library provides clearer semantics by providing `HasAllFlags` and `HasAnyFlags` extension methods on `Enum`.

6. There are other operations that may be desirable for flags.
	+ This library provides `HasNoFlags`, `HasOnlyFlags`, and `HasExactlyFlags` to expand the operations that a user can perform out of the box with enums.
	+ `HasNoFlags`, `HasAnyFlags`, and `HasAllFlags` can be used without argument to determine whether no, any, or all flags are set.
	+ `EnumExt` contains a method for extracting flags of a value.
