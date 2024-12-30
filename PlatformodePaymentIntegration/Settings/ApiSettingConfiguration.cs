using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformodePaymentIntegration.Contract.Response;
using System;
using System.Text.RegularExpressions;

namespace PlatformodePaymentIntegration.Settings
{
    public class ApiSettingConfiguration
    {
        public ApiSettings Configuration()
        {
            IHost host = Host.CreateDefaultBuilder(null).Build();
            using (host)
            {
                IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
          
                var app_id = "a2a7cb8131890a35353629182274c756";
                var app_secret = "a9b5c28211388c7a6a705122a24e6f46";
                var base_address = "https://testapp.platformode.com.tr/ccpayment/";
                var merchant_key = "$2y$10$e8Mzadv9523RVrAqL7K3.efxdERBrbaVZNaJ3AwlmZriYNrygSH72";

                if (string.IsNullOrWhiteSpace(app_id))
                    throw new ArgumentException("app_id bilgisi bulunmadı. Lütfen appsettings.json dosyasındaki bilgileri kontrol ediniz.");

                if (string.IsNullOrWhiteSpace(app_secret))
                    throw new ArgumentException("app_secret bilgisi bulunmadı. Lütfen appsettings.json dosyasındaki bilgileri kontrol ediniz.");

                var checkBaseAddress = IsValidURL(base_address);
                if (!checkBaseAddress)
                    throw new ArgumentException("base_address bilgisi bulunmadı. Lütfen appsettings.json dosyasındaki bilgileri kontrol ediniz.");

                if (!base_address.EndsWith("/"))
                    base_address += "/";

                if (string.IsNullOrWhiteSpace(merchant_key))
                    throw new ArgumentException("merchant_key bilgisi bulunmadı. Lütfen appsettings.json dosyasındaki bilgileri kontrol ediniz.");

                return new ApiSettings
                {
                    AppId = app_id,
                    AppSecret = app_secret,
                    BaseAddress = base_address,
                    MerchantKey = merchant_key
                };
            }
        }

        bool IsValidURL(string url)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(url);
        }
    }
}
