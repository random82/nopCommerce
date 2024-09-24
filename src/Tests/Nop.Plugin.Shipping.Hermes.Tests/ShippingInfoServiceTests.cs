using DocumentFormat.OpenXml.Wordprocessing;
using System;
using Moq;
using Nop.Core;
using Nop.Plugin.Shipping.Hermes.Models;
using Nop.Plugin.Shipping.Hermes.Services;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Shipping.Hermes.Tests;

public class ShippingInfoServiceTests
{
    [Fact]
    public async void Test1Async()
    {
        var orderGuid = Guid.Parse("8B3809EA-A9B7-4553-B4E6-50FC8ED269FE");
        var shippingAddressId = 5678;
        var orderResult = new Order
        {
            ShippingAddressId = shippingAddressId
        };

        var orderService = Mock.Of<IOrderService>();
        Mock.Get(orderService).Setup(os => os.GetOrderByGuidAsync(orderGuid)).ReturnsAsync(orderResult);

        var countryService = Mock.Of<ICountryService>();
        Mock.Get(countryService).Setup(cs => cs.GetAllCountriesForShippingAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(
            new List<Country>
            {
                new Country
                {
                    Id = 101,
                    AllowsShipping = true,
                    Name = "Australia",
                    Published = true,
                },
                new Country
                {
                    Id = 102,
                    AllowsShipping = true,
                    Name = "New Zealand",
                    Published = true,
                }
            });

        var stateProvinceService = Mock.Of<IStateProvinceService>();
        Mock.Get(stateProvinceService).Setup(sps => sps.GetStateProvincesAsync(It.IsAny<bool>())).ReturnsAsync(
            new List<StateProvince>
            {
                new StateProvince
                {
                    CountryId = 101,
                    Abbreviation = "NSW",
                    Id = 201,
                    Name = "New South Wales"
                },
                new StateProvince
                {
                    CountryId = 101,
                    Abbreviation = "QLD",
                    Id = 202,
                    Name = "Queensland"
                }
            });

        var addressService = Mock.Of<IAddressService>();

        Mock.Get(addressService).Setup(ads => ads.GetAddressByIdAsync(shippingAddressId)).ReturnsAsync(
            new Address
            {
               Id = shippingAddressId
            });

        var service = new ShippinInfoService(
            orderService,
            addressService,
            stateProvinceService,
            countryService);

        var shippingInfoModel = new ShippingCallbackModel()
        {
            Address1 = "My Address 1",
            Address2 = "My Address 2",
            City = "My City",
            Country = "Australia",
            PostalCode = "2000",
            State = "NSW",
            OrderPlacedId = orderGuid,
            DeliveryNotes = "Notes"
        };

        await service.UpdateShippingInfoAsync(shippingInfoModel);
        Mock.Get(addressService).Verify(ads => ads.UpdateAddressAsync(It.Is<Address>(addr => 
            addr.Address1 == "My Address 1"
            && addr.Address2 == "My Address 2"
            && addr.City == "My City"
            && addr.County == "Australia"
            && addr.ZipPostalCode == "2000"
            && addr.CountryId == 101
            && addr.StateProvinceId == 201
            //&& addr.Email == "gift-recipient@test.com"
            //&& addr.FirstName == "Space"
            //&& addr.LastName == "Karen"
            )
        ));
    }
}