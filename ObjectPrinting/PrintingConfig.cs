using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        public PrintingConfig<TOwner> ExcludeType<TPropType>()
        {
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
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyInfo ExcludingProperty = null;
        
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
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
                sb.Append(GetPropertiesWithoutExcluded(propertyInfo, ExcludingProperty, identation, nestingLevel, obj));
            
            return sb.ToString();
        }

        private string GetPropertiesWithoutExcluded(PropertyInfo propertyInfo, PropertyInfo excludingProperty, string identation, int nestingLevel, object obj)
        {
            var sb = new StringBuilder();
            if (excludingProperty != null)
            {
                if (excludingProperty.Name != propertyInfo.Name)
                {
                    sb.Append(identation + propertyInfo.Name + " == " +
                              PrintToString(propertyInfo.GetValue(obj),
                                  nestingLevel + 1));
                }
            }
            else
            {
                sb.Append(identation + propertyInfo.Name + " = " +
                          PrintToString(propertyInfo.GetValue(obj),
                              nestingLevel + 1));
            }
            return sb.ToString();
        }
    }
}