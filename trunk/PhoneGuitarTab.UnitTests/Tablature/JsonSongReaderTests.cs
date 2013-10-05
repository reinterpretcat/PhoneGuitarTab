namespace PhoneGuitarTab.UnitTests.Tablature
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using PhoneGuitarTab.Tablatures.Readers;

    [TestClass]
    public class JsonSongReaderTests
    {
        [TestMethod]
        public void CanParseDefault()
        {
            using (var stream = Application.GetResourceStream(new Uri("/PhoneGuitarTab.UnitTests;component/Resources/opeth_requiem.json", 
                                                             UriKind.Relative)).Stream)
            {
                using(var reader = new StreamReader(stream))
                {
                    var tabContent = reader.ReadToEnd();
                    JsonSongReader jsonReader = new JsonSongReader(tabContent);

                    var song = jsonReader.ReadSong();

                    Assert.AreEqual(song.Artist,"Opeth");
                    Assert.AreEqual(song.Name, "Requiem");
                    Assert.AreEqual(song.Tracks.Count, 3);
                    Assert.AreEqual(song.Tracks.First().Name, "Acoustic Guitar I");
                    Assert.AreEqual(song.Tracks.First().Measures.Count, 32);
                }
                
            }
        }
    }
}
