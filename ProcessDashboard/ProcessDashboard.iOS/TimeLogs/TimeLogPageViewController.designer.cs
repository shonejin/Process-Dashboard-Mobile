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
    [Register ("TimeLogPageViewController")]
    partial class TimeLogPageViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StaticLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TimelogsTable { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (StaticLabel != null) {
                StaticLabel.Dispose ();
                StaticLabel = null;
            }

            if (TimelogsTable != null) {
                TimelogsTable.Dispose ();
                TimelogsTable = null;
            }
        }
    }
}