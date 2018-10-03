// Copyright © Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Ookii.Dialogs
{
    /// <summary>
    /// Base class for windows forms with extended functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     In order to use this class, create a new Form in Visual Studio, then modify the base
    ///     class in the .cs (or in Visual Basic, the .Designer.vb) file for the form to this class.
    /// </para>
    /// <para>
    ///     This class provides two main pieces of functionality: the ability to use the system font
    ///     and the ability to use client area glass in Windows Vista.
    /// </para>
    /// <para>
    ///     Windows Forms applications will normally always use MS Sans Serif 8pt, even when the
    ///     system font is something else. For example, on Windows XP (English), the system font
    ///     is Tahoma 8pt. On Windows XP Japanese, the system font is MS UI Gothic 8pt. On Windows
    ///     Vista it is Segoe UI 9pt. By setting the <see cref="UseSystemFont"/> property to <see langword="true" /> for a form,
    ///     that form will automatically use the proper system font.
    /// </para>
    /// <para>
    ///     Because the font metrics of these different fonts can be vary greatly
    ///     (especially on Vista which uses a larger font), please make sure the 
    ///     <see cref="ContainerControl.AutoScaleMode" /> property is set to
    ///     <see cref="AutoScaleMode.Font"/>, and make special provisions for resizing
    ///     graphics and other elements.
    ///     Please note that some system fonts (such as MS UI Gothic) can also be smaller than MS Sans Serif
    ///     which means the form will be scaled down, not up.
    /// </para>
    /// <note>
    ///     If you set <see cref="UseSystemFont"/> to <see langword="true" />, it is strongly recommended you
    ///     test your application with various different font and DPI settings.
    /// </note>
    /// <para>
    ///     To use glass with your form, use the <see cref="GlassMargin"/> property. You can also
    ///     specify whether it's possible to drag the form by its glass areas using the <see cref="AllowGlassDragging"/>
    ///     property. Glass requires Windows Vista with the Desktop Window Manager enabled. The glass related
    ///     properties have no effect on other operating systems.
    /// </para>
    /// <note>
    ///     Many Windows Forms controls will not display correctly when placed over a glass area. Use the 
    ///     <see cref="Glass.DrawCompositedText"/> method to display text on a glass area.
    /// </note>
    /// <para>
    ///     This class will automatically handle changes in the system font or DWM settings (including enabling/disabling
    ///     of the DWM) while the application is running.
    /// </para>
    /// </remarks>
    /// <threadsafety instance="false" static="true" />
    public class ExtendedForm : Form
    {
        private bool _useSystemFont;
        private Padding _glassMargin;
        private bool _allowGlassDragging = true;

        /// <summary>
        /// Raised when Desktop Window Manager (DWM) composition has been enabled or disabled.
        /// </summary>
        /// <remarks>
        /// <note>
        ///   This event is only raised on Windows Vista or later.
        /// </note>
        /// <para>
        ///   Use the <see cref="Glass.IsDwmCompositionEnabled"/> property to determine the
        ///   current composition state.
        /// </para>
        /// </remarks>
        public event EventHandler DwmCompositionChanged;

        /// <summary>
        /// Creates a new instance of the <see cref="ExtendedForm" /> class.
        /// </summary>
        public ExtendedForm()
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the form automatically uses the system default font.
        /// </summary>
        /// <value>
        /// <see langword="true" /> when the form's font is automatically adjusted to the system font; 
        /// otherwise, <see langword="false" />. The default value is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This property has no effect at design time; the font settings will only be applied at
        ///   run time.
        /// </para>
        /// <para>
        ///   When this property is set to <see langword="true" />, the form will use the configured 
        ///   system font and font size automatically, e.g. it will use Segoe UI on Windows Vista and 
        ///   Tahoma on Windows XP. Please make sure the <see cref="ContainerControl.AutoScaleMode" /> 
        ///   property is set to <see cref="AutoScaleMode.Font"/> 
        ///   to account for the different metrics of the various fonts, and make special provisions 
        ///   to scale graphics and other element. Note that the system font can also be smaller than
        ///   the font you used at design time, causing the form to be scaled down. For example the font
        ///   MS Gothic UI, used on Japanese versions of Windows (pre-Vista), has smaller metrics than
        ///   MS Sans Serif.
        /// </para>
        /// <note>
        ///   It is strongly recommended to test your application with different font and DPI settings if you set this property
        ///   to <see langword="true" />.
        /// </note>
        /// </remarks>
        [Category("Appearance"), DefaultValue(false), Description("Indicates whether or not the form automatically uses the system default font.")]
        public bool UseSystemFont
        {
            get { return _useSystemFont; }
            set { _useSystemFont = value; }
        }

        /// <summary>
        /// Gets or sets the glass margins of the form.
        /// </summary>
        /// <value>
        /// A <see cref="Padding"/> that indicates the glass margins of the form. The default value is
        /// <see cref="Padding.Empty"/>.
        /// </value>
        /// <remarks>
        /// <note>
        ///   Client-area glass requires Windows Vista or later with the Desktop Window Manager enabled. If the Desktop
        ///   Window Manager is not enabled, or an older version of Windows is used, this property is ignored.
        /// </note>
        /// <para>
        ///   Client-area glass extends the glass frame used by the Windows Vista Aero user interface into the client
        ///   area of your window.
        /// </para>
        /// <para>
        ///   Use negative margins to create the "sheet of glass" effect where the client area is rendered 
        ///   as a completely glass surface.
        /// </para>
        /// <para>
        ///   Text rendered with the <see cref="TextRenderer" /> will not display correctly on the glass area (this includes
        ///   text drawn by most controls). To draw text on the glass area, use <see cref="Glass.DrawCompositedText"/>.
        /// </para>
        /// <para>
        ///   If the form is scaled, which can happen for instance if <see cref="UseSystemFont"/> is <see langword="true" />,
        ///   the glass margin will also be scaled.
        /// </para>
        /// <para>
        ///   At design time, the glass area will be indicated by a pattern drawn onto the form. This pattern will not
        ///   be visible at runtime regardless of whether the glass is enabled or not.
        /// </para>
        /// </remarks>
        [Category("Appearance"), Description("The glass margins of the form.")]
        public Padding GlassMargin
        {
            get { return _glassMargin; }
            set
            {
                if( _glassMargin != value )
                {
                    _glassMargin = value;
                    EnableGlass();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the form can be dragged by the glass areas inside the client area.
        /// </summary>
        /// <value>
        /// <see langword="true" /> when the form can be dragged using the client area glass; otherwise, <see langword="false" />.
        /// The default value is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// This property has no effect on operating systems older than Windows Vista, when the Desktop Window Manager is disabled
        /// or when the <see cref="GlassMargin" /> property is set to zero.
        /// </remarks>
        [Category("Behavior"), Description("Indicates whether the form can be dragged by the glass areas inside the client area."), DefaultValue(true)]
        public bool AllowGlassDragging
        {
            get { return _allowGlassDragging; }
            set { _allowGlassDragging = value; }
        }

        /// <summary>
        /// Raises the <see cref="DwmCompositionChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> containing the event data.</param>
        protected virtual void OnDwmCompositionChanged(EventArgs e)
        {
            EventHandler handler = DwmCompositionChanged;
            if( handler != null )
                handler(this, e);
        }

        /// <summary>
        /// Overrides the <see cref="Form.OnLoad"/> method.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            if( !DesignMode && _useSystemFont )
            {
                Font = System.Drawing.SystemFonts.IconTitleFont;
                Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            }
            base.OnLoad(e);
        }

        /// <summary>
        /// Overrides the <see cref="Form.OnFormClosed"/> method.
        /// </summary>
        /// <param name="e">A <see cref="FormClosedEventArgs"/> that contains the event data.</param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Microsoft.Win32.SystemEvents.UserPreferenceChanged -= new Microsoft.Win32.UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="pevent">A <see cref="PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if( DesignMode || Glass.IsDwmCompositionEnabled )
            {
                if( DesignMode )
                {
                    using( HatchBrush brush = new HatchBrush(HatchStyle.OutlinedDiamond, Color.SkyBlue, BackColor) )
                    {
                        PaintGlassArea(pevent, brush);
                    }
                }
                else
                    PaintGlassArea(pevent, Brushes.Black);
            }
            else
                base.OnPaintBackground(pevent);
        }

        /// <summary>
        /// This member overrides <see cref="Form.OnResize" />.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if( _glassMargin.All != 0 )
                Invalidate();
        }

        /// <summary>
        /// This member overrides <see cref="Form.OnHandleCreated" />.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            EnableGlass();
            base.OnHandleCreated(e);
        }

        /// <summary>
        /// This member overrides <see cref="Form.WndProc" />.
        /// </summary>
        /// <param name="m">The Windows <see cref="Message" /> to process.</param>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch( m.Msg )
            {
            case NativeMethods.WM_NCHITTEST:
                if( _allowGlassDragging && m.Result == new IntPtr((int)NativeMethods.HitTestResult.Client) && Glass.IsDwmCompositionEnabled )
                {
                    if( _glassMargin.Left == -1 && _glassMargin.Top == -1 && _glassMargin.Right == -1 && _glassMargin.Bottom == -1 )
                    {
                        m.Result = new IntPtr((int)NativeMethods.HitTestResult.Caption);
                        return;
                    }
                    else
                    {
                        Point p = new Point((int)m.LParam & 0xffff, (int)m.LParam >> 16);
                        p = PointToClient(p);
                        if( p.X < _glassMargin.Left || p.X > ClientSize.Width - _glassMargin.Right || p.Y < _glassMargin.Top || p.Y > ClientSize.Height - _glassMargin.Bottom )
                        {
                            m.Result = new IntPtr((int)NativeMethods.HitTestResult.Caption);
                            return;
                        }
                    }
                }
                break;
            case NativeMethods.WM_DWMCOMPOSITIONCHANGED:
                // If the DWM has been disabled we don't need to do anything; if it was enabled we need to set the glass margins.
                if( _glassMargin.All != 0 )
                    EnableGlass();
                OnDwmCompositionChanged(EventArgs.Empty);
                m.Result = IntPtr.Zero;
                break;
            }
        }

        /// <summary>
        /// Overrides <see cref="Form.ScaleControl" />.
        /// </summary>
        /// <param name="factor">The height and width of the control's bounds.</param>
        /// <param name="specified">A <see cref="BoundsSpecified" /> value that specifies the bounds of the control to use when defining its size and position.</param>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            float width = factor.Width;
            Padding newMargin = this.GlassMargin;
            if( width != 1f )
            {
                if( newMargin.Left > 0 )
                    newMargin.Left = (int)Math.Round(newMargin.Left * width);
                if( newMargin.Right > 0 )
                    newMargin.Right = (int)Math.Round(newMargin.Right * width);
            }
            float height = factor.Height;
            if( height != 1f )
            {
                if( newMargin.Top > 0 )
                    newMargin.Top = (int)Math.Round(newMargin.Top * height);
                if( newMargin.Bottom > 0 )
                    newMargin.Bottom = (int)Math.Round(newMargin.Bottom * height);
            }
            GlassMargin = newMargin;
            base.ScaleControl(factor, specified);
        }

        private void EnableGlass()
        {
            if( !DesignMode && Glass.IsDwmCompositionEnabled )
            {
                Glass.ExtendFrameIntoClientArea(this, GlassMargin);
                Invalidate();
            }
        }

        private void PaintGlassArea(PaintEventArgs pevent, Brush brush)
        {
            if( _glassMargin.Left == -1 && _glassMargin.Top == -1 && _glassMargin.Right == -1 && _glassMargin.Bottom == -1 )
                pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
            else
            {
                Rectangle rect = new Rectangle(_glassMargin.Left, _glassMargin.Top, ClientSize.Width - _glassMargin.Right, ClientSize.Height - _glassMargin.Bottom);
                pevent.Graphics.FillRectangle(new System.Drawing.SolidBrush(BackColor), rect);

                if( _glassMargin.Left != 0 )
                    pevent.Graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, _glassMargin.Left, ClientSize.Height));
                if( _glassMargin.Right != 0 )
                    pevent.Graphics.FillRectangle(brush, new System.Drawing.Rectangle(ClientSize.Width - _glassMargin.Right, 0, ClientSize.Width, ClientSize.Height));
                if( _glassMargin.Top != 0 )
                    pevent.Graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, ClientSize.Width, _glassMargin.Top));
                if( _glassMargin.Bottom != 0 )
                    pevent.Graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, ClientSize.Height - _glassMargin.Bottom, ClientSize.Width, ClientSize.Height));
            }
        }

        private void SystemEvents_UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            if( e.Category == Microsoft.Win32.UserPreferenceCategory.Window && _useSystemFont )
                this.Font = System.Drawing.SystemFonts.IconTitleFont;
        }
    }
}
