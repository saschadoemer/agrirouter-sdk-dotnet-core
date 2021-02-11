using System;
using System.Collections.Generic;
using Agrirouter.Cloud.Registration;
using Agrirouter.Request;
using Agrirouter.Sdk.Api.Definitions;
using Agrirouter.Sdk.Api.Dto.Messaging;
using Agrirouter.Sdk.Api.Service.Messaging;
using Agrirouter.Sdk.Api.Service.Messaging.Vcu;
using Agrirouter.Sdk.Api.Service.Parameters;
using Agrirouter.Sdk.Impl.Service.Common;
using Google.Protobuf;

namespace Agrirouter.Sdk.Impl.Service.Messaging.Vcu
{
    /// <summary>
    ///     Service to onboard VCUs.
    /// </summary>
    public class OnboardVcuService : IOnboardVcuService
    {
        private readonly IMessagingService<MessagingParameters> _messagingService;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="messagingService">-</param>
        public OnboardVcuService(IMessagingService<MessagingParameters> messagingService)
        {
            _messagingService = messagingService;
        }

        /// <summary>
        ///     Please see <seealso cref="IMessagingService{T}.Send" /> for documentation.
        /// </summary>
        /// <param name="onboardVcuParameters">-</param>
        /// <returns>-</returns>
        public MessagingResult Send(OnboardVcuParameters onboardVcuParameters)
        {
            var encodedMessages = new List<string> {Encode(onboardVcuParameters).Content};
            var messagingParameters = onboardVcuParameters.BuildMessagingParameter(encodedMessages);
            return _messagingService.Send(messagingParameters);
        }

        /// <summary>
        ///     Please see <seealso cref="IEncodeMessageService{T}.Encode" /> for documentation.
        /// </summary>
        /// <param name="onboardVcuParameters"></param>
        /// <returns>-</returns>
        public EncodedMessage Encode(OnboardVcuParameters onboardVcuParameters)
        {
            var messageHeaderParameters = new MessageHeaderParameters
            {
                ApplicationMessageId = onboardVcuParameters.ApplicationMessageId,
                TeamSetContextId = onboardVcuParameters.TeamsetContextId ?? "",
                TechnicalMessageType = TechnicalMessageTypes.DkeCloudOnboardEndpoints,
                Mode = RequestEnvelope.Types.Mode.Direct
            };

            var messagePayloadParameters = new MessagePayloadParameters
            {
                TypeUrl = OnboardingRequest.Descriptor.FullName
            };

            var onboardingRequest = new OnboardingRequest();
            foreach (var onboardingRequestEntry in onboardVcuParameters.OnboardingRequests)
                onboardingRequest.OnboardingRequests.Add(onboardingRequestEntry);

            messagePayloadParameters.Value = onboardingRequest.ToByteString();

            var encodedMessage = new EncodedMessage
            {
                Id = Guid.NewGuid().ToString(),
                Content = EncodeMessageService.Encode(messageHeaderParameters, messagePayloadParameters)
            };

            return encodedMessage;
        }
    }
}