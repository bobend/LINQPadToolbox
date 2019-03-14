using System;

namespace Bobend.LINQPadToolbox
{
    public static class Extensions
    {
        public static T ToEnum<T>(this string value) => (T)Enum.Parse(typeof(T), value);
        public static void UseAsArgsIn<T1, T2>(this ValueTuple<T1, T2> v, Action<T1, T2> a) => a(v.Item1, v.Item2);
        public static void UseValueTuple<T1, T2>(this Action<T1, T2> a, ValueTuple<T1, T2> v) => a(v.Item1, v.Item2);
        public static DateTime Epoch = new DateTime(1970, 1, 1);
        public static int ToSecsSinceEpoch(this DateTime x) => (int)(x.ToUniversalTime() - Epoch).TotalSeconds;
    }
}
