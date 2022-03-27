using System;
using System.Collections.Generic;
using System.Net.Http;
using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }
        public IEnumerable<Platform> ReturnAllPlatforms(bool isDev)
        {
            Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcPlatform"]}");

            var grpcOptions = new GrpcChannelOptions 
            {
                HttpClient = isDev 
                    ? _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted")
                    : _httpClientFactory.CreateClient()
            };
            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"], grpcOptions);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call grpc Server {ex.Message}");
                return null;
            }
        }
    }
}