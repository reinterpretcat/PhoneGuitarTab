namespace PhoneGuitarTab.Tablatures.Models
{
    public class Beat
    {
        public static int MaxVoices = 2;

        public long Start { get; set; }
        public Chord Chord { get; set; }
        public Text Text { get; set; }
        public Voice[] Voices { get; set; }
        public Stroke Stroke { get; set; }

        public Beat()
        {
            Start = Duration.QuarterTime;
            Stroke = new Stroke();
            Voices = new Voice[MaxVoices];
            for (int i = 0; i < MaxVoices; i ++)
            {
                Voices[i] = new Voice(i);
            }
        }
    }
}