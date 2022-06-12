using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Gyldendal.Jini.ExternalClients.Interfaces;
using Gyldendal.Jini.Services.Core.Product.Services;
using Gyldendal.Jini.Services.Utils;
using Hangfire.Server;


namespace Gyldendal.Jini.Services.Core.GpmSubscription
{
    public class GpmMessageConsumerService : IGpmMessageConsumerService
    {

        static string connectionString = System.Configuration.ConfigurationManager.AppSettings["ServiceBusConnectionString"];
        static string topicName = System.Configuration.ConfigurationManager.AppSettings["TopicName"];
        static string subscriptionName = System.Configuration.ConfigurationManager.AppSettings["SubscriptionName"];
        static ServiceBusClient _client;
        static ServiceBusProcessor _processor;
        private readonly ILogger _logger;
        private readonly IGpmApiClient _gpmApiClient;
        private readonly IProductService _productService;

        public GpmMessageConsumerService(ILogger logger, IGpmApiClient gpmApiClient, IProductService productService)
        {
            _logger = logger;
            _gpmApiClient = gpmApiClient;
            _productService = productService;
            _client = new ServiceBusClient(connectionString, new ServiceBusClientOptions()
            {
                RetryOptions = new ServiceBusRetryOptions()
                {
                    Delay = TimeSpan.FromMilliseconds(50),
                    MaxDelay = TimeSpan.FromSeconds(1),
                    MaxRetries = 2
                }
            });
            _processor = _client.CreateProcessor(topicName, subscriptionName,
                new ServiceBusProcessorOptions { MaxConcurrentCalls = 50, PrefetchCount = 50 });
        }
        public async Task ConsumeGpmEvents(PerformContext context)
        {
            var cancellationToken = context.CancellationToken.ShutdownToken;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_processor.IsProcessing)
                {
                    await ConsumeAndProcessGpmEvent(cancellationToken);
                }
            }

            await StopAndDisposeGpmEvent(cancellationToken);
        }
        private async Task StopAndDisposeGpmEvent(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInfo("GpmEventConsumerJob is Ended");
                    await _processor.StopProcessingAsync();

                }
                finally
                {
                    await _processor.DisposeAsync();
                    await _client.DisposeAsync();
                }
            }
        }
        public async Task ConsumeAndProcessGpmEvent(CancellationToken cancellationToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;
            await _processor.StartProcessingAsync();
        }
        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            
            try
            {
                var values = ReadGpmMessagePropertiesFromAsbMessage(args);
                if (values.ChangeType.Equals("Update", StringComparison.InvariantCultureIgnoreCase))
                {
                    var payload =
                        await _gpmApiClient.FetchBusinessObjectPayloadAsync(values.AffectedScopeId,
                            values.BusinessObjectId).ConfigureAwait(false);
                    if(values.ContainerTypeId=="3")
                        await _productService.AddContainer(values.ContainerTypeId, payload).ConfigureAwait(false);
                }

                await args.CompleteMessageAsync(args.Message).ConfigureAwait(false);
                _logger.LogInfo(
                    $"Received and Completed message {values.BusinessObjectId} with message ID {args.Message.MessageId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch data for message with ID {args.Message.MessageId}", ex);
            }
 
        }

        private async Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception.Message, args.Exception);
        }

        private static GpmMessageProperties ReadGpmMessagePropertiesFromAsbMessage(ProcessMessageEventArgs message)
        {
            var values = message.Message.ApplicationProperties;
            var gpmMessageProperties = new GpmMessageProperties();
            if (values.TryGetValue("businessobject-id", out var businessObjectId))
                gpmMessageProperties.BusinessObjectId = businessObjectId.ToString();
            if (values.TryGetValue("affected-scope", out var scopeId))
                gpmMessageProperties.AffectedScopeId = scopeId.ToString();
            if (values.TryGetValue("businessoject-version", out var businessObjectVersion))
                gpmMessageProperties.BusinessObjectVersion = businessObjectVersion.ToString();
            if (values.TryGetValue("change-type", out var changeType))
                gpmMessageProperties.ChangeType = changeType.ToString();
            if (values.TryGetValue("container-type-id", out var containerTypeId))
                gpmMessageProperties.ContainerTypeId = containerTypeId.ToString();
            if (values.TryGetValue("subscription-id", out var subscriptionId))
                gpmMessageProperties.SubscriptionId = subscriptionId.ToString();
            if (values.TryGetValue("timestamp", out var timestamp))
                gpmMessageProperties.Timestamp = timestamp.ToString();
            return gpmMessageProperties;
        }

    }
    public class GpmMessageProperties
    {
        public string BusinessObjectId { get; set; }
        public string AffectedScopeId { get; set; }
        public string BusinessObjectVersion { get; set; }
        public string ChangeType { get; set; }
        public string ContainerTypeId { get; set; }
        public string SubscriptionId { get; set; }
        public string Timestamp { get; set; }
    }
}
