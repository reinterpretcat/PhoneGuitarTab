namespace PhoneGuitarTab.Tablatures.Models.Effects
{
    public class EffectGrace
    {
        public static int TransitionNone = 0;
        public static int TransitionSlide = 1;
        public static int TransitionBend = 2;
        public static int TransitionHammer = 3;

        public int Fret { get; set; }
        public int Duration { get; set; }
        public int Dynamic { get; set; }
        public int Transition { get; set; }
        public bool IsOnBeat { get; set; }
        public bool IsDead { get; set; }

        public EffectGrace()
        {
            Fret = 0;
            Duration = 1;
            Dynamic = Velocities.Default;
            Transition = TransitionNone;
            IsOnBeat = false;
            IsDead = false;
        }
    }
}