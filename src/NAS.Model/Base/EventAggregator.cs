namespace NAS.Model.Base
{
  public class EventAggregator
  {
    private static readonly Lazy<EventAggregator> _instance = new(() => new EventAggregator());

    private readonly Dictionary<Type, AggregatedEventBase> _events;

    private EventAggregator()
    {
      _events = new Dictionary<Type, AggregatedEventBase>();
    }

    public static EventAggregator Instance => _instance.Value;

    public T GetEvent<T>() where T : AggregatedEventBase, new()
    {
      lock (_events)
      {
        if (!_events.TryGetValue(typeof(T), out var existingEvent))
        {
          var newEvent = new T();
          _events[typeof(T)] = newEvent;
          return newEvent;
        }
        else
        {
          return (T)existingEvent;
        }
      }
    }
  }
}
