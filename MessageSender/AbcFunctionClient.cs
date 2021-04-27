using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageSender
{
    public class AbcFunctionClient
    {
        private readonly HttpClient _client;
        private readonly IOptions<FunctionSettings> _settings;
        private readonly ILogger _logger;

        public AbcFunctionClient(HttpClient client, IOptions<FunctionSettings> settings,
            ILogger<AbcFunctionClient> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SendResult> SendMessage(Message message)
        {
            if (message == null)
                throw new ArgumentException(nameof(message));

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(message));
                var response = await _client.PostAsync(_settings.Value.Endpoint, content);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        return SendResult.Ok;
                    case System.Net.HttpStatusCode.Forbidden:
                        return SendResult.Forbidden;
                    case System.Net.HttpStatusCode.NotFound:
                        return SendResult.NotFound;
                    default:
                        return SendResult.ServerError;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while sending message:{0}...", message);
                return SendResult.ServerError;
            }
        }
    }
}
