[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Plossum.Pipes")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Plossum")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Plossum.Collections")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Plossum")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Plossum.Collections.IMultiDictionary`2")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "Plossum.Collections.IMultiDictionary`2")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Plossum.Collections.MultiHashDictionary`2")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "Plossum.Collections.MultiHashDictionary`2")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Plossum.Collections.MultiHashDictionary`2..ctor(C5.Fun`1<C5.ICollection`1<V>>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Plossum.Collections.MultiHashDictionary`2..ctor(C5.Fun`1<C5.ICollection`1<TValue>)")]

// This is actually not an intialization, but assigning null to private members to allow GC to free the resources.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member", Target = "Plossum.StringFormatter+OptimalWordWrappedString..ctor(System.String,System.Int32)")]

// Not possible, nor needed here
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Scope = "member", Target = "Plossum.CommandLine.AttributeException..ctor(System.Type,System.Reflection.MemberInfo,System.String)")]

// Not possible, nor needed here
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Scope = "member", Target = "Plossum.CommandLine.AttributeException..ctor(System.Type,System.Type,System.String)")]

// Not that complex in practice, just a little long
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "Plossum.CommandLine.CommandLineParser..ctor(System.Object,System.Globalization.NumberFormatInfo)")]

// Not that complex in practice, just a little long
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "Plossum.CommandLine.CommandLineParser.Parse(System.IO.TextReader,System.Boolean):System.Void")]

// Not that complex in practice, just a little long
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "Plossum.CommandLine.Option..ctor(Plossum.CommandLine.CommandLineOptionAttribute,System.Reflection.MemberInfo,System.Object,C5.ICollection`1<Plossum.CommandLine.OptionGroup>,System.Globalization.NumberFormatInfo)")]

// Ehh? Don't want this in a string table.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Scope = "member", Target = "Plossum.CommandLine.OptionFileToken..ctor(System.String)", MessageId = "Plossum.CommandLine.Token.#ctor(Plossum.CommandLine.Token+TokenTypes,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Plossum.IO")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Plossum.CommandLine.InvalidEnumerationValueException..ctor(System.String,System.Exception)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Plossum.CommandLine.OptionOccurenceException..ctor(System.String,System.Exception)")]
