using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BMTA.Utillity
{
    public class AutoCompleteBoxEx : AutoCompleteBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (SelectedItem != null && e.Key == Key.Enter)
            {
                if (Command != null)
                    Command.Execute(SelectedItem);

            }
            base.OnKeyDown(e);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                        typeof(ICommand),
                        typeof(AutoCompleteBoxEx),
                        null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
    }
}
