using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Commands;

[SlashCommandGroup( "ticket", "Ticket commands" )]
public class TicketCommands : ApplicationCommandModule
{
    [SlashCommand( "test", "testing ticket command" )]
    public async Task TestTicketCommandAsync( InteractionContext ctx )
    {
        var options = new List<DiscordSelectComponentOption>();
        
        foreach ( var type in Config.Current.TicketTypes )
            if ( !string.IsNullOrWhiteSpace( type.Emoji ) )
                options.Add( new DiscordSelectComponentOption( type.Title, type.Title, null, false,
                    new DiscordComponentEmoji( type.Emoji ) ) );
            else
                options.Add( new DiscordSelectComponentOption( type.Title, type.Title ) );

        var dropdown = new DiscordSelectComponent( "create_ticket_dropdown", null, options );

        await ctx.RespondAsync( new DiscordInteractionResponseBuilder()
            .WithContent( "__**Create a Ticket**__" )
            .AddComponents( dropdown ) 
        );
        
        
    }
}