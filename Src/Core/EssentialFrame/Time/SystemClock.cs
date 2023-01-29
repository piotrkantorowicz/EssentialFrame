using System;

namespace EssentialFrame.Time;

public static class SystemClock
{
    private static DateTimeOffset? _customDateTimeOffset;

    public static DateTimeOffset Now => _customDateTimeOffset ?? DateTimeOffset.UtcNow;

    public static void Set(DateTimeOffset? customDateTimeOffset)
    {
        _customDateTimeOffset = customDateTimeOffset;
    }

    public static void Reset()
    {
        _customDateTimeOffset = null;
    }
}