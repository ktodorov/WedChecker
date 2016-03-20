using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace WedChecker.Extensions
{
    public static class CompositeExtensions
    {
        public static T ReadValue<T>(this ApplicationDataCompositeValue composite, string key)
        {
            if (composite.ContainsKey(key) && composite[key] is T)
            {
                return (T)composite[key];
            }

            return default(T);
        }

        //public static string ReadString(this ApplicationDataCompositeValue composite, string key)
        //{
        //    if (composite.ContainsKey(key) && composite[key] is string)
        //    {
        //        return (string)composite[key];
        //    }

        //    return default(string);
        //}

        //public static int ReadInt(this ApplicationDataCompositeValue composite, string key)
        //{
        //    if (composite.ContainsKey(key) && composite[key] is int)
        //    {
        //        return (int)composite[key];
        //    }

        //    return default(int);
        //}

        //public static bool ReadBool(this ApplicationDataCompositeValue composite, string key)
        //{
        //    if (composite.ContainsKey(key) && composite[key] is bool)
        //    {
        //        return (bool)composite[key];
        //    }

        //    return default(bool);
        //}
    }
}
