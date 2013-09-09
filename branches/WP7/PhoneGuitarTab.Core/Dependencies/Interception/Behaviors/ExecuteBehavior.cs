using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneGuitarTab.Core.Dependencies.Interception.Behaviors
{
    /// <summary>
    /// Executes method
    /// </summary>
    public class ExecuteBehavior: IBehavior
    {
        public ExecuteBehavior()
        {
            Name = "execute";
        }

        public string Name { get; private set; }

        /// <summary>
        /// Executes method
        /// </summary>
        public virtual IMethodReturn Invoke(MethodInvocation methodInvocation)
        {
            var methodBase = TypeHelper.GetMethodBySign(methodInvocation.Target.GetType(), 
                methodInvocation.MethodBase, methodInvocation.GenericTypes.ToArray());
            var result = methodBase.Invoke(methodInvocation.Target, methodInvocation.Parameters.Values.ToArray());
            methodInvocation.IsInvoked = true;
            return methodInvocation.Return = new MethodReturn(result);
        }

    }
}
