namespace PhoneGuitarTab.Tablatures.Models
{
    public abstract class Velocities
    {
        public static int MinVelocity = 15;
        public static int VelocityIncrement = 16;
        public static int PianoPianissimo = (MinVelocity);
        public static int Pianissimo = (MinVelocity + VelocityIncrement);
        public static int Piano = (MinVelocity + (VelocityIncrement*2));
        public static int MezzoPiano = (MinVelocity + (VelocityIncrement*3));
        public static int MezzoForte = (MinVelocity + (VelocityIncrement*4));
        public static int Forte = (MinVelocity + (VelocityIncrement*5));
        public static int Fortissimo = (MinVelocity + (VelocityIncrement*6));
        public static int ForteFortissimo = (MinVelocity + (VelocityIncrement*7));
        public static int Default = Forte;
    }
}