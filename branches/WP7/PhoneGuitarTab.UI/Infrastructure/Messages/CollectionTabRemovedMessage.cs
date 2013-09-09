namespace PhoneGuitarTab.UI.Infrastructure.Messages
{
    public class CollectionTabRemovedMessage
    {
        public int Id { get; set; }

        public CollectionTabRemovedMessage(int id)
        {
            this.Id = id;
        }
    }
}
