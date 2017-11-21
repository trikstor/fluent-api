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
            var printingConfig = ((IPropertyPrintingConfig<TOwner, double>) propertyPrintingConfig).PrintingConfig;
            printingConfig.AddCustomCultureForType(typeof(double), cultureInfo);
            
            return printingConfig;
        }
        
        public static PrintingConfig<TOwner> TrimTo<TOwner>(this PropertyPrintingConfig<TOwner, string> propertyPrintingConfig, int trimLength)
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, string>) propertyPrintingConfig).PrintingConfig;
            printingConfig.TrimLength = trimLength;
            
            return printingConfig;
        }
    }
    
}