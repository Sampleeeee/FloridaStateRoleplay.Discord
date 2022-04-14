using System.Reflection.Metadata;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using FloridaStateRoleplay.Discord.Commands;
using FloridaStateRoleplay.Discord.Entities;
using FloridaStateRoleplay.Discord.Extensions;
using FuzzySharp;
using Timer = System.Timers.Timer;

namespace FloridaStateRoleplay.Discord;

public class Program
{
    public const ulong GuildId = 917912577768566835;
    public const ulong MutedRoleId = 917912578179604544;
    public const ulong ModLogChannelId = 917912581690232952;
    public const ulong LogChannelId = 932434964010655784;
    public const ulong GuestRoleId = 917912577768566839;    
    
    public static DiscordGuild FloridaStateRoleplay { get; set; }
    public static DiscordClient Discord { get; set; }

    private static void Main( string[] args )
    {
        Config.Current = new Config();
        
        new Program().MainAsync().GetAwaiter().GetResult();
    }

    public async Task MainAsync()
    {
        Config.Initalize();
        
        Discord = new DiscordClient( new DiscordConfiguration
        {
            Token = Config.Current.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        } );

        FloridaStateRoleplay = await Discord.GetGuildAsync( GuildId );
        await Member.Initialize();

        var commands = Discord.UseSlashCommands();
        commands.RegisterCommands<UtilityCommands, InformationCommands, OldModerationCommands>( 917912577768566835 );

        var timer = new Timer
        {
            Interval = 2 * 60 * 1000,
            AutoReset = true
        };

        var interactivity = Discord.UseInteractivity();

        timer.Elapsed += ( _, _ ) => TimerElapsed().GetAwaiter().GetResult();
        timer.Start();
        
        Discord.GuildMemberAdded += OnMemberAdded;
        Discord.GuildMemberRemoved += OnMemberRemoved;
        Discord.MessageCreated += OnMessageCreated;
        Discord.ComponentInteractionCreated += OnComponentInteractionCreated;
        Discord.ModalSubmitted +=  HandleSuggestionModalSubmit;

        Discord.GuildDownloadCompleted += async ( _, args ) =>
        {
            if ( !args.Guilds.ContainsKey( 917912577768566835 ) )
                throw new Exception();

            FloridaStateRoleplay = args.Guilds[917912577768566835];

            // await Task.Delay( 20000 );
            await TimerElapsed();
        };
        
        commands.SlashCommandErrored += OnSlashCommandErrored;

        await Discord.ConnectAsync();
        await Task.Delay( -1 );
    }

    private async Task TimerElapsed()
    {
        await UpdateMembers();
        await HandleThreads();
    }

    private async Task HandleThreads()
    {
        foreach ( ( ulong channelId, ulong threadId ) in Config.Current.MediaOnlyChannels )
        {
            var channel = FloridaStateRoleplay.GetChannel( channelId );
            var thread = ( await channel.ListPublicArchivedThreadsAsync() ).Threads.FirstOrDefault( x => x.Id == threadId );

            if ( thread?.ThreadMetadata.IsArchived ?? false )
                await thread.SendMessageAsync( "This thread is no longer archived." );
        }
    }

    private async Task HandleSuggestionModalSubmit( DiscordClient sender, ModalSubmitEventArgs e )
    {
        await HandleSuggestionModalSubmit( e );
        await HandleBugReportModalSubmit( e );
    }

    private async Task OnComponentInteractionCreated( DiscordClient sender, ComponentInteractionCreateEventArgs e )
    {
        await HandleSuggestionButton( e );
        await HandleBugReportButton( e );
    }

    private async Task HandleSuggestionModalSubmit( ModalSubmitEventArgs e )
    {
        if ( e.Interaction.Data.CustomId != "feature-request" ) return;

        var channel = e.Interaction.Guild.GetChannel( 917912579660197956 );

        var button = new DiscordButtonComponent( ButtonStyle.Primary, "create_feature_request",
            "Create Feature Request" );

        var builder = new DiscordMessageBuilder()
            .WithContent(
                $"__**Quick Description**__\n{e.Values["fr-desc"]}\n\n__**Additional Context**__\n{e.Values["fr-context"]}\n\n__**Links**__\n{e.Values["fr-links"]}\n\nSubmitted by: <@{e.Interaction.User.Id}>" )
            .AddComponents( button );

        var message = await channel.SendMessageAsync( builder );
        await message.CreateThreadAsync( e.Values["fr-title"], AutoArchiveDuration.Week );
    }

    private async Task HandleBugReportModalSubmit( ModalSubmitEventArgs e )
    {
        var button =
            new DiscordButtonComponent( ButtonStyle.Primary, "create_bug_report", "Create Bug Report" );

        var builder = new DiscordMessageBuilder()
            .WithContent(
                $"__**Describe the bug**__\n{e.Values["br-desc"]}\n\n__**To Reproduce**__\n{e.Values["br-steps"]}\n\n__**Expected Behavior**__\n{e.Values["br-behavior"]}\n\n__**Screenshots**__\n{e.Values["br-screenshots"]}\n\nSubmitted by: <@{e.Interaction.User.Id}>" )
            .AddComponents( button );

        var channel = e.Interaction.Guild.GetChannel( 917912579660197955 );
        
        var message = await channel.SendMessageAsync( builder );
        await message.CreateThreadAsync( e.Values["br-title"], AutoArchiveDuration.Week );
    }

    private async Task HandleSuggestionButton( ComponentInteractionCreateEventArgs e )
    {
        if ( e.Id != "create_feature_request" ) return;

        var modal = UtilityCommands.GetFeatureRequestModal();
        await e.Interaction.CreateResponseAsync( InteractionResponseType.Modal, modal );
    }

    private async Task HandleBugReportButton( ComponentInteractionCreateEventArgs e )
    {
        if ( e.Id != "create_bug_report" ) return;

        var modal = UtilityCommands.GetBugReportModal();
        await e.Interaction.CreateResponseAsync( InteractionResponseType.Modal, modal );
    }
    
    private Task OnSlashCommandErrored( SlashCommandsExtension sender, SlashCommandErrorEventArgs e )
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = $"Command `/{e.Context.CommandName}` Errored",
            Description = $"{e.Context.Member.Mention} has ran `/{e.Context.CommandName}` and encountered an exception of type `{e.Exception.GetType()}`.",
            Color = DiscordColor.DarkRed
        };

        if ( e.Exception is not BadRequestException badRequest )
        {
            embed.AddField( $"`{e.Exception.GetType()}.Message`", $"```{e.Exception.Message}```" );
            embed.AddField( $"`{e.Exception.GetType()}.StackTrace`", $"```{e.Exception.StackTrace}```" );
        }
        else
        {
            embed.AddField( "`BadRequestException.JsonMessage`", $"```{badRequest.JsonMessage}```" );
            embed.AddField( "`BadRequestException.Errors`", $"```{badRequest.Errors}```" );
        }
        
        e.Context.Channel.SendMessageAsync( embed );
        return Task.CompletedTask;
    }

    private async Task OnMessageCreated( DiscordClient sender, MessageCreateEventArgs e )
    {
        if ( await DeleteLinksAsync( e ) ) return;
        if ( await DeleteBlacklistedWordsAsync( e ) ) return;
        if ( await DeleteMediaOnlyMessagesAsync( e ) ) return;
        
        await HandleLevels( e );
        await HandleSticky( e );
    }

    private async Task<bool> DeleteMediaOnlyMessagesAsync( MessageCreateEventArgs e )
    {
        if ( e.Author.IsBot ) 
            return false;

        if ( e.Message.Content.ToLower().Contains( "http://" ) || e.Message.Content.Contains( "https://" ) )
            return false;

        if ( e.Message.Attachments.Count > 0 )
            return false;

        var inMediaOnlyChannel = false;

        foreach ( var x in Config.Current.MediaOnlyChannels )
        {
            if ( x.ChannelId != e.Channel.Id )
                continue;
            
            inMediaOnlyChannel = true;
            break;
        }

        if ( !inMediaOnlyChannel )
            return false;

        var member = await e.Guild.GetMemberAsync( e.Author.Id );

        if ( member.IsManagemenet() )
            return false;
        
        await e.Message.DeleteAsync();
        await member.CreateDmChannelAsync();
        
        await member.SendMessageAsync(
            $"Your message in #{e.Channel.Name} was deleted. You may only upload media to this channel." );
            
        return true;
    }

    private async Task HandleSticky( MessageCreateEventArgs e )
    {
        var sticky = Config.Current.Stickies.FirstOrDefault( x => x.Channel == e.Channel.Id );
        if ( sticky is null ) return;
        if ( sticky.LastMessage == e.Message.Id ) return;

        if ( sticky.LastMessage != 0 )
        {
            var message = await e.Channel.GetMessageAsync( sticky.LastMessage );
            try
            {
                await message.DeleteAsync();
            }
            catch { }
        }

        var response = await e.Channel.SendMessageAsync( $"__**{sticky.Title}**__\n{sticky.Message}" );
        sticky.LastMessage = response?.Id ?? 0;
    }
    
    /// <returns>Was there a link deleted?</returns>
    private async Task<bool> DeleteLinksAsync( MessageCreateEventArgs e )
    {
        // TODO comment this out until we have a list of whitelisted links
        
        /*
        var text = e.Message.Content.ToLower();
        
        if ( text.Contains( "https://" ) || text.Contains( "http://" ) )
        {
            await e.Message.DeleteAsync();
            return true;
        }
        
        if ( text.Contains( "discord.gg" ) && !text.Contains( "discord.gg/floridasrp" ) )
        {
            await e.Message.DeleteAsync();
            return true;
        }
        */
        
        return false;
    }
    
    /// <returns>Was there a word deleted?</returns>
    private async Task<bool> DeleteBlacklistedWordsAsync( MessageCreateEventArgs e )
    {
        if ( !e.Message.Content.Split( " " )
                .SelectMany( _ => Config.Current.BlacklistedWords, ( str, badWord ) => new { str, badWord } )
                .Where( t => Fuzz.Ratio( t.str, t.badWord ) >= Config.Current.BlacklistedWordMaxRatio )
                .Select( t => t.str ).Any() ) return false;
        
        await e.Message.DeleteAsync();
        return true;
    }

    private async Task HandleLevels( MessageCreateEventArgs e )
    {
        if ( !Config.Current.LevelingEnabled ) return;
        if ( e.Author.IsBot ) return;

        var member = await Member.FromId( e.Author.Id );
        
        if ( member is null ) return;
        if ( member.NextXpDrop >= DateTime.Now ) return;

        float modifier = Math.Max( e.Message.Attachments.Count, 5f ) / 10;
        modifier += 1f;

        await member.AddExperienceAsync( e.Guild, e.Author, true, modifier );
        await member.SaveAsync();
    }

    private async Task OnMemberRemoved( DiscordClient sender, GuildMemberRemoveEventArgs e )
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Guild Member Removed",
            Description = $"{e.Member.Username}#{e.Member.Discriminator} [{e.Member.Id}] has left the server.",
            Color = DiscordColor.Red
        };

        await StaffLogAsync( LogChannelId, embed );
    }

    private async Task OnMemberAdded( DiscordClient sender, GuildMemberAddEventArgs e )
    {
        await e.Member.GrantRoleAsync( e.Guild.GetRole( GuestRoleId ) );

        var embed = new DiscordEmbedBuilder
        {
            Title = "Guild Member Added",
            Description = $"{e.Member.Mention} has joined the server.",
            Color = DiscordColor.Green
        };

        embed.AddField( "Account Creation Date", $"{e.Member.CreationTimestamp:D}", true );
        // embed.AddField( "", $"{e.Member.}", true );
        
        await StaffLogAsync( LogChannelId, embed );
    }

    public static async Task StaffModLogAsync( DiscordEmbedBuilder builder )
    {
        builder.Color = DiscordColor.Red;

        await StaffLogAsync( ModLogChannelId, builder );
    }

    public static async Task StaffLogAsync( ulong id, DiscordEmbedBuilder builder )
    {
        await FloridaStateRoleplay.GetChannel( id ).SendMessageAsync( builder );
    }

    private async Task UpdateMembers()
    {
        #region Birthdays
        
        foreach ( var member in Member.All.Where( x => x.Birthday?.Date == DateTime.Today ) )
        {
            if ( member.Birthday is null ) continue;
            if ( member.Birthday.Awarded ) continue;

            var general = FloridaStateRoleplay.GetChannel( 923603635898834995 );

            await general.SendMessageAsync( member.Mention, new DiscordEmbedBuilder
            {
                Title = "Happy Birthday! 🎉",
                Description = $"Happy birthday {member.Mention}! 🎂",
                Color = DiscordColor.Cyan,
            } );

            member.Birthday.Awarded = true;
        }
        
        #endregion
        #region Punishments

        foreach ( var member in Member.All )
        {
            if ( member.Punishments.Count <= 0 ) continue;

            foreach ( var punishment in member.Punishments.Where( punishment => !punishment.Expired )
                         .Where( punishment => punishment.ExpiresAt <= DateTime.Now ) )
            {
                switch ( punishment.Type )
                {
                    case PunishmentType.Mute:
                        await member.Unmute( new Member { Id = 0 }, "Automatic" );
                        break;
                    case PunishmentType.Ban:
                        await member.Unban( new Member { Id = 0 }, "Automatic" );
                        break;
                }

                punishment.Expired = true;
            }
        }
        
        #endregion

        await Member.SaveAllAsync();
    }
}
