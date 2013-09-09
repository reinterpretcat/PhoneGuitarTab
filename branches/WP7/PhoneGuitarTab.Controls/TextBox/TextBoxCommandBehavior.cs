using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneGuitarTab.Controls
{
    public class TextBoxCommandBehavior : CommandBehaviorBase<TextBox>
    {
        public TextBoxCommandBehavior(TextBox textBoxObject)
            : base(textBoxObject)
        {
            textBoxObject.KeyUp += (s, e) =>
                                       {
                                           string input = (s as TextBox).Text;
                                           //TODO validate user input here
                                           if ((e.Key == Key.Enter) 
                                               && (!String.IsNullOrEmpty(input)))
                                           {
                                               this.CommandParameter = input;
                                               ExecuteCommand();
                                           }
                                       };
        }
    }
}
