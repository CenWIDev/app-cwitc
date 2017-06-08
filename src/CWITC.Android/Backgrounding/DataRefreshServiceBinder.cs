﻿using System;

using Android.OS;

namespace CWITC.Droid
{
    public class DataRefreshServiceBinder : Binder
    {
        DataRefreshService service;

        public DataRefreshServiceBinder (DataRefreshService service)
        {
            this.service = service;
        }

        public DataRefreshService GetDemoService ()
        {
            return service;
        }
    }
}