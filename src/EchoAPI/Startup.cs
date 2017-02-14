using System;
using Akka.Actor;
using Akka.Configuration;
using EchoAPI.Actors;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Owin;

namespace EchoAPI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    //policy.WithOrigins("*")
                    policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            // make sure the client gets camelCase
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SignalRContractResolver()
            };
            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(policy =>
            {
                //policy.WithOrigins("*");
                policy.WithOrigins("http://localhost:3000");
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();
            });


            var config = new HubConfiguration
            {
                EnableDetailedErrors = true
            };

            ConfigureAkka(app);

        }

        private void ConfigureAkka(IApplicationBuilder app)
        {
            //var actorSystem = ActorSystem.Create("SignalRChatAPI");
            var actorSystem = CreateChatActorSystem("SignalRChatAPI");

            var echoActor = actorSystem.ActorOf(Props.Create(() => new SignalREchoActor()), "echoActor");

            app.UseAppBuilder(appBuilder => appBuilder.Use((ctx, next1) =>
            {
                // make the actor system available via the owin environment
                ctx.Environment["akka.actorsystem"] = actorSystem;
                return next1();
            }).MapSignalR());


//            app.UseOwin(addToPipeline =>
//            {
//                addToPipeline(next =>
//                {
//                    var appBuilder = new AppBuilder();
//                    appBuilder.Properties["builder.DefaultApp"] = next;
//                    //((Action<IAppBuilder>)(appBuilder => appBuilder.MapSignalR(config)))(appBuilder1);
//
//                    appBuilder.Use((ctx, next1) =>
//                    {
//                        // make the actor system available via the environment
//                        ctx.Environment["akka.actorsystem"] = actorSystem;
//                        return next1();
//                    }).MapSignalR(config)
//
//
//                    return appBuilder.Build<AppFunc>();
//                });
//            });


            var config = new HubConfiguration
            {
                EnableDetailedErrors = true
            };
        }

        private static ActorSystem CreateChatActorSystem(String name)
        {
            var config = ConfigurationFactory.ParseString(@"akka {  
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    suppress-json-serializer-warning: on
    remote {
        helios.tcp {
		    port = 0
		    hostname = localhost
        }
    }
}
");

            return ActorSystem.Create(name, config);
        }


    }
}
