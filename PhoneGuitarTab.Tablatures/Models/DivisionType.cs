namespace PhoneGuitarTab.Tablatures.Models
{
    public class DivisionType
    {
        public static DivisionType Normal = NewDivisionType(1, 1);

        public int Enters { get; set; }
        public int Times { get; set; }

        public DivisionType()
        {
            Enters = 1;
            Times = 1;
        }

        private static DivisionType NewDivisionType(int enters, int times)
        {
            DivisionType divisionType = new DivisionType();
            divisionType.Enters = enters;
            divisionType.Times = times;
            return divisionType;
        }
    }
}