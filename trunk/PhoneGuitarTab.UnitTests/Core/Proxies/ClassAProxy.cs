namespace PhoneGuitarTab.UnitTests.Core.Proxies
{
    using System.Reflection;
    using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;
    using PhoneGuitarTab.UnitTests.Core.Stubs;

    public class ClassAProxy : ProxyBase, IClassA
    {
        public System.Int32 Add(System.Int32 a, System.Int32 b)
        {
            var methodInvocation = BuildMethodInvocation(MethodBase.GetCurrentMethod(), a, b);
            return this.RunBehaviors(methodInvocation).GetReturnValue<System.Int32>();
        }

        public System.String SayHello(System.String name)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), name);
            return RunBehaviors(methodInvocation).GetReturnValue<System.String>();
        }

    }
}
