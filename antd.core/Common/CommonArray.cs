using System;

namespace antd.core {
    public class CommonArray {
        public static T[] Merge<T>(T[] array1, T[] array2) {
            T[] newArray = new T[array1.Length + array2.Length];
            Array.Copy(array1, newArray, array1.Length);
            Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
            return newArray;
        }

        public static T[] Merge<T>(T[] array1, T[] array2, T[] array3) {
            T[] newArray1 = Merge(array1, array2);
            T[] newArray2 = Merge(newArray1, array3);
            return newArray2;
        }

        public static T[] Merge<T>(T[] array1, T[] array2, T[] array3, T[] array4) {
            T[] newArray1 = Merge(array1, array2);
            T[] newArray2 = Merge(array3, array4);
            T[] newArray3 = Merge(newArray1, newArray2);
            return newArray3;
        }

        public static string[] ToString<T>(T[] array1) {
            var array = new string[array1.Length];
            for (var i = 0; i < array1.Length; i++) {
                array[i] = array1[i].ToString();
            }
            return array;
        }
    }
}
