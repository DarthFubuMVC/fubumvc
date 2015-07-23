using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;
using Rhino.Mocks;

namespace FubuMVC.Tests.TestSupport
{
    public class SelfMockingServiceLocator : IServiceLocator
    {
        private readonly Cache<Type, object> _mocks = new Cache<Type, object>(t =>
        {
            var makerType = typeof(MockMaker<>).MakeGenericType(t);
            return Activator.CreateInstance(makerType).As<MockMaker>().Make();
        });

        private readonly Dictionary<string, object> _namedMocks = new Dictionary<string, object>();

        public TService GetInstance<TService>()
        {
            return (TService)_mocks[typeof(TService)];
        }

        public T MockFor<T>()
        {
            return (T)_mocks[typeof(T)];
        }

        public object GetService(Type serviceType)
        {
            return _mocks[serviceType];
        }

        public object GetInstance(Type serviceType)
        {
            return _mocks[serviceType];
        }

        public T GetInstance<T>(string name)
        {
            if (!_namedMocks.ContainsKey(name))
            {
                var t = typeof(T);
                var makerType = typeof(MockMaker<>).MakeGenericType(t);
                _namedMocks[name] = Activator.CreateInstance(makerType).As<MockMaker>().Make();
            }

            return (T)_namedMocks[name];
        }


        public void Stub<T>(T stub)
        {
            _mocks[typeof(T)] = stub;
        }

        #region Nested type: MockMaker

        public interface MockMaker
        {
            object Make();
        }

        public class MockMaker<T> : MockMaker where T : class
        {
            public object Make()
            {
                return MockRepository.GenerateMock<T>();
            }
        }

        #endregion
    }
}