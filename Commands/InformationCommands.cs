using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Entities;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Commands;

public class InformationCommands : ApplicationCommandModule
{
    [SlashCommand( "cad", "Information about our CAD system." )]
    public async Task CadCommandAsync( InteractionContext ctx )
    {
        const string str = "All information about our cad can be found here: https://docs.floridasrp.com/tutorials/cad";
        await ctx.RespondAsync( str );
    }

    [SlashCommand( "roster", "Link to our community roster." )]
    public async Task RosterCommandAsync( InteractionContext ctx )
    {
        const string str = "Our community roster can be found at https://docs.floridasrp.com/important-links/roster";

        // if ( ctx.Member.IsGuest() )
            // await ctx.RespondAsEphemeralAsync( str );
        // else
            await ctx.RespondAsync( str );
    }

    [SlashCommand( "teamspeak", "Our TeamSpeak connection address." )]
    public async Task TeamspeakCommandAsync( InteractionContext ctx )
    {
        const string str = "You can join our teamspeak by connecting to ts.floridasrp.com";

        // if ( ctx.Member.IsGuest() )
            // await ctx.RespondAsEphemeralAsync( str );
        // else
            await ctx.RespondAsync( str );
    }

    [SlashCommand( "rules", "Our server rules" )]
    public async Task RulesCommandAsync( InteractionContext ctx )
    {
        const string str = "You can find our server rules here: https://docs.floridasrp.com/important-links/rules";

        // if ( ctx.Member.IsGuest() )
            // await ctx.RespondAsEphemeralAsync( str );
        // else
            await ctx.RespondAsync( str );
    }

    [SlashCommand( "ip", "Get a link to join our server." )]
    public async Task IpCommandAsync( InteractionContext ctx )
    {
        const string str = "You can join our server by clicking here: https://cfx.re/join/bvkvzp";
        await ctx.RespondAsync( str );
    }

    [SlashCommand( "banappeal", "Get a link to create a ban appeal." )]
    public async Task BanAppealCommmandAsync( InteractionContext ctx )
    {
        const string str = "You can create a ban appeal by reacting to this message: https://canary.discord.com/channels/917912577768566835/917912578708090882/924427892434296862";
        await ctx.RespondAsync( str );
    }

    [SlashCommand( "pingjay", "ping jay and maybe if you're lucky say something other than hi" )]
    public async Task PingJayCommandAsync( InteractionContext ctx )
    {
        var list = new List<string>
        {
            "gay jay",
            "howdy",
            "how was chick fil a",
            "give me all of your money",
            "hey baby girl can i get your number"
        };

        for ( var i = 0; i < 500 - list.Count; i++ )
            list.Add( "hi" );

        await ctx.RespondAsync( $"<@285945823311953922> {list[new Random().Next( list.Count )]}" );
    }

    [SlashCommand( "birthday", "Set your birthday!" )]
    public async Task BirthdayCommandAsync( InteractionContext ctx, [Option("birthday", "Your birthdate, or 'remove' to remove your birthday from our database.")] string input = "remove" )
    {
        var member = await Member.FromId( ctx.Member.Id );
        if ( member.DiscordMember is null ) return;

        if ( input is "" or "remove" )
        {
            member.Birthday = null;
            await member.SaveAsync();

            return;
        }
        
        bool success = DateTime.TryParse( input, out var date );

        if ( !success )
            await ctx.RespondAsEphemeralAsync( "Please enter a valid date." );
        else
        {
            member.Birthday = new Birthday { Date = date.Date };
            await member.SaveAsync();
            
            await ctx.RespondAsEphemeralAsync( $"We have updated your birthday to be {date.Date:d}" );
        }
    }

    [SlashCommand( "getallbirthdays", "Get every member's birthday in our database." )]
    public async Task GetAllBirthdaysCommandAsync( InteractionContext ctx )
    {
        string response = ( from member in Member.All where member.DiscordMember is not null where member.Birthday is not null select member ).Aggregate( string.Empty, ( current, member ) => current + $"**{member.DiscordMember.DisplayName}**: {member.Birthday.Date:d}\n" );

        if ( response == string.Empty )
            response = "No birthdays have been set!";
        
        await ctx.RespondAsync( response );
    }

    [SlashCommand( "getbirthday", "Get a member's birthday" )]
    public async Task GetBirthdayCommandAsync( InteractionContext ctx, [Option("mention", "the mention of the member you want to target")] DiscordUser user )
    {
        var member = await Member.FromId( user.Id );
        if ( member?.DiscordMember is null ) return;
        
        if ( member.Birthday is null )
            await ctx.RespondAsync(
                $"{member.DiscordMember.Mention} has not set a birthday yet! (they can do so by typing /birthday!)" );
        else
            await ctx.RespondAsync( $"{member.DiscordMember.Mention}'s birthday is on {member.Birthday.Date:d}" );
    }

    [SlashCommand( "whois", "Get information about a user" )]
    public async Task WhoIsCommandAsync( InteractionContext ctx,
        [Option( "user", "The user to get information about" )] DiscordUser user )
    {
        var dMember = await ctx.Guild.GetMemberAsync( user.Id );
        if ( dMember is null )
        {
            await ctx.RespondAsEphemeralAsync( "This user does not belong to this guild." );
            return;
        }

        var embed = new DiscordEmbedBuilder
        {
            Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"{dMember.Username}#{dMember.Discriminator}",
                IconUrl = dMember.AvatarUrl
            },
            Description = $"Information about {dMember.Mention}"
        };

        embed.AddField( "Joined", $"{dMember.JoinedAt:D}", true );
        embed.AddField( "Registered", $"{dMember.CreationTimestamp:D}", true );

        var member = await Member.FromUser( dMember );
        if ( member is null )
        {
            await ctx.RespondAsEphemeralAsync( "Failed to get user from database." );
            return;
        }
        
        if ( member.Birthday is not null )
            embed.AddField( "Birthday", $"{member.Birthday.Date:d}", true );

        embed.AddField( "Level", $"{member?.Level}", true );
        embed.AddField( "Experience", $"{member?.Experience}", true );
        
        var numRoles = 0;
        var roleMentions = string.Empty;
        foreach ( var role in dMember.Roles.OrderByDescending( x => x.Position ) )
        {
            roleMentions += $"{role.Mention} ";
            numRoles++;
        }

        embed.AddField( "Top Level Role", $"{dMember.Roles.OrderByDescending( x => x.Position ).FirstOrDefault()?.Mention}" );
        embed.AddField( $"Roles [{numRoles}]", roleMentions );

        await ctx.RespondAsync( embed );
    }

    [SlashCommand( "rank", "Get a user's rank" )]
    public async Task RankCommandAsync( InteractionContext ctx, [Option( "user", "The user's rank to get" )] DiscordUser user )
    {
        var member = await Member.FromUser( user );
        if ( member is null )
        {
            await ctx.RespondAsEphemeralAsync( "We were not able to find this user in your system." );
            return;
        }

        await ctx.RespondAsync(
            $"{member.Mention} is level {member.Level} ({member.Experience} experience)" );
    }

    [SlashCommand( "leaderboard", "Get level leaderboard for this server" )]
    public async Task LeaderboardCommandAsync( InteractionContext ctx )
    {
        IEnumerable<Member> top5 = Member.All.OrderByDescending( x => x.Experience ).Take( 5 );

        var response = string.Empty;

        foreach ( var member in top5 )
            response += $"{member.Mention} - Level: {member.Level}, Experience: {member.Experience}\n";

        await ctx.RespondAsync( response );
    }

    [SlashCommand( "listthreads", "a" )]
    private async Task ListThreadsCommandAsync( InteractionContext ctx )
    {
        await ctx.RespondAsEphemeralAsync( $"{ctx.Channel.Threads.Count}" );
    }
}