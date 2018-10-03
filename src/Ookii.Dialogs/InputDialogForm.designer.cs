namespace Ookii.Dialogs
{
    partial class InputDialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputDialogForm));
            this._primaryPanel = new System.Windows.Forms.Panel();
            this._inputTextBox = new System.Windows.Forms.TextBox();
            this._secondaryPanel = new System.Windows.Forms.Panel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this._primaryPanel.SuspendLayout();
            this._secondaryPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _primaryPanel
            // 
            this._primaryPanel.Controls.Add(this._inputTextBox);
            resources.ApplyResources(this._primaryPanel, "_primaryPanel");
            this._primaryPanel.Name = "_primaryPanel";
            this._primaryPanel.Paint += new System.Windows.Forms.PaintEventHandler(this._primaryPanel_Paint);
            // 
            // _inputTextBox
            // 
            resources.ApplyResources(this._inputTextBox, "_inputTextBox");
            this._inputTextBox.Name = "_inputTextBox";
            // 
            // _secondaryPanel
            // 
            this._secondaryPanel.Controls.Add(this._cancelButton);
            this._secondaryPanel.Controls.Add(this._okButton);
            resources.ApplyResources(this._secondaryPanel, "_secondaryPanel");
            this._secondaryPanel.Name = "_secondaryPanel";
            this._secondaryPanel.Paint += new System.Windows.Forms.PaintEventHandler(this._secondaryPanel_Paint);
            // 
            // _cancelButton
            // 
            resources.ApplyResources(this._cancelButton, "_cancelButton");
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _okButton
            // 
            resources.ApplyResources(this._okButton, "_okButton");
            this._okButton.Name = "_okButton";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // InputDialogForm
            // 
            this.AcceptButton = this._okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._primaryPanel);
            this.Controls.Add(this._secondaryPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputDialogForm";
            this.ShowInTaskbar = false;
            this.UseSystemFont = true;
            this.Load += new System.EventHandler(this.NewInputBoxForm_Load);
            this._primaryPanel.ResumeLayout(false);
            this._primaryPanel.PerformLayout();
            this._secondaryPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _primaryPanel;
        private System.Windows.Forms.Panel _secondaryPanel;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.TextBox _inputTextBox;
    }
}