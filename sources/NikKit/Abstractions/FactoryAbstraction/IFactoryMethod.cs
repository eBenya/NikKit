namespace NikKit.FactoryAbstraction;

public interface IFactoryMethod<out TResult, in TSource>
    where TResult : class, IFactoryMethod<TResult, TSource>
    where TSource : class
{
    static abstract TResult Create(TSource source);
}
