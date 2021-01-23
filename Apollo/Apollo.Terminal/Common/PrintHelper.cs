using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Apollo.Terminal.Common
{
    public static class PrintHelper
    {
        public static void PrintFrameworkElement(FrameworkElement element, string title)
        {
            var printDlg = new PrintDialog();
            if (printDlg.ShowDialog() != true)
            {
                throw new InvalidOperationException("Unable to open print dialog");
            }

            var capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

            var scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / element.ActualWidth,
                capabilities.PageImageableArea.ExtentHeight / element.ActualHeight);
            element.LayoutTransform = new ScaleTransform(scale, scale);
            var size = new Size(capabilities.PageImageableArea.ExtentWidth,
                capabilities.PageImageableArea.ExtentHeight);

            element.Measure(size);
            element.Arrange(new Rect(
                new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight),
                size));

            printDlg.PrintVisual(element, title);
        }
    }
}