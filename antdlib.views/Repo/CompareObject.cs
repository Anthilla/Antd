using System;

namespace antdlib.views.Repo {
    public static class CompareObject {
        public static T UpdatePropertiesOf<T>(this object obj, T objectToUpdate) where T : EntityModel, new() {
            if (ReferenceEquals(obj, objectToUpdate)) return objectToUpdate;
            if ((obj == null) || (objectToUpdate == null)) return objectToUpdate;
            if (obj.GetType() != objectToUpdate.GetType()) return objectToUpdate;
            foreach (var property in obj.GetType().GetProperties()) {
                var objValue = property.GetValue(obj);
                if (objValue == null) continue;
                var propertyInfo = objectToUpdate.GetType().GetProperty(property.Name);
                Console.WriteLine($"update {property.Name}");
                propertyInfo.SetValue(objectToUpdate, objValue);
            }
            return objectToUpdate;
        }

        public static bool IsNullOrEmpty(this string s) {
            if (s == null) return true;
            return s != "NULL" && string.IsNullOrEmpty(s.Trim());
        }

        public static bool ToBoolean(this bool? b) {
            return b ?? false;
        }
    }
}
