using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using CWITC.Clients.Portable;
using Firebase;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;

namespace CWITC.Droid
{
	//You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);

			if (!string.IsNullOrWhiteSpace(ApiKeys.VSMobileCenterApiKeyAndroid) && ApiKeys.VSMobileCenterApiKeyIOS != nameof(ApiKeys.VSMobileCenterApiKeyAndroid))
			{
				MobileCenter
					.Start(ApiKeys.VSMobileCenterApiKeyAndroid,
					typeof(Com.Microsoft.Azure.Mobile.Analytics.AndroidAnalytics),
					typeof(Crashes));
			}
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity) 
        {
        } 
    }
}