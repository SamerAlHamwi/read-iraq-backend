using Abp.Domain.Entities;

namespace ReadIraq.Domain
{
    public class ActiveEntity : ActiveEntity<int>, IActiveEntity
    {
    }

    public class ActiveEntity<T> : Entity<T>, IActiveEntity<T>
    {
        public bool IsActive { get; set; }
    }

    public interface IActiveEntity<T> : IEntity<T>
    {
        public bool IsActive { get; set; }
    }


    public interface IActiveEntity : IActiveEntity<int>
    {
    }

}
