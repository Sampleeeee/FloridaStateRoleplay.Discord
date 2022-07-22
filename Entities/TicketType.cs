using DSharpPlus.Entities;

namespace FloridaStateRoleplay.Discord.Entities;

public class TicketType
{
    /// <summary>
    /// The role belonging to the ticket
    /// </summary>
    public ulong Role { get; set; }
    
    /// <summary>
    /// The category to put the tickets in.
    /// </summary>
    public ulong Category { get; set; }
    
    /// <summary>
    /// The title of the ticket, this must be unique.
    /// </summary>
    /// <remarks>Must be unique.</remarks>
    public string Title { get; set; }

    /// <summary>
    /// The emoji to show next to the ticket
    /// </summary>
    public ulong? Emoji { get; set; }
    
    /// <summary>
    /// The automatically sent message when the ticket is first created.
    /// </summary>
    public string OriginalMessage { get; set; }
    
    /// <param name="role"><see cref="Role"/></param>
    /// <param name="category"><see cref="Category"/></param>
    /// <param name="title"><see cref="Title"/></param>
    /// <param name="message"><see cref="OriginalMessage" /></param>
    /// <param name="emoji"><see cref="Emoji" /></param>
    public TicketType( ulong role, ulong category, string title, string message, ulong? emoji = null )
    {
        Role = role;
        Title = title;
        OriginalMessage = message;
        Emoji = emoji;
    }
}