using System.Collections.Concurrent;

namespace caportal.Services.Security;

public class LoginRateLimiter
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    private class AttemptRecord
    {
        public int FailedCount { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public DateTime LastAttempt { get; set; } = DateTime.UtcNow;
    }

    private readonly ConcurrentDictionary<string, AttemptRecord> _records = new();

    private static string GetKey(string ip, string username) =>
        $"{ip.Trim()}:{username.Trim().ToLowerInvariant()}";

    /// <summary>
    /// Checks if an IP or Username is currently locked out.
    /// </summary>
    public bool IsLockedOut(string ip, string username, out int remainingSeconds)
    {
        remainingSeconds = 0;
        var key = GetKey(ip, username);
        if (_records.TryGetValue(key, out var record))
        {
            if (record.LockoutUntil.HasValue)
            {
                if (DateTime.UtcNow < record.LockoutUntil.Value)
                {
                    remainingSeconds = (int)Math.Ceiling((record.LockoutUntil.Value - DateTime.UtcNow).TotalSeconds);
                    return true;
                }
                // Lockout expired -> reset record
                _records.TryRemove(key, out _);
            }
        }
        return false;
    }

    /// <summary>
    /// Records a failed login attempt. Returns remaining attempts before lockout.
    /// </summary>
    public int RecordFailedAttempt(string ip, string username, out bool justLockedOut)
    {
        justLockedOut = false;
        var key = GetKey(ip, username);

        var record = _records.AddOrUpdate(
            key,
            _ => new AttemptRecord { FailedCount = 1 },
            (_, existing) =>
            {
                existing.FailedCount++;
                existing.LastAttempt = DateTime.UtcNow;
                return existing;
            });

        if (record.FailedCount >= MaxFailedAttempts)
        {
            record.LockoutUntil = DateTime.UtcNow.Add(LockoutDuration);
            justLockedOut = true;
            return 0;
        }

        return MaxFailedAttempts - record.FailedCount;
    }

    /// <summary>
    /// Resets failed attempts after a successful login.
    /// </summary>
    public void ResetAttempts(string ip, string username)
    {
        var key = GetKey(ip, username);
        _records.TryRemove(key, out _);
    }
}
