namespace Core.Containers
{
    public interface IContainer
    {
        T Resolve<T>();
    }
}
