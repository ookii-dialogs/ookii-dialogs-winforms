using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using System.Drawing;

namespace Ookii.Dialogs
{
    static class NativeMethods
    {
        public static bool IsWindowsVistaOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 6000);
            }
        }

        public static bool IsWindowsXPOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(5, 1, 2600);
            }
        }

        #region LoadLibrary

        public const int ErrorFileNotFound = 2;

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Ookii.Dialogs.SafeModuleHandle LoadLibraryEx(
            string lpFileName,
            IntPtr hFile,
            LoadLibraryExFlags dwFlags
            );

        [DllImport("kernel32", SetLastError = true),
        ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [Flags]
        public enum LoadLibraryExFlags : uint
        {
            DontResolveDllReferences = 0x00000001,
            LoadLibraryAsDatafile = 0x00000002,
            LoadWithAlteredSearchPath = 0x00000008,
            LoadIgnoreCodeAuthzLevel = 0x00000010
        }

        #endregion

        #region Task Dialogs

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetCurrentThreadId();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist"), DllImport("comctl32.dll", PreserveSig = false)]
        public static extern void TaskDialogIndirect([In] ref TASKDIALOGCONFIG pTaskConfig, out int pnButton, out int pnRadioButton, [MarshalAs(UnmanagedType.Bool)] out bool pfVerificationFlagChecked);


        public delegate uint TaskDialogCallback(IntPtr hwnd, uint uNotification, IntPtr wParam, IntPtr lParam, IntPtr dwRefData);


        public const int WM_USER = 0x400;
        public const int WM_GETICON = 0x007F;
        public const int WM_SETICON = 0x0080;
        public const int ICON_SMALL = 0;

        public enum TaskDialogNotifications
        {
            Created = 0,
            Navigated = 1,
            ButtonClicked = 2,            // wParam = Button ID
            HyperlinkClicked = 3,            // lParam = (LPCWSTR)pszHREF
            Timer = 4,            // wParam = Milliseconds since dialog created or timer reset
            Destroyed = 5,
            RadioButtonClicked = 6,            // wParam = Radio Button ID
            DialogConstructed = 7,
            VerificationClicked = 8,             // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
            Help = 9,
            ExpandoButtonClicked = 10            // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
        }

        [Flags]
        public enum TaskDialogCommonButtonFlags
        {
            OkButton = 0x0001, // selected control return value IDOK
            YesButton = 0x0002, // selected control return value IDYES
            NoButton = 0x0004, // selected control return value IDNO
            CancelButton = 0x0008, // selected control return value IDCANCEL
            RetryButton = 0x0010, // selected control return value IDRETRY
            CloseButton = 0x0020  // selected control return value IDCLOSE
        };

        [Flags]
        public enum TaskDialogFlags
        {
            EnableHyperLinks = 0x0001,
            UseHIconMain = 0x0002,
            UseHIconFooter = 0x0004,
            AllowDialogCancellation = 0x0008,
            UseCommandLinks = 0x0010,
            UseCommandLinksNoIcon = 0x0020,
            ExpandFooterArea = 0x0040,
            ExpandedByDefault = 0x0080,
            VerificationFlagChecked = 0x0100,
            ShowProgressBar = 0x0200,
            ShowMarqueeProgressBar = 0x0400,
            CallbackTimer = 0x0800,
            PositionRelativeToWindow = 0x1000,
            RtlLayout = 0x2000,
            NoDefaultRadioButton = 0x4000,
            CanBeMinimized = 0x8000
        };

        public enum TaskDialogMessages
        {
            NavigatePage = WM_USER + 101,
            ClickButton = WM_USER + 102, // wParam = Button ID
            SetMarqueeProgressBar = WM_USER + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
            SetProgressBarState = WM_USER + 104, // wParam = new progress state
            SetProgressBarRange = WM_USER + 105, // lParam = MAKELPARAM(nMinRange, nMaxRange)
            SetProgressBarPos = WM_USER + 106, // wParam = new position
            SetProgressBarMarquee = WM_USER + 107, // wParam = 0 (stop marquee), wParam != 0 (start marquee), lparam = speed (milliseconds between repaints)
            SetElementText = WM_USER + 108, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
            ClickRadioButton = WM_USER + 110, // wParam = Radio Button ID
            EnableButton = WM_USER + 111, // lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID
            EnableRadioButton = WM_USER + 112, // lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID
            ClickVerification = WM_USER + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
            UpdateElementText = WM_USER + 114, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
            SetButtonElevationRequiredState = WM_USER + 115, // wParam = Button ID, lParam = 0 (elevation not required), lParam != 0 (elevation required)
            UpdateIcon = WM_USER + 116  // wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)
        }

        public enum TaskDialogElements
        {
            Content,
            ExpandedInformation,
            Footer,
            MainInstruction
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TASKDIALOG_BUTTON
        {
            public int nButtonID;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszButtonText;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable"), StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TASKDIALOGCONFIG
        {
            public uint cbSize;
            public IntPtr hwndParent;
            public IntPtr hInstance;
            public TaskDialogFlags dwFlags;
            public TaskDialogCommonButtonFlags dwCommonButtons;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszWindowTitle;
            public IntPtr hMainIcon;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszMainInstruction;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszContent;
            public uint cButtons;
            //[MarshalAs(UnmanagedType.LPArray)]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr pButtons;
            public int nDefaultButton;
            public uint cRadioButtons;
            //[MarshalAs(UnmanagedType.LPArray)]
            public IntPtr pRadioButtons;
            public int nDefaultRadioButton;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszVerificationText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszExpandedInformation;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszExpandedControlText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszCollapsedControlText;
            public IntPtr hFooterIcon;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszFooterText;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public TaskDialogCallback pfCallback;
            public IntPtr lpCallbackData;
            public uint cxWidth;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Activation Context

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static ActivationContextSafeHandle CreateActCtx(ref ACTCTX actctx);
        [DllImport("kernel32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public extern static void ReleaseActCtx(IntPtr hActCtx);
        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool ActivateActCtx(ActivationContextSafeHandle hActCtx, out IntPtr lpCookie);
        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool DeactivateActCtx(uint dwFlags, IntPtr lpCookie);

        public const int ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x004;

        public struct ACTCTX
        {
            public int cbSize;
            public uint dwFlags;
            public string lpSource;
            public ushort wProcessorArchitecture;
            public ushort wLangId;
            public string lpAssemblyDirectory;
            public string lpResourceName;
            public string lpApplicationName;
        }


        #endregion

        #region File Operations Definitions

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;
        }

        internal enum FDAP
        {
            FDAP_BOTTOM = 0x00000000,
            FDAP_TOP = 0x00000001,
        }

        internal enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,
            FDESVR_ACCEPT = 0x00000001,
            FDESVR_REFUSE = 0x00000002
        }

        internal enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,
            FDEOR_ACCEPT = 0x00000001,
            FDEOR_REFUSE = 0x00000002
        }

        internal enum SIATTRIBFLAGS
        {
            SIATTRIBFLAGS_AND = 0x00000001, // if multiple items and the attirbutes together.
            SIATTRIBFLAGS_OR = 0x00000002, // if multiple items or the attributes together.
            SIATTRIBFLAGS_APPCOMPAT = 0x00000003, // Call GetAttributes directly on the ShellFolder for multiple attributes
        }

        internal enum SIGDN : uint
        {
            SIGDN_NORMALDISPLAY = 0x00000000,           // SHGDN_NORMAL
            SIGDN_PARENTRELATIVEPARSING = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
            SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,  // SHGDN_FORPARSING
            SIGDN_PARENTRELATIVEEDITING = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
            SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            SIGDN_FILESYSPATH = 0x80058000,             // SHGDN_FORPARSING
            SIGDN_URL = 0x80068000,                     // SHGDN_FORPARSING
            SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            SIGDN_PARENTRELATIVE = 0x80080001           // SHGDN_INFOLDER
        }

        [Flags]
        internal enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items.
            FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
            FOS_DEFAULTNOMINIMODE = 0x20000000
        }

        internal enum CDCONTROLSTATE
        {
            CDCS_INACTIVE = 0x00000000,
            CDCS_ENABLED = 0x00000001,
            CDCS_VISIBLE = 0x00000002
        }

        #endregion

        #region KnownFolder Definitions

        internal enum FFFP_MODE
        {
            FFFP_EXACTMATCH,
            FFFP_NEARESTPARENTMATCH
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct KNOWNFOLDER_DEFINITION
        {
            internal NativeMethods.KF_CATEGORY category;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszCreator;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszDescription;
            internal Guid fidParent;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszRelativePath;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszParsingName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszToolTip;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszLocalizedName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszIcon;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSecurity;
            internal uint dwAttributes;
            internal NativeMethods.KF_DEFINITION_FLAGS kfdFlags;
            internal Guid ftidType;
        }

        internal enum KF_CATEGORY
        {
            KF_CATEGORY_VIRTUAL = 0x00000001,
            KF_CATEGORY_FIXED = 0x00000002,
            KF_CATEGORY_COMMON = 0x00000003,
            KF_CATEGORY_PERUSER = 0x00000004
        }

        [Flags]
        internal enum KF_DEFINITION_FLAGS
        {
            KFDF_PERSONALIZE = 0x00000001,
            KFDF_LOCAL_REDIRECT_ONLY = 0x00000002,
            KFDF_ROAMABLE = 0x00000004,
        }


        // Property System structs and consts
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct PROPERTYKEY
        {
            internal Guid fmtid;
            internal uint pid;
        }

        #endregion

        #region Shell Parsing Names

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

        public static Interop.IShellItem CreateItemFromParsingName(string path)
        {
            object item;
            Guid guid = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"); // IID_IShellItem
            int hr = NativeMethods.SHCreateItemFromParsingName(path, IntPtr.Zero, ref guid, out item);
            if( hr != 0 )
                throw new System.ComponentModel.Win32Exception(hr);
            return (Interop.IShellItem)item;
        }

        #endregion

        #region String resources

        [Flags()]
        public enum FormatMessageFlags
        {
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
            FORMAT_MESSAGE_FROM_STRING = 0x00000400,
            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern int LoadString(SafeModuleHandle hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint FormatMessage([MarshalAs(UnmanagedType.U4)] FormatMessageFlags dwFlags, IntPtr lpSource,
           uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer,
           uint nSize, string[] Arguments);

        #endregion

        #region Desktop Window Manager

        public const int WM_NCHITTEST = 0x0084;
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        public enum HitTestResult
        {
            Error = -2,
            Transparent = -1,
            Nowhere = 0,
            Client = 1,
            Caption = 2,
            SysMenu = 3,
            GrowBox = 4,
            Size = GrowBox,
            Menu = 5,
            HScroll = 6,
            VScroll = 7,
            MinButton = 8,
            MaxButton = 9,
            Left = 10,
            Right = 11,
            Top = 12,
            TopLeft = 13,
            TopRight = 14,
            Bottom = 15,
            BottomLeft = 16,
            BottomRight = 17,
            Border = 18,
            Reduce = MinButton,
            Zoom = MaxButton,
            SizeFirst = Left,
            SizeLast = BottomRight,
            Object = 19,
            Close = 20,
            Help = 21
        }

        public struct MARGINS
        {
            public MARGINS(Padding value)
            {
                Left = value.Left;
                Right = value.Right;
                Top = value.Top;
                Bottom = value.Bottom;
            }
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, [In] ref MARGINS pMarInset);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmIsCompositionEnabled();
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern SafeDeviceHandle CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(SafeDeviceHandle hDC, SafeGDIHandle hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, SafeDeviceHandle hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        [DllImport("UxTheme.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DrawThemeTextEx(IntPtr hTheme, SafeDeviceHandle hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);
        [DllImport("gdi32.dll")]
        public static extern SafeGDIHandle CreateDIBSection(IntPtr hdc, BITMAPINFO pbmi, uint iUsage, IntPtr ppvBits, IntPtr hSection, uint dwOffset);
        [DllImport("UxTheme.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetThemeTextExtent(IntPtr hTheme, SafeDeviceHandle hdc, int iPartId, int iStateId, string text, int iCharCount, int dwTextFlags, [In]ref RECT bounds, out RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct DTTOPTS
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.U4)]
            public DrawThemeTextFlags dwFlags;
            public int crText;
            public int crBorder;
            public int crShadow;
            public int iTextShadowType;
            public Point ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public bool fApplyOverlay;
            public int iGlowSize;
            public int pfnDrawTextCallback;
            public IntPtr lParam;
        }

        [Flags()]
        public enum DrawThemeTextFlags
        {
            TextColor = 1 << 0,
            BorderColor = 1 << 1,
            ShadowColor = 1 << 2,
            ShadowType = 1 << 3,
            ShadowOffset = 1 << 4,
            BorderSize = 1 << 5,
            FontProp = 1 << 6,
            ColorProp = 1 << 7,
            StateId = 1 << 8,
            CalcRect = 1 << 9,
            ApplyOverlay = 1 << 10,
            GlowSize = 1 << 11,
            Callback = 1 << 12,
            Composited = 1 << 13
        }

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
            public byte bmiColors_rgbBlue;
            public byte bmiColors_rgbGreen;
            public byte bmiColors_rgbRed;
            public byte bmiColors_rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle rectangle)
            {
                Left = rectangle.X;
                Top = rectangle.Y;
                Right = rectangle.Right;
                Bottom = rectangle.Bottom;
            }

            public override string ToString()
            {
                return "Left: " + Left + ", " + "Top: " + Top + ", Right: " + Right + ", Bottom: " + Bottom;
            }
        }

        public static SafeGDIHandle CreateDib(Rectangle bounds, IntPtr primaryHdc, SafeDeviceHandle memoryHdc)
        {
            BITMAPINFO info = new BITMAPINFO();
            info.biSize = Marshal.SizeOf(info);
            info.biWidth = bounds.Width;
            info.biHeight = -bounds.Height;
            info.biPlanes = 1;
            info.biBitCount = 32;
            info.biCompression = 0; // BI_RGB
            SafeGDIHandle dib = CreateDIBSection(primaryHdc, info, 0, IntPtr.Zero, IntPtr.Zero, 0);
            SelectObject(memoryHdc, dib);
            return dib;
        }

        //private static void InitThemeText(Font font, Rectangle bounds, IntPtr primaryHdc, out IntPtr memoryHdc, out SafeGDIHandle dib, IntPtr fontHandle)
        //{
        //    // Create a memory DC so we can work offscreen
        //    memoryHdc = CreateCompatibleDC(primaryHdc);

        //    // Create a device-independent bitmap and select it into our DC
        //    BITMAPINFO info = new BITMAPINFO();
        //    info.biSize = Marshal.SizeOf(info);
        //    info.biWidth = bounds.Width;
        //    info.biHeight = -bounds.Height;
        //    info.biPlanes = 1;
        //    info.biBitCount = 32;
        //    info.biCompression = 0; // BI_RGB
        //    dib = CreateDIBSection(primaryHdc, info, 0, IntPtr.Zero, IntPtr.Zero, 0);
        //    SelectObject(memoryHdc, dib);

        //    // Create and select font
        //    SelectObject(memoryHdc, fontHandle);
        //}

        #endregion

        #region Credentials

        internal const int CREDUI_MAX_USERNAME_LENGTH = 256 + 1 + 256;
        internal const int CREDUI_MAX_PASSWORD_LENGTH = 256;

        [Flags]
        public enum CREDUI_FLAGS
        {
            INCORRECT_PASSWORD = 0x1,
            DO_NOT_PERSIST = 0x2,
            REQUEST_ADMINISTRATOR = 0x4,
            EXCLUDE_CERTIFICATES = 0x8,
            REQUIRE_CERTIFICATE = 0x10,
            SHOW_SAVE_CHECK_BOX = 0x40,
            ALWAYS_SHOW_UI = 0x80,
            REQUIRE_SMARTCARD = 0x100,
            PASSWORD_ONLY_OK = 0x200,
            VALIDATE_USERNAME = 0x400,
            COMPLETE_USERNAME = 0x800,
            PERSIST = 0x1000,
            SERVER_CREDENTIAL = 0x4000,
            EXPECT_CONFIRMATION = 0x20000,
            GENERIC_CREDENTIALS = 0x40000,
            USERNAME_TARGET_CREDENTIALS = 0x80000,
            KEEP_USERNAME = 0x100000
        }

        [Flags]
        public enum CredUIWinFlags
        {
            Generic = 0x1,
            Checkbox = 0x2,
            AutoPackageOnly = 0x10,
            InCredOnly = 0x20,
            EnumerateAdmins = 0x100,
            EnumerateCurrentUser = 0x200,
            SecurePrompt = 0x1000,
            Pack32Wow = 0x10000000
        }

        internal enum CredUIReturnCodes
        {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004
        }

        internal enum CredTypes
        {
            CRED_TYPE_GENERIC = 1,
            CRED_TYPE_DOMAIN_PASSWORD = 2,
            CRED_TYPE_DOMAIN_CERTIFICATE = 3,
            CRED_TYPE_DOMAIN_VISIBLE_PASSWORD = 4
        }

        internal enum CredPersist
        {
            Session = 1,
            LocalMachine = 2,
            Enterprise = 3
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        internal struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszMessageText;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        extern static internal CredUIReturnCodes CredUIPromptForCredentials(
            ref CREDUI_INFO pUiInfo,
            string targetName,
            IntPtr Reserved,
            int dwAuthError,
            StringBuilder pszUserName,
            uint ulUserNameMaxChars,
            StringBuilder pszPassword,
            uint ulPaswordMaxChars,
            [MarshalAs(UnmanagedType.Bool), In(), Out()] ref bool pfSave,
            CREDUI_FLAGS dwFlags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern CredUIReturnCodes CredUIPromptForWindowsCredentials(
            ref CREDUI_INFO pUiInfo,
            uint dwAuthError,
            ref uint pulAuthPackage,
            IntPtr pvInAuthBuffer,
            uint ulInAuthBufferSize,
            out IntPtr ppvOutAuthBuffer,
            out uint pulOutAuthBufferSize,
            [MarshalAs(UnmanagedType.Bool)]ref bool pfSave,
            CredUIWinFlags dwFlags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredReadW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static internal bool CredRead(string TargetName, CredTypes Type, int Flags, out IntPtr Credential);

        [DllImport("advapi32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        extern static internal void CredFree(IntPtr Buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredDeleteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static internal bool CredDelete(string TargetName, CredTypes Type, int Flags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredWriteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static internal bool CredWrite(ref CREDENTIAL Credential, int Flags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredPackAuthenticationBuffer(uint dwFlags, string pszUserName, string pszPassword, IntPtr pPackedCredentials, ref uint pcbPackedCredentials);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredUnPackAuthenticationBuffer(uint dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref uint pcchMaxUserName, StringBuilder pszDomainName, ref uint pcchMaxDomainName, StringBuilder pszPassword, ref uint pcchMaxPassword);

        // Disable the "Internal field is never assigned to" warning.
#pragma warning disable 649
        // This type does not own the IntPtr native resource; when CredRead is used, CredFree must be called on the
        // IntPtr that the struct was marshalled from to release all resources including the CredentialBlob IntPtr,
        // When allocating the struct manually for CredWrite you should also manually deallocate the CredentialBlob.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        public struct CREDENTIAL
        {
            public int Flags;
            public CredTypes Type;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string TargetName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Comment;
            public long LastWritten;
            public uint CredentialBlobSize;
            // Since the resource pointed to must be either released manually or by CredFree, SafeHandle is not appropriate here
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            public IntPtr CredentialBlob;
            [MarshalAs(UnmanagedType.U4)]
            public CredPersist Persist;
            public int AttributeCount;
            public IntPtr Attributes;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string TargetAlias;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string UserName;
        }
#pragma warning restore 649

        #endregion
    
    }
}
