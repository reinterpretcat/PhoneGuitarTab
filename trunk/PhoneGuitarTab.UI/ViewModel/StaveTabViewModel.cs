namespace PhoneGuitarTab.UI.ViewModel
{
    using System;
    using System.IO;
    using System.Windows;

    using PhoneGuitarTab.Data;
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.UI.Infrastructure;

    public class StaveTabViewModel : TabViewModelBase
    {
        [Dependency]
        public StaveTabViewModel(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            
        }

        protected override void ReadNavigationParameters()
        {
            try
            {
                base.ReadNavigationParameters();

                /*using (var stream = FileSystem.OpenFile(Tablature.Path, FileMode.Open))
                {
                    TabContent = (new StreamReader(stream)).ReadToEnd();
                }*/

                // expect binary format (e.g. gp5)
                using (var stream = FileSystem.OpenFile(Tablature.Path, FileMode.Open))
                {
                    var bytes = default(byte[]);
                    using (var memstream = new MemoryStream())
                    {
                        var buffer = new byte[512];
                        var bytesRead = default(int);
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            memstream.Write(buffer, 0, bytesRead);
                        bytes = memstream.ToArray();
                    }

                    TabContent = Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
