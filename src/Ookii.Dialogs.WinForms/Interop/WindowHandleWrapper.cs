// Copyright (c) Sven Groot (Ookii.org) 2009
// BSD license; see LICENSE for details.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ookii.Dialogs.WinForms.Interop
{
    class WindowHandleWrapper : IWin32Window
    {
        private IntPtr _handle;

        public WindowHandleWrapper(IntPtr handle)
        {
            _handle = handle;
        }

        #region IWin32Window Members

        public IntPtr Handle
        {
            get { return _handle; }
        }

        #endregion
    }
}
