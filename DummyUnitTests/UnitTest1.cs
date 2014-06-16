using System;
using DummyDataSample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DummyUnitTests
{
    public class xyz : Service
    {
        
    }

    public class abc : Service
    {
        public virtual bool Mocked()
        {
            return false;
        }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dummy = new DummyServiceProvider<xyz>();
            dummy.RegisterSetup<abc>(x => x.Setup(y => y.Mocked()).Returns(true));
            Assert.IsTrue(dummy.Get<xyz>() is xyz);
            Assert.IsTrue(dummy.Get<abc>().Mocked());
        }
    }
}
