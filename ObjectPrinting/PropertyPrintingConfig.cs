using System;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }
        
        public PrintingConfig<TOwner> Using(Func<TPropType, string> serializeFunc)
        {
            var config = new PrintingConfig<TOwner>();
            config.CustomPrinting = serializeFunc;
            return config;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>
            .PrintingConfig => printingConfig;
    }

}