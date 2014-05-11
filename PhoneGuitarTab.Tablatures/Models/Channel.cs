namespace PhoneGuitarTab.Tablatures.Models
{
    public class Channel
    {
        public static short DefaultPercussionChannel = 9;

        public static short DefaultInstrument = 25;
        public static short DefaultVolume = 127;
        public static short DefaultBalance = 64;
        public static short DefaultChorus = 0;
        public static short DefaultReverb = 0;
        public static short DefaultPhaser = 0;
        public static short DefaultTremolo = 0;

        public short ChannelCode { get; set; }
        public short EffectChannel { get; set; }
        public short Instrument { get; set; }
        public short Volume { get; set; }
        public short Balance { get; set; }
        public short Chorus { get; set; }
        public short Reverb { get; set; }
        public short Phaser { get; set; }
        public short Tremolo { get; set; }

        public Channel()
        {
            ChannelCode = 0;
            EffectChannel = 0;
            Instrument = DefaultInstrument;
            Volume = DefaultVolume;
            Balance = DefaultBalance;
            Chorus = DefaultChorus;
            Reverb = DefaultReverb;
            Phaser = DefaultPhaser;
            Tremolo = DefaultTremolo;
        }

        public bool IsPercussionChannel
        {
            get { return (ChannelCode == DefaultPercussionChannel); }
        }
    }
}