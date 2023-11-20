using System.Text;
using Newtonsoft.Json;

namespace KYC.Services;

public class VeriffService : IVeriffService
{
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly string _sharedSecretKey;

        public VeriffService(IConfiguration configuration)
        {
            _baseUrl = configuration["Veriff:BaseUrl"];
            _apiKey = configuration["Veriff:ApiKey"];
            _sharedSecretKey = configuration["Veriff:SharedSecretKey"];
        }
    
        public class VerificationRequest
        {
            public Verification verification { get; set; }

            public class Verification
            {
                public string callback { get; set; }
                public Person person { get; set; }
                public Document document { get; set; }
                public Address address { get; set; }
                public string vendorData { get; set; }
            }

            public class Person
            {
                public string firstName { get; set; }
                public string lastName { get; set; }
                public string idNumber { get; set; }
            }

            public class Document
            {
                public string number { get; set; }
                public string type { get; set; }
                public string country { get; set; }
            }

            public class Address
            {
                public string fullAddress { get; set; }
            }
        }
        public class VeriffApiResponse
        {
            public string Status { get; set; }
            public VerificationRes Verification { get; set; }

            public class VerificationRes
            {
                public string Id { get; set; }
                public string Url { get; set; }
                public string VendorData { get; set; }
                public string Host { get; set; }
                public string Status { get; set; }
                public string SessionToken { get; set; }
            }
        }

        public async Task<VeriffApiResponse> GenerateSession()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
    
            // Updated to use X-AUTH-CLIENT header
            client.DefaultRequestHeaders.Add("X-AUTH-CLIENT", _apiKey);


            var verificationRequest = new VerificationRequest
            {
                verification = new VerificationRequest.Verification
                {
                    callback = "https://veriff.com", // Change this to your actual callback URL
                    person = new VerificationRequest.Person
                    {
                        firstName = "John",
                        lastName = "Smith",
                        idNumber = "123456789"
                    },
                    document = new VerificationRequest.Document
                    {
                        number = "B01234567",
                        type = "PASSPORT",
                        country = "EE"
                    },
                    address = new VerificationRequest.Address
                    {
                        fullAddress = "Lorem Ipsum 30, 13612 Tallinn, Estonia"
                    },
                    vendorData = "11111111"
                }
            };

            var jsonRequest = JsonConvert.SerializeObject(verificationRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/v1/sessions", content);
            if (response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                var sessionResponse = JsonConvert.DeserializeObject<VeriffApiResponse>(contentResponse);
                return sessionResponse; // Adjusted to return the session response
            }
            else
            {
                // Handle errors or unsuccessful response
                return null;
            }
        }
}