namespace PhoneGuitarTab.UI.Infrastructure.Messages
{
    public class GroupTabRemovedMessage
    {
        public int Id { get; set; }

        public GroupTabRemovedMessage(int id)
        {
            this.Id = id;
        }
    }
}
