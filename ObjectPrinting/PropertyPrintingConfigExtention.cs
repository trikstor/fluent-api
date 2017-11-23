using System.Globalization;
using FluentAssertions;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtention
    {
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, int> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, int>) propertyPrintingConfig).PrintingConfig;
            printingConfig.AddCustomCultureForType(typeof(int), cultureInfo);
            
            return printingConfig;
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, double> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            return (propertyPrintingConfig as IPropertyPrintingConfig<TOwner, double>).PrintingConfig
                .AddCustomCultureForType(typeof(double), cultureInfo);
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, float> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, float>) propertyPrintingConfig).PrintingConfig;
            printingConfig.AddCustomCultureForType(typeof(float), cultureInfo);
            
            return printingConfig;
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, long> propertyPrintingConfig, CultureInfo cultureInfo)
        {
            return (propertyPrintingConfig as IPropertyPrintingConfig<TOwner, long>).PrintingConfig
                .AddCustomCultureForType(typeof(long), cultureInfo);
        }
        
        public static PrintingConfig<TOwner> TrimTo<TOwner>(this PropertyPrintingConfig<TOwner, string> propertyPrintingConfig, int trimLength)
        {
            var printingConfig = (propertyPrintingConfig as IPropertyPrintingConfig<TOwner, string>).PrintingConfig;
            return printingConfig.AddTrimLength(trimLength);
        }
    }
    
}