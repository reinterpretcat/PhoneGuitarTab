using System;
using System.Collections.Generic;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Windows;

namespace PhoneGuitarTab.UI.ViewModel
{
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Core.IsolatedStorage;
    using PhoneGuitarTab.Core.Navigation;

    public class DataContextViewModel: Core.ViewModel
    {
        [Dependency]
        protected INavigationService NavigationService { get; private set; }

        [Dependency]
        protected IFileSystemService FileSystem { get; private set; }

        protected IDataContextService Database { get; private set; }

        protected bool IsRequireBinding { get; set; }

      

        public DataContextViewModel(IDataContextService database)
        {
            Database = database;
            Database.OnChanged += (o, e) =>
                {
                    IsRequireBinding = true;
                };
            IsRequireBinding = true;
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            if (IsRequireBinding)
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                {
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
