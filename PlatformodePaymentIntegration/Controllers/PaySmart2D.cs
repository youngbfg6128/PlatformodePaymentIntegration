using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Mvc;
using PlatformodePaymentIntegration.Contract.Request;
using PlatformodePaymentIntegration.Contract.Response;
using PlatformodePaymentIntegration.Generate;
using PlatformodePaymentIntegration.Settings;
using PlatformodePaymentIntegration;


namespace PlatformodePaymentIntegration.Controllers
{
    public class PaySmart2DController : Controller
    {
        private const string URL = "api/paySmart2D";
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public PaySmart2DController()
        {
            _httpClient = new HttpClient();
            _apiSettings = new ApiSettingConfiguration().Configuration();
        }

        public async Task<ActionResult> Index()
        {
            var paySmart2DRequest = CreateRequestParameter(_apiSettings);
            var response = await GetAsync();

            ViewBag.RequestData = paySmart2DRequest;
            ViewBag.ResponseData = response;

            return View();
        }

        private async Task<PaySmart2DResponse> GetAsync()
        {
            var tokenResponse = await new TokenApi().GetAsync();
            var paySmart2DRequest = CreateRequestParameter(_apiSettings);
            var jsonRequest = JsonSerializer.Serialize(paySmart2DRequest);

            var httpContent = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse?.data?.token);

            try
            {
                var httpResponse = await _httpClient.PostAsync($"{_apiSettings.BaseAddress}{URL}", httpContent);
                var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PaySmart2DResponse>(jsonResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Hata: {ex.Message}");
            }
        }

        private PaySmart2DRequest CreateRequestParameter(ApiSettings apiSettings)
        {
            var paySmart2DRequest = new PaySmart2DRequest
            {
                cc_holder_name = "Test kart",
                cc_no = "4132260000000003",
                expiry_month = "12",
                expiry_year = "2024",
                cvv = "555",
                currency_code = "TRY",
                installments_number = 1,
                invoice_id = InvoiceGenerator.GenerateInvoiceId(),
                invoice_description = "INVOICE TEST DESCRIPTION",
                total = 16.43,
                items = new List<Item2D>
                {
                    new Item2D { name = "item", price = 16.43, quantity = 1, description = "item description bilal" }
                },
                name = "John",
                surname = "Dao",
                merchant_key = apiSettings.MerchantKey
            };

            var hashGenerator = new HashGenerator();
            paySmart2DRequest.hash_key = hashGenerator.GenerateHashKey(
                false,
                paySmart2DRequest.total.ToString().Replace(",", "."),
                paySmart2DRequest.installments_number.ToString(),
                paySmart2DRequest.currency_code,
                paySmart2DRequest.merchant_key,
                paySmart2DRequest.invoice_id);

            return paySmart2DRequest;
        }
    }
}
