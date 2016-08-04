#region
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
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

            login.Click += (sender, args) =>
            {
                DataSetLocationResolver dslr = new DataSetLocationResolver();

                if (token.Text.Equals("") || username.Text.Equals("") || password.Text.Equals(""))
                {
                    Toast.MakeText(this.Activity, "Please enter a valid value", ToastLength.Short).Show();

                }
                else
                {

                   //Check uzsername and password

                    System.Net.HttpWebResponse resp;
                    try
                    {
                        var req = System.Net.WebRequest.CreateHttp(Settings.GetInstance()._baseurl + "api/v1/datasets/" + Settings.GetInstance().Dataset + "/");
                        req.Method = "GET";
                        req.AllowAutoRedirect = false;
                        String credential = username.Text + ":" + password.Text;
                        req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credential)));
                        resp = (System.Net.HttpWebResponse)req.GetResponse();
                    }
                    catch (Exception e)
                    {
                        ProgressDialog pd = new ProgressDialog(this.Activity);
                        pd.SetMessage("DataToken or credentials not recognized");
                        pd.Show();
                        return;
                    }


                    if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ProgressDialog pd = new ProgressDialog(this.Activity);
                        pd.SetMessage("Wrong credentials");
                        pd.Show();
                        return;
                    }
                    else if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        if (resp.GetResponseStream().CanRead)
                        {
                            byte[] buffer = new byte[10001];
                            resp.GetResponseStream().Read(buffer, 0, 10000);
                            String responseStr = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                            if (responseStr.Contains("auth-required"))
                            {
                                ProgressDialog pd = new ProgressDialog(this.Activity);
                                pd.Indeterminate = false;
                                pd.SetMessage("Wrong credentials");
                                pd.Show();
                            }
                            else if (responseStr.Contains("permission-denied"))
                            {
                                ProgressDialog pd = new ProgressDialog(this.Activity);
                                pd.Indeterminate = false;
                                pd.SetMessage("You may not have permission to access the given dataset.");
                                pd.Show();
                            }
                            else if (responseStr.Contains("dataset"))
                            {
                                ProgressDialog pd = new ProgressDialog(this.Activity);
                                pd.SetMessage("Loading");
                                pd.SetCancelable(false);

                                pd.Show();
                                Settings.GetInstance()._username = username.Text;
                                Settings.GetInstance()._password = password.Text;

                                dslr.ResolveFromToken(token.Text, out Settings.GetInstance()._baseurl,
                                    out Settings.GetInstance().Dataset);

                                AccountStorage ase = new AccountStorage();
                                ase.SetContext(this.Activity);
                                ase.Set(username.Text, password.Text, Settings.GetInstance()._baseurl, Settings.GetInstance().Dataset);

                                ((MainActivity)Activity).FragmentManager.PopBackStack();
                                ((MainActivity)Activity).SetDrawerState(true);
                                // Switch to next screen
                                pd.Dismiss();
                                ((MainActivity)Activity).SwitchToFragment(MainActivity.FragmentTypes.Home);

                            }
                        }



                     
                    }
                }
            }
                ;
            
            return lf;
        }

        private void Login_Click(object sender, EventArgs e)
        {
            // Login logic
            
        }
    }
}