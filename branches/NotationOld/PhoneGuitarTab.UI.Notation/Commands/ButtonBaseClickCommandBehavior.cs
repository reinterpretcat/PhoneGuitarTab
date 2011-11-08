using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace  Microsoft.Practices.Prism.Commands
{
    public class ButtonBaseClickCommandBehavior : CommandBehaviorBase<ButtonBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonBaseClickCommandBehavior"/> class and hooks up the Click event of 
        /// <paramref name="clickableObject"/> to the ExecuteCommand() method. 
        /// </summary>
        /// <param name="clickableObject">The clickable object.</param>
        public ButtonBaseClickCommandBehavior(ButtonBase clickableObject)
            : base(clickableObject)
        {
            clickableObject.Click += (s, e) => ExecuteCommand();
            
        }

       
    }
}