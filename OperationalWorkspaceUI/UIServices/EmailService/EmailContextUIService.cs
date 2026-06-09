using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.UIServices.EmailService
{
    public class EmailContextUIService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public EmailContextUIService(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        // ---------------------------------------------------------------------
        // ⭐ UPGRADED PROCESSING PIPELINE: LOADS ENTIRE COMPOSITE SAGE CONTEXT
        // ---------------------------------------------------------------------
        public async Task<(bool IsUnknownSender, EmailContextDto? Context)> LoadEmailContextWithStatusAsync(string emailId)
        {
            if (string.IsNullOrWhiteSpace(emailId))
            {
                return (true, null);
            }

            try
            {
                // Queries the status intelligence router endpoint using the message string identifier
                var response = await _http.GetAsync($"api/email/{emailId}/context");

                if (!response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                // Parse the top-level dynamic JSON envelope structure safely
                using var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                var rootElement = jsonDocument.RootElement;

                // Extract your boolean status flag rule directly
                bool isUnknownSender = false;
                if (rootElement.TryGetProperty("isUnknownSender", out var unknownProperty))
                {
                    isUnknownSender = unknownProperty.GetBoolean();
                }

                // Isolate and deserialize the nested EmailContextDto data block cleanly
                EmailContextDto? contextDto = null;
                if (rootElement.TryGetProperty("payload", out var payloadProperty) && payloadProperty.ValueKind != JsonValueKind.Null)
                {
                    contextDto = JsonSerializer.Deserialize<EmailContextDto>(payloadProperty.GetRawText(), JsonOptions);
                }

                return (isUnknownSender, contextDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadEmailContextWithStatusAsync failed internally: {ex.Message}");
                return (true, null);
            }
        }
    }
}
