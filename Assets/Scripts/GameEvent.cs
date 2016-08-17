using System;

public struct GameEvent
{
    public object EventType { get; set; }
    public object Args { get; set; }

    public GameEvent(object type, object args) {
        EventType = type;
        Args = args;
    }
}