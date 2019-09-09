using Core.Containers;
using NUnit.Framework;

namespace Core.Tests.Containers
{
    public class ContainerAndBuilderDependenciesTests
    {
        private ContainerBuilder GetContainerBuilder() => new ContainerBuilder();

        [Test]
        public void Should_resolve_with_dependencies()
        {
            //Arrange
            IContainer container = GetContainerBuilder()
                .Register<Class2>()
                .Register<IClass1, Class1>()
                .Build();

            //Act
            Class2 class2 = container.Resolve<Class2>();

            //Assert
            Assert.NotNull(class2);
            Assert.NotNull(class2.Class1);
        }

        [Test]
        public void Should_resolve_with_missing_dependencies()
        {
            //Arrange
            IContainer container = GetContainerBuilder()
                .Register<Class2>()
                .Build();

            //Act
            Class2 class2 = container.Resolve<Class2>();

            //Assert
            Assert.NotNull(class2);
            Assert.Null(class2.Class1);
        }

        [Test]
        public void Should_resolve_factory_with_dependencies()
        {
            //Arrange
            IContainer container = GetContainerBuilder()
                .Register<Class2>((IContainer inner) => new Class2(inner.Resolve<IClass1>()))
                .Register<IClass1, Class1>()
                .Build();

            //Act
            Class2 class2 = container.Resolve<Class2>();

            //Assert
            Assert.NotNull(class2);
            Assert.NotNull(class2.Class1);
        }



        public interface IClass1
        {
            string Greet(string name);
        }

        public class Class1 : IClass1
        {
            public string Greet(string name) => $"Hello {name}";
        }

        public class Class2
        {
            public readonly IClass1 Class1;

            public Class2(IClass1 class1)
            {
                Class1 = class1;
            }
        }
    }
}
