using System.Globalization;

namespace ObjectPrinting
{
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