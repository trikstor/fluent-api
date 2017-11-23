using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private List<string> ExcludingPropertyNames;
        private List<Type> ExcludingTypes;
        private Dictionary<Type, Delegate> CustomTypePrinters;
        private Dictionary<string, Delegate> CustomPropPrinters;
        private Dictionary<Type, CultureInfo> CustomCultures;
        private int TrimLength = 0;

        public PrintingConfig()
        {
            ExcludingPropertyNames = new List<string>();
            ExcludingTypes = new List<Type>();
            CustomCultures = new Dictionary<Type, CultureInfo>();
            CustomPropPrinters = new Dictionary<string, Delegate>();
            CustomTypePrinters = new Dictionary<Type, Delegate>();
        }

        public PrintingConfig<TOwner> AddCustomPrinter<TPropType>(Delegate func)
        {
            CustomTypePrinters.Add(typeof(TPropType), func);
            return this;
        }

        public void AddCustomPrinter(string propName, Delegate func)
        {
            CustomPropPrinters.Add(propName, func);
        }
        
        public PrintingConfig<TOwner> AddCustomCultureForType(Type type, CultureInfo culture)
        {
            CustomCultures.Add(type, culture);
            return this;
        }

        public PrintingConfig<TOwner> AddTrimLength(int trimLength)
        {
            TrimLength = trimLength;
            return this;
        }
        
        public PrintingConfig<TOwner> ExcludeType<TPropType>()
        {
            var printingConfig = new PrintingConfig<TOwner>();
            printingConfig.ExcludingTypes.Add(typeof(TPropType));
            return printingConfig;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        public PrintingConfig<TOwner> GetOtherSerialize<T>(T type, Func<T, string> func)
        {
            return this;
        }

        public PrintingConfig<TOwner> ExcludeProperty<TPropType>(Expression<Func<TOwner, TPropType>> serializeFunc)
        {
            var propInfo =
                ((MemberExpression) serializeFunc.Body)
                .Member as PropertyInfo;

            var newPrintingConfig = new PrintingConfig<TOwner>();
            newPrintingConfig.ExcludingPropertyNames.Add(propInfo.Name);
            
            return newPrintingConfig;
        }
        
        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> selector)
        {
            var propInfo =
                ((MemberExpression) selector.Body)
                .Member as PropertyInfo;
            return new PropertyPrintingConfig<TOwner, TPropType>(this, propInfo.Name);
        }
        
        private string PrintToString(object obj, int nestingLevel)
        {
            var printers = new Printers(CustomTypePrinters, CustomPropPrinters, CustomCultures);
            
            if (obj == null)
                return $"null {Environment.NewLine}";

            var finalTypes = new[]
            {
                typeof(int), 
                typeof(uint),
                typeof(double), 
                typeof(float), 
                typeof(decimal),
                typeof(short),
                typeof(ushort),
                typeof(long),
                typeof(ulong),
                typeof(string),
                typeof(byte),
                typeof(sbyte),
                typeof(DateTime), 
                typeof(TimeSpan)
            };
            
            if (finalTypes.Contains(obj.GetType()))
            {
                if (obj is string str && TrimLength != 0)
                    return str.Substring(0, str.Length - TrimLength) + Environment.NewLine;
                return obj + Environment.NewLine;
            }

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (ExcludingTypes.Contains(propertyInfo.PropertyType)) continue;
                if (ExcludingPropertyNames.Contains(propertyInfo.Name)) continue;

                var serializedProp = printers.PrintProperty(obj, propertyInfo, nestingLevel, identation);
                sb.Append(serializedProp ?? SimplePrintProperty(propertyInfo, identation, nestingLevel, obj));
            }

            return sb.ToString();
        }

        private string SimplePrintProperty(PropertyInfo propertyInfo, string identation, int nestingLevel, object obj)
        {
            return $"{identation}{propertyInfo.Name} == {PrintToString(propertyInfo.GetValue(obj), nestingLevel + 1)}";
        }
    }
}