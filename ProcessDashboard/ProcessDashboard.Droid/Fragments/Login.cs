#region
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Plugin.Connectivity;
using Debug = System.Diagnostics.Debug;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class Login : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var lf = inflater.Inflate(Resource.Layout.Login, container, false);
            var login = lf.FindViewById<Button>(Resource.Id.login_login);

            var token = lf.FindViewById<TextView>(Resource.Id.login_token);
            var username = lf.FindViewById<TextView>(Resource.Id.login_username);
            var password = lf.FindViewById<TextView>(Resource.Id.login_password);

            ProgressDialog pd;

            login.Click += (sender, args) =>
            {
                if (token.Text.Equals("") || username.Text.Equals("") || password.Text.Equals(""))
                {
                    Toast.MakeText(Activity, "Please check the values you have entered", ToastLength.Short).Show();
                }
                else
                {
                    Debug.WriteLine("We are checking network connection");
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                        builder.SetTitle("Unable to connect")
                            .SetMessage("Please check your internet connection and try again")
                            .SetNeutralButton("Okay", (sender2, args2) => { builder.Dispose(); })
                            .SetCancelable(false);
                        AlertDialog alert = builder.Create();
                        alert.Show();
                    }
                    else
                    {
                        CheckCredentials(token.Text, username.Text, password.Text);
                    }
                }
            };
            return lf;
        }

        public async Task<int> CheckCredentials(string datatoken, string userid, string password)
        {
            //Check username and password
            System.Diagnostics.Debug.WriteLine("We are inside the outer task");
            ProgressDialog pd = new ProgressDialog(Activity);
            pd.SetMessage("Checking username and password");
            pd.SetCancelable(false);
            pd.Show();
            AlertDialog.Builder builder = new AlertDialog.Builder((Activity));
            await Task.Run(() =>
            {
                Debug.WriteLine("We are checking username");
                HttpWebResponse resp;
                try
                {
                    var req =
                        WebRequest.CreateHttp(Settings.GetInstance()._baseurl + "api/v1/datasets/" +
                                              Settings.GetInstance().Dataset + "/");
                    req.Method = "GET";
                    req.AllowAutoRedirect = false;
                    string credential = userid + ":" + password;
                    req.Headers.Add("Authorization",
                        "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credential)));
                    // req.Get

                    resp = (HttpWebResponse) req.GetResponse();

                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        if (resp.GetResponseStream().CanRead)
                        {
                            Stream data = resp.GetResponseStream();
                            var reader = new StreamReader(data);

                            string responseStr = reader.ReadToEnd();
                            Debug.WriteLine(responseStr);

                            if (responseStr.Contains("auth-required"))
                            {
                                Debug.WriteLine("Wrong credentials 2");
                                Activity.RunOnUiThread(() =>
                                {
                                    if (pd.IsShowing)
                                        pd.Dismiss();

                                    builder.SetTitle("Wrong Credentials")
                                        .SetMessage("Please check your username and password and try again.")
                                        .SetNeutralButton("Okay", (sender2, args2) => { builder.Dispose(); })
                                        .SetCancelable(false);
                                    AlertDialog alert = builder.Create();
                                    alert.Show();
                                    Debug.WriteLine("We should have shown the dialog now");

                                });
                               
                            }
                            else if (responseStr.Contains("permission-denied"))
                            {
                                Debug.WriteLine("permission issue");
                                Activity.RunOnUiThread(() =>
                                {
                                    if (pd.IsShowing)
                                        pd.Dismiss();

                                    builder.SetTitle("Access Denied")
                                        .SetMessage("You donot have access to this dataset")
                                        .SetNeutralButton("Okay", (sender2, args2) => { builder.Dispose(); })
                                        .SetCancelable(false);
                                    AlertDialog alert = builder.Create();

                                    alert.Show();
                                });

                                
                            }
                            else if (responseStr.Contains("dataset"))
                            {
                                Debug.WriteLine("Username and password was correct");
                                Activity.RunOnUiThread(() =>
                                {
                                    
                                pd.SetMessage("Getting Account Info");
                                pd.SetCancelable(false);
                                if (!pd.IsShowing)
                                    pd.Show();

                                });
                                Task.Run(() =>
                                {
                                    //LOAD METHOD TO GET ACCOUNT INFO
                                    Settings.GetInstance()._username = userid;
                                    Settings.GetInstance()._password = password;
                                    DataSetLocationResolver dslr = new DataSetLocationResolver();

                                    dslr.ResolveFromToken(datatoken, out Settings.GetInstance()._baseurl,
                                        out Settings.GetInstance().Dataset);

                                    Debug.WriteLine("We are going to store the values");
                                    AccountStorage ase = new AccountStorage();
                                    ase.SetContext(Activity);

                                    ase.Set(userid, password, Settings.GetInstance()._baseurl,
                                        Settings.GetInstance().Dataset);

                                    Debug.WriteLine("We have stored the values");
                                    Debug.WriteLine(ase.BaseUrl);
                                    Debug.WriteLine(ase.DataSet);
                                    Debug.WriteLine(ase.Password);
                                    Debug.WriteLine(ase.UserId);

                                    // Switch to next screen

                                    //HIDE PROGRESS DIALOG
                                    Activity.RunOnUiThread(() =>
                                    {

                                    if (pd.IsShowing)
                                        pd.Dismiss();

                                    Toast.MakeText(Activity, "Logged in", ToastLength.Short).Show();
                                    ((MainActivity) Activity).FragmentManager.PopBackStack();
                                    ((MainActivity) Activity).SetDrawerState(true);
                                    ((MainActivity) Activity).SwitchToFragment(
                                        MainActivity.FragmentTypes.Home);
                                    });
                                });
                            }
                        }
                    }
                }
                catch (WebException e)
                {
                    Debug.WriteLine("We have a problem");
                    Activity.RunOnUiThread(() =>
                    {

                    if (pd.IsShowing)
                    {
                        pd.Dismiss();
                    }

                    });
                    using (WebResponse response = e.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse) response;
                        Console.WriteLine("Error code: {0}", httpResponse.StatusCode);

                        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            Debug.WriteLine("Wrong credentials");
                             Activity.RunOnUiThread(() =>
                               {
                                   try
                                   {

                                       builder.SetTitle("Unauthorized")
                                           .SetMessage(
                                               "Please check your username and password and data token and try again.")
                                           .SetNeutralButton("Okay", (sender2, args2) => { builder.Dispose(); })
                                           .SetCancelable(false);
                                       Debug.WriteLine("Wrong credentials : 1");

                                       AlertDialog alert = builder.Create();
                                       Debug.WriteLine("Wrong credentials : 2");

                                       alert.Show();
                                       Debug.WriteLine("Wrong credentials : The dialog should have been shown now");
                                   }
                                   catch (Exception e2)
                                   {
                                       Debug.WriteLine("We have hit an error while showing the dialog :" + e2.Message);
                                   }
                               });

                               
                           
                        }
                    }
                }
                catch (Exception e)
                {
                    // Catching any generic exception
                    Debug.WriteLine("We have hit a generic exception :" + e.Message);
                    Activity.RunOnUiThread(() =>
                    {
                        AlertDialog.Builder builder2 = new AlertDialog.Builder(Activity);
                        builder2.SetTitle("Error occured")
                            .SetMessage(e.Message +
                                        ". Please report this error to the developers. We are sorry for the inconvenience.")
                            .SetNeutralButton("Okay", (sender2, args2) => { builder2.Dispose(); })
                            .SetCancelable(false);
                        AlertDialog alert2 = builder2.Create();
                        alert2.Show();
                    });
                }

                return true;
            });

            //    pd.Dismiss();

            System.Diagnostics.Debug.WriteLine("We are done with the outer task");
            return 0;
        }
    }
}