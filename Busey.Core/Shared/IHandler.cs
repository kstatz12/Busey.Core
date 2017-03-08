namespace Busey.Core.Shared
{
    public interface IHandler<T>
    {
        void Handle(T input);
    }
}
