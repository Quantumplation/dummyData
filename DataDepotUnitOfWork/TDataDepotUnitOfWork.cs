using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataDepotUnitOfWork
{
    public class Foo : Persistable
    {
        public Bar Bar { get; set; }
    }

    public class Bar : Persistable
    {
        public Baz Baz { get; set; }
    }

    public class Baz : Persistable
    {
        public string Name { get; set; }
    }


    public class FooDepot : IDataDepot
    {
        public Foo Foo { get; private set; }

        public void Populate(UnitOfWork uow, DataDepotRepo depotRepo)
        {
            Foo = uow.Add(new Foo
            {
                Bar = depotRepo.Data<BarDepot>().Bar
            });
        }
    }

    public class BarDepot : IDataDepot
    {
        public Bar Bar { get; private set; }

        public void Populate(UnitOfWork uow, DataDepotRepo depotRepo)
        {
            Bar = uow.Add(new Bar
            {
                Baz = uow.Add(new Baz
                {
                    Name = "Original unmodified name"
                })
            });
        }
    }
    
    [TestClass]
    public class TDataDepotUnitOfWork
    {
        private UnitOfWork _uow;
        private DataDepotRepo _data;

        [TestInitialize]
        public void Setup()
        {
            _uow = new UnitOfWork();
            _data = new DataDepotRepo(_uow);
            _data.Register<BarDepot>();
            _data.Register<FooDepot>();
        }

        [TestMethod]
        public void ShouldChangeUnitOfWorkContentsWhenModified()
        {
            var baz = _data.Data<BarDepot>().Bar.Baz;
            baz.Name = "different name than it had before";

            Assert.AreEqual(baz.Name, _uow.Get<Baz>(baz.Id).Name);
        }

        [TestMethod]
        public void ShouldCopyChangesMadeOnSave()
        {
            var depotBaz = _data.Data<BarDepot>().Bar.Baz;
            var uowBaz = _uow.Get<Baz>(depotBaz.Id);
            Assert.AreNotSame(depotBaz, uowBaz);

            uowBaz.Name = "different name";
            Assert.AreNotEqual(uowBaz.Name, depotBaz.Name);

            _uow.Save(uowBaz);
            Assert.AreEqual(uowBaz.Name, depotBaz.Name);
        }
    }
}
