using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Commands;

[SlashCommandGroup( "ticket", "Ticket commands" )]
public class TicketComands : ApplicationCommandModule
{
    [SlashCommand( "test", "testing ticket command" )]
    public async Task TestTicketCommandAsync( InteractionContext ctx )
    {
        var options = new List<DiscordSelectComponentOption>()
        {
            new DiscordSelectComponentOption(
                "Label, no description",
                "label_no_desc"),

            new DiscordSelectComponentOption(
                "Label, Description",
                "label_with_desc",
                "This is a description!"),

            new DiscordSelectComponentOption(
                "Label, Description, Emoji",
                "label_with_desc_emoji",
                "This is a description!",
                emoji: new DiscordComponentEmoji(854260064906117121)),

            new DiscordSelectComponentOption(
                "Label, Description, Emoji (Default)",
                "label_with_desc_emoji_default",
                "This is a description!",
                isDefault: true,
                new DiscordComponentEmoji(854260064906117121))
        };

        foreach ( var type in Config.Current.TicketTypes )
            if ( type.Emoji is string emoji )
                options.Add( new DiscordSelectComponentOption( type.Title, type.Title, null, false,
                    new DiscordComponentEmoji( DiscordEmoji.FromName( Program.Discord, emoji ) ) ) );
            else
                options.Add( new DiscordSelectComponentOption( type.Title, type.Title ) );

        var dropdown = new DiscordSelectComponent("dropdown", null, options, false, 1, 2);

        await ctx.RespondAsync( new DiscordInteractionResponseBuilder()
            .WithCustomId( "create_ticket_button" )
            .AddComponents( dropdown ) 
        );
    }
}