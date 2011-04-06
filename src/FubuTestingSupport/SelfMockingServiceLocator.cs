using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;
using Microsoft.Practices.ServiceLocation;
using Rhino.Mocks;

namespace FubuTestingSupport
{
    public class SelfMockingServiceLocator : IServiceLocator
    {
        public T MockFor<T>()
        {
            return (T)_mocks[typeof (T)];
        }

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

        private readonly Cache<Type, object> _mocks = new Cache<Type,object>(t =>
        {
            var makerType = typeof (MockMaker<>).MakeGenericType(t);
            return Activator.CreateInstance(makerType).As<MockMaker>().Make();
        });

        public object GetService(Type serviceType)
        {
            return _mocks[serviceType];
        }

        public object GetInstance(Type serviceType)
        {
            return _mocks[serviceType];
        }

        public object GetInstance(Type serviceType, string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<TService>()
        {
            return (TService)_mocks[typeof (TService)];
        }

        public TService GetInstance<TService>(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            throw new NotImplementedException();
        }

        public void Stub<T>(T stub)
        {
            _mocks[typeof (T)] = stub;
        }
    }
}