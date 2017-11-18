using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> PrintingConfig {get;}
    }

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

        public PrintingConfig<TOwner, TPropType> ExcludeProperty<TPropType>(Expression<Func<TOwner, TPropType>> serializeFunc)
        {
            return this;
        }
        
        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> selector)
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            //TODO apply configurations
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
            {
                sb.Append(identation + propertyInfo.Name + " = " +
                          PrintToString(propertyInfo.GetValue(obj),
                              nestingLevel + 1));
            }
            return sb.ToString();
        }
    }

    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }
        
        public PrintingConfig<TOwner> Using(Func<TPropType, string> serializeFunc)
        {
            return printingConfig;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>
            .PrintingConfig => printingConfig;
    }

    public static class PropertyPrintingConfigExtention
    {
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, int> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            return ((IPropertyPrintingConfig<TOwner, int>) propertyPrintingConfig).PrintingConfig;
        }
        
        public static PrintingConfig<TOwner> TrimTo<TOwner>(this PropertyPrintingConfig<TOwner, string> propertyPrintingConfig, int trimLength)
        {
            return ((IPropertyPrintingConfig<TOwner, string>) propertyPrintingConfig).PrintingConfig;
        }
    }
}