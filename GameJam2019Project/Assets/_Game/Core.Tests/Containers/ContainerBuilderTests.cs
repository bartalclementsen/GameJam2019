using NUnit.Framework;
using Core.Containers;

namespace Core.Tests.Containers
{
    public class ContainerBuilderTests : TestingContext<ContainerBuilder>
    {
        [Test]
        public void Should_be_able_to_register_class()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            ContainerBuilder builder = unit.Register<Class1>();

            //Assert
            Assert.AreEqual(unit, builder);
        }

        [Test]
        public void Should_be_able_to_register_class_with_factory()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            ContainerBuilder builder = unit.Register<Class1>((IContainer inner) => new Class1());

            //Assert
            Assert.AreEqual(unit, builder);
        }

        [Test]
        public void Should_be_able_to_register_class_and_interface()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            ContainerBuilder builder = unit.Register<IClass1, Class1>();

            //Assert
            Assert.AreEqual(unit, builder);
        }

        [Test]
        public void Should_be_able_to_register_interface_with_factory()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            ContainerBuilder builder = unit.Register<IClass1>((IContainer inner) => new Class1());

            //Assert
            Assert.AreEqual(unit, builder);
        }

        [Test]
        public void Should_be_able_to_register_singleton_class()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            ContainerBuilder builder = unit.RegisterSingleton<Class1>();

            //Assert
            Assert.AreEqual(unit, builder);
        }

        [Test]
        public void Should_be_able_to_register_singleton_class_with_factory()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            ContainerBuilder builder = unit.RegisterSingleton<Class1>((IContainer inner) => new Class1());

            //Assert
            Assert.AreEqual(unit, builder);
        }

        [Test]
        public void Should_be_able_to_register_singleton_interface_with_factory()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            ContainerBuilder builder = unit.RegisterSingleton<IClass1>((IContainer inner) => new Class1());

            //Assert
            Assert.AreEqual(unit, builder);
        }

        [Test]
        public void Should_be_able_to_register_singleton_class_and_interface()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            ContainerBuilder builder = unit.RegisterSingleton<IClass1, Class1>();

            //Assert
            Assert.AreEqual(unit, builder);
        }

        [Test]
        public void Should_build_container()
        {
            //Arrange
            ContainerBuilder unit = GetUnit();

            //Act
            IContainer container = unit.Build();

            //Assert
            Assert.NotNull(container);
        }

        public interface IClass1
        {
            string Greet(string name);
        }

        public class Class1 : IClass1
        {
            public string Greet(string name) => $"Hello {name}";
        }
    }
}