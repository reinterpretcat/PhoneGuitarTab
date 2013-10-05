using System;

namespace PhoneGuitarTab.UnitTests.Core.Stubs
{
    public class ClassB: IClassB
    {

        public int Add(int a, int b)
        {
            return a + b;
        }

        public string SayHello(string name)
        {
            return String.Format("Hello, {0}", name);
        }
    }
}
