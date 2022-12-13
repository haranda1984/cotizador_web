using System.Text.Json.Serialization;

namespace HeiLiving.Quotes.Api.Models
{
    public class CustomerViewModel
    {
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastname")]
        public string LastName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("tax-id")]
        public string TaxId { get; set; }
        [JsonPropertyName("beneficiaries")]
        public string[] Beneficiaries { get; set; }
        [JsonPropertyName("crm-track")]
        public bool TrackInCRM { get; set; }
    }
}