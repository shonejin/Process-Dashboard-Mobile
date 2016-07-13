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
    [Register ("TaskDetailsViewController")]
    partial class TaskDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TdActualLb { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TdCheckboxBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TdCompleteLb { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TdNotesTf { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TdPauseBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TdPlannedLb { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TdPlayBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel tdProjectNameLb { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TdTaskNameLb { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TdTimelogBtn { get; set; }

        [Action ("UIButton334_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton334_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (TdActualLb != null) {
                TdActualLb.Dispose ();
                TdActualLb = null;
            }

            if (TdCheckboxBtn != null) {
                TdCheckboxBtn.Dispose ();
                TdCheckboxBtn = null;
            }

            if (TdCompleteLb != null) {
                TdCompleteLb.Dispose ();
                TdCompleteLb = null;
            }

            if (TdNotesTf != null) {
                TdNotesTf.Dispose ();
                TdNotesTf = null;
            }

            if (TdPauseBtn != null) {
                TdPauseBtn.Dispose ();
                TdPauseBtn = null;
            }

            if (TdPlannedLb != null) {
                TdPlannedLb.Dispose ();
                TdPlannedLb = null;
            }

            if (TdPlayBtn != null) {
                TdPlayBtn.Dispose ();
                TdPlayBtn = null;
            }

            if (tdProjectNameLb != null) {
                tdProjectNameLb.Dispose ();
                tdProjectNameLb = null;
            }

            if (TdTaskNameLb != null) {
                TdTaskNameLb.Dispose ();
                TdTaskNameLb = null;
            }

            if (TdTimelogBtn != null) {
                TdTimelogBtn.Dispose ();
                TdTimelogBtn = null;
            }
        }
    }
}