// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEditorInternal;
using UnityEditor.Web;
using System.Text;

namespace UnityEditor.Connect
{
    internal delegate void StateChangedDelegate(ConnectInfo state);
    internal delegate void ProjectStateChangedDelegate(ProjectInfo state);
    internal delegate void UserStateChangedDelegate(UserInfo state);

    [InitializeOnLoad]
    internal partial class UnityConnect
    {
        public event StateChangedDelegate StateChanged;
        public event ProjectStateChangedDelegate ProjectStateChanged;
        public event UserStateChangedDelegate UserStateChanged;

        private static readonly UnityConnect s_Instance;

        [Flags]
        internal enum UnityErrorPriority
        {
            Critical = 0,
            Error,
            Warning,
            Info,
            None
        };

        [Flags]
        internal enum UnityErrorBehaviour
        {
            Alert = 0,
            Automatic,
            Hidden,
            ConsoleOnly,
            Reconnect
        };

        [Flags]
        internal enum UnityErrorFilter
        {
            ByContext = 1,
            ByParent  = 2,
            ByChild   = 4,
            All       = 7
        };

        private UnityConnect()
        {
            // Nothing to do
        }

        public void GoToHub(string page)
        {
            UnityEditor.Connect.UnityConnectServiceCollection.instance.ShowService(UnityEditor.Web.HubAccess.kServiceName, page, true);
        }


        public void UnbindProject()
        {
            UnbindCloudProject();
            UnityConnectServiceCollection.instance.UnbindAllServices();
        }

        // For Javascript Only
        public ProjectInfo GetProjectInfo()
        {
            return projectInfo;
        }


        public UserInfo GetUserInfo()
        {
            return userInfo;
        }

        public ConnectInfo GetConnectInfo()
        {
            return connectInfo;
        }

        public string GetConfigurationUrlByIndex(int index)
        {
            if (index == 0)
                return GetConfigurationURL(CloudConfigUrl.CloudCore);
            if (index == 1)
                return GetConfigurationURL(CloudConfigUrl.CloudCollab);
            if (index == 2)
                return GetConfigurationURL(CloudConfigUrl.CloudWebauth);
            if (index == 3)
                return GetConfigurationURL(CloudConfigUrl.CloudLogin);
            // unityeditor-cloud only called this API with index as {0,1,2,3}.
            // We add the new URLs in case some module might need them in the future
            if (index == 6)
                return GetConfigurationURL(CloudConfigUrl.CloudIdentity);
            if (index == 7)
                return GetConfigurationURL(CloudConfigUrl.CloudPortal);

            return "";
        }

        public string GetCoreConfigurationUrl()
        {
            return GetConfigurationURL(CloudConfigUrl.CloudCore);
        }

        public bool DisplayDialog(string title, string message, string okBtn, string cancelBtn)
        {
            return EditorUtility.DisplayDialog(title, message, okBtn, cancelBtn);
        }

        public bool SetCOPPACompliance(int compliance)
        {
            return SetCOPPACompliance((COPPACompliance)compliance);
        }

        // End for Javascript Only

        [MenuItem("Window/Unity Connect/Computer GoesToSleep", false, 1000, true)]
        public static void TestComputerGoesToSleep()
        {
            instance.ComputerGoesToSleep();
        }

        [MenuItem("Window/Unity Connect/Computer DidWakeUp", false, 1000, true)]
        public static void TestComputerDidWakeUp()
        {
            instance.ComputerDidWakeUp();
        }

        [MenuItem("Window/Unity Connect/Reset AccessToken", false, 1000, true)]
        public static void TestClearAccessToken()
        {
            instance.ClearAccessToken();
        }

        public static UnityConnect instance
        {
            get
            {
                return s_Instance;
            }
        }

        static UnityConnect()
        {
            s_Instance = new UnityConnect();
            JSProxyMgr.GetInstance().AddGlobalObject("unity/connect", s_Instance);
        }

        private static void OnStateChanged()
        {
            var handler = instance.StateChanged;
            if (handler != null)
                handler(instance.connectInfo);
        }

        private static void OnProjectStateChanged()
        {
            var handler = instance.ProjectStateChanged;
            if (handler != null)
                handler(instance.projectInfo);
        }

        private static void OnUserStateChanged()
        {
            var handler = instance.UserStateChanged;
            if (handler != null)
                handler(instance.userInfo);
        }
    };
}