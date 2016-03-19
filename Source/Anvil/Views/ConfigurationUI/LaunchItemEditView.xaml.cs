using System.Windows;

using Anvil.Framework;
using Anvil.Framework.MVVM;

using Microsoft.Win32;

using Ookii.Dialogs.Wpf;

namespace Anvil.Views.ConfigurationUI
{
    public partial class LaunchItemEditView : LaunchItemEditViewImpl
    {
        public LaunchItemEditView()
        {
            InitializeComponent();
        }

        private void _LookupFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = FileTextBox.Text ?? "";
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;
            dialog.Filter = new FileFilterBuilder()
                .With("Executable files", ".exe", ".bat", ".cmd", ".com", ".lnk", ".ps1")
                .WithAllFiles()
                .ToFileFilter();

            if(dialog.ShowDialog(Window.GetWindow(this)) ?? false)
            {
                FileTextBox.Text = dialog.FileName;
            }
        }

        private void _LookupWorkingDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();

            if(dialog.ShowDialog(Window.GetWindow(this)) ?? false)
            {
                WorkingDirectoryTextBox.Text = dialog.SelectedPath;
            }
        }
    }

    public abstract class LaunchItemEditViewImpl : View<LaunchItemEditViewModel>
    {
    }
}
