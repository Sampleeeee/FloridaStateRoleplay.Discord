namespace FloridaStateRoleplay.Discord.Entities;

public class Punishment
{
    public ulong StaffId { get; set; }
    public ulong TargetId { get; set; }
    public ulong RevokerId { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string RevokeReason { get; set; } = string.Empty;
    
    public PunishmentType Type { get; set; }
    
    public DateTime InvokedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public TimeAway TimeAway { get; set; }

    /// <summary>
    /// Has the expiration been processed yet?
    /// </summary>
    public bool Expired { get; set; } = false;
}

public enum PunishmentType
{
    Mute, Ban, Kick, Warn
}