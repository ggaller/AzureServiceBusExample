using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using static System.Console;
using MessageBuilder = System.Action<MessageSender.Message, string>;

namespace MessageSender
{
    internal class HostService : IHostedService
    {
        private static readonly Dictionary<SendResult, string> ResponseMessages = new Dictionary<SendResult, string>
            {
                [SendResult.Ok] = "Message successfully sended",
                [SendResult.Forbidden] = "You don't have permissions. Check From section and try again",
                [SendResult.NotFound] = "Recipient not found. Check To section and try again",
                [SendResult.ServerError] = "Server error. Try again later",
            };

        private readonly AbcFunctionClient _client;

        private readonly IHostApplicationLifetime _appLifetime;

        public HostService(AbcFunctionClient client, IHostApplicationLifetime appLifetime)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(client));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(RunInteractiveSession);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private void RunInteractiveSession()
        {
            Task.Run(() =>
            {
                var messageBuilder = new[] {
                   ("Enter from:", (MessageBuilder)((msg, text) => msg.From = text?.ToLower())),
                   ("Enter to:", (MessageBuilder)((msg, text) => msg.To = text?.ToLower())),
                   ("Enter body:", (MessageBuilder)((msg, text) => msg.Body = text))
                };

                WriteLine("Welcome to message sender!");
                WriteLine("Send message or press Ctrl+C for exit");
                while (!_appLifetime.ApplicationStopping.IsCancellationRequested)
                {
                    var message = new Message();
                    foreach (var (description, builder) in messageBuilder)
                    {
                        WriteLine(description);
                        builder(message, ReadLine());
                        if (!_appLifetime.ApplicationStopping.IsCancellationRequested)
                            return;
                    }
                    WriteLine("Sending...");
                    var result = _client.SendMessage(message).Result;
                    WriteLine(ResponseMessages[result]);
                    WriteLine();
                }
            });
        }
    }
}
