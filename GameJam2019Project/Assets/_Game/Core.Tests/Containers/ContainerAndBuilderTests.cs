using Core.Containers;
using NUnit.Framework;

namespace Core.Tests.Containers
{
    public class ContainerAndBuilderTests
    {
        private ContainerBuilder GetContainerBuilder() => new ContainerBuilder();

        [Test]
        public void Should_resolve_class()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .Register<Class1>()
                .Build();

            //Act
            Class1 class1 = containerBuilder.Resolve<Class1>();
            Class1 class2 = containerBuilder.Resolve<Class1>();

            //Assert
            Assert.NotNull(class1);
            Assert.AreNotEqual(class1, class2);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_interface()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .Register<IClass1, Class1>()
                .Build();

            //Act
            IClass1 class1 = containerBuilder.Resolve<IClass1>();
            IClass1 class2 = containerBuilder.Resolve<IClass1>();

            //Assert
            Assert.NotNull(class1);
            Assert.AreNotEqual(class1, class2);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_factory()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .Register((IContainer inner) => new Class1())
                .Build();

            //Act
            Class1 class1 = containerBuilder.Resolve<Class1>();
            Class1 class2 = containerBuilder.Resolve<Class1>();

            //Assert
            Assert.NotNull(class1);
            Assert.AreNotEqual(class1, class2);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_factory_with_interface()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .Register<IClass1>((IContainer inner) => new Class1())
                .Build();

            //Act
            IClass1 class1 = containerBuilder.Resolve<IClass1>();
            IClass1 class2 = containerBuilder.Resolve<IClass1>();

            //Assert
            Assert.NotNull(class1);
            Assert.AreNotEqual(class1, class2);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_class_singleton()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .RegisterSingleton<Class1>()
                .Build();

            //Act
            Class1 class1 = containerBuilder.Resolve<Class1>();
            Class1 class2 = containerBuilder.Resolve<Class1>();

            //Assert
            Assert.NotNull(class1);
            Assert.AreEqual(class1, class2);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_interface_singleton()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .RegisterSingleton<IClass1, Class1>()
                .Build();

            //Act
            IClass1 class1 = containerBuilder.Resolve<IClass1>();
            IClass1 class2 = containerBuilder.Resolve<IClass1>();

            //Assert
            Assert.NotNull(class1);
            Assert.AreEqual(class1, class2);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_factory_singleton()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .RegisterSingleton((IContainer inner) => new Class1())
                .Build();

            //Act
            Class1 class1 = containerBuilder.Resolve<Class1>();
            Class1 class2 = containerBuilder.Resolve<Class1>();

            //Assert
            Assert.NotNull(class1);
            Assert.AreEqual(class1, class2);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_factory_with_interface_singleton()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .RegisterSingleton<IClass1>((IContainer inner) => new Class1())
                .Build();

            //Act
            IClass1 class1 = containerBuilder.Resolve<IClass1>();
            IClass1 class2 = containerBuilder.Resolve<IClass1>();

            //Assert
            Assert.NotNull(class1);
            Assert.AreEqual(class1, class2);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_derived_class()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .Register<IClass2, Class2>()
                .Build();

            //Act
            IClass1 class1 = containerBuilder.Resolve<IClass1>();

            //Assert
            Assert.NotNull(class1);
            Assert.IsInstanceOf<Class2>(class1);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_multi_derived_class()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .Register<IClass3, Class3>()
                .Build();

            //Act
            IClass1 class1 = containerBuilder.Resolve<IClass1>();

            //Assert
            Assert.NotNull(class1);
            Assert.IsInstanceOf<Class3>(class1);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        [Test]
        public void Should_resolve_most_specific_first()
        {
            //Arrange
            IContainer containerBuilder = GetContainerBuilder()
                .Register<IClass3, Class3>()
                .Register<IClass1, Class1>()
                .Build();

            //Act
            IClass1 class1 = containerBuilder.Resolve<IClass1>();

            //Assert
            Assert.NotNull(class1);
            Assert.IsInstanceOf<Class1>(class1);
            Assert.AreEqual("Hello test", class1.Greet("test"));
        }

        public interface IClass1
        {
            string Greet(string name);
        }

        public class Class1 : IClass1
        {
            public string Greet(string name) => $"Hello {name}";
        }

        public class Class1Decorator : IClass1
        {
            private readonly IClass1 _decoratee;

            public Class1Decorator(IClass1 decoratee)
            {
                _decoratee = decoratee;
            }

            public string Greet(string name) => _decoratee.Greet(name);
        }

        public interface IClass2 : IClass1
        { }

        public class Class2 : IClass2
        {

            public string Greet(string name) => $"Hello {name}";
        }

        public interface IClass3 : IClass2
        { }

        public class Class3 : IClass3
        {

            public string Greet(string name) => $"Hello {name}";
        }
    }
}
