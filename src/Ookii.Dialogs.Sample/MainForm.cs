using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Ookii.Dialogs.Sample
{
    public partial class MainForm : ExtendedForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _dialogComboBox.SelectedIndex = 0;
        }

        private void _showDialogButton_Click(object sender, EventArgs e)
        {
            switch( _dialogComboBox.SelectedIndex )
            {
            case 0:
                ShowTaskDialog();
                break;
            case 1:
                ShowTaskDialogWithCommandLinks();
                break;
            case 2:
                ShowProgressDialog();
                break;
            case 3:
                ShowInputDialog();
                break;
            case 4:
                ShowCredentialDialog();
                break;
            case 5:
                ShowVistaFolderBrowserDialog();
                break;
            case 6:
                ShowVistaOpenFileDialog();
                break;
            case 7:
                ShowVistaSaveFileDialog();
                break;
            }
        }

        private void ShowTaskDialog()
        {
            if( TaskDialog.OSSupportsTaskDialogs )
            {
                TaskDialogButton button = _sampleTaskDialog.ShowDialog(this);
                if( button == _customTaskDialogButton )
                    MessageBox.Show(this, "You clicked the custom button", "Task Dialog Sample");
                else if( button == _okTaskDialogButton )
                    MessageBox.Show(this, "You clicked the OK button.", "Task Dialog Sample");
            }
            else
            {
                MessageBox.Show(this, "This operating system does not support task dialogs.", "Task Dialog Sample");
            }
        }

        private void ShowTaskDialogWithCommandLinks()
        {
            if( TaskDialog.OSSupportsTaskDialogs )
            {
                _sampleCommandLinkTaskDialog.ShowDialog(this);
            }
            else
            {
                MessageBox.Show(this, "This operating system does not support task dialogs.", "Task Dialog Sample");
            }
        }

        private void ShowProgressDialog()
        {
            if( _sampleProgressDialog.IsBusy )
                MessageBox.Show(this, "The progress dialog is already displayed.", "Progress dialog sample");
            else
                _sampleProgressDialog.Show(this); // Show a modeless dialog; this is the recommended mode of operation for a progress dialog.
        }

        private void ShowInputDialog()
        {
            if( _sampleInputDialog.ShowDialog(this) == DialogResult.OK )
                MessageBox.Show(this, "The text was: " + _sampleInputDialog.Input, "Sample input dialog");
        }

        private void ShowCredentialDialog()
        {
            if( _sampleCredentialDialog.ShowDialog(this) == DialogResult.OK )
            {
                MessageBox.Show(this, string.Format("You entered the following information:\nUser name: {0}\nPassword: {1}", _sampleCredentialDialog.Credentials.UserName, _sampleCredentialDialog.Credentials.Password), "Credential dialog sample");
                // Normally, you should verify if the credentials are correct before calling ConfirmCredentials.
                // ConfirmCredentials will save the credentials if and only if the user checked the save checkbox.
                _sampleCredentialDialog.ConfirmCredentials(true);
            }
        }

        private void ShowVistaFolderBrowserDialog()
        {
            if( !VistaFolderBrowserDialog.IsVistaFolderDialogSupported )
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            if( _sampleVistaFolderBrowserDialog.ShowDialog(this) == DialogResult.OK )
                MessageBox.Show(this, "The selected folder was: " + _sampleVistaFolderBrowserDialog.SelectedPath, "Sample folder browser dialog");
        }

        private void ShowVistaOpenFileDialog()
        {
            /* As of .Net 3.5 and .Net 2.0 SP1, the regular System.Windows.Forms.OpenFileDialog also supports using the new
             * dialog style. The VistaOpenFileDialog class is still present as a sample on how to use the new IFileDialog API,
             * and because the regular OpenFileDialog still reverts to the old dialog with some settings (e.g. ShowReadOnly set to true).
             */
            if( !VistaFileDialog.IsVistaFileDialogSupported )
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular open file dialog will be used. Please use Windows Vista to see the new dialog.", "Sample open file dialog");
            if( _sampleVistaOpenFileDialog.ShowDialog(this) == DialogResult.OK )
                MessageBox.Show(this, "The selected file was: " + _sampleVistaOpenFileDialog.FileName, "Sample open file dialog");
        }

        private void ShowVistaSaveFileDialog()
        {
            /* As of .Net 3.5 and .Net 2.0 SP1, the regular System.Windows.Forms.SaveFileDialog also supports using the new
             * dialog style. The VistaSaveFileDialog class is still present as a sample on how to use the new IFileDialog API.
             */
            if( !VistaFileDialog.IsVistaFileDialogSupported )
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular save file dialog will be used. Please use Windows Vista to see the new dialog.", "Sample save file dialog");
            if( _sampleVistaSaveFileDialog.ShowDialog(this) == DialogResult.OK )
                MessageBox.Show(this, "The selected file was: " + _sampleVistaSaveFileDialog.FileName, "Sample save file dialog");
        }

        private void _sampleTaskDialog_HyperlinkClicked(object sender, HyperlinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Href);
        }

        private void _sampleProgressDialog_DoWork(object sender, DoWorkEventArgs e)
        {
            // Implement the operation that the progress bar is showing progress of here, same as you would do with a background worker.
            for( int x = 0; x <= 100; ++x )
            {
                Thread.Sleep(500);
                // Periodically check CancellationPending and abort the operation if required.
                if( _sampleProgressDialog.CancellationPending )
                    return;
                // ReportProgress can also modify the main text and description; pass null to leave them unchanged.
                // If _sampleProgressDialog.ShowTimeRemaining is set to true, the time will automatically be calculated based on
                // the frequency of the calls to ReportProgress.
                _sampleProgressDialog.ReportProgress(x, null, string.Format(System.Globalization.CultureInfo.CurrentCulture, "Processing: {0}%", x));
            }
        }
    }
}
