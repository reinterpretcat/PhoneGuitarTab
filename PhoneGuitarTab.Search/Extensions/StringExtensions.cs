using System.Text;

namespace PhoneGuitarTab.Search.Extensions

{
    public static class StringExtensions
    {
        public static string TransLiterate(this string str)
        {
            //Convert non-english characters to English.
            string unaccentedString = str.ToLower().StripAccents();
            unaccentedString = unaccentedString.Replace("'", "");
            unaccentedString = unaccentedString.Replace("+", " ");
            unaccentedString = unaccentedString.Replace("&", " ");
            unaccentedString = unaccentedString.Replace("/", " ");
            unaccentedString = unaccentedString.Replace(".", "");
            return unaccentedString.Replace("the ", "");
        }
       
    }


    public static class StringUtil
    {
        static char[] englishReplace = { 'a', 'a', 'a', 'a', 'c', 'e', 'e', 'e', 'e', 'i', 'i', 'o', 'o', 'u', 'u', 'u', 'o', 's', 'i', 'o', 'u', 'o', 'o', 'u', 'a', 'c', 'e', 'l', 'n', 'o', 's', 'z', 'z', 'a', 'a', 'a', 'a', 'e', 'e', 'i', 'o', 'o', 'o', 'u', 'u', 'a', 'a', 'a', 'c', 'd', 'e', 'e', 'i', 'n', 'o', 'r', 's', 't', 'u', 'u', 'y', 'z', 'c', 'e', 'e', 'g', 'i', 'i', 'o', 'o', 'u', 's' };
        static char[] englishAccents = { 'à', 'â', 'ä', 'æ', 'ç', 'é', 'è', 'ê', 'ë', 'î', 'ï', 'ô', 'œ', 'ù', 'û', 'ü', 'ö', 'ß', 'í', 'ó', 'ú', 'ò', 'ó', 'ú', 'ą', 'ć', 'ę', 'ł', 'ń', 'ó', 'ś', 'ż', 'ź', 'ã', 'á', 'â', 'à', 'é', 'ê', 'í', 'õ', 'ó', 'ô', 'ú', 'ü', 'ã', 'á', 'á', 'č', 'ď', 'é', 'ě', 'í', 'ň', 'ó', 'ř', 'š', 'ť', 'ú', 'ů', 'ý', 'ž', 'ç', 'é', 'ë', 'ğ', 'ı', 'ï', 'ó', 'ö', 'ü', 'ş' };


        static StringBuilder sbStripAccents = new StringBuilder();

        public static string StripAccents(this string str)
        {
             sbStripAccents.Length = 0;  
             sbStripAccents.Append(str); 
              for (int i = 0; i < englishAccents.Length; i++)
                  {
                     sbStripAccents.Replace(englishAccents[i], englishReplace[i]);
                  }

            return sbStripAccents.ToString();

        }
   
    
 }
    
}
