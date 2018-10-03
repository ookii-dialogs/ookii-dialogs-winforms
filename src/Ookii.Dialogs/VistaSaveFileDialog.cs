// Copyright (c) Sven Groot (Ookii.org) 2006
// See license.txt for details
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using Ookii.Dialogs.Interop;

namespace Ookii.Dialogs
{
    /// <summary>
    /// Prompts the user to select a location for saving a file.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This class will use the Vista style file dialog if possible, and automatically fall back to the old-style 
    ///   dialog on versions of Windows older than Vista.
    /// </para>
    /// <para>
    ///   As of .Net 3.5 and .Net 2.0 SP1, the regular <see cref="System.Windows.Forms.FileDialog"/> class will also use
    ///   the new Vista style dialogs. However, certain options, such as settings <see cref="System.Windows.Forms.OpenFileDialog.ShowReadOnly"/>,
    ///   still cause that class to revert to the old style dialogs. For this reason, this class is still provided.
    ///   It is recommended that you use the <see cref="System.Windows.Forms.SaveFileDialog"/> class whenever possible.
    /// </para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [Designer("System.Windows.Forms.Design.SaveFileDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.SaveFileDialog), "SaveFileDialog.bmp"), Description("Prompts the user to open a file.")]
    public class VistaSaveFileDialog : VistaFileDialog
    {
        /// <summary>
        /// Creates a new instance of <see cref="VistaSaveFileDialog" /> class.
        /// </summary>
        public VistaSaveFileDialog()
            : this(false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="VistaSaveFileDialog" /> class.
        /// </summary>
        /// <param name="forceDownlevel">When <see langword="true"/>, the old style common file dialog will always be used even if the OS supports the Vista style.</param>
        public VistaSaveFileDialog(bool forceDownlevel)
        {
            if( forceDownlevel || !IsVistaFileDialogSupported )
                DownlevelDialog = new SaveFileDialog();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box prompts the user for permission to create a file if the 
        /// user specifies a file that does not exist.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the dialog box prompts the user before creating a file if the user specifies a file name that does not exist; 
        /// <see langword="false"/> if the dialog box automatically creates the new file without prompting the user for permission. The default 
        /// value is <see langword="false"/>.
        /// </value>
        [DefaultValue(false), Category("Behavior"), Description("A value indicating whether the dialog box prompts the user for permission to create a file if the user specifies a file that does not exist.")]
        public bool CreatePrompt
        {
            get
            {
                if( DownlevelDialog != null )
                    return ((SaveFileDialog)DownlevelDialog).CreatePrompt;
                return GetOption(NativeMethods.FOS.FOS_CREATEPROMPT);
            }
            set
            {
                if( DownlevelDialog != null )
                    ((SaveFileDialog)DownlevelDialog).CreatePrompt = value;
                else
                    SetOption(NativeMethods.FOS.FOS_CREATEPROMPT, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Save As dialog box displays a warning if the user 
        /// specifies a file name that already exists.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the dialog box prompts the user before overwriting an existing file if the user specifies a file 
        /// name that already exists; <see langword="false"/> if the dialog box automatically overwrites the existing file without 
        /// prompting the user for permission. The default value is <see langword="true"/>.
        /// </value>
        [Category("Behavior"), DefaultValue(true), Description("A value indicating whether the Save As dialog box displays a warning if the user specifies a file name that already exists.")]
        public bool OverwritePrompt
        {
            get
            {
                if( DownlevelDialog != null )
                    return ((SaveFileDialog)DownlevelDialog).OverwritePrompt;
                return GetOption(NativeMethods.FOS.FOS_OVERWRITEPROMPT);
            }
            set
            {
                if( DownlevelDialog != null )
                    ((SaveFileDialog)DownlevelDialog).OverwritePrompt = value;
                else
                    SetOption(NativeMethods.FOS.FOS_OVERWRITEPROMPT, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if( DownlevelDialog == null )
            {
                OverwritePrompt = true;
            }
        }

        /// <summary>
        /// Opens the file with read/write permission selected by the user.
        /// </summary>
        /// <returns>The read/write file selected by the user.</returns>
        /// <exception cref="ArgumentNullException"><see cref="VistaFileDialog.FileName"/> is <see langword="null"/> or an empty string.</exception>
        public System.IO.Stream OpenFile()
        {
            if( DownlevelDialog != null )
                return ((SaveFileDialog)DownlevelDialog).OpenFile();
            else
            {
                string fileName = FileName;
                if( string.IsNullOrEmpty(fileName) )
                    throw new ArgumentNullException("FileName");
                return new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the <see cref="VistaFileDialog.FileOk" /> event.
        /// </summary>
        /// <param name="e">A <see cref="System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>        
        protected override void OnFileOk(CancelEventArgs e)
        {
            // For reasons unknown, .Net puts the OFN_FILEMUSTEXIST and OFN_CREATEPROMPT flags on the save file dialog despite 
            // the fact that these flags only works on open file dialogs, and then prompts manually. Similarly, the 
            // FOS_CREATEPROMPT and FOS_FILEMUSTEXIST flags don't actually work on IFileSaveDialog, so we have to implement 
            // the prompt manually.
            if( DownlevelDialog == null )
            {
                if( CheckFileExists && !File.Exists(FileName) )
                {
                    PromptUser(ComDlgResources.FormatString(ComDlgResources.ComDlgResourceId.FileNotFound, Path.GetFileName(FileName)), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }
                if( CreatePrompt && !File.Exists(FileName) )
                {
                    if( !PromptUser(ComDlgResources.FormatString(ComDlgResources.ComDlgResourceId.CreatePrompt, Path.GetFileName(FileName)), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) )
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            base.OnFileOk(e);
        }

        #endregion

        #region Internal Methods

        internal override Ookii.Dialogs.Interop.IFileDialog CreateFileDialog()
        {
            return new Interop.NativeFileSaveDialog();
        }

        #endregion

    }
}
