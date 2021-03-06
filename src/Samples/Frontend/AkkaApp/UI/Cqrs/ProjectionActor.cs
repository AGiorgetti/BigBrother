﻿extern alias akka;

using akka::Akka.Actor;
using Frontend.Shared.Cqrs.Projections;
using System;

namespace Frontend.AkkaApp.UI.Cqrs
{
    /// <summary>
    /// An actor that manages projecting new elements
    /// </summary>
    internal class ProjectionActor : ReceiveActorWithLog
    {
        private readonly ProjectionEngine _projectionEngine;

        public ProjectionActor(ProjectionEngine projectionEngine)
        {
            _projectionEngine = projectionEngine;
            Become(Waiting);
        }

        private void Waiting()
        {
            Receive<PollImmediately>(Handle);
            Receive<StartPolling>(Handle);
        }

        private void Polling()
        {
            Receive<PollImmediately>(Handle);
            Receive<StopPolling>(Handle);
            Receive<ReceiveTimeout>(timeout => Poll());
        }

        private Boolean Handle(PollImmediately obj)
        {
            Poll();
            return true;
        }

        private Boolean Handle(StopPolling obj)
        {
            SetReceiveTimeout(null);
            Become(Waiting);
            return true;
        }

        private Boolean Handle(StartPolling obj)
        {
            Become(Polling);
            SetReceiveTimeout(TimeSpan.FromSeconds(3));
            Poll();
            return true;
        }

        private void Poll()
        {
            _projectionEngine.Poll();
        }
    }

    public class StartPolling
    {
    }

    public class StopPolling
    {
    }

    public class PollImmediately
    {
    }
}
