namespace PhoneGuitarTab.UI.Infrastructure.Messages
{
    public class HistoryTabRemovedMessage
    {
        public int Id { get; set; }

        public HistoryTabRemovedMessage(int id)
        {
            this.Id = id;
        }
    }
}
