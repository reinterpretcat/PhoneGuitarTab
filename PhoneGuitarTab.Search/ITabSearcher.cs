namespace PhoneGuitarTab.Search
{
    public interface ITabSearcher
    {
        void Run(int pageNumber, TabulatureType type);
    }
}