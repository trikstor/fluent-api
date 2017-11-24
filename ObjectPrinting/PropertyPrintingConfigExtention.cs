using System;
using System.Globalization;
using FluentAssertions;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtention
    {
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, int> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, int>) propertyPrintingConfig).PrintingConfig;
            return printingConfig.AddCustomCultureForType(typeof(int), cultureInfo);
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, double> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            return (propertyPrintingConfig as IPropertyPrintingConfig<TOwner, double>).PrintingConfig
                .AddCustomCultureForType(typeof(double), cultureInfo);
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, float> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, float>) propertyPrintingConfig).PrintingConfig;
            return printingConfig.AddCustomCultureForType(typeof(float), cultureInfo);
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, long> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            return (propertyPrintingConfig as IPropertyPrintingConfig<TOwner, long>).PrintingConfig
                .AddCustomCultureForType(typeof(long), cultureInfo);
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, DateTime> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            return (propertyPrintingConfig as IPropertyPrintingConfig<TOwner, DateTime>).PrintingConfig
                .AddCustomCultureForType(typeof(DateTime), cultureInfo);
        }
        
        public static PrintingConfig<TOwner> TrimTo<TOwner>(this PropertyPrintingConfig<TOwner, string> propertyPrintingConfig, int trimmedLength)
        {
            var printingConfig = (propertyPrintingConfig as IPropertyPrintingConfig<TOwner, string>).PrintingConfig;
            return printingConfig.AddTrimmedLength(trimmedLength);
        }
    }
    
}