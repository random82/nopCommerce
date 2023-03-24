using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Services.Common.Pdf
{
    static partial class DocumentUtils
    {
        static public string[] FormatAddress(AddressItem address)
        {
            var format = EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync("Address.LineFormat").Result;
            var seporate = ";";
            var adressfields = new string[7];

            adressfields[0] = string.Format($"{(!string.IsNullOrEmpty(address.Country) ? $"Country: {address.Country}" : string.Empty)}{seporate}");
            adressfields[1] = string.Format($"{(!string.IsNullOrEmpty(address.StateProvinceName) ? $"State: {address.StateProvinceName}" : string.Empty)}{seporate}");
            adressfields[2] = string.Format($"{(!string.IsNullOrEmpty(address.City) ? $"City: {address.City}" : string.Empty)}{seporate}");
            adressfields[3] = string.Empty + seporate;
            adressfields[4] = string.Format($"{(!string.IsNullOrEmpty(address.Address) ? $"Address: {address.Address}" : string.Empty)}{seporate}");
            adressfields[5] = string.Format($"{(!string.IsNullOrEmpty(address.Address2) ? $"Address2: {address.Address2}" : string.Empty)}{seporate}");
            adressfields[6] = string.Format($"{(!string.IsNullOrEmpty(address.ZipPostalCode) ? $"ZipPostalCode: {address.ZipPostalCode}" : string.Empty)}{seporate}");
            var formatedLine = string.Format(format, adressfields);

            return formatedLine.Split(seporate, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
