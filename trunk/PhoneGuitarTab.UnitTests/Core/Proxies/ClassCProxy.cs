namespace PhoneGuitarTab.UnitTests.Core.Proxies
{
    using System.Reflection;

    using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;
    using PhoneGuitarTab.UnitTests.Core.Stubs;

    public class ClassCProxy : ProxyBase, IClassC
    {
        public void Run(System.String param1, System.String param2)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), param1, param2);
            RunBehaviors(methodInvocation);
        }

        public System.String GenerateResult(System.String fileName)
        {
            var methodInvocation = this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), fileName);
            return RunBehaviors(methodInvocation).GetReturnValue<System.String>();
        }

        public System.Boolean CanRun
        {
            get
            {
                return RunBehaviors(this.BuildMethodInvocation(MethodBase.GetCurrentMethod())).GetReturnValue<System.Boolean>();
            }
            set
            {
                RunBehaviors(this.BuildMethodInvocation(MethodBase.GetCurrentMethod(), value));
            }
        }

    }
}
