using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Commands;

[SlashCommandGroup("banappeal", "Ban appeal commands")]
public class BanAppealCommands : ApplicationCommandModule
{
    [SlashCommand( "link", "Get a link to create a ban appeal." )]
    public async Task LinkCommmandAsync( InteractionContext ctx )
    {
        const string str =
            "You can create a ban appeal by reacting to this message: https://canary.discord.com/channels/917912577768566835/917912578708090882/924427892434296862";
        
        await ctx.RespondAsync( str );
    }

    [SlashCommand( "accept", "Accepts a ban appeal" )]
    public async Task AcceptCommandAsync( InteractionContext ctx, [Option( "user", "user" )] DiscordUser user )
    {
        if ( !ctx.Member.IsTicketSupport() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );
            return;
        }

        var embed = new DiscordEmbedBuilder
        {
            Title = "Ban Appeal Accepted",
            Description =
                $"This message shall serve as a notice of a decision made to your recent ban appeal to the Administration Team of Florida State Roleplay. After deliberation and careful consideration of your motion to appeal, __the staff team have decided to reverse your ban__. Please allow up to 24 hours for your ban to be revoked. If for some reason you are still banned then please <#917912578708090882> and a member of staff will be glad to assist. We wish you the best.",
            Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                IconUrl = "https://cdn.penathon.wtf/i/logo.png",
                Name = "Florida State Roleplay",
                Url = "https://floridasrp.com"
            },
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url =
                    "https://cdn.discordapp.com/attachments/570263774205050912/775466722492022794/Webp.net-resizeimage_3.png",
                Height = 80,
                Width = 80,
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = $"Reviewed by {ctx.Member.DisplayName}"
            },
            Color = DiscordColor.Green
        };

        var channel = ctx.Guild.GetChannel( 917912578464841746 );
        await channel.SendMessageAsync( user.Mention, embed: embed );

        await ctx.RespondAsEphemeralAsync(
            $"Successfully accepted {user.Mention}'s ban appeal." );
    }

    [SlashCommand( "reduce", "Reduces a ban" )]
    public async Task ReduceCommandAsync( InteractionContext ctx, [Option( "user", "user" )] DiscordUser user, [Option("length", "the staff team have decided to reduce your ban to {length}.")] string length )
    {
        if ( !ctx.Member.IsTicketSupport() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );
            return;
        }

        var embed = new DiscordEmbedBuilder
        {
            Title = "Ban Appeal Accepted (Reduced)",
            Description =
                $"This message shall serve as a notice of a decision made to your recent ban appeal to the Administration Team of Florida State Roleplay. After deliberation and careful consideration of your motion to appeal, __the staff team have decided to reduce your ban to {length.Trim()}__. Please allow up to 24 hours for your ban to be revoked after the allotted time. If for some reason you are still banned then please <#917912578708090882> and a member of Staff will be glad to assist. In the meantime, please review the rules. We wish you the best. ",
            Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                IconUrl = "https://cdn.penathon.wtf/i/logo.png",
                Name = "Florida State Roleplay",
                Url = "https://floridasrp.com"
            },
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url =
                    "https://cdn.discordapp.com/attachments/570263774205050912/775466722492022794/Webp.net-resizeimage_3.png",
                Height = 80,
                Width = 80,
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = $"Reviewed by {ctx.Member.DisplayName}"
            },
            Color = DiscordColor.Orange
        };

        var channel = ctx.Guild.GetChannel( 917912578464841746 );
        await channel.SendMessageAsync( user.Mention, embed: embed );

        await ctx.RespondAsEphemeralAsync(
            $"Successfully reduced {user.Mention}'s ban appeal by {length}." );
    }

    [SlashCommand( "deny", "Denies a ban appeal" )]
    public async Task DenyCommandAsync( InteractionContext ctx, [Option( "user", "user" )] DiscordUser user )
    {
        if ( !ctx.Member.IsTicketSupport() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );
            return;
        }

        var embed = new DiscordEmbedBuilder
        {
            Title = "Ban Appeal Denied",
            Description =
                $"This message shall serve as a notice of a decision made to your recent ban appeal to the Administration Team of Florida State Roleplay. After deliberation and careful consideration of your motion to appeal, __the staff team have decided to uphold their original decision__.",
            Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                IconUrl = "https://cdn.penathon.wtf/i/logo.png",
                Name = "Florida State Roleplay",
                Url = "https://floridasrp.com"
            },
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = "https://cdn.discordapp.com/attachments/570263774205050912/775470298899546132/Webp.net-resizeimage_4.png",
                Height = 80,
                Width = 80,
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = $"Reviewed by {ctx.Member.DisplayName}"
            },
            Color = DiscordColor.Red
        };

        var channel = ctx.Guild.GetChannel( 917912578464841746 );
        await channel.SendMessageAsync( user.Mention, embed: embed );

        await ctx.RespondAsEphemeralAsync(
            $"Successfully denied {user.Mention}'s ban appeal." );
    }
}