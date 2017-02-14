using System;
using System.Reflection;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace EchoAPI
{

    /// <summary>
    /// Taken from 
    /// http://stackoverflow.com/questions/30005575/signalr-use-camel-case#answer-30019100
    /// </summary>
    public class SignalRContractResolver : IContractResolver
    {

        private readonly Assembly assembly;
        private readonly IContractResolver camelCaseContractResolver;
        private readonly IContractResolver defaultContractSerializer;

        public SignalRContractResolver()
        {
            defaultContractSerializer = new DefaultContractResolver();
            camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
            assembly = typeof(Connection).Assembly;
        }

        public JsonContract ResolveContract(Type type)
        {
            return type.Assembly.Equals(assembly) 
                ? defaultContractSerializer.ResolveContract(type) 
                : camelCaseContractResolver.ResolveContract(type);
        }

    }

}
