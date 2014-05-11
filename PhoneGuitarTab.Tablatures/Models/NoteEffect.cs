using PhoneGuitarTab.Tablatures.Models.Effects;

namespace PhoneGuitarTab.Tablatures.Models
{
    public class NoteEffect
    {
        public EffectBend Bend { get; set; }
        public EffectTremoloBar TremoloBar { get; set; }
        public EffectHarmonic Harmonic { get; set; }
        public EffectGrace Grace { get; set; }
        public EffectTrill Trill { get; set; }
        public EffectTremoloPicking TremoloPicking { get; set; }
        public bool IsVibrato { get; set; }
        public bool IsDeadNote { get; set; }
        public bool IsSlide { get; set; }
        public bool IsHammer { get; set; }
        public bool IsGhostNote { get; set; }
        public bool IsAccentuatedNote { get; set; }
        public bool IsHeavyAccentuatedNote { get; set; }
        public bool IsPalmMute { get; set; }
        public bool IsStaccato { get; set; }
        public bool IsTapping { get; set; }
        public bool IsSlapping { get; set; }
        public bool IsPopping { get; set; }
        public bool IsFadeIn { get; set; }

        public NoteEffect()
        {
            Bend = null;
            TremoloBar = null;
            Harmonic = null;
            Grace = null;
            Trill = null;
            TremoloPicking = null;
            IsVibrato = false;
            IsDeadNote = false;
            IsSlide = false;
            IsHammer = false;
            IsGhostNote = false;
            IsAccentuatedNote = false;
            IsHeavyAccentuatedNote = false;
            IsPalmMute = false;
            IsStaccato = false;
            IsTapping = false;
            IsSlapping = false;
            IsPopping = false;
            IsFadeIn = false;
        }
    }
}