using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using com.dke.data.agrirouter.api.dto.onboard;
using com.dke.data.agrirouter.api.logging;

namespace com.dke.data.agrirouter.impl.service.common
{
    /**
     * Service to create HTTP clients for the communication process.
     */
    public class HttpClientService
    {
        /**
         * Create an authenticated HTTP client for the given onboarding response.
         */
        public HttpClient AuthenticatedHttpClient(OnboardingResponse onboardingResponse)
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ClientCertificates.Add(new X509Certificate2(
                Convert.FromBase64String(onboardingResponse.Authentication.Certificate),
                onboardingResponse.Authentication.Secret));
            var httpClient = new HttpClient(new LoggingHandler(httpClientHandler));
            return httpClient;
        }
    }
}