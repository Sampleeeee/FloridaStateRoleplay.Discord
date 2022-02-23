namespace FloridaStateRoleplay.Discord.Entities;

public class Sticky
{
    public string Title { get; set; }
    public string Message { get; set; }
    
    public ulong Channel { get; set; }
    public ulong LastMessage { get; set; }
}