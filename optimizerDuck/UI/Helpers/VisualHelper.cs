using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace optimizerDuck.UI.Helpers;

public static class VisualHelper
{
    public static Frame? FindMainFrame(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is Frame frame && frame.Name == "PART_Frame")
                return frame;

            var result = FindMainFrame(child);
            if (result != null)
                return result;
        }

        return null;
    }
}