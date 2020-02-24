using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Library
{
    public static class Extensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        public static T ToEnum<T>(this string target)
        {
            if (!(typeof(T).IsEnum))
                throw new ArgumentException("The given type is not of type enum");

            if (Enum.TryParse(typeof(T), target, out var result))
                return (T)result;
            else
                throw new ArgumentException("The string " + target + "could not be converted to the target enum");
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            if (enumerableList != null)
            {
                var observableCollection = new ObservableCollection<T>();

                foreach (var item in enumerableList)
                    observableCollection.Add(item);

                return observableCollection;
            }
            return null;
        }
    }
}
