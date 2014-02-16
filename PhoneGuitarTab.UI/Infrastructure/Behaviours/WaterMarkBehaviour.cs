using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public class Watermark : Behavior<TextBox>
    {
        private bool _hasWatermark;
        private Brush _textBoxForeground;
        private FontStyle _textBoxFontStyle;

        public String Text { get; set; }
        public Brush Foreground { get; set; }
        public FontStyle FontStyle { get; set; }

        protected override void OnAttached()
        {
            _textBoxForeground = AssociatedObject.Foreground;
            _textBoxFontStyle = AssociatedObject.FontStyle;

            base.OnAttached();
            if (Text != null && string.IsNullOrEmpty(AssociatedObject.Text))
                SetWatermarkText();
            AssociatedObject.GotFocus += GotFocus;
            AssociatedObject.LostFocus += LostFocus;
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.Text.Length == 0)
                if (Text != null)
                    SetWatermarkText();
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (_hasWatermark)
                RemoveWatermarkText();
        }

        private void RemoveWatermarkText()
        {
            AssociatedObject.Foreground = _textBoxForeground;
            AssociatedObject.FontStyle = _textBoxFontStyle;
            AssociatedObject.Text = "";
            _hasWatermark = false;
        }

        private void SetWatermarkText()
        {
            AssociatedObject.Foreground = Foreground;
            AssociatedObject.FontStyle = FontStyle;
            AssociatedObject.Text = Text;
            _hasWatermark = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= GotFocus;
            AssociatedObject.LostFocus -= LostFocus;
        }
    }
}
