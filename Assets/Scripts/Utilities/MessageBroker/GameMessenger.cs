using System;
using System.Collections.Generic;

namespace Utilities.MessageBroker
{
    public class GameMessenger : PocoSingleton<GameMessenger>
    {
        private Dictionary<Type, List<Delegate>> _messageMappings;

        public GameMessenger()
        {
            _messageMappings = new Dictionary<Type, List<Delegate>>();
        }

        public void RegisterSubscriberToMessageTypeOf<T>(Action<T> messageHandler)
        {
            var messageType = typeof(T);
            if(!_messageMappings.ContainsKey(messageType))
            {
                _messageMappings.Add(messageType, new List<Delegate>());
            }
            
            _messageMappings[messageType].Add(messageHandler);
        }

        public void SendMessageOfType<T>(T message)
        {

            var messageType = typeof(T);
            if (!_messageMappings.ContainsKey(messageType)) return;

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
