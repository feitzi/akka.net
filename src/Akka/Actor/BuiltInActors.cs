﻿using Akka.Dispatch.SysMsg;
using Akka.Event;

namespace Akka.Actor
{
    /// <summary>
    ///     Class EventStreamActor.
    /// </summary>
    public class EventStreamActor : UntypedActor
    {
        /// <summary>
        ///     Processor for user defined messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void OnReceive(object message)
        {
        }
    }

    /// <summary>
    ///     Class GuardianActor.
    /// </summary>
    public class GuardianActor : UntypedActor
    {
        /// <summary>
        ///     Processor for user defined messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void OnReceive(object message)
        {
            Unhandled(message);
        }
    }

    /// <summary>
    /// System Guardian - responsible for all system-wide actors
    /// </summary>
    class SystemGuardianActor : UntypedActor
    {
        /// <summary>
        /// Processor for messages that are sent to the root system guardian
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            //TODO need to add termination hook support
        }
    }

    /// <summary>
    ///     Class DeadLetterActorRef.
    /// </summary>
    public class DeadLetterActorRef : EmptyLocalActorRef
    {
        private readonly EventStream _eventStream;

        public DeadLetterActorRef(ActorRefProvider provider, ActorPath path, EventStream eventStream) : base(provider,path,eventStream)
        {
            _eventStream = eventStream;
        }

        protected override void HandleDeadLetter(DeadLetter deadLetter)
        {
            if(!SpecialHandle(deadLetter.Message,deadLetter.Sender))
                _eventStream.Publish(deadLetter);
        }

        protected override bool SpecialHandle(object message, ActorRef sender)
        {
            var w = message as Watch;
            if(w != null)
            {
                if(w.Watchee != this && w.Watcher != this)
                {
                    w.Watcher.Tell(new DeathWatchNotification(w.Watchee, existenceConfirmed: false, addressTerminated: false));
                }
                return true;
            }
            return base.SpecialHandle(message, sender);
        }
    }
}