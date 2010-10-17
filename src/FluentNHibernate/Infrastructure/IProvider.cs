namespace FluentNHibernate.Infrastructure
{
    public interface IProvider
    {
        IMappingAction GetAction();
    }
}