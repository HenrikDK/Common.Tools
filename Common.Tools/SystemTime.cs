namespace Common.Tools;

/// <summary>
/// Used for getting DateTime.Now or DateTime.UtcNow, time is mockable for testing purposes.
/// </summary>
public static class SystemTime
{
    public static Func<DateTime> Now = () => DateTime.Now;
    
    public static Func<DateTime> UtcNow = () => DateTime.UtcNow;

    /// <summary>
    /// Set the system time to a specific value
    /// </summary>
    /// <param name="value">DateTime value to use</param>
    public static void Set(DateTime value)
    {
        var utcNow = value.ToUniversalTime();
        Now = () => value;
        UtcNow = () => utcNow;
    }

    /// <summary>
    /// Resets the system time
    /// </summary>
    public static void Reset()
    {
        Now = () => DateTime.Now;
        UtcNow = () => DateTime.UtcNow;
    }
}