namespace Catnip.Core.Data;

public sealed class MaybeW { }

/// <summary>
/// The Maybe type represents an optional value, either Just or Nothing.
/// </summary>
/// <typeparam name="A"></typeparam>
public abstract record Maybe<A> : HKT<MaybeW, A>
{
    public sealed record Just(A Value) : Maybe<A>;
    public sealed record Nothing() : Maybe<A>;

    // Projection function to extract the Maybe<A> from the HKT.
    public static Maybe<A> Proj(HKT<MaybeW, A> h) => (Maybe<A>)h;
}

/// <summary>
/// The Maybe type represents an optional value, either Just or Nothing.
/// </summary>
/// <typeparam name="A"></typeparam>
public static class Maybe
{
    public static Maybe<A> Just<A>(A value) => new Maybe<A>.Just(value);
    public static Maybe<A> Nothing<A>() => new Maybe<A>.Nothing();
}