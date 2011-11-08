using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;

namespace Microsoft.Practices.Prism.Commands
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
                                             
            /*textBoxObject.LostFocus += (s, e) =>
                                           {
                                               string input = (s as TextBox).Text;
                                               if (!String.IsNullOrEmpty(input))
                                               {
                                                   this.CommandParameter = input;
                                                   ExecuteCommand();
                                               }
                                           };*/

        }
    }
}
