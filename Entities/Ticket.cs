namespace FloridaStateRoleplay.Discord.Entities;

public class Ticket
{
    public ulong Id { get; set; }
    public TicketType Type { get; set; }
    public ulong ClaimedBy { get; set; }
}