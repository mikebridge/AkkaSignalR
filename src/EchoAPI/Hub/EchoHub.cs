using System;
using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using EchoAPI.Messages;
using Microsoft.AspNet.SignalR.Owin;

namespace EchoAPI.Hub
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class EchoHub : Microsoft.AspNet.SignalR.Hub
    {

        public void SendMessage(String message)
        {
            var actorSystem = FindActorSystem();
            var echoActorRef = actorSystem.ActorSelection("/user/echoActor");
            echoActorRef.Tell(new EchoRequest(message));

        }

        private ActorSystem FindActorSystem()
        {
            var ctx = Context.Request as ServerRequest;
            if (ctx == null)
            {
                throw new Exception("The context was not initialized");
            }
            var actorSystem = ctx.Environment["akka.actorsystem"] as ActorSystem;
            if (actorSystem == null)
            {
                throw new Exception("The ActorSystem was not initialized");
            }
            return actorSystem;
        }
    }
}
