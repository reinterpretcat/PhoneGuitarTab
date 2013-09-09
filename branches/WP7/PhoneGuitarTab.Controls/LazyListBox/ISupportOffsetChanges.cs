using System;

namespace PhoneGuitarTab.Controls
{
    interface ISupportOffsetChanges
    {
        void HorizontalOffsetChanged(double offset);
        void VerticalOffsetChanged(double offset);
    }
}
