// Copyright © Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Ookii.Dialogs
{
    /// <summary>
    /// Provides functionality to use Aero Glass.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This class provides functionality to extend the Windows Vista Aero glass window frame into a window's
    ///   client area, and to draw text on the glass area.
    /// </para>
    /// <para>
    ///   The <see cref="ExtendedForm"/> class provides more comprehensive support for client area glass than
    ///   using <see cref="ExtendFrameIntoClientArea" /> by using its <see cref="ExtendedForm.GlassMargin"/> property. 
    ///   In addition to the basic client area glass support, <see cref="ExtendedForm"/> will also
    ///   respond to changes in the Desktop Window Manager state and enable/disable client area glass as necessary. It also
    ///   allows the window to be dragged using the client area glass areas using the <see cref="ExtendedForm.AllowGlassDragging"/>
    ///   property.
    /// </para>
    /// <note>
    ///   Use of glass requires Windows Vista or later with the Desktop Window Manager enabled.
    /// </note>
    /// </remarks>
    /// <threadsafety instance="false" static="true" />
    public static class Glass
    {
        /// <summary>
        /// Gets a value that indicates whether the operating system supports composition through the Desktop Window Manager.
        /// </summary>
        /// <value>
        /// <see langword="true" /> on operating systems that support the Desktop Window Manager (Windows Vista and higher); otherwise, <see langword="false" />.
        /// </value>
        public static bool OSSupportsDwmComposition
        {
            get
            {
                return NativeMethods.IsWindowsVistaOrLater;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether desktop composition is enabled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> when desktop composition is supported and enabled; otherwise, <see langword="false" />.
        /// </value>
        public static bool IsDwmCompositionEnabled
        {
            get
            {
                return OSSupportsDwmComposition && NativeMethods.DwmIsCompositionEnabled();
            }
        }

        /// <summary>
        /// Extends the glass window frame into the client area of the specified window.
        /// </summary>
        /// <param name="window">The <see cref="IWin32Window"/> on which to enable client area glass.</param>
        /// <param name="glassMargin">The the margins to use when extending the frame into the client area.</param>
        /// <remarks>
        /// <para>
        ///   Use negative margins to create the "sheet of glass" effect where the client area is rendered 
        ///   as a completely glass surface.
        /// </para>
        /// <para>
        ///   The glass area must be filled with either a black brush or the <see cref="Form.TransparencyKey" /> color
        ///   in order to display correctly. If the <see cref="Form.TransparencyKey"/> method is used, clicks in the
        ///   glass area will "fall through" the window to the window below it. If the black brush method is used, text
        ///   rendered with the <see cref="TextRenderer" /> will not display correctly on the glass area (this includes
        ///   text drawn by most controls). To draw text on the glass area, use <see cref="DrawCompositedText"/>.
        /// </para>
        /// <para>
        ///   This method needs to be called again if the state of the Desktop Window Manager is toggled.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="window"/> is <see langword="null" />.</exception>
        /// <exception cref="NotSupportedException">The current operating system does not support glass, or the Desktop Window Manager is not enabled.</exception>
        public static void ExtendFrameIntoClientArea(this System.Windows.Forms.IWin32Window window, Padding glassMargin)
        {
            if( !IsDwmCompositionEnabled )
                throw new NotSupportedException(Properties.Resources.GlassNotSupportedError);

            if( window == null )
                throw new ArgumentNullException("window");

            NativeMethods.MARGINS margins = new NativeMethods.MARGINS(glassMargin);
            NativeMethods.DwmExtendFrameIntoClientArea(window.Handle, ref margins);
        }

        /// <summary>
        /// Draws composited text onto the glass area of a form.
        /// </summary>
        /// <param name="dc">The <see cref="IDeviceContext"/> onto which the composited text should be drawn.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
        /// <param name="bounds">The <see cref="Rectangle" /> that represents the bounds of the text.</param>
        /// <param name="padding">The <see cref="Padding"/> around the text; necessary to allow space for the glow effect.</param>
        /// <param name="foreColor">The <see cref="Color" /> to apply to the drawn text.</param>
        /// <param name="textFormat">A bitwise combination of the <see cref="TextFormatFlags" /> values.</param>
        /// <param name="glowSize">Specifies the size of a glow that will be drawn on the background prior to any text being drawn.</param>
        /// <remarks>
        /// <para>
        ///   Do not use this method to draw text on non-glass areas of a form.
        /// </para>
        /// </remarks>
        /// <exception cref="NotSupportedException">The current operating system does not support glass, or the Desktop Window Manager is not enabled.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dc"/>, <paramref name="text"/> or <paramref name="font"/> is <see langword="null"/>.</exception>
        public static void DrawCompositedText(IDeviceContext dc, string text, Font font, Rectangle bounds, Padding padding, Color foreColor, int glowSize, TextFormatFlags textFormat)
        {
            if( !IsDwmCompositionEnabled )
                throw new NotSupportedException(Properties.Resources.GlassNotSupportedError);

            if( dc == null )
                throw new ArgumentNullException("dc");
            if( text == null )
                throw new ArgumentNullException("text");
            if( font == null )
                throw new ArgumentNullException("font");

            IntPtr primaryHdc = dc.GetHdc();
            try
            {
                using( SafeDeviceHandle memoryHdc = NativeMethods.CreateCompatibleDC(primaryHdc) )
                using( SafeGDIHandle fontHandle = new SafeGDIHandle(font.ToHfont(), true) )
                using( SafeGDIHandle dib = NativeMethods.CreateDib(bounds, primaryHdc, memoryHdc) )
                {
                    NativeMethods.SelectObject(memoryHdc, fontHandle);

                    // Draw glowing text
                    System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Window.Caption.Active);
                    NativeMethods.DTTOPTS dttOpts = new NativeMethods.DTTOPTS();
                    dttOpts.dwSize = Marshal.SizeOf(typeof(NativeMethods.DTTOPTS));
                    dttOpts.dwFlags = NativeMethods.DrawThemeTextFlags.Composited | NativeMethods.DrawThemeTextFlags.GlowSize | NativeMethods.DrawThemeTextFlags.TextColor;
                    dttOpts.crText = ColorTranslator.ToWin32(foreColor);
                    dttOpts.iGlowSize = glowSize;
                    NativeMethods.RECT textBounds = new NativeMethods.RECT(padding.Left, padding.Top, bounds.Width - padding.Right, bounds.Height - padding.Bottom);
                    NativeMethods.DrawThemeTextEx(renderer.Handle, memoryHdc, 0, 0, text, text.Length, (int)textFormat, ref textBounds, ref dttOpts);

                    // Copy to foreground
                    const int SRCCOPY = 0x00CC0020;
                    NativeMethods.BitBlt(primaryHdc, bounds.Left, bounds.Top, bounds.Width, bounds.Height, memoryHdc, 0, 0, SRCCOPY);
                }
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        /// <summary>
        /// Provides the size, in pixels, of the specified text.
        /// </summary>
        /// <param name="dc">The device context in which to measure the text.</param>
        /// <param name="text">The text to measure.</param>
        /// <param name="font">The <see cref="Font"/> to apply to the measured text.</param>
        /// <param name="textFormat">A bitwise combination of the <see cref="TextFormatFlags" /> values.</param>
        /// <returns>The <see cref="Size"/>, in pixels, of <paramref name="text"/> drawn with the specified <paramref name="font"/> and format.</returns>
        /// <exception cref="NotSupportedException">The current operating system does not support glass, or the Desktop Window Manager is not enabled.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dc"/>, <paramref name="text"/> or <paramref name="font"/> is <see langword="null"/>.</exception>
        public static Size MeasureCompositedText(IDeviceContext dc, string text, Font font, TextFormatFlags textFormat)
        {
            if( !IsDwmCompositionEnabled )
                throw new NotSupportedException(Properties.Resources.GlassNotSupportedError);

            if( dc == null )
                throw new ArgumentNullException("dc");
            if( text == null )
                throw new ArgumentNullException("text");
            if( font == null )
                throw new ArgumentNullException("font");

            IntPtr primaryHdc = dc.GetHdc();
            try
            {
                Rectangle bounds = new Rectangle(0, 0, int.MaxValue, int.MaxValue);

                using( SafeDeviceHandle memoryHdc = NativeMethods.CreateCompatibleDC(primaryHdc) )
                using( SafeGDIHandle fontHandle = new SafeGDIHandle(font.ToHfont(), true) )
                using( SafeGDIHandle dib = NativeMethods.CreateDib(bounds, primaryHdc, memoryHdc) )
                {
                    NativeMethods.SelectObject(memoryHdc, fontHandle);

                    System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Window.Caption.Active);
                    NativeMethods.RECT bounds2 = new NativeMethods.RECT(bounds);
                    NativeMethods.RECT rect;
                    NativeMethods.GetThemeTextExtent(renderer.Handle, memoryHdc, 0, 0, text, text.Length, (int)textFormat, ref bounds2, out rect);
                    return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                }
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }    
    }
}
