using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Commands;

public class UtilityCommands : ApplicationCommandModule
{
    [SlashCommand( "bug", "Submit a new bug report." )]
    public async Task BugCommand( InteractionContext ctx )
    {
        await ctx.RespondAsync( "Success!" );
    }

    [SlashCommand( "suggestion", "Submit a new suggestion" )]
    public Task SuggestionCommand( InteractionContext ctx )
    {
        return Task.CompletedTask;
    }
}