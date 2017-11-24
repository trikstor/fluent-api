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
            if (string.IsNullOrEmpty(propertyName))
                return printingConfig.AddCustomPrinter<TPropType>(serializeFunc);
            return printingConfig.AddCustomPrinter(propertyName, serializeFunc);
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>
            .PrintingConfig => printingConfig;
    }

}