using System;

namespace ObjectPrinting
{
    public static class ObjectExtensions
    {   
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> printingConfigFunc)
        {
            return ObjectPrinter.For<T>().PrintToString(obj);
        }
    }
}