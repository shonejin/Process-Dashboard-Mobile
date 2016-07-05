using System;
using System.Net.Http;
using Fusillade;
using ModernHttpClient;
using ProcessDashboard.Service.Interface;
using Refit;

namespace ProcessDashboard.Service
{
    /*
        DO NOT CHANGE
        This class uses Fusilade to allocate priority to a network request.

    */

    public class ApiTypes : IApiTypes
    {
        public const string ApiBaseAddress = "https://pdes.tuma-solutions.com/api/v1/";
        public ApiTypes(string apiBaseAddress = null)
        {
            Func<HttpMessageHandler, IPDashApi> createClient = messageHandler =>
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(apiBaseAddress ?? ApiBaseAddress)
                };

                // Return a concrete implementation of IPDashAPi
                return RestService.For<IPDashApi>(client);
            };

            _background = new Lazy<IPDashApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.Background)));

            _userInitiated = new Lazy<IPDashApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.UserInitiated)));

            _speculative = new Lazy<IPDashApi>(() => createClient(
                new RateLimitedHttpMessageHandler(new NativeMessageHandler(), Priority.Speculative)));
        }

        private readonly Lazy<IPDashApi> _background;
        private readonly Lazy<IPDashApi> _userInitiated;
        private readonly Lazy<IPDashApi> _speculative;

        public IPDashApi Background
        {
            get { return _background.Value; }
        }

        public IPDashApi UserInitiated
        {
            get { return _userInitiated.Value; }
        }

        public IPDashApi Speculative
        {
            get { return _speculative.Value; }
        }
    }
}