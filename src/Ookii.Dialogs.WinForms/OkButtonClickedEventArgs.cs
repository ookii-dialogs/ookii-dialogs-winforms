#region Copyright 2009-2021 Ookii Dialogs Contributors
//
// Licensed under the BSD 3-Clause License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ookii.Dialogs.WinForms
{
    /// <summary>
    /// Provides data for the <see cref="InputDialog.OkButtonClicked"/> event.
    /// </summary>
    /// <threadsafety instance="false" static="true" />
    public class OkButtonClickedEventArgs : CancelEventArgs
    {
        private string _input;
        private System.Windows.Forms.IWin32Window _inputBoxWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="OkButtonClickedEventArgs"/> class with the specified input text
        /// and input box window.
        /// </summary>
        /// <param name="input">The current value of the input field on the dialog.</param>
        /// <param name="inputBoxWindow">The input box window.</param>
        public OkButtonClickedEventArgs(string input, System.Windows.Forms.IWin32Window inputBoxWindow)
        {
            _input = input;
            _inputBoxWindow = inputBoxWindow;
        }

        /// <summary>
        /// Gets the current value of the input field on the dialog.
        /// </summary>
        /// <value>
        /// The current value of the input field on the dialog.
        /// </value>
        /// <remarks>
        /// The <see cref="InputDialog.Input"/> property will not be updated until the dialog has been closed,
        /// so this property can be used to determine the value entered by the user when this event is raised.
        /// </remarks>
        public string Input
        {
            get { return _input; }
        }

        /// <summary>
        /// Gets the input box window.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Forms.IWin32Window"/> for the input box window.
        /// </value>
        /// <remarks>
        /// You can use this property if you need to display a modal dialog (for example to alert the user if the current input value
        /// is invalid) and you want the input box to be the parent of that dialog.
        /// </remarks>
        public System.Windows.Forms.IWin32Window InputBoxWindow
        {
            get { return _inputBoxWindow; }
        }
	
    }
}
