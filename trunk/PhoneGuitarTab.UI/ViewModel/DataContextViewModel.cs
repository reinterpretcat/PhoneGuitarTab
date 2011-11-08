using System;
using System.Collections.Generic;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class DataContextViewModel: Core.ViewModel
    {
        protected IDataContextService Database { get; private set; }
        protected bool IsRequireBinding { get; set; }

        protected virtual void DataBind()
        {
            
        }

        public DataContextViewModel()
        {
            Database = Container.Resolve<IDataContextService>();
            Database.OnChanged += (o, e) => { IsRequireBinding = true; };
            IsRequireBinding = true;
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            if (IsRequireBinding)
            {
                DataBind();
                IsRequireBinding = false;
            }
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            
        }
    }
}
