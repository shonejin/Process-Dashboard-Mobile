#region
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using ProcessDashboard.DTO;
using ProcessDashboard.SyncLogic;
using Debug = System.Diagnostics.Debug;
#endregion
namespace ProcessDashboard.Droid.Fragments
{
    public class ListOfProjects : ListFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            // Create your fragment here
            ((MainActivity) Activity).SetTitle("List of Projects");
            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MainActivity) Activity).SetTitle("List of projects");
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            var listView = ListView;
            listView.ItemClick += (sender, e) =>
            {
                // var p = e.Parent.SelectedItem.Class;
                var p = (ProjectsAdapter) listView.Adapter;
                Debug.WriteLine("Type:" + p.GetProject(e.Position));
                var projectId = p.GetProject(e.Position).Id;
                //  ((MainActivity)this.Activity).switchToFragment(MainActivity.fragmentTypes.listoftasks);
                ((MainActivity) Activity).ListOfProjectsCallback(projectId);
            };
            Debug.WriteLine("We are now assigning values to the list view");
            GetData(listView, ((MainActivity) Activity).Ctrl);
        }

        private async void GetData(ListView listView, Controller ctrl)
        {
            try
            {
                var output = await ctrl.GetProjects(Settings.GetInstance().Dataset);

                var listAdapter = new ProjectsAdapter(Activity, Android.Resource.Layout.SimpleListItem1,
                    output.ToArray());

                listView.Adapter = listAdapter;
                SetListShown(true);
            }
            catch (CannotReachServerException)
            {
            }
            catch (CancelTimeLoggingException)
            {
            }
            catch (StatusNotOkayException)
            {
            }
            catch (Exception)
            {
                // For any other weird exceptions
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void LoadDummyData(ListView listView)
        {
            string[] items = {"Sample Project", "Linux Kernel", "Windows 11 Ultimate", "Mobile Process Dashboard"};
            ArrayAdapter listAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, items);
            listView.Adapter = listAdapter;
        }
    }

    public class ProjectsAdapter : ArrayAdapter
    {
        private Project[] _projectList;

        public ProjectsAdapter(Context context, int resource, Project[] objects) : base(context, resource, objects)
        {
            _projectList = objects;
        }

        public Project GetProject(int position)
        {
            return _projectList[position];
        }
    }
}