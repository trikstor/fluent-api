using System;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly string propertyName;
        
        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, string propertyName = null)
        {
            this.printingConfig = printingConfig;
            this.propertyName = propertyName;
        }
        
        public PrintingConfig<TOwner> Using(Func<TPropType, string> serializeFunc)
        {
            var config = new PrintingConfig<TOwner>();
            
            if (string.IsNullOrEmpty(propertyName))
                config.AddCustomPrinter<TPropType>(serializeFunc);
            else
                config.AddCustomPrinter(propertyName, serializeFunc);
            return config;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>
            .PrintingConfig => printingConfig;
    }

}