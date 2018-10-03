# Ookii.Dialogs.WinForms [![NuGet Version](http://img.shields.io/nuget/v/Ookii.Dialogs.WinForms.svg?style=flat)](https://www.nuget.org/packages/Ookii.Dialogs.WinForms/)

## Overview

**Ookii.Dialogs.WinForms** is a class library for .NET Windows Forms applications providing several common dialogs. Included are classes for task dialogs, credential dialogs, progress dialogs, input dialogs, and common file dialogs.

### Getting started

Install the [Ookii.Dialogs.WinForms](https://www.nuget.org/packages/Ookii.Dialogs.WinForms/) package from NuGet:

```powershell
Install-Package Ookii.Dialogs.WinForms
```

The included sample application [`Ookii.Dialogs.WinForms.Sample`](sample/Ookii.Dialogs.WinForms.Sample/) demonstrate the dialogs for Windows Forms. View the source of this application to see how to use the dialogs.

N.B.: **Ookii.Dialogs.WinForms** requires [Microsoft .NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653) or higher.

### WPF compatibility

If you're looking to use these common dialogs on a WPF application, check out [Ookii.Dialogs.Wpf](https://github.com/caioproiete/ookii-dialogs-wpf).

## Included dialogs

### Task dialog

[Task dialogs](https://docs.microsoft.com/en-us/windows/desktop/Controls/task-dialogs-overview) are a new type of dialog first introduced in Windows Vista. They provide a superset of the message box functionality.

![A task dialog](assets/sample-task-dialog.png)

The `Ookii.Dialogs.WinForms.TaskDialog` classe provide access to the task dialog functionality. The `TaskDialog` class inherits from `System.ComponentModel.Component` and offers full support for the Windows Forms designer and component designer of Visual Studio.

The `TaskDialog` class requires Windows Vista or a later version of Windows. Windows XP is not supported. Note that it is safe to instantiate the `TaskDialog` class and set any of its properties; only when the dialog is shown will a `NotSupportedException` be thrown on unsupported operating systems.

### Progress dialog

Progress dialogs are a common dialog to show progress during operations that may take a long time. They are used extensively in the Windows shell, and an API has been available since Windows 2000.

![A progress dialog as it appears on Windows Vista and later](assets/sample-progress-dialog.png)

The `Ookii.Dialogs.WinForms.ProgressDialog` class provide a wrapper for the Windows progress dialog API. The `ProgressDialog` class inherits from `System.ComponentModel.Component` and offers full support for the Windows Forms designer and component designer of Visual Studio. The `ProgressDialog` class resembles the `System.ComponentModel.BackgroundWorker` class and can be used in much the same way as that class.

The progress dialog's behaviour of the `ShowDialog` function is slightly different than that of other .NET dialogs; It is recommended to use a non-modal dialog with the `Show` function.

The `ProgressDialog` class is supported on Windows XP and later versions of Windows. However, the progress dialog has a very different appearance on Windows Vista and later (the image above shows the Vista version), so it is recommended to test on both operating systems to see if it appears to your satisfaction.

When using Windows 7, the `ProgressDialog` class automatically provides progress notification in the application's task bar button.

### Credential dialog

The `Ookii.Dialogs.WinForms.CredentialDialog` class provide wrappers for the `CredUI` functionality first introduced in Windows XP. This class provides functionality for saving and retrieving generic credentials, as well as displaying the credential UI dialog. This class does not support all functionality of `CredUI`; only generic credentials are supported, thing such as domain credentials or alternative authentication providers (e.g. smart cards or biometric devices) are not supported.

![A credential dialog as it appears on Windows Vista and later](assets/sample-credential-dialog.png)

The `CredentialDialog` class inherits from `System.ComponentModel.Component` and offers full support for the Windows Forms designer and component designer of Visual Studio.

On Windows XP, the `CredentialDialog` class will use the `CredUIPromptForCredentials` function to show the dialog; on Windows Vista and later, the `CredUIPromptForWindowsCredentials` function is used instead to show the new dialog introduced with Windows Vista. Because of the difference in appearance in the two versions (the image above shows the Vista version), it is recommended to test on both operating systems to see if it appears to your satisfaction.

### Input dialog

The input dialog is a dialog that can be used to prompt the user for a single piece of text. Its functionality is reminiscent of the Visual Basic `InputBox` function, only with a cleaner API and UI.

![An input dialog as it appears on Windows Vista and later](assets/sample-input-dialog.png)

The `Ookii.Dialogs.WinForms.InputDialog` class provides the input dialog functionality for Windows Forms.

Unlike the other classes in this package, this class is not a wrapper for any existing API; the dialog uses a custom implementation in Windows Forms. This dialog is supported on Windows XP and later versions of windows; on Windows Vista and later, the visual styles API is used to draw the dialog to mimic the appearance of task dialogs, as shown in the image above.

The `InputDialog` class inherits from `System.ComponentModel.Component` and offers full support for the Windows Forms designer and component designer of Visual Studio.

### Vista-style common file dialogs

Windows Vista introduced a new style of common file dialogs. As of .NET 3.5 SP1, the Windows Forms `OpenFileDialog` and `SaveFileDialog` class will automatically use the new style under most circumstances; however, some settings (such as setting `ShowReadOnly` to `true`) still cause it to revert to the old dialog. The `FolderBrowserDialog` still uses the old style.

![The Vista-style folder browser dialog on Windows 7](assets/sample-folderbrowser-dialog.png)

The `Ookii.Dialogs.WinForms.VistaOpenFileDialog`, `Ookii.Dialogs.WinForms.VistaSaveFileDialog` and `Ookii.Dialogs.WinForms.VistaFolderBrowserDialog` provide these dialogs for Windows Forms (note that in the case of the `OpenFileDialog` and `SaveFileDialog` it is recommended to use the built-in .NET classes unless you hit one of the scenarios where those classes use the old dialogs).

The classes have been designed to resemble the original Windows Forms classes to make it easy to switch. When the classes are used on Windows XP, they will automatically fall back to the old style dialog.

The Vista-style file and folder dialogs classes for Windows Forms inherit from `System.ComponentModel.Component` and offer full support for the Windows Forms designer and component designer of Visual Studio.

## Additional functionality

Three additional classes are provided in the **Ookii.Dialogs.WinForms** library. These classes are used to support the `InputDialog` (with the exception of the Aero glass functionality, which the `InputDialog` does not use) but are made public for your convenience.

The `AdditionalVisualStyleElements` class provides some visual style elements used by task dialogs on Windows Vista and later.

The `Glass` class provides functionality for extending Aero glass into the client area of a window on Windows Vista and later, and for drawing text on a glass surface.

The `ExtendedForm` class serves as an alternative base class for forms, and provides functionality to automatically use the system font (e.g. Tahoma on XP and Segoe UI on Vista and later), and easy access to some of the functionality of the `Glass` class.

---

Copyright (c) Sven Groot 2009

Copyright (c) Caio Proiete 2018

See [LICENSE](LICENSE) for details
