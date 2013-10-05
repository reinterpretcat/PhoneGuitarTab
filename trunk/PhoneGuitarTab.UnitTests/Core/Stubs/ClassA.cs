namespace PhoneGuitarTab.UnitTests.Core.Stubs
{
    using System;

    public class ClassA: IClassA
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
