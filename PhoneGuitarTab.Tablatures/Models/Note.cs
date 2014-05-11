namespace PhoneGuitarTab.Tablatures.Models
{
    public class Note
    {
        public int Value { get; set; }
        public int Velocity { get; set; }
        public int Str { get; set; }
        public bool IsTiedNote { get; set; }
        public NoteEffect Effect { get; set; }
        public Voice Voice { get; set; }

        public Note()
        {
            Value = 0;
            Velocity = Velocities.Default;
            Str = 1;
            IsTiedNote = false;
            Effect = new NoteEffect();
        }
    }
}