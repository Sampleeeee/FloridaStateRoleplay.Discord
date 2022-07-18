using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Entities;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Commands;

[SlashCommandGroup( "app", "Commands regarding department applications / interviews" )]
public class InterviewCommands : ApplicationCommandModule
{
    [SlashCommand( "accept", "Accepts the user's application and gives them instructions to get an interview." )]
    public async Task AcceptApplicationCommandAsync( InteractionContext ctx,
        [Option( "user", "The user's application to accept." )] DiscordUser user,
        [Option( "Role", "What role this user needs an interview for." )] DiscordRole role )
    {
        if ( !ctx.Member.HasApplicationPermissions() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to do this." );
            return;
        }

        var member = await Member.FromUser( user );
        if ( member is null )
        {
            await ctx.RespondAsEphemeralAsync( "This user does not exist." );
            return;
        }
        
        await member.NeedsInterviewFor( role );

        var embed = new DiscordEmbedBuilder
        {
            Title = "Application Accepted",
            Description = $"Your application to {role.Mention} has been accepted! Please head over to <#917912578708090881> to begin the interview process.\n\nIf you require any assistance, please contact {ctx.Member.Mention}.",
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

        var discordMember = await member.GetDiscordMemberAsync();
        if ( discordMember is null )
            throw new Exception( "DiscordMember is null" );
        
        await discordMember.GrantRoleAsync( ctx.Guild.GetRole( 917912578087338035 ) );
        
        var channel = ctx.Guild.GetChannel( 917912578464841745 );
        await channel.SendMessageAsync( user.Mention, embed: embed );

        await ctx.RespondAsEphemeralAsync( $"Successfully accepted {user.Mention} for an interview with {role.Mention}." );
    }

    [SlashCommand( "deny", "Denies the user's application." )]
    public async Task DenyApplicationCommandAsync( InteractionContext ctx,
        [Option( "user", "The user's application to deny." )] DiscordUser user,
        [Option( "Role", "The department the user was denied from." )] DiscordRole role )
    {
        if ( !ctx.Member.HasApplicationPermissions() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to do this." );
            return;
        }

        var member = await Member.FromUser( user );
        if ( member is null )
        {
            await ctx.RespondAsEphemeralAsync( "This user does not exist." );
            return;
        }

        var embed = new DiscordEmbedBuilder
        {
            Title = "Application Denied",
            Description =
                $"We regret to inform you that your {role.Mention} application has been denied. \n\nIf you require any assistance, please contact {ctx.Member.Mention}.",
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
            Color = DiscordColor.Red
        };

        await member.NoLongerNeedsInterviewFor( role );

        var channel = ctx.Guild.GetChannel( 917912578464841745 );
        await channel.SendMessageAsync( user.Mention, embed: embed );

        await ctx.RespondAsEphemeralAsync( $"Successfully denied {user.Mention} for an interview with {role.Mention}." );
    }

    [SlashCommand( "interviewed", "Marks a user as successfully interviewed and gives them the correct roles." )]
    public async Task InterviewedCommandAsync( InteractionContext ctx,
        [Option( "user", "The user's application to deny." )]
        DiscordUser user,
        [Option( "Role", "The department the user was denied from." )]
        DiscordRole role )
    {
        if ( !ctx.Member.HasApplicationPermissions() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to do this." );
            return;
        }
        
        // TODO make sure they can't give super powerful roles w heierarchy

        var member = await Member.FromUser( user );
        if ( member is null )
        {
            await ctx.RespondAsEphemeralAsync( "This user does not exist." );
            return;
        }

        await member.NoLongerNeedsInterviewFor( role );

        var discordMember = await member.GetDiscordMemberAsync();
        if ( discordMember is null )
            throw new Exception( "DiscordMember is null" );
        
        // interview needed role
        if ( member.InterviewNeededRoles.Count == 0 )
            await discordMember.RevokeRoleAsync( ctx.Guild.GetRole( 917912578087338035 ) );

        // Guest role
        await discordMember.RevokeRoleAsync( ctx.Guild.GetRole( 917912577768566839 ) );
        await discordMember.GrantRoleAsync( ctx.Guild.GetRole( role.Id ) );

        await ctx.RespondAsEphemeralAsync( $"Successfully marked {user.Mention} as interviewed." );
    }

    [SlashCommand( "list", "Lists the members who need an interview for a role." )]
    public async Task ListCommandAsync( InteractionContext ctx, [Option( "role", "The role to check" )] DiscordRole role, [Option("silent", "Should the message be sent as a silent response")] bool silent = true )
    {
        var list = new List<DiscordMember>();
        
        foreach ( var member in Member.All )
        {
            if ( member._interviewNeededRoleIds.Contains( role.Id ) )
            {
                var dmember = await member.GetDiscordMemberAsync();
                
                if ( dmember is not null )
                    list.Add( dmember );
            }
        }

        string message = $"Users in need of an interview for {role.Mention} [{list.Count}]: ";

        foreach ( var member in list )
            message += $"{member.Mention} ";

        message = message.Trim();

        if ( list.Count == 0 )
            message = $"No one needs an interview for {role.Mention}.";

        if ( silent )
            await ctx.RespondAsEphemeralAsync( message );
        else
            await ctx.RespondAsync( message );
    }
}