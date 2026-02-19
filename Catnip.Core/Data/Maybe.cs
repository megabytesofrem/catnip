namespace Catnip.Core.Data;

using Catnip.Core.Control;

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
public sealed class Maybe
    : Eq<MaybeW>,
      Functor<MaybeW>,
      Applicative<MaybeW>,
      Monad<MaybeW>,
      Semigroup<MaybeW>
{
    public static Maybe<A> Just<A>(A value) => new Maybe<A>.Just(value);
    public static Maybe<A> Nothing<A>() => new Maybe<A>.Nothing();

    #region Applicative/Monad

    // ap : Maybe (a -> b) -> Maybe a -> Maybe b
    public HKT<MaybeW, B> Ap<A, B>(HKT<MaybeW, Func<A, B>> mf, HKT<MaybeW, A> ma)
    {
        var mf_ = Maybe<Func<A, B>>.Proj(mf);
        var ma_ = Maybe<A>.Proj(ma);
        return (mf_, ma_) switch
        {
            (Maybe<Func<A, B>>.Nothing, _) => Nothing<B>(),
            (_, Maybe<A>.Nothing) => Nothing<B>(),
            (Maybe<Func<A, B>>.Just f, Maybe<A>.Just a) => Just(f.Value(a.Value)),

            _ => throw new InvalidOperationException("Invalid Maybe value")
        };
    }

    // fmap : Maybe a -> (a -> b) -> Maybe b
    public HKT<MaybeW, B> FMap<A, B>(Func<A, B> f, HKT<MaybeW, A> ma)
    {
        var ma_ = Maybe<A>.Proj(ma);
        return ma_ switch
        {
            Maybe<A>.Nothing => Nothing<B>(),
            Maybe<A>.Just a => Just(f(a.Value)),
            _ => throw new InvalidOperationException("Invalid Maybe value")
        };
    }

    // bind : Maybe a -> (a -> Maybe b) -> Maybe b
    public HKT<MaybeW, B> Bind<A, B>(HKT<MaybeW, A> ma, Func<A, HKT<MaybeW, B>> f)
    {
        var ma_ = Maybe<A>.Proj(ma);
        return ma_ switch
        {
            Maybe<A>.Nothing => Nothing<B>(),
            Maybe<A>.Just a => f(a.Value),
            _ => throw new InvalidOperationException("Invalid Maybe value")
        };
    }

    // pure : a -> Maybe a
    public HKT<MaybeW, A> Pure<A>(A a) => Just(a);
    #endregion

    #region Eq & Semigroup

    // eq : Eq a => Maybe a -> Maybe a -> bool
    public bool Eq<A>(HKT<MaybeW, A> x, HKT<MaybeW, A> y)
    {
        var mx = Maybe<A>.Proj(x);
        var my = Maybe<A>.Proj(y);
        return (mx, my) switch
        {
            (Maybe<A>.Nothing, Maybe<A>.Nothing) => true,
            (Maybe<A>.Just a, Maybe<A>.Just b) => EqualityComparer<A>.Default.Equals(a.Value, b.Value),
            _ => false
        };
    }

    // sconcat : Maybe a -> Maybe a -> Maybe a
    public HKT<MaybeW, A> SConcat<A>(HKT<MaybeW, A> x, HKT<MaybeW, A> y)
    {
        var mx = Maybe<A>.Proj(x);
        var my = Maybe<A>.Proj(y);
        return (mx, my) switch
        {
            (Maybe<A>.Nothing, _) => y,
            (_, Maybe<A>.Nothing) => x,
            (Maybe<A>.Just a, Maybe<A>.Just b) => Just(a.Value),
            _ => throw new InvalidOperationException("Invalid Maybe value")
        };
    }
    #endregion
}

public static class MaybeExt
{
    private static readonly Maybe _m = new Maybe();

    public static Maybe<B> FMap<A, B>(this Maybe<A> ma, Func<A, B> f) => (Maybe<B>)_m.FMap(f, ma);
    public static Maybe<B> Ap<A, B>(this Maybe<Func<A, B>> mf, Maybe<A> ma) => (Maybe<B>)_m.Ap(mf, ma);
    public static Maybe<B> Bind<A, B>(this Maybe<A> ma, Func<A, Maybe<B>> f) => (Maybe<B>)_m.Bind(ma, f);

    #region LINQ
    public static Maybe<B> Select<A, B>(this Maybe<A> ma, Func<A, B> f) => ma.FMap(f);
    public static Maybe<B> SelectMany<A, B>(this Maybe<A> ma, Func<A, Maybe<B>> f) => ma.Bind(f);
    public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> ma, Func<A, Maybe<B>> f, Func<A, B, C> g) =>
            ma.Bind(a => f(a).FMap(b => g(a, b)));
    #endregion

}