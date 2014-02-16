using System;

namespace PhoneGuitarTab.UnitTests.Core.Stubs
{
    public interface IClassC
    {
        void Run(string param1, string param2);
        bool CanRun { get; set; }
        string GenerateResult(string fileName);

    }
}
