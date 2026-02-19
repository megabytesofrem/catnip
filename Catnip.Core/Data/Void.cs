namespace Catnip.Core.Data;

using Catnip.Core;

public sealed class VoidW { }

/// <summary>
/// The Void type represents a type that has no values.
/// </summary>
/// <typeparam name="A"></typeparam>
public record Void<A> : HKT<VoidW, A>;