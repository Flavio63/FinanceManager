using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FinanceManager.Events
{
    public static class EnableControl
    {
        public static void EnableControlInGrid(Grid grid, string name, bool blSwitch)
        {
            foreach (object obj in grid.Children)
            {
                if (obj.GetType() == typeof(Grid))
                {
                    EnableControlInGrid(obj as Grid, name, blSwitch);
                }
                if (obj.GetType() == typeof(ScrollViewer))
                {
                    ScrollViewer SV = obj as ScrollViewer;
                    if (SV.Content.GetType() == typeof(Grid))
                    {
                        EnableControlInGrid(SV.Content as Grid, name, blSwitch);
                    }
                }
                Control control = obj as Control;
                if (control != null)
                {
                    if (control.GetType().Name == "ComboBox" && control.Name == name)
                    {
                        control.IsEnabled = blSwitch;
                        return;
                    }
                    if (control.GetType().Name == "Button" && control.Name == name)
                    {
                        control.IsEnabled = blSwitch;
                        return;
                    }
                }
                TextBox textBox = obj as TextBox;
                if (textBox != null)
                    if (textBox.Name == name)
                    {
                        textBox.IsEnabled = blSwitch;
                        return;
                    }
                StackPanel SP = obj as StackPanel;
                if (SP != null)
                    if (SP.Name == name)
                    {
                        SP.IsEnabled = blSwitch;
                        return;
                    }
            }
        }

        public static void VisibleControlInGrid(Grid grid, string name, System.Windows.Visibility visibility)
        {
            foreach (object obj in grid.Children)
            {
                if (obj.GetType() == typeof(Grid))
                {
                    VisibleControlInGrid(obj as Grid, name, visibility);
                }
                if (obj.GetType() == typeof(ScrollViewer))
                {
                    ScrollViewer SV = obj as ScrollViewer;
                    if (SV.Content.GetType() == typeof(Grid))
                    {
                        VisibleControlInGrid(SV.Content as Grid, name, visibility);
                    }
                }
                Control control = obj as Control;
                if (control != null)
                {
                    if (control.GetType().Name == "ComboBox" && control.Name == name)
                    {
                        control.Visibility = visibility;
                        return;
                    }
                    if (control.GetType().Name == "Button" && control.Name == name)
                    {
                        control.Visibility = visibility;
                        return;
                    }
                }
                TextBox textBox = obj as TextBox;
                if (textBox != null)
                    if (textBox.Name == name)
                    {
                        textBox.Visibility = visibility;
                        return;
                    }
                StackPanel SP = obj as StackPanel;
                if (SP != null)
                    if (SP.Name == name)
                    {
                        SP.Visibility = visibility;
                        return;
                    }
            }
        }
    }
}
