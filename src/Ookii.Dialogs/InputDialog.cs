// Copyright © Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Ookii.Dialogs
{
    /// <summary>
    /// Represents a dialog that allows the user to input a single text value.
    /// </summary>
    /// <remarks>
    /// Using a modal dialog to ask the user for input is not often the best design decision. Carefully
    /// evaluate alternative methods, such as in-place editing, if applicable, before using this dialog.
    /// </remarks>
    /// <threadsafety instance="false" static="true" />
    [DefaultProperty("MainInstruction"), DefaultEvent("ButtonClicked"), Description("A dialog that allows the user to input a single text value.")]
    public partial class InputDialog : Component, IBindableComponent
    {
        private string _mainInstruction;
        private string _content;
        private string _windowTitle;
        private string _input;
        private int _maxLength = Int16.MaxValue;
        private bool _usePasswordMasking;

        /// <summary>
        /// Event raised when the value of the <see cref="Input"/> property changes.
        /// </summary>
        /// <remarks>
        /// The value of the <see cref="Input"/> property is updated only when the user clicks OK and the dialog is closed, not while
        /// the user is using the dialog.
        /// </remarks>
        [Category("Property Changed"), Description("Event raised when the value of the Input property changes.")]
        public event EventHandler InputChanged;
        /// <summary>
        /// Event raised when the user clicks the OK button on the dialog.
        /// </summary>
        /// <remarks>
        /// Set the <see cref="CancelEventArgs.Cancel"/> property to <see langword="true" /> to prevent the dialog from closing.
        /// </remarks>
        [Category("Action"), Description("Event raised when the user clicks the OK button on the dialog.")]
        public event EventHandler<OkButtonClickedEventArgs> OkButtonClicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDialog"/> class.
        /// </summary>
        public InputDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDialog"/> class, adding it to the specified container.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> to add the component to.</param>
        public InputDialog(IContainer container)
        {
            if( container != null )
                container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the dialog's main instruction.
        /// </summary>
        /// <value>
        /// The dialog's main instruction. The default value is an empty string ("").
        /// </value>
        /// <remarks>
        /// When running on Windows Vista or newer, the main instruction of the input dialog will be displayed in a larger font and a different color than
        /// the other text of the input dialog. Otherwise, it will be shown in bold.
        /// </remarks>
        [Localizable(true), Category("Appearance"), Description("The dialog's main instruction."), DefaultValue(""), Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string MainInstruction
        {
            get { return _mainInstruction ?? string.Empty; }
            set 
            { 
                _mainInstruction = string.IsNullOrEmpty(value) ? null : value;
            }
        }

        /// <summary>
        /// Gets or sets the dialog's primary content.
        /// </summary>
        /// <value>
        /// The dialog's primary content. The default value is an empty string ("").
        /// </value>
        [Localizable(true), Category("Appearance"), Description("The dialog's primary content."), DefaultValue(""), Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string Content
        {
            get { return _content ?? string.Empty; }
            set 
            { 
                _content = string.IsNullOrEmpty(value) ? null : value;
            }
        }

        /// <summary>
        /// Gets or sets the window title of the dialog.
        /// </summary>
        /// <value>
        /// The window title of the dialog. The default is an empty string ("").
        /// </value>
        [Localizable(true), Category("Appearance"), Description("The window title of the task dialog."), DefaultValue("")]
        public string WindowTitle
        {
            get
            {
                return _windowTitle ?? string.Empty;
            }
            set
            {
                _windowTitle = string.IsNullOrEmpty(value) ? null : value;
            }
        }

        /// <summary>
        /// Gets or sets the text specified by the user.
        /// </summary>
        /// <value>
        /// The initial text of the input field, or the text specified by the user. The default vaue is an empty string ("").
        /// </value>
        /// <remarks>
        /// Setting this property before calling <see cref="ShowDialog()"/> determines the initial text in the input field. Retrieving
        /// the property after the user has clicked OK will return the text entered by the user.
        /// </remarks>
        [Localizable(true), Category("Appearance"), Description("The text specified by the user."), DefaultValue("")]
        public string Input
        {
            get { return _input ?? string.Empty; }
            set 
            {
                _input = value = string.IsNullOrEmpty(value) ? null : value;
                OnInputChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of characters that can be entered into the input field of the dialog.
        /// </summary>
        /// <value>
        /// The number of characters that can be entered into the input field. The default value is 32767.
        /// </value>
        [Localizable(true), Category("Behavior"), Description("The maximum number of characters that can be entered into the input field of the dialog."), DefaultValue((int)Int16.MaxValue)]
        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the input will be masked using the system password character.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the input is masked; otherwise, <see langword="false" />. The default value is <see langword="false" />.
        /// </value>
        [Category("Behavior"), Description("Indicates whether the input will be masked using the system password character."), DefaultValue(false)]
        public bool UsePasswordMasking
        {
            get { return _usePasswordMasking; }
            set { _usePasswordMasking = value; }
        }

        /// <summary>
        /// Raises the <see cref="InputChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> containing data for the event.</param>
        protected virtual void OnInputChanged(EventArgs e)
        {
            if( InputChanged != null )
                InputChanged(this, e);
        }

        /// <summary>
        /// Raises the <see cref="OkButtonClicked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="OkButtonClickedEventArgs"/> containing data for the event.</param>
        protected virtual void OnOkButtonClicked(OkButtonClickedEventArgs e)
        {
            if( OkButtonClicked != null )
                OkButtonClicked(this, e);
        }

        /// <summary>
        /// Displays the input box as a modal dialog box.
        /// </summary>
        /// <returns>The <see cref="DialogResult"/> value that corresponds to the button the user clicked.</returns>
        public DialogResult ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        /// Displays the input box as a modal dialog box with the specified owner.
        /// </summary>
        /// <param name="owner">The <see cref="System.Windows.Forms.IWin32Window"/> that will be the owner of the dialog box.</param>
        /// <returns>The <see cref="DialogResult"/></returns>
        public DialogResult ShowDialog(System.Windows.Forms.IWin32Window owner)
        {
            using( InputDialogForm frm = new InputDialogForm() )
            {
                frm.MainInstruction = MainInstruction;
                frm.Content = Content;
                frm.Text = WindowTitle;
                frm.Input = Input;
                frm.UsePasswordMasking = UsePasswordMasking;
                frm.MaxLength = MaxLength;
                frm.OkButtonClicked += new EventHandler<OkButtonClickedEventArgs>(InputBoxForm_OkButtonClicked);
                DialogResult result = frm.ShowDialog(owner);
                if( result == DialogResult.OK )
                    Input = frm.Input;
                return result;
            }
        }

        private void InputBoxForm_OkButtonClicked(object sender, OkButtonClickedEventArgs e)
        {
            OnOkButtonClicked(e);
        }

        #region IBindableComponent Members

        private BindingContext _context;

        /// <summary>
        /// Gets or sets the collection of currency managers for the <see cref="InputDialog"/>.
        /// </summary>
        /// <value>
        /// The collection of currency managers for the <see cref="TaskDialog"/>.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Advanced)]
        public BindingContext BindingContext
        {
            get
            {
                return _context ?? (_context = new BindingContext());
            }
            set
            {
                _context = value;
            }
        }

        private ControlBindingsCollection _bindings;

        /// <summary>
        /// Gets the collection of data-binding objects for this <see cref="InputDialog"/>.
        /// </summary>
        /// <value>
        /// The collection of data-binding objects for this <see cref="TaskDialog"/>.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data"), RefreshProperties(RefreshProperties.All), ParenthesizePropertyName(true)]
        public ControlBindingsCollection DataBindings
        {
            get { return _bindings ?? (_bindings = new ControlBindingsCollection(this)); }
        }

        #endregion
    }
}
