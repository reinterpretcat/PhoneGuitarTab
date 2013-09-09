namespace PhoneGuitarTab.Core.Diagnostic
{
    /// <summary>
    /// Represents trace category
    /// </summary>
    public class TraceCategory
    {
        public string Name { get; private set; }
        
        public TraceCategory(string name)
        {
            Name = name;
        }
    }
}
