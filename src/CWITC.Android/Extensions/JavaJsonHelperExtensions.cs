using System;
using System.Linq;
using Android.Runtime;
using Java.Lang;
using Java.Util;
using Org.Json;

namespace CWITC.Droid
{
    public static class JavaJsonHelperExtensions
    {
        public static Java.Lang.Object ToJSON(Java.Lang.Object @object)
        {
            if (@object is IMap)
            {
                JSONObject json = new JSONObject();
                IMap map = (IMap)@object;
                foreach (Java.Lang.Object key in map.KeySet())
                {
                    json.Put(key.ToString(), ToJSON(map.Get(key)) as Java.Lang.Object);
                }
                return json;
            }
            else if (@object is IIterable)
            {
                JSONArray json = new JSONArray();
                var values = (@object as IIterable).ToEnumerable();
                foreach (Java.Lang.Object value in values)
                {
                    json.Put(value);
                }
                return json;
            }
            else if(@object is JSONObject || @object is JSONArray)
            {
                return @object;
            }
            else 
            {
                return JSONObject.Null;
            }
        }

        public static bool IsEmptyJsonObject(JSONObject @object)
        {
            return @object.Names() == null;
        }

        public static IMap GetJavaMap(this JSONObject @object, string key)
        {
            return ToJavaMap(@object.GetJSONObject(key));
        }

        public static IMap ToJavaMap(this JSONObject @object)
        {
            IMap map = new HashMap();
            IIterator keys = @object.Keys();
            while (keys.HasNext)
            {
                var key = ((Java.Lang.String)keys.Next()).ToString();
                map.Put(key, FromJson(@object.Get(key)));
            }
            return map;
        }

        public static ArrayList ToJavaList(this JSONArray array)
        {
            var list = new ArrayList();
            for (int i = 0; i < array.Length(); i++)
            {
                list.Add(FromJson(array.Get(i)));
            }
            return list;
        }

        static Java.Lang.Object FromJson(Java.Lang.Object json)
        {
            if (json == JSONObject.Null)
            {
                return null;
            }
            else if (json is JSONObject)
            {
                return ToJavaMap((JSONObject)json) as Java.Lang.Object;
            }
            else if (json is JSONArray)
            {
                return ToJavaList((JSONArray)json);
            }
            else
            {
                return json;
            }
        }
    }
}