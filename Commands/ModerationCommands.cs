using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Entities;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Commands;

[SlashCommandGroup( "mod", "Moderation commands" )]
public class ModerationCommands : ApplicationCommandModule
{
    [SlashCommand( "kick", "Kick a member of the server" )]
    public async Task KickCommandAsync( InteractionContext ctx,
        [Option( "target", "The user to kick" )] DiscordUser user,
        [Option( "reason", "The reason for kicking this user." )] string reason )
    {
        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/kick' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );
            
            return;
        }
        
        var target = await ctx.Guild.GetMemberAsync( user.Id );
        if ( target is null )
        {
            await ctx.RespondAsEphemeralAsync( "We could not find that user." );
            return;
        }
        
        if ( !ctx.Member.CanKick( target ) )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to kick this user." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/kick' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );
            
            return;
        }

        var member = await Member.FromUser( target );
        var staff = await Member.FromUser( ctx.Member );

        if ( member is null || staff is null ) return;

        await ctx.RespondAsync( $"{member.Mention} was kicked by {staff.Mention} for reason: {reason}" );

        {
            var builder = new DiscordEmbedBuilder
            {
                Title = "User Kicked",
                Description =
                    $"{ctx.Member.Mention} ran command '/kick' on <@{user.Id}>."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );   
        }

        await member.Kick( staff, reason );
    }

    [SlashCommand( "warn", "Warn a member of the server" )]
    public async Task WarnCommandAsync( InteractionContext ctx,
        [Option( "target", "The user to kick" )]
        DiscordUser user,
        [Option( "reason", "The reason for kicking this user." )]
        string reason )
    {
        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/warn' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );
            return;
        }

        var target = await ctx.Guild.GetMemberAsync( user.Id );
        if ( target is null )
        {
            await ctx.RespondAsEphemeralAsync( "We could not find that user." );
            return;
        }

        if ( !ctx.Member.CanWarn( target ) )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to warn this user." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/warn' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );

            return;
        }

        var member = await Member.FromUser( target );
        var staff = await Member.FromUser( ctx.Member );

        if ( member is null || staff is null ) return;

        {
            var builder = new DiscordEmbedBuilder
            {
                Title = "User Warned",
                Description =
                    $"{ctx.Member.Mention} ran command '/warn' on <@{user.Id}>."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );
        }

        await ctx.RespondAsync( $"{member.Mention} was warned by {staff.Mention} for reason: {reason}" );
        await member.Warn( staff, reason );
    }

    [SlashCommand( "ban", "Ban a member of the server" )]
    public async Task BanCommandAsync( InteractionContext ctx,
        [Option( "target", "The user to ban" )] DiscordUser user,
        [Option("length", "The length of the ban")] string length,
        [Option( "reason", "The reason for banning this user." )] string reason )
    {
        var timeAway = new TimeAway( length );

        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/ban' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );
            builder.AddField( "Length", $"{timeAway.FormattedTime()}" );

            await Program.StaffModLogAsync( builder );
            return;
        }
        
        var target = await ctx.Guild.GetMemberAsync( user.Id );
        if ( target is null )
        {
            await ctx.RespondAsEphemeralAsync( "We could not find that user." );
            return;
        }

        if ( !ctx.Member.CanBan( target ) )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to ban this user." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/ban' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason, true );
            builder.AddField( "Length", $"{timeAway.FormattedTime()}", true );
            
            await Program.StaffModLogAsync( builder );
            
            return;
        }

        var member = await Member.FromUser( target );
        var staff = await Member.FromUser( ctx.Member );

        if ( member is null || staff is null ) return;

        {
            var builder = new DiscordEmbedBuilder
            {
                Title = "User Banned",
                Description =
                    $"{ctx.Member.Mention} ran command '/ban' on <@{user.Id}>."
            };

            builder.AddField( "Reason", reason );
            builder.AddField( "Length", $"{timeAway.FormattedTime()}" );

            await Program.StaffModLogAsync( builder );
        }

        await ctx.RespondAsync( $"{member.Mention} was banned by {staff.Mention} for {timeAway.FormattedTime()}, for reason: {reason}" );
        await member.Ban( staff, timeAway, reason );
        
    }

    [SlashCommand( "mute", "Mute a member of the server" )]
    public async Task MuteCommandAsync( InteractionContext ctx,
        [Option( "target", "The user to mute" )]
        DiscordUser user,
        [Option( "length", "The length of the mute" )]
        string length,
        [Option( "reason", "The reason for muting this user" )]
        string reason )
    {
        var timeAway = new TimeAway( length );

        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/mute' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason, true );
            builder.AddField( "Length", $"{timeAway.FormattedTime()}" );
            
            await Program.StaffModLogAsync( builder );
            return;
        }

        var target = await ctx.Guild.GetMemberAsync( user.Id );
        if ( target is null )
        {
            await ctx.RespondAsEphemeralAsync( "We could not find that user." );
            return;
        }

        if ( !ctx.Member.CanMute( target ) )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to mute this user." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/mute' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason, true );
            builder.AddField( "Length", $"{timeAway.FormattedTime()}" );
            
            await Program.StaffModLogAsync( builder );
            return;
        }

        {
            var builder = new DiscordEmbedBuilder
            {
                Title = "User Muted",
                Description =
                    $"{ctx.Member.Mention} ran command '/mute' on <@{user.Id}>."
            };

            builder.AddField( "Reason", reason );
            builder.AddField( "Length", $"{timeAway.FormattedTime()}" );

            await Program.StaffModLogAsync( builder );
        }

        var member = await Member.FromUser( target );
        var staff = await Member.FromUser( ctx.Member );

        if ( member is null || staff is null ) return;

        await ctx.RespondAsync( $"{member.Mention} was muted by {staff.Mention} for {timeAway.FormattedTime()}, for reason: {reason}" );
        await member.Mute( staff, timeAway, reason );
    }

    [SlashCommand( "punishments", "List all of a user's warns" )]
    public async Task PunishmentsCommandAsync( InteractionContext ctx, [Option("target", "The user to check")] DiscordUser user )
    {
        var member = await Member.FromUser( user );

        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );
            return;
        }

        var builder = new DiscordEmbedBuilder
        {
            Title = $"{user.Username}'s Past Punishments",
            Color = DiscordColor.Red,
        };

        if ( member is null ) return;

        if ( member.Punishments.Count == 0 )
        {
            await ctx.RespondAsync( $"{user.Mention} does not have any past punishments." );
            return;
        }

        foreach ( var punishment in member.Punishments )
        {
            string staffName = $"{punishment.StaffId}";

            var staffMember = await ctx.Guild.GetMemberAsync( punishment.StaffId );
            if ( staffMember is not null )
                staffName = staffMember.DisplayName;

            switch ( punishment.Type )
            {
                case PunishmentType.Warn:
                    builder.AddField( $"Warned on {punishment.InvokedAt:d} by {staffName}", punishment.Reason, true );
                    break;
                case PunishmentType.Kick:
                    builder.AddField( $"Kicked on {punishment.InvokedAt:d} by {staffName}", punishment.Reason, true );
                    break;
                case PunishmentType.Mute:
                    builder.AddField( $"Muted on {punishment.InvokedAt:d} by {staffName} for {(punishment.ExpiresAt - punishment.InvokedAt)}", punishment.Reason, true );
                    break;
                case PunishmentType.Ban:
                    builder.AddField(
                        $"Banned on {punishment.InvokedAt:d} by <@{punishment.StaffId}> for {( punishment.ExpiresAt - punishment.InvokedAt )}",
                        punishment.Reason, true );
                    break;
            }
        }

        await ctx.RespondAsync( builder );
    }

    [SlashCommand( "unmute", "Unmute a user" )]
    public async Task UnmuteCommandAsync( InteractionContext ctx,
        [Option( "user", "The user to unmute" )] DiscordUser user,
        [Option( "reason", "The reason the user is being unmuted" )] string reason )
    {
        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/unmute' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );

            return;
        }

        var target = await ctx.Guild.GetMemberAsync( user.Id );
        if ( target is null )
        {
            await ctx.RespondAsEphemeralAsync( "We could not find that user." );
            return;
        }

        if ( !ctx.Member.CanUnmute( target ) )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to unmute this user." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/unmute' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );

            return;
        }

        var member = await Member.FromUser( target );
        var staff = await Member.FromUser( ctx.Member );

        if ( member is null || staff is null ) return;

        {
            var builder = new DiscordEmbedBuilder
            {
                Title = "User Unmuted",
                Description =
                    $"{ctx.Member.Mention} ran command '/unmute' on <@{user.Id}>."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );
        }

        await ctx.RespondAsync( $"{member.Mention} was unmuted by {staff.Mention} for reason: {reason}" );
        await member.Unmute( staff, reason );
    }
    
    [SlashCommand( "unban", "Unban a user" )]
    public async Task UnbanCommandAsync( InteractionContext ctx,
        [Option( "user", "The user to unmute" )] DiscordUser user,
        [Option( "reason", "The reason the user is being unbanned" )] string reason )
    {
        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/unban' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );

            return;
        }

        var target = await ctx.Guild.GetMemberAsync( user.Id );
        if ( target is null )
        {
            await ctx.RespondAsEphemeralAsync( "We could not find that user." );
            return;
        }

        if ( !ctx.Member.CanUnmute( target ) )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to unban this user." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/unban' on <@{user.Id}> but does not have the proper permissions."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );

            return;
        }

        var member = await Member.FromUser( target );
        var staff = await Member.FromUser( ctx.Member );

        if ( member is null || staff is null ) return;

        {
            var builder = new DiscordEmbedBuilder
            {
                Title = "User Unbanned",
                Description =
                    $"{ctx.Member.Mention} ran command '/unban' on <@{user.Id}>."
            };

            builder.AddField( "Reason", reason );

            await Program.StaffModLogAsync( builder );
        }

        await ctx.RespondAsync( $"{member.Mention} was unbanned by {staff.Mention} for reason: {reason}" );
        await member.Unban( staff, reason );
    }

    [SlashCommand( "awardmessages", "Add extra messages to boost a user's experience" )]
    public async Task AwardMessagesCommandAsync( InteractionContext ctx,
        [Option( "user", "The user to add messages to" )] DiscordUser user,
        [Option( "amount", "The amount of messages to award." )] long amount )
    {
        if ( !ctx.Member.IsManagemenet() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have the proper permissions to do this!" );
            return;
        }

        var member = await Member.FromId( user.Id );
        if ( member is null ) return;

        uint beforeLevel = member.Level;

        for ( var i = 0; i < amount; i++ )
            await member.AddExperienceAsync( ctx.Guild, user, false );

        if ( beforeLevel != 0 )
            if ( beforeLevel != member.Level )
                await ctx.Guild.GetChannel( Config.Current.LevelUpChannelId )
                    .SendMessageAsync( $"{member.Mention} has leveled up to level {member.Level}!" );

        await member.Save();
        await ctx.RespondAsEphemeralAsync( $"Successfully added {amount} messages to {member.Mention}" );
    }

    [SlashCommand( "sticky", "Stick a message to the bottom of the current channel." )]
    private async Task StickyCommandAsync( InteractionContext ctx,
        [Option( "title", "The title of the sticky" )] string title,
        [Option( "text", "The text of the sticky" )] string text )
    {
        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/sticky' but does not have the proper permissions."
            };

            builder.AddField( "Title", title );
            builder.AddField( "Reason", text );

            await Program.StaffModLogAsync( builder );

            return;
        }

        List<Sticky> stickies = Config.Current.Stickies.Where( x => x.Channel == ctx.Channel.Id ).ToList();

        foreach ( var sticky in stickies )
            Config.Current.Stickies.Remove( sticky );

        Config.Current.Stickies.Add( new Sticky
        {
            Title = title,
            Message = text,
            Channel = ctx.Channel.Id
        } );

        await ctx.Interaction.CreateResponseAsync( InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent( "Sticky updated." ) );
    }

    [SlashCommand( "unsticky", "Stick a message to the bottom of the current channel." )]
    private async Task UnstickyCommandAsync( InteractionContext ctx )
    {
        if ( !ctx.Member.IsStaff() )
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/unsticky' but does not have the proper permissions."
            };

            await Program.StaffModLogAsync( builder );

            return;
        }

        List<Sticky> stickies = Config.Current.Stickies.Where( x => x.Channel == ctx.Channel.Id ).ToList();

        foreach ( var sticky in stickies )
            Config.Current.Stickies.Remove( sticky );
        
        await ctx.Interaction.CreateResponseAsync( InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent( "Sticky removed." ) );
    }
}