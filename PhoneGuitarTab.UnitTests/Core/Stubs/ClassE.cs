
namespace PhoneGuitarTab.UnitTests.Core.Stubs
{
    using PhoneGuitarTab.Core.Dependencies;

    public class ClassE: IClassE
    {
        private readonly IClassA _a;
        private readonly IClassB _b;
        private readonly IClassC _c;

        [Dependency]
        public ClassE(IClassA a, IClassB b, IClassC c)
        {
            _a = a;
            _b = b;
            _c = c;
        }
        public string Hello(string name)
        {
            _c.Run(name, name);
            return _a.SayHello(name) +
                   _b.SayHello(name);

        }
    }
}
