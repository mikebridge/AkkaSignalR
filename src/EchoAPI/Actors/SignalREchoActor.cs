using Akka.Actor;
using EchoAPI.Hub;
using EchoAPI.Messages;
using Microsoft.AspNet.SignalR;

namespace EchoAPI.Actors
{
    public class SignalREchoActor : TypedActor,
        IHandle<EchoRequest>
    {

        private IHubContext _context;

        protected override void PreStart()
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<EchoHub>();
        }

        public void Handle(EchoRequest message)
        {          
            // send the message back to the client.
            _context.Clients.All.echoMessage($"ECHO: {message.Message}");
        }
    }
}
