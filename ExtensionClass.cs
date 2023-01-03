using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPartNumber
{
    public static class ExtensionClass
    {
        public static List<T> ToTeklaObjectList<T>(this IEnumerator enumerator)
        {
            var list = new List<T>();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is T)
                {
                    list.Add((T)item);
                }
            }
            return list;
        }
    }
}
