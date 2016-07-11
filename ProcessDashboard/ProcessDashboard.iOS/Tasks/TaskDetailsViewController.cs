using Foundation;
using System;
using UIKit;

namespace ProcessDashboard.iOS
{
    public partial class TaskDetailsViewController : UIViewController
    {
		public string taskId;
        public TaskDetailsViewController (IntPtr handle) : base (handle)
        {
        }
    }
}