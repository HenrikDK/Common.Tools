namespace Common.Tools;

public static class TransactionUtils
{
    /// <summary>
    /// TransactionScope default constructor is considered harmful, it defaults to an incorrect isolation level that degrades performance,
    /// despite this microsoft has still not changed its behaviour even in .net core and later versions
    /// </summary>
    /// <returns>A properly constructed transaction scope</returns>
    public static TransactionScope CreateScope()
    {
        var options = new TransactionOptions
        {
            IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.MaximumTimeout
        };

        return new TransactionScope(TransactionScopeOption.Required, options);
    }

    /// <summary>
    /// On windows TransactionScope inherits a maximum timeout form Machine.Config. This is an archaic remnant from
    /// the before times, when ancient beasts of burden know as "IT Admins" would maintain such configurations
    /// on physical calculation machines know as "Servers". Since Microsoft provides no way to override this in code,
    /// we use the forbidden magic known amongst dark wizards as "Reflection" to get around this.
    /// Note: the names of the fields manipulated have changed between frameworks before and might change again!
    /// </summary>
    /// <param name="value">Timespan to override the timeout with</param>
    public static void OverrideMaximumTimeout(TimeSpan value)
    {
        var type = typeof(TransactionManager);
        var cachedMaxTimeout = type.GetField("s_cachedMaxTimeout", BindingFlags.NonPublic | BindingFlags.Static);
        cachedMaxTimeout.SetValue(null, true);

        var maxTimeout = type.GetField("s_maximumTimeout", BindingFlags.NonPublic | BindingFlags.Static);
        maxTimeout.SetValue(null, value);
    }
}