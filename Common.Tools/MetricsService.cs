

namespace Common.Tools;

public interface IMetricsService
{
    /// <summary>
    /// Measure the time taken and number of successes of failures of a given method
    /// </summary>
    /// <param name="action"></param>
    /// <param name="jobName"></param>
    /// <param name="operation"></param>
    void ExecuteJob(Action action, string jobName = null, string operation = null);
}

public class MetricsService : IMetricsService
{
    private readonly ILogger<MetricsService> _logger;
    private readonly string _serviceName;
    private readonly Counter _jobCounter;
    private readonly Gauge _jobGauge;

    public MetricsService(ILogger<MetricsService> logger, string key)
    {
        _serviceName = Assembly.GetExecutingAssembly().GetName().Name ?? "unknown";
        _logger = logger;
        
        _jobCounter = Metrics.CreateCounter($"{key}_job_counter", "Measuring number of runs for a given job/operation", "serviceName", "jobName", "result", "operation");
        _jobGauge = Metrics.CreateGauge($"{key}_workflow_duration_seconds", "Measuring duration in seconds for a given job/operation", "serviceName", "jobName", "operation");
    }

    [DebuggerStepThrough]
    public void ExecuteJob(Action action, string jobName = null, string operation = null)
    {
        operation ??= action.Method.Name ?? "unknown";
        jobName ??= action.Method.DeclaringType?.Name ?? "unknown";
        
        _logger.LogDebug("Inside metrics service");

        var stopwatch = Stopwatch.StartNew();

        try
        {
            action.Invoke();
            stopwatch.Stop();
            _jobCounter.Labels(_serviceName, jobName, "success", operation).Inc();

        }
        catch
        {
            _jobCounter.Labels(_serviceName, jobName, "failure", operation).Inc();
            throw;
        }
        finally
        {
            _jobGauge.Labels(_serviceName, jobName, operation).Set(stopwatch.Elapsed.TotalSeconds);
        }
    }
}