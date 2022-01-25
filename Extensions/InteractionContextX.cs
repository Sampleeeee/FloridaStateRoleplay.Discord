using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace FloridaStateRoleplay.Discord.Extensions;

public static class InteractionContextX
{
    public static async Task RespondAsync( this InteractionContext ctx, string message, InteractionResponseType type = InteractionResponseType.ChannelMessageWithSource )
    {
        await ctx.CreateResponseAsync( type,
            new DiscordInteractionResponseBuilder().WithContent( message ) );
    }

    public static async Task RespondAsync( this InteractionContext ctx, DiscordEmbedBuilder builder )
    {
        await ctx.CreateResponseAsync( InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed( builder ) );
    }

    public static async Task RespondAsEphemeralAsync( this InteractionContext ctx, string message )
    {
        await ctx.CreateResponseAsync( InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral( true ).WithContent( message ) );
    }
}