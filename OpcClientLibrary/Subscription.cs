using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Workstation.ServiceModel.Ua;

namespace OpcClientLibrary
{
    class Subscription
    {
        private readonly ActionBlock<PublishResponse> actionBlock;
        private OpcConnection opcConnection;
        private IProgress<CommunicationState> progress;
        private volatile TaskCompletionSource<bool> whenSubscribed;
        private volatile TaskCompletionSource<bool> whenUnsubscribed;
        private volatile bool isPublishing;
        private MonitoredItemBaseCollection monitoredItems = new MonitoredItemBaseCollection();
        private uint subscriptionId;
        private readonly ILogger logger;

        public Subscription(OpcConnection opc, IProgress<CommunicationState> prog, ActionBlock<PublishResponse> action, ILogger log)
        {
            opcConnection = opc;
            progress = prog;
            actionBlock = action;
            logger = log;
            whenSubscribed = new TaskCompletionSource<bool>();
            whenUnsubscribed = new TaskCompletionSource<bool>();
            whenUnsubscribed.TrySetResult(true);
        }

        internal async Task SubscribeAsync(CancellationToken token = default(CancellationToken))
        {
            while (!token.IsCancellationRequested)
            {
                await whenSubscribed.Task;
                progress.Report(CommunicationState.Opening);

                try
                {
                    try
                    {
                        var linkToken = await CreateSubscriptionAsync();

                        try
                        {
                            //Add items to the subscription...using AddTagAsync

                            progress.Report(CommunicationState.Opened);

                            try
                            {
                                await Task.WhenAny(opcConnection.WhenChannelClosingAsync(opcConnection.Channel, token), whenUnsubscribed.Task);
                            }
                            catch
                            {

                            }
                            finally
                            {
                                progress.Report(CommunicationState.Closing);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError($"Error creating MonitoredItems. {ex.Message}");
                            progress.Report(CommunicationState.Faulted);
                        }
                        finally
                        {
                            linkToken.Dispose();
                        }
                        if (opcConnection.Channel.State == CommunicationState.Opened)
                        {
                            try
                            {
                                var deleteRequest = new DeleteSubscriptionsRequest
                                {
                                    SubscriptionIds = new uint[] { subscriptionId }
                                };
                                await opcConnection.Channel.DeleteSubscriptionsAsync(deleteRequest);
                            }
                            catch (Exception ex)
                            {
                                this.logger?.LogError($"Error deleting subscription. {ex.Message}");
                                await Task.Delay(2000);
                            }
                        }
                        progress.Report(CommunicationState.Closed);
                    }
                    catch (Exception ex)
                    {
                        this.logger?.LogError($"Error creating subscription. {ex.Message}");
                        this.progress.Report(CommunicationState.Faulted);
                        await Task.Delay(2000);
                    }
                }
                catch (Exception ex)
                {
                    this.logger?.LogTrace($"Error getting channel. {ex.Message}");
                    this.progress.Report(CommunicationState.Faulted);
                    await Task.Delay(2000);
                }
            }
        }

        private async Task<IDisposable> CreateSubscriptionAsync()
        {
            //Create a subscription
            var subscriptionRequest = new CreateSubscriptionRequest
            {
                RequestedPublishingInterval = 500,
                RequestedMaxKeepAliveCount = 10,
                RequestedLifetimeCount = 30,
                PublishingEnabled = true
            };
            var subscriptionResponse = await opcConnection.Channel.CreateSubscriptionAsync(subscriptionRequest).ConfigureAwait(false);
            var id = subscriptionId = subscriptionResponse.SubscriptionId;
            var linkToken = opcConnection.Channel.LinkTo(actionBlock, pr => pr.SubscriptionId == id);
            return linkToken;
        }

        public void CancelSubscription()
        {
            this.whenSubscribed = new TaskCompletionSource<bool>();
            this.whenUnsubscribed.TrySetResult(true);
        }

        public void EnableSubscription()
        {
            this.whenUnsubscribed = new TaskCompletionSource<bool>();
            this.whenSubscribed.TrySetResult(true);
        }

        private async void OpcTag_ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isPublishing || string.IsNullOrEmpty(e.PropertyName))
            {
                return;
            }
            if (monitoredItems.TryGetValueByName(e.PropertyName, out MonitoredItemBase item))
            {
                if (item.TryGetValue(out DataValue value))
                {
                    StatusCode statusCode;
                    var writeRequest = new WriteRequest
                    {
                        NodesToWrite = new[] { new WriteValue { NodeId = item.NodeId, AttributeId = item.AttributeId, IndexRange = item.IndexRange, Value = value } }
                    };
                    try
                    {
                        var writeResponse = await opcConnection.Channel.WriteAsync(writeRequest).ConfigureAwait(false);
                        statusCode = writeResponse.Results[0];
                    }
                    catch (ServiceResultException ex)
                    {
                        statusCode = ex.StatusCode;
                    }
                    catch (Exception)
                    {
                        statusCode = StatusCodes.BadServerNotConnected;
                    }

                    item.OnWriteResult(statusCode);
                    if (StatusCode.IsBad(statusCode))
                    {
                        logger?.LogError($"Error writing value for {item.NodeId}.  {StatusCodes.GetDefaultMessage(statusCode)}");
                    }
                }
            }
        }

        private void CheckNotifications(DataChangeNotification dcn)
        {
            foreach (var min in dcn.MonitoredItems)
            {
                if (this.monitoredItems.TryGetValueByClientId(min.ClientHandle, out MonitoredItemBase item))
                {
                    try
                    {
                        item.Publish(min.Value);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError($"Error publishing value for NodeId {item.NodeId}. {ex.Message}");
                    }
                }
            }
        }

        internal void OnPublishResponse(PublishResponse publishResponse)
        {
            isPublishing = true;
            try
            {
                var nd = publishResponse.NotificationMessage?.NotificationData;
                if (nd == null)
                    return;
                foreach (var n in nd)
                {
                    if (n is DataChangeNotification dcn)
                    {
                        CheckNotifications(dcn);
                    }
                }
            }
            finally
            {
                isPublishing = false;
            }
        }
    }
}
