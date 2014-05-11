namespace PhoneGuitarTab.Tablatures.Models
{
    public class Chord
    {
        public int FirstFret { get; set; }
        public int[] Strings { get; set; }
        public string Name { get; set; }

        public Chord(int length)
        {
            Strings = new int[length];
            for (int i = 0; i < Strings.Length; i++)
            {
                Strings[i] = -1;
            }
        }

        public int GetFretValue(int str)
        {
            if (str >= 0 && str < Strings.Length)
            {
                return Strings[str];
            }
            return -1;
        }
    }
}