using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ObjectPrinting
{
    public class Printers
    {
        private readonly Dictionary<Type, Delegate> CustomTypePrinters;
        private readonly Dictionary<string, Delegate> CustomPropPrinters;
        private readonly Dictionary<Type, CultureInfo> CustomCultures;

        public Printers(Dictionary<Type, Delegate> customTypePrinters,
            Dictionary<string, Delegate> customPropPrinters, Dictionary<Type, CultureInfo> customCultures)
        {
            CustomTypePrinters = customTypePrinters;
            CustomPropPrinters = customPropPrinters;
            CustomCultures = customCultures;
        }
        
        public string PrintProperty(object obj, PropertyInfo propInfo, int nestingLevel, string identation)
        {
            if (CustomPropPrinters.ContainsKey(propInfo.Name))
                return 
                    $"{identation}{propInfo.Name} == " +
                    $"{CustomPropPrinters[propInfo.Name].DynamicInvoke(propInfo.GetValue(obj))}{Environment.NewLine}";

            if (CustomTypePrinters.ContainsKey(propInfo.PropertyType))
                return 
                    $"{identation}{propInfo.Name} == " +
                    $"{CustomTypePrinters[propInfo.PropertyType].DynamicInvoke(propInfo.GetValue(obj))}{Environment.NewLine}";

            if (!CustomCultures.ContainsKey(propInfo.GetType()))
                return null;
            
            var culturicalProp = ((IFormattable) propInfo.GetValue(obj))
                .ToString(null, CustomCultures[propInfo.PropertyType]);
            return $"{identation}{propInfo.Name} == {culturicalProp}{Environment.NewLine}";
        }
    }
}