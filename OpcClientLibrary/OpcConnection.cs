using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace OpcClientLibrary
{
    public class OpcConnection
    {
        private readonly ActionBlock<PublishResponse> actionBlock;
        private readonly IProgress<CommunicationState> progress;
        private CommunicationState state;
        public UaTcpSessionChannel Channel;
        private readonly ILogger logger;
        private ILoggerFactory loggerFactory;
        private IUserIdentity anonId = new AnonymousIdentity();        
        private CancellationTokenSource stateMachineCts;
        private Dictionary<uint, string> tagList = new Dictionary<uint, string>();
        public CommunicationState State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    State = value;
                    ConnectionStateChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        private Subscription subscription;
        public event EventHandler ConnectionStateChanged;
        
        public OpcConnection()
        {
            loggerFactory = new LoggerFactory();
            progress = new Progress<CommunicationState>(s => State = s);

            EnableSubscription();

            // register the action to be run on the ui thread, if there is one.
            if (SynchronizationContext.Current != null)
            {
                actionBlock = new ActionBlock<PublishResponse>(pr => subscription?.OnPublishResponse(pr), new ExecutionDataflowBlockOptions { SingleProducerConstrained = true, TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext() });
            }
            else
            {
                actionBlock = new ActionBlock<PublishResponse>(pr => subscription?.OnPublishResponse(pr), new ExecutionDataflowBlockOptions { SingleProducerConstrained = true });
            }
            stateMachineCts = new CancellationTokenSource();             
        }

        private void EnableSubscription()
        {
            subscription = new Subscription(this, progress, actionBlock, logger);
        }

        public async Task ConnectAsync(EndpointDescription endpoint, ApplicationDescription application, ICertificateStore certificate)
        {
            Channel = new UaTcpSessionChannel(application, certificate, new AnonymousIdentity(), endpoint, loggerFactory);
            try
            {
                await Channel.OpenAsync(stateMachineCts.Token);
                
                await subscription.SubscribeAsync();
            }
            catch (ServiceResultException ex)
            {
                if ((uint)ex.HResult == StatusCodes.BadSecurityChecksFailed)
                {
                   
                }

                await Channel.AbortAsync(stateMachineCts.Token);
                throw;
            }

        }

        internal async Task WhenChannelClosingAsync(UaTcpSessionChannel currentChannel, CancellationToken token = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<bool>();
            EventHandler handler = (o, e) => { tcs.TrySetResult(true); };
            using (token.Register(state => ((TaskCompletionSource<bool>)state).TrySetCanceled(), tcs, false))
            {
                try
                {
                    currentChannel.Closing += handler;
                    if (currentChannel.State == CommunicationState.Opened)
                    {
                        await tcs.Task;
                    }
                }
                finally
                {
                    currentChannel.Closing -= handler;
                }
            }
        }

        internal void GetFullTagInfo()
        {
            
        }
        
        public async Task CloseAsync()
        {
            await Channel.CloseAsync(stateMachineCts.Token);
        }
    }
}
