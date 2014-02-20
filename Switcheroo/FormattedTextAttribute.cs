using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Switcheroo
{
    public class FormattedTextAttribute
    {
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.RegisterAttached(
            "FormattedText",
            typeof(string),
            typeof(FormattedTextAttribute),
            new UIPropertyMetadata("", FormattedTextChanged));

        public static void SetFormattedText(DependencyObject textBlock, string value)
        {
            textBlock.SetValue(FormattedTextProperty, value);
        }

        public static string GetFormattedText(DependencyObject textBlock)
        {
            return (string)textBlock.GetValue(FormattedTextProperty);
        }

        private static void FormattedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
            {
                return;
            }

            var formattedText = (string)e.NewValue ?? string.Empty;
            formattedText =
                @"<Span xml:space=""preserve"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                    formattedText +
                "</Span>";

            textBlock.Inlines.Clear();
            var result = (Span)XamlReader.Parse(formattedText);
            textBlock.Inlines.Add(result);
        }
    }
}