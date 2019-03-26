using System;
using System.Collections.Generic;

namespace Utilities.MessageBroker
{
    public class GameMessenger : PocoSingleton<GameMessenger>
    {
        private readonly Dictionary<Type, List<Delegate>> _messageMappings;

        public GameMessenger()
        {
            _messageMappings = new Dictionary<Type, List<Delegate>>();
        }

        public void RegisterSubscriberToMessageTypeOf<T>(Action<T> messageHandler) where T : struct
        {
            var messageType = typeof(T);
            List<Delegate> result;
            if (!_messageMappings.TryGetValue(messageType, out result))
            {
                _messageMappings.Add(messageType, new List<Delegate> {messageHandler});
                return;
            }

            _messageMappings[messageType].Add(messageHandler);
        }

        public void SendMessageOfType<T>(T message) where T : struct
        {
            var messageType = typeof(T);
            List<Delegate> result;
            if (!_messageMappings.TryGetValue(messageType, out result) || result?.Count == 0) return;

            foreach (var listener in _messageMappings[messageType])
            {
                listener.DynamicInvoke(message);
            }
        }

        public void UnRegisterAllMessagesForObject(object targetObject)
        {
            var listsOfActions = _messageMappings.Values;
            foreach (var listOfActions in listsOfActions)
            {
                listOfActions.RemoveAll(x => x.Target == targetObject);
            }
        }
    }
}