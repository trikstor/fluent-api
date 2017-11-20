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
        public PrintingConfig<TOwner> ExcludeType<TPropType>()
        {
            var printingConfig = new PrintingConfig<TOwner>();
            printingConfig.ExcludingType = typeof(TPropType);
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
            newPrintingConfig.ExcludingProperty = propInfo;
            
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

        public PropertyInfo ExcludingProperty = null;
        public Type ExcludingType = null;
        public Delegate CustomPrinting = null;
        public Type CustomPrintingType = null;
        public string CustomPrintingPropertyName = null;
        public int TrimLength = 0;
        public CultureInfo CustomCulture = null;
        public Type CustomCultureType = null;
        
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
                if (obj is string str && TrimLength != null)
                    return str.Substring(0, str.Length - TrimLength) + Environment.NewLine;
                return obj + Environment.NewLine;
            }

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (ExcludingType == null || ExcludingType != propertyInfo.PropertyType)
                {
                    if (ExcludingProperty == null || ExcludingProperty.Name != propertyInfo.Name)
                    {
                        if (propertyInfo.PropertyType == CustomPrintingType ||
                            propertyInfo.Name == CustomPrintingPropertyName)
                        {
                            sb.Append(CustomPropertyPrint(obj, propertyInfo, CustomPrinting, identation));
                        }
                        else if (CustomCultureType == propertyInfo.PropertyType)
                        {
                            sb.Append(PropertyPrintWithCulture(obj, propertyInfo, CustomCulture, identation));
                        }
                        else
                        {
                            sb.Append(PrintProperty(propertyInfo, identation, nestingLevel, obj));
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private string CustomPropertyPrint(object obj, PropertyInfo propInfo, Delegate customPrinting, string identation)
        {
            return identation + propInfo.Name + " == " +
                   customPrinting.DynamicInvoke(propInfo.GetValue(obj)) + '\n'.ToString();
        }

        private string PropertyPrintWithCulture(object obj, PropertyInfo propInfo, CultureInfo culture, string identation)
        {
            var culturicalProp = ((IFormattable) propInfo.GetValue(obj)).ToString(null, CultureInfo.CurrentCulture);
            culturicalProp = identation + propInfo.Name + " == " + culturicalProp + '\n'.ToString();
            return culturicalProp;
        }

        private string PrintProperty(PropertyInfo propertyInfo, string identation, int nestingLevel, object obj)
        {
            return identation + propertyInfo.Name + " == " +
                PrintToString(propertyInfo.GetValue(obj),
                    nestingLevel + 1);
        }
    }
}