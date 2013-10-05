namespace PhoneGuitarTab.UnitTests.Core
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Core.Dependencies.Interception.Behaviors;
    using PhoneGuitarTab.UnitTests.Core.Proxies;
    using PhoneGuitarTab.UnitTests.Core.Stubs;
    

    [TestClass]
    public class InterceptionTests
    {
        [TestMethod]
        public void CanComponentWithSingleton()
        {
            using(IContainer container = new Container())
            {
                container.Register(Component.For<IClassC>()
                                                .Use<ClassC>()
                                                .Proxy<ClassCProxy>()
                                                .AddBehavior(new ExecuteBehavior())
                                                .Singleton());

                var classC = container.Resolve<IClassC>();
                Assert.IsInstanceOfType(classC, typeof(ClassCProxy));

                var result = classC.GenerateResult("1");
                Assert.AreEqual("result1", result);

                var classC2 = container.Resolve<IClassC>();
                Assert.AreSame(classC, classC2);
            }
        }

        [TestMethod]
        public void CanInterceptComponentWithTransient()
        {
            using (IContainer container = new Container())
            {
                container.Register(Component.For<IClassC>()
                                                .Use<ClassC>()
                                                .Proxy<ClassCProxy>()
                                                .AddBehavior(new ExecuteBehavior())
                                                .Transient());

                var classC = container.Resolve<IClassC>();
                Assert.IsInstanceOfType(classC, typeof(ClassCProxy));
                var result = classC.GenerateResult("1");
                Assert.AreEqual("result1", result);
                var classC2 = container.Resolve<IClassC>();
                Assert.AreNotSame(classC, classC2);
            }
        }
    }
}
