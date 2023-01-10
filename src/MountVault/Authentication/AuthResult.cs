namespace MountVault.Authentication;

public record AuthResult(string Token, uint LeaseDuration = 0)
{
    public DateTimeOffset IssueDate { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Expiration => IssueDate.AddSeconds(LeaseDuration);
    public bool IsExpired => Expiration < DateTimeOffset.UtcNow;
}