using System.Collections.Generic;
using System.Windows;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class DataContextViewModel: ViewModel
    {
        [Dependency]
        protected INavigationService NavigationService { get; set; }

        [Dependency]
        protected IFileSystemService FileSystem { get; set; }

        protected MessageHub Hub { get; set; }

        protected IDataContextService Database { get; set; }

        protected bool IsRequireBinding { get; set; }     

        public DataContextViewModel(IDataContextService database, MessageHub hub)
        {
            Database = database;
            Database.OnChanged += (o, e) =>
                {
                    IsRequireBinding = true;
                };
            Hub = hub;
            IsRequireBinding = true;
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            if (IsRequireBinding)
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                {
                    //restore back the ViewModel here.
                    DataBind();
                    IsRequireBinding = false;
                });
            }
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            
        }

        protected virtual void DataBind()
        {

        }
    }
}
