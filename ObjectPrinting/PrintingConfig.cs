using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using FluentAssertions;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        public List<string> ExcludingPropertyNames;
        public List<Type> ExcludingTypes;
        public Dictionary<Type, Delegate> CustomTypePrinting;
        public Dictionary<string, Delegate> CustomPropPrinting;
        public Dictionary<Type, CultureInfo> CustomCultures;
        public int TrimLength = 0;

        public PrintingConfig()
        {
            ExcludingPropertyNames = new List<string>();
            ExcludingTypes = new List<Type>();
            CustomCultures = new Dictionary<Type, CultureInfo>();
            CustomPropPrinting = new Dictionary<string, Delegate>();
            CustomTypePrinting = new Dictionary<Type, Delegate>();
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
            if (obj == null)
                return "null" + Environment.NewLine;

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
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
                
                sb.Append(PropertyPrinting(obj, propertyInfo, nestingLevel, identation));
            }

            return sb.ToString();
        }

        private string PropertyPrinting(object obj, PropertyInfo propInfo, int nestingLevel, string identation)
        {
            if(CustomPropPrinting.ContainsKey(propInfo.Name))
                return identation + propInfo.Name + " == " +
                       CustomPropPrinting[propInfo.Name].DynamicInvoke(propInfo.GetValue(obj)) + '\n'.ToString();
            
            if(CustomTypePrinting.ContainsKey(propInfo.PropertyType))
                return identation + propInfo.Name + " == " +
                       CustomTypePrinting[propInfo.PropertyType].DynamicInvoke(propInfo.GetValue(obj)) + '\n'.ToString();

            if (!CustomCultures.ContainsKey(propInfo.GetType()))
                return SimplePropertyPrinting(propInfo, identation, nestingLevel, obj);
            
            var culturicalProp = ((IFormattable) propInfo.GetValue(obj))
                .ToString(null, CustomCultures[propInfo.PropertyType]);
            return identation + propInfo.Name + " == " + culturicalProp + '\n'.ToString();
        }

        private string PropertyPrintWithCulture(object obj, PropertyInfo propInfo, int nestingLevel, string identation)
        {
            if (!CustomCultures.ContainsKey(propInfo.GetType()))
                return SimplePropertyPrinting(propInfo, identation, nestingLevel, obj);
            
            var culturicalProp = ((IFormattable) propInfo.GetValue(obj))
                .ToString(null, CustomCultures[propInfo.PropertyType]);
            return identation + propInfo.Name + " == " + culturicalProp + '\n'.ToString();
        }

        private string SimplePropertyPrinting(PropertyInfo propertyInfo, string identation, int nestingLevel, object obj)
        {
            return identation + propertyInfo.Name + " == " +
                PrintToString(propertyInfo.GetValue(obj),
                    nestingLevel + 1);
        }
    }
}