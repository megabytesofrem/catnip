namespace Catnip.Core.Data;

using Catnip.Core.Control;

public sealed class EitherW<L> { }

/// <summary>
/// The Either type represents a value that can be one of two types, Left or Right.
/// 
/// Left is the error state, and Right is the success state.
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public abstract record Either<L, R> : HKT<EitherW<L>, R>
{
    public sealed record Left(L Value) : Either<L, R>;
    public sealed record Right(R Value) : Either<L, R>;

    // Projection function to extract the Either<L, R> from the HKT.
    public static Either<L, R> Proj(HKT<EitherW<L>, R> h) => (Either<L, R>)h;
}

public sealed class EitherK<L>
    : Eq<EitherW<L>>,
      Functor<EitherW<L>>,
      Applicative<EitherW<L>>,
      Monad<EitherW<L>>,
      Semigroup<EitherW<L>>
{

    // Dummy instance needed to satisfy the type class constraints
    public static readonly EitherK<L> Instance = new EitherK<L>();

    #region Applicative/Monad

    // ap : Either a (b -> c) -> Either a b -> Either a c
    public HKT<EitherW<L>, B> Ap<A, B>(HKT<EitherW<L>, Func<A, B>> mf, HKT<EitherW<L>, A> ma)
    {
        var ef = Either<L, Func<A, B>>.Proj(mf);
        var ea = Either<L, A>.Proj(ma);

        return (ef, ea) switch
        {
            (Either<L, Func<A, B>>.Left(var l), _) => Either.Left<L, B>(l),
            (_, Either<L, A>.Left(var l)) => Either.Left<L, B>(l),
            (Either<L, Func<A, B>>.Right(var f), Either<L, A>.Right(var a)) => Either.Right<L, B>(f(a)),
            _ => throw new InvalidOperationException()
        };
    }

    // bind : Either a b -> (b -> Either a c) -> Either a c
    public HKT<EitherW<L>, B> Bind<A, B>(HKT<EitherW<L>, A> ma, Func<A, HKT<EitherW<L>, B>> f)
    {
        var ea = Either<L, A>.Proj(ma);
        return ea switch
        {
            Either<L, A>.Left(var l) => Either.Left<L, B>(l),
            Either<L, A>.Right(var a) => f(a),
            _ => throw new InvalidOperationException()
        };
    }

    // fmap : Either a b -> (b -> c) -> Either a c
    public HKT<EitherW<L>, B> FMap<A, B>(Func<A, B> f, HKT<EitherW<L>, A> ma)
    {
        var e = Either<L, A>.Proj(ma);
        return e switch
        {
            Either<L, A>.Left(var l) => Either.Left<L, B>(l),
            Either<L, A>.Right(var r) => Either.Right<L, B>(f(r)),
            _ => throw new InvalidOperationException()
        };
    }

    // pure : a -> Either a b
    public HKT<EitherW<L>, A> Pure<A>(A a) => Either.Right<L, A>(a);
    #endregion

    #region Eq & Semigroup

    // eq : Eq a => Either a b -> Either a b -> bool
    public bool Eq<A>(HKT<EitherW<L>, A> x, HKT<EitherW<L>, A> y)
    {
        var ex = Either<L, A>.Proj(x);
        var ey = Either<L, A>.Proj(y);
        return (ex, ey) switch
        {
            (Either<L, A>.Left(var l1), Either<L, A>.Left(var l2)) => EqualityComparer<L>.Default.Equals(l1, l2),
            (Either<L, A>.Right(var r1), Either<L, A>.Right(var r2)) => EqualityComparer<A>.Default.Equals(r1, r2),
            _ => false
        };
    }

    // sconcat : Either a b -> Either a b -> Either a b
    public HKT<EitherW<L>, A> SConcat<A>(HKT<EitherW<L>, A> x, HKT<EitherW<L>, A> y)
    {
        var ex = Either<L, A>.Proj(x);
        var ey = Either<L, A>.Proj(y);
        return (ex, ey) switch
        {
            (Either<L, A>.Left(var l1), Either<L, A>.Left(var l2)) => Either.Left<L, A>(l1), // or l2, they are equal
            (Either<L, A>.Left(var l), _) => Either.Left<L, A>(l),
            (_, Either<L, A>.Left(var l)) => Either.Left<L, A>(l),
            (Either<L, A>.Right(var r1), Either<L, A>.Right(var r2)) => Either.Right<L, A>(r1), // or r2, they are equal
            _ => throw new InvalidOperationException()
        };
    }

    #endregion
}

/// <summary>
/// The Either type represents a value that can be one of two types, Left or Right.
/// 
/// Left is the error state, and Right is the success state.
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public static class Either
{
    public static Either<L, R> Left<L, R>(L value) => new Either<L, R>.Left(value);
    public static Either<L, R> Right<L, R>(R value) => new Either<L, R>.Right(value);
}

// ...existing code...

public static class EitherExt
{
    public static Either<L, B> FMap<L, A, B>(this Either<L, A> ma, Func<A, B> f) =>
        (Either<L, B>)EitherK<L>.Instance.FMap(f, ma);

    public static Either<L, B> Ap<L, A, B>(this Either<L, Func<A, B>> mf, Either<L, A> ma) =>
        (Either<L, B>)EitherK<L>.Instance.Ap(mf, ma);

    public static Either<L, B> Bind<L, A, B>(this Either<L, A> ma, Func<A, Either<L, B>> f) =>
        (Either<L, B>)EitherK<L>.Instance.Bind(ma, f);

    #region LINQ
    public static Either<L, B> Select<L, A, B>(this Either<L, A> ma, Func<A, B> f) =>
        ma.FMap(f);

    public static Either<L, B> SelectMany<L, A, B>(this Either<L, A> ma, Func<A, Either<L, B>> f) =>
        ma.Bind(f);

    public static Either<L, C> SelectMany<L, A, B, C>(this Either<L, A> ma, Func<A, Either<L, B>> f, Func<A, B, C> g) =>
        ma.Bind(a => f(a).FMap(b => g(a, b)));
    #endregion
}