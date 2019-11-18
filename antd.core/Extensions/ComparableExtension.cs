using System;
namespace anthilla.core {
    public static class ComparableExtension {
        public static bool IsWithin<T>(this T value, T minimum, T maximum) where T : IComparable<T> {
            if(value.CompareTo(minimum) < 0)
                return false;
            if(value.CompareTo(maximum) > 0)
                return false;
            return true;
        }
    }
}
