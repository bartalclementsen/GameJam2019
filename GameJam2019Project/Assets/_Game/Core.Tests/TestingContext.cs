using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Core.Tests
{
    [ExcludeFromCodeCoverage]
    public abstract class TestingContext<TUnitUnderTest> where TUnitUnderTest : class
    {
        private readonly Dictionary<Type, Object> _registeredMocks;
        private readonly Type _typeOfUnitUnderTest;
        private readonly ConstructorInfo[] _constructorInfos;
        private readonly ConstructorInfo _constructorInfoToUse;

        public TestingContext()
        {
            _registeredMocks = new Dictionary<Type, object>();
            _typeOfUnitUnderTest = typeof(TUnitUnderTest);
            _constructorInfos = _typeOfUnitUnderTest.GetConstructors();

            //Get constructor with most parameters
            if (_constructorInfos?.Any() == true)
                _constructorInfoToUse = _constructorInfos.OrderByDescending(ci => ci.GetParameters().Count()).FirstOrDefault();
        }

        protected virtual void RegisterMock<T>(object o)
        {
            if (_registeredMocks.ContainsKey(typeof(T)))
                _registeredMocks[typeof(T)] = o; //Replace current item
            else
                _registeredMocks.Add(typeof(T), o); //Add new mock to dictionary
        }

        protected virtual object TryGetMockByType(Dictionary<Type, Object> mocks, Type type)
        {
            if (mocks == null)
                return null;
            object paramter;
            mocks.TryGetValue(type, out paramter);
            return paramter;
        }

        protected virtual ConstructorInfo GetConstructorInfoToUse(Type unitUnderTestType)
        {
            return _constructorInfoToUse;
        }

        protected virtual TUnitUnderTest GetUnit()
        {
            return GetUnit(null);
        }

        protected virtual TUnitUnderTest GetUnit(Dictionary<Type, Object> customMocks)
        {
            ConstructorInfo constructorInfo = GetConstructorInfoToUse(_typeOfUnitUnderTest);

            List<object> paramters = new List<object>();
            if (constructorInfo != null)
            {
                foreach (ParameterInfo paramterInfo in constructorInfo.GetParameters())
                {
                    object paramter = TryGetMockByType(customMocks, paramterInfo.ParameterType);
                    if (paramter == null)
                        paramter = TryGetMockByType(_registeredMocks, paramterInfo.ParameterType);

                    paramters.Add(paramter);
                }
            }

            return (TUnitUnderTest)Activator.CreateInstance(_typeOfUnitUnderTest, paramters.ToArray());
        }
    }
}
