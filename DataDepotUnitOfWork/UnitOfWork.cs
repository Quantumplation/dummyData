using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDepotUnitOfWork
{
    public class UnitOfWork
    {
        private readonly IDictionary<Tuple<Type, int>, Persistable> _persistables;

        public UnitOfWork()
        {
            _persistables = new Dictionary<Tuple<Type, int>, Persistable>();
        }

        public T Get<T>(int id) where T : Persistable, new()
        {
            var origP = (T)GetOriginal(typeof (T), id);
            var newP = new T {Id = id};
            foreach (var prop in typeof (T).GetProperties())
            {
                object newValue;
                if (typeof (Persistable).IsAssignableFrom(prop.PropertyType))
                {
                    var propId = ((Persistable) prop.GetValue(origP)).Id;
                    newValue = prop.PropertyType.GetConstructor(new Type[0]).Invoke(null);
                    prop.PropertyType.GetProperty("Id", typeof (int)).SetValue(newValue, propId);
                }
                else
                {
                    newValue = prop.GetValue(origP);
                }
                prop.SetValue(newP, newValue);
            }
            return newP;
        }

        public void Save<T>(T persistable) where T : Persistable, new()
        {
            T oldP;
            if (persistable.Id == 0)
            {
                oldP = new T();
                SaveOriginal(oldP);
                persistable.Id = oldP.Id;
            }
            else
            {
                oldP = (T) GetOriginal(typeof (T), persistable.Id);
            }

            foreach (var prop in persistable.GetType().GetProperties())
            {
                if (typeof (Persistable).IsAssignableFrom(prop.PropertyType))
                {
                    var id = ((Persistable) prop.GetValue(persistable)).Id;
                    prop.SetValue(oldP, GetOriginal(prop.PropertyType, id));
                }
                else
                {
                    prop.SetValue(oldP, prop.GetValue(persistable));
                }
            }
        }

        public T Add<T>(T persistable) where T : Persistable
        {
            SaveOriginal(persistable);
            return persistable;
        }

        private Persistable GetOriginal(Type type, int id)
        {
            return _persistables[Tuple.Create(type, id)];
        }

        private void SaveOriginal(Persistable persistable)
        {
            var id = _persistables.Count + 1;
            persistable.Id = id;
            _persistables.Add(Tuple.Create(persistable.GetType(), id), persistable);
        }
    }

    public class DataDepotRepo
    {
        private readonly UnitOfWork _uow;
        private readonly IDictionary<Type, IDataDepot> _depots;

        public DataDepotRepo(UnitOfWork uow)
        {
            _uow = uow;
            _depots = new Dictionary<Type, IDataDepot>();
        }

        public void Register<T>() where T : IDataDepot, new()
        {
            if (_depots.ContainsKey(typeof (T)))
                return;

            var depot = new T();
            depot.Populate(_uow, this);
            _depots.Add(typeof(T), depot);
        }

        public T Data<T>() where T : IDataDepot
        {
            return (T) _depots[typeof (T)];
        }

    }

    public interface IDataDepot
    {
        void Populate(UnitOfWork uow, DataDepotRepo depotRepo);
    }

    public class Persistable
    {
        public int Id { get; set; }
    }
}
