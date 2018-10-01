using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using CWITC.Clients.Portable;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.CurrentActivity;
//using Microsoft.Azure.Mobile;
//using Microsoft.Azure.Mobile.Crashes;

namespace CWITC.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);

#if !DEBUG
			if (!string.IsNullOrWhiteSpace(ApiKeys.VSMobileCenterApiKeyAndroid))
			{
				Microsoft.AppCenter.AppCenter
					.Start(ApiKeys.VSMobileCenterApiKeyAndroid,
					typeof(Analytics),
					typeof(Crashes));
			}
#endif
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