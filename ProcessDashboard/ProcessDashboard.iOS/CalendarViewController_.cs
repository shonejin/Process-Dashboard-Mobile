using System;
using TimesSquare.iOS;
using Foundation;
using UIKit;

namespace ProcessDashboard.iOS
{
    public partial class CalendarViewController : UIViewController
    {

		public CalendarViewController(IntPtr handle) : base(handle)
        {

		}

		NSCalendar _calendar;

       
		public NSCalendar Calendar
		{
			get
			{
				return _calendar;
			}
			set
			{
				_calendar = value;
				NavigationItem.Title = Calendar.Identifier;
				TabBarItem.Title = Calendar.Identifier;
			}
		}

		public override void LoadView()
		{
			base.LoadView();

			var onePixel = 1.0f / UIScreen.MainScreen.Scale;

			var calendarView = new TSQCalendarView(View.Bounds)
		    {
				Calendar = new NSCalendar(NSCalendarType.Gregorian),
				RowCellClass = new ObjCRuntime.Class("TSQTACalendarRowCell"),
				FirstDate = NSDate.FromTimeIntervalSinceNow(-60 * 60 * 24 * 30 * 1), // Need to process later 

				LastDate = NSDate.FromTimeIntervalSinceNow(60 * 60 * 24 * 365 * 5),
				BackgroundColor = UIColor.FromRGBA(0.84f, 0.85f, 0.86f, 1.0f),
				PagingEnabled = true,
				ContentInset = new UIEdgeInsets(0.0f, onePixel, 0.0f, onePixel),

			};


			calendarView.DidSelectDate += (sender, e) =>
			{
				InvokeOnMainThread(() =>
				{
					var netDate = (DateTime)e.Date;
					new UIAlertView("You selected", netDate.ToLongDateString(), null, "Ok", null).Show();
				});
			};

			View.Add(calendarView);
		}
    }
}