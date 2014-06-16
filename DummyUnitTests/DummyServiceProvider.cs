using System;
using System.Collections.Generic;
using DummyDataSample;
using Moq;
using IServiceProvider = DummyDataSample.IServiceProvider;

namespace DummyUnitTests
{
    public class DummyServiceProvider<U> : IServiceProvider where U : Service, new()
    {
        private Dictionary<Type, Action<object>> serviceSetups = new Dictionary<Type, Action<object>>();  

        public T Get<T>() where T : Service, new()
        {
            if (typeof (T) == typeof (U)) return new U() {ServiceProvider = this} as T;
            var mock = new Mock<T>();
            mock.Setup(x => x.ServiceProvider).Returns(this);
            serviceSetups[typeof (T)](mock);
            return mock.Object;
        }

        public void RegisterSetup<T>(Action<Mock<T>> setup) where T : Service, new()
        {
            serviceSetups.Add(typeof (T), x => setup(x as Mock<T>));
        }
    }
}
