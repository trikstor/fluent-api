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
        private int? TrimmedLength = null;

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

        public PrintingConfig<TOwner> AddCustomPrinter(string propName, Delegate func)
        {
            CustomPropPrinters.Add(propName, func);
            return this;
        }
        
        public PrintingConfig<TOwner> AddCustomCultureForType(Type type, CultureInfo culture)
        {
            CustomCultures.Add(type, culture);
            return this;
        }

        public PrintingConfig<TOwner> AddTrimmedLength(int trimmedLength)
        {
            TrimmedLength = trimmedLength;
            return this;
        }
        
        public PrintingConfig<TOwner> ExcludeType<TPropType>()
        {
            this.ExcludingTypes.Add(typeof(TPropType));
            return this;
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
            this.ExcludingPropertyNames.Add(propInfo.Name);
            
            return this;
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
                typeof(bool),
                typeof(DateTime), 
                typeof(TimeSpan)
            };
            
            if (finalTypes.Contains(obj.GetType()))
            {
                if (obj is string str && TrimmedLength.HasValue)
                {
                    if(TrimmedLength.Value > 0 && TrimmedLength.Value <= str.Length)
                        return str.Substring(0, TrimmedLength.Value) + Environment.NewLine;
                }
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
                
                var propValue = propertyInfo.GetValue(obj);
                
                if (CustomPropPrinters.ContainsKey(propertyInfo.Name))
                    propValue = CustomPropPrinters[propertyInfo.Name].DynamicInvoke(propValue);

                if (CustomTypePrinters.ContainsKey(propertyInfo.PropertyType))
                    propValue = CustomTypePrinters[propertyInfo.PropertyType].DynamicInvoke(propValue);

                if (CustomCultures.ContainsKey(propertyInfo.GetType()))
                    propValue = ((IFormattable) propertyInfo.GetValue(obj))
                        .ToString(null, CustomCultures[propertyInfo.PropertyType]);

                sb.Append(PrintProperty(propertyInfo, propValue, identation, nestingLevel));
            }

            return sb.ToString();
        }

        private string PrintProperty(PropertyInfo propertyInfo, object obj, string identation, int nestingLevel)
        {
            return $"{identation}{propertyInfo.Name} == {PrintToString(obj, nestingLevel + 1)}";
        }
    }
}