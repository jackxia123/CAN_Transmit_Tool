using RBT.Universal;
using RBT.Universal.Keywords;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TDAPIOLELib;

namespace RBT.QcUtility
{
    [DataContract]
    public class QcImporter : Model
    {
        private string _domain = "";
        private string _project = "";
        ObservableCollection<TestScript> _test2Import = new ObservableCollection<TestScript>();

        public string UserName { get; set; }
        public string ServerURL { get; set; }
        public string Password { get; set; }

        public string Domain
        {
            get { return _domain; }
            set { SetProperty(ref _domain, value); OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("AuthrizedProjects")); }
        }

        public string Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }

        private static TDConnection _tdOLE = new TDConnection();

        public QcImporter()
        {
            ServerURL = @"http://fe-hpalm12.de.bosch.com/qcbin";
            UserName = Environment.UserName;
            TdOLE.InitConnectionEx(ServerURL);

        }
        public TDConnection TdOLE
        {
            get { return _tdOLE; }

        }

        public ObservableCollection<string> AuthrizedDomains
        {
            get
            {
                if (IsLoggedIn == false) return null;

                ObservableCollection<string> _tempPrjList = new ObservableCollection<string>();
                foreach (var prj in TdOLE.VisibleDomains)
                {
                    _tempPrjList.Add(prj.ToString());

                }
                return _tempPrjList;

            }
            set { }

        }

        public ObservableCollection<string> AuthrizedProjects
        {
            get
            {
                if (IsLoggedIn == false) return null;

                ObservableCollection<string> _tempPrjList = new ObservableCollection<string>();
                foreach (var prj in TdOLE.VisibleProjects[Domain])
                {
                    _tempPrjList.Add(prj.ToString());

                }
                return _tempPrjList;

            }
            set { }


        }
        public void ConnectProject(string pwd)
        {

            TdOLE.ConnectProjectEx(Domain, Project, UserName, pwd);

            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsConnected"));

        }

        public void Disconnect()
        {

            if (IsConnected)
            {
                TdOLE.Disconnect();
            }

            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsConnected"));


        }

        public void Login(string pwd)
        {

            TdOLE.Login(UserName, pwd);
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("AuthrizedDomains"));
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsLoggedIn"));

        }

        public void Logout()
        {
            if (IsLoggedIn) { TdOLE.Logout(); }

            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsLoggedIn"));
        }

        public void ReleaseConnection()
        {
            TdOLE.ReleaseConnection();
        }

        public bool IsLoggedIn
        {

            get { return TdOLE.LoggedIn; }

        }

        public bool IsConnected
        {

            get { return TdOLE.Connected; }

        }

        public ObservableCollection<TestScript> Test2Import
        {
            get { return _test2Import; }
            set { SetProperty(ref _test2Import, value); }
        }

        public void ImportQC(TestScript ts)
        {
            TreeManager treeM = TdOLE.TreeManager;
            SubjectNode node = treeM.get_NodeByPath("Subject") as SubjectNode;

            SubjectNode TargetNode = (SubjectNode)GetTargetNode(node, ts.QcFolderPath);

            TestFactory TstFac = TargetNode.TestFactory;

            List existingTests = TstFac.NewList("");

            bool exists = false;
            Test tarTest = null;
            foreach (Test tst in existingTests)
            {
                if (tst.Name == ts.TestCaseName)
                {
                    exists = true;
                    tarTest = tst;
                    continue;
                }
            }



            if (exists) // checkout and update
            {
                VCS vcs = tarTest.VCS;
                if (vcs.IsLocked)
                {
                    vcs.UndoCheckout(true);
                    vcs.CheckOut("-1", "Test Update " + DateTime.Now.ToString(), true);
                }
                else
                {

                    vcs.CheckOut("-1", "Test Update " + DateTime.Now.ToString(), true);
                }



                AssignValue2Test(tarTest, ts);
                vcs.CheckIn("", "");
                tarTest.Post();

            }
            else   // new creation
            {

                tarTest = TstFac.AddItem(DBNull.Value);
                AssignValue2Test(tarTest, ts);
                tarTest.Post();

            }

        }

        private void AssignValue2Test(Test qcTest, TestScript test)
        {

            qcTest["TS_USER_01"] = "other";
            qcTest["TS_USER_02"] = test.DoorsLink;
            qcTest["TS_NAME"] = test.TestCaseName; // Todo: to create a TestCase class to wrap TestScript class, to be improved
            qcTest["TS_USER_06"] = test.Name;
            qcTest["TS_USER_15"] = "released";

            //todo it is only for Regression test
            qcTest["TS_USER_03"] = test.CAT;
            /*
            qcTest["TS_USER_32"] = "x";
            qcTest["TS_USER_33"] = "x";
            qcTest["TS_USER_34"] = "x";
            qcTest["TS_USER_35"] = "x";
            qcTest["TS_USER_36"] = "x";
            */
            qcTest["TS_DESCRIPTION"] = test.TestDescription;
            qcTest["TS_USER_26"] = test.ExpectedResult;


        }

        private ISysTreeNode GetTargetNode(SubjectNode subjectNode, string folderPath)
        {
            string[] nodes = folderPath.Split(System.Char.Parse(@"\"));
            ISysTreeNode root = subjectNode;

            foreach (var node in nodes)
            {
                List SubNodes = root.NewList();
                bool exists = false;

                foreach (SubjectNode sub in SubNodes)
                {
                    if (sub.Name == node)
                    {
                        exists = true;
                        continue;

                    }

                }



                if (!exists) //not exists
                {
                    root = root.AddNode(node);
                    root.Post();

                }
                else
                {
                    root = root.FindChildNode(node);

                }
            }

            return root;


        }

    }
}
