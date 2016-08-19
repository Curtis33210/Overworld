using UnityEngine;
using System.Collections.Generic;

public enum Priority
{
    /// <summary>
    /// Event will not be queued, instead the event will fire instantly
    /// </summary>
    Realtime = 0,

    /// <summary>
    /// High priority is the highest priority queue (Real time will still be run first)
    /// </summary>
    High = 0,

    /// <summary>
    /// The default priority. This is run after high priority, but before low.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Low priority is run AFTER normal and high priority events
    /// </summary>
    Low = 2,

    /// <summary>
    /// Number of event queue priorities. Note: This does not include Realtime
    /// </summary>
    Length = 3
}

public delegate void GameEventListener(GameEvent e); // Delegate for all listeners callbacks

public class EventManager
{
    #region Shared Methods/Variables

    private static bool _hasBeenInitialized;

    private static List<GameEvent>[] _newEvents; // List of events that have been added since the last processing
    private static Queue<GameEvent>[] _currentEvents; // Queue of events that are currently being processed
    private static Dictionary<object, GameEventListener> _eventListeners; // All of the callbacks for each type of event

    /// <summary>
    /// Initializes the static variables. <para/>
    /// This function is called when the first instance of EventManager is made.
    /// </summary>
    private static void InitializeEventManager() {
        _newEvents = new List<GameEvent>[(int)Priority.Length];
        _currentEvents = new Queue<GameEvent>[(int)Priority.Length];

        // Initializes each list/Queue
        for (int i = 0; i < (int)Priority.Length; i++) {
            _newEvents[i] = new List<GameEvent>();
            _currentEvents[i] = new Queue<GameEvent>();
        }

        _eventListeners = new Dictionary<object, GameEventListener>();

        _hasBeenInitialized = true;
    }
    
    /// <summary>
    /// Processes every event (In all priority levels except real time)
    /// </summary>
    public static void ProcessEvents() {

        ProcessEvents(Priority.High);
        ProcessEvents(Priority.Normal);
        ProcessEvents(Priority.Low);
    }

    /// <summary>
    /// Processes the events in a priority level queued at the time of calling this method.
    /// </summary>
    /// <param name="priorityLevel">Priority level to process</param>
    private static void ProcessEvents(Priority priorityLevel) {
        if (_hasBeenInitialized == false || _newEvents[(int)priorityLevel].Count == 0)
            return;

        Profiler.BeginSample("Process Events");

        var priority = (int)priorityLevel; // ( Is actually the integer representation to save having to constantly cast )

        Profiler.BeginSample("Copying Events");
        // Adds each new event into the event queue for this priority
        for (int i = 0; i < _newEvents[priority].Count; i++) {
            _currentEvents[priority].Enqueue(_newEvents[priority][i]);
        }
        Profiler.EndSample();


        Profiler.BeginSample("Clear");
        // All the events have been registered so it can be cleared 
        _newEvents[priority] = new List<GameEvent>();

        Profiler.EndSample();

        Profiler.BeginSample("Going through event Queue");
        // Actually process each event
        while (_currentEvents[priority].Count > 0) {
            ProcessEvent(_currentEvents[priority].Dequeue());
        }
        Profiler.EndSample();

        Profiler.EndSample();
    }

    /// <summary>
    /// Processes a single specific event
    /// </summary>
    /// <param name="gameEvent">The event to be processed</param>
    private static void ProcessEvent(GameEvent gameEvent) {
        // Event only gets processed if there are any listeners. 
        if (_eventListeners.ContainsKey(gameEvent.EventType))
            _eventListeners[gameEvent.EventType](gameEvent);
    }

    /// <summary>
    /// Places the event into the queue to be processed at a later date <para/>
    /// Unless it has a realtime priority.
    /// </summary>
    /// <param name="priority">The order in which the event should be processed</param>
    /// <param name="eventType">The type for this event</param>
    /// <param name="eventArgs">The values to be passed along with this event</param>
    private static void QueueEvent(Priority priority, object eventType, object eventArgs) {
        // Check if the event is real time. If it is process the event instantly.
        if (priority == Priority.Realtime) {
            ProcessEvent(new GameEvent(eventType, eventArgs));
            return;
        }
        
        // Queue the event under the correct priority.
        _newEvents[(int)priority].Add(new GameEvent(eventType, eventArgs));
    }

    #endregion

    /// <summary>
    /// Constructs the EventManager object. <para/> 
    /// Note this also initializes private static variables used across all Event Managers
    /// </summary>
    public EventManager() {
        if (_currentEvents == null)
            InitializeEventManager();
    }

    /// <summary>
    /// Queues the specified event type. <para/>
    /// This event has no parameters and defaults to a normal priority.
    /// </summary>
    /// <param name="eventType">Type of the event to be queued</param>
    public void RegisterEvent(object eventType) {
        QueueEvent(Priority.Normal, eventType, null);
    }

    /// <summary>
    /// Queues the specified event type. <para/>
    /// This event has no parameters and defaults to a normal priority.
    /// </summary>
    /// <param name="eventType">Type of the event to be queued</param>
    /// <param name="eventArgs">Argument values to be passed with the event</param>
    public void RegisterEvent(object eventType, object eventArgs) {
        QueueEvent(Priority.Normal, eventType, eventArgs);
    }

    /// <summary>
    /// Queues the specified event type. <para/>
    /// This event has no parameters and defaults to a normal priority.
    /// </summary>
    /// <param name="priority">Priority of the event. Realtime will be run instantly</param>
    /// <param name="eventType">Type of the event to be queued</param>
    /// <param name="eventArgs">Argument values to be passed with the event</param>
    public void RegisterEvent(Priority priority, object eventType, object eventArgs) {
        QueueEvent(priority, eventType, eventArgs);
    }

    /// <summary>
    /// Adds the listener to the event type.
    /// </summary>
    /// <param name="eventType">Event to listen for</param>
    /// <param name="callback">Callback for when the event occurs</param>
    public void RegisterListener(object eventType, GameEventListener callback) {
        if (_eventListeners.ContainsKey(eventType)) 
            _eventListeners[eventType] += callback;
        else 
            _eventListeners.Add(eventType, callback);
    }

    /// <summary>
    /// Removes the listener for the specific event type <para/>
    /// If the resulting number of listeners is 0 (Delegate == null), it is removed completely.
    /// </summary>
    /// <param name="eventType">Type of the event being listened for</param>
    /// <param name="callback">Specific callback to be removed</param>
    public void RemoveListener(object eventType, GameEventListener callback) {
        _eventListeners[eventType] -= callback;

        if (_eventListeners[eventType] == null)
            _eventListeners.Remove(eventType);
    }
}