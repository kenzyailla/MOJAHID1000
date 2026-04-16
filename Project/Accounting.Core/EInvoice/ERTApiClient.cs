using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accounting.Core.EInvoice
{
    public static class ERTApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<InvoiceResponse> SendXmlToERTAsync(string xmlContent)
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            string clientId = EInvoiceConfig.ClientId;
            string secretKey = EInvoiceConfig.SecretKey;
            string apiUrl = EInvoiceConfig.ApiUrl;

            // تجنب BOM في الترميز
            byte[] xmlBytes = new UTF8Encoding(false).GetBytes(xmlContent);
            string base64Xml = Convert.ToBase64String(xmlBytes);

            var requestData = new { invoice = base64Xml };
            string json = JsonConvert.SerializeObject(requestData);

            // إعادة تعيين الهيدرات (للتأكد من عدم تراكم قيم سابقة)
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-Id", clientId);
            _httpClient.DefaultRequestHeaders.Add("Secret-Key", secretKey);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
            string responseText = await response.Content.ReadAsStringAsync();

            JObject jsonResult = null;
            try
            {
                jsonResult = JObject.Parse(responseText);
            }
            catch { /* إذا لم يكن JSON صالح، نكمل بدونه */ }

            if (response.IsSuccessStatusCode)
            {
                return new InvoiceResponse
                {
                    Status = jsonResult?["EINV_STATUS"]?.ToString() ?? response.StatusCode.ToString(),
                    Uuid = jsonResult?["EINV_INV_UUID"]?.ToString(),
                    InvoiceNumber = jsonResult?["EINV_NUM"]?.ToString(),
                    QrCode = jsonResult?["EINV_QR"]?.ToString(),
                    SignedInvoiceBase64 = jsonResult?["EINV_SINGED_INVOICE"]?.ToString(),
                    Message = "تم الإرسال بنجاح",
                    FullResponse = responseText
                };
            }
            else
            {
                return new InvoiceResponse
                {
                    Status = "FAILED",
                    Message = $"خطأ في الإرسال: {response.StatusCode} - {responseText}",
                    FullResponse = responseText
                };
            }

        }
        public static string DecodeBase64ToString(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return string.Empty;
            byte[] data = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(data);
        }
    }

   
    }



