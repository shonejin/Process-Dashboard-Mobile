// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ProcessDashboard.iOS
{
    [Register ("TaskTimeLogViewController")]
    partial class TaskTimeLogViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TaskTimeLogTable { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TaskTimeLogTable != null) {
                TaskTimeLogTable.Dispose ();
                TaskTimeLogTable = null;
            }
        }
    }
}