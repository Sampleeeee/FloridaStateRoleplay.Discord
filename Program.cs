using System.Reflection.Metadata;
using System.Text;
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
using Newtonsoft.Json;
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
        Config.Initialize();
        
        Discord = new DiscordClient( new DiscordConfiguration
        {
            Token = Config.Current.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        } );

        FloridaStateRoleplay = await Discord.GetGuildAsync( GuildId );
        await Member.Initialize();

        var commands = Discord.UseSlashCommands();
        commands.RegisterCommands<UtilityCommands, InformationCommands, OldModerationCommands, InterviewCommands, BanAppealCommands, TicketCommands>( 917912577768566835 );

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
        Discord.MessageDeleted += OnMessageDeleted;
        Discord.MessageUpdated += OnMessageUpdated;

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
        await HandleCreateTicketDropdownAsync( e );
    }

    private async Task HandleCreateTicketDropdownAsync( ComponentInteractionCreateEventArgs e )
    {
        Console.WriteLine( JsonConvert.SerializeObject( e ) );
    }

    private async Task HandleSuggestionModalSubmit( ModalSubmitEventArgs e )
    {
        if ( e.Interaction.Data.CustomId != "feature-request" ) return;

        var channel = e.Interaction.Guild.GetChannel( 917912579660197956 );

        var button = new DiscordButtonComponent( ButtonStyle.Primary, "create_feature_request",
            "Create Feature Request" );

        string content =
            $"__**Quick Description**__\n{e.Values["fr-desc"]}\n\n__**Additional Context**__\n{e.Values["fr-context"]}";

        if ( !string.IsNullOrWhiteSpace( e.Values["fr-links"] ) )
        {
            content += $"\n\n__**Links**__\n{e.Values["fr-links"]}";
        }

        content += $"\n\nSubmitted by: <@{e.Interaction.User.Id}>";

        var builder = new DiscordMessageBuilder()
            .WithContent( content )
            .AddComponents( button );

        var message = await channel.SendMessageAsync( builder );
        var thread = await message.CreateThreadAsync( e.Values["fr-title"], AutoArchiveDuration.Week );
        
        var ping = await thread.SendMessageAsync( $"<@{e.Interaction.User.Id}>" );
        await ping.DeleteAsync();

        await e.Interaction.CreateResponseAsync( InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral().WithContent( "Suggestion Submitted" ) );
    }

    private async Task HandleBugReportModalSubmit( ModalSubmitEventArgs e )
    {
        var button =
            new DiscordButtonComponent( ButtonStyle.Primary, "create_bug_report", "Create Bug Report" );

        string content =
            $"__**Describe the bug**__\n{e.Values["br-desc"]}\n\n__**To Reproduce**__\n{e.Values["br-steps"]}\n\n__**Expected Behavior**__\n{e.Values["br-behavior"]}";

        if ( !string.IsNullOrWhiteSpace( e.Values["br-screenshots"] ) )
        {
            content += $"\n\n__**Screenshots**__\n{e.Values["br-screenshots"]}";
        }

        content += $"\n\nSubmitted by: <@{e.Interaction.User.Id}>";
        
        var builder = new DiscordMessageBuilder()
            .WithContent( content )
            .AddComponents( button );

        var channel = e.Interaction.Guild.GetChannel( 917912579660197955 );
        
        var message = await channel.SendMessageAsync( builder );
        var thread = await message.CreateThreadAsync( e.Values["br-title"], AutoArchiveDuration.Week );
        
        var ping = await thread.SendMessageAsync( $"<@{e.Interaction.User.Id}>" );
        await ping.DeleteAsync();
        
        await e.Interaction.CreateResponseAsync( InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral().WithContent( "Bug Report Submitted" ) );
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

    private async Task OnSlashCommandErrored( SlashCommandsExtension sender, SlashCommandErrorEventArgs e )
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = $"Command `/{e.Context.CommandName}` Errored",
            Description =
                $"{e.Context.Member.Mention} has ran `/{e.Context.CommandName}` and encountered an exception of type `{e.Exception.GetType()}`.",
            Color = DiscordColor.DarkRed
        };

        var type = e.Exception.GetType();

        var str =
            $@"<Information>
    <Time>{DateTime.Now:MM/dd/yyyy h:mm:ss tt}</Time>
    <Command>{e.Context.CommandName}</Command>
    <Channel>{e.Context.Channel.Id}</Channel>
    <RanBy>
        <DisplayName>{e.Context.Member.DisplayName}</DisplayName>
        <Id>{e.Context.Member.Id}</Id>
    </RanBy>
</Information>

<Exception>
    <Type>{type}</Type>
    <Message>{e.Exception.Message}</Message>
    <StackTrace>
        {e.Exception.StackTrace}
    </StackTrace>
";

        if ( e.Exception is BadRequestException badRequest )
        {
            str += $@"
    <JsonMessage>
        {badRequest.JsonMessage}
    </JsonMessage>
    <Errors>
        {badRequest.Errors}
    </Errors>";
        }

        str += "</Exception>";

        var path = $"./data/errors/{DateTime.Now:yy/MM/dd}/{e.Context.CommandName}";
        Directory.CreateDirectory( path );

        var id = Guid.NewGuid();
        var file = File.Create( $"{path}/{id}.xml" );
        var buffer = Encoding.Default.GetBytes( str );
        file.Write( buffer, 0, buffer.Length );
        file.Close();

        await e.Context.RespondAsEphemeralAsync(
            $"There was an error running this command. Please try again and contact server developers if the issue persists. `{id}`" );

        await using var fs = new FileStream( $"{path}/{id}.xml", FileMode.Open, FileAccess.Read );

        await FloridaStateRoleplay.GetChannel( Config.Current.ErrorLogChannel )
            .SendMessageAsync(
                new DiscordMessageBuilder()
                    .WithContent(
                        $"Command {e.Context.CommandName} errored! Ran by {e.Context.Member.Mention} in {e.Context.Channel.Mention}." )
                    .WithFiles( new Dictionary<string, Stream> { { $"{id}.xml", fs } } )
            );
    }

    private async Task OnMessageDeleted( DiscordClient sender, MessageDeleteEventArgs e )
    {
        if ( e.Message is null ) return;
        
        var member = await e.Guild.GetMemberOrNullAsync( e.Message.Author.Id );
        if ( member?.IsManagemenet() ?? false ) return;
        if ( e.Message.Author.IsBot ) return;
        
        ulong authorId = e.Message.Author.Id;
        string username = e.Message.Author.Username;
        string discriminator = e.Message.Author.Discriminator;

        ulong id = e.Message.Id;
        string content = e.Message.Content;
        ulong channelId = e.Channel.Id;
        string channelName = e.Channel.Name;
        
        string str = $@"<Time>{DateTime.Now:MM/dd/yyyy h:mm:ss tt}</Time>
<Message>
    <Id>{id}</Id>
    <Content>{content}</Content>
</Message>
<Author>
    <Id>{authorId}</Id>
    <Username>{username}</Username>
    <Discriminator>{discriminator}</Discriminator>
</Author>
<Channel>
    <Id>{channelId}</Id>
    <Name>{channelName}</Name>
</Channel>";

        string path = $"./data/messages/{e.Channel.Id}/deleted";
        Directory.CreateDirectory( path );

        var file = File.Create( $"{path}/{e.Message.Id}.xml" );
        byte[] buffer = Encoding.Default.GetBytes( str );
        file.Write( buffer, 0, buffer.Length );
        file.Close();
        
        await using var fs = new FileStream( $"{path}/{e.Message.Id}.xml", FileMode.Open, FileAccess.Read );

        await FloridaStateRoleplay.GetChannel( Config.Current.MessageLogChannel )
            .SendMessageAsync(
                new DiscordMessageBuilder()
                    .WithContent(
                        $"__**Message Deleted**__" )
                    .WithFiles( new Dictionary<string, Stream> { { $"{e.Message.Id}.xml", fs } } )
            );
    }
    
    private async Task OnMessageUpdated( DiscordClient sender, MessageUpdateEventArgs e )
    {
        if ( e.Message is null || e.MessageBefore is null ) return;
        
        var member = await e.Guild.GetMemberOrNullAsync( e.Message.Author.Id );
        if ( member?.IsManagemenet() ?? false ) return;
        if ( e.Message.Author.IsBot ) return;

        if ( e.Message.Content == e.MessageBefore.Content ) return;
        
        ulong authorId = e.Message.Author.Id;
        string username = e.Message.Author.Username;
        string discriminator = e.Message.Author.Discriminator;
        
        ulong channelId = e.Channel.Id;
        string channelName = e.Channel.Name;
        
        string str = $@"<Time>{DateTime.Now:MM/dd/yyyy h:mm:ss tt}</Time>
<OriginalMessage>
    <Id>{e.MessageBefore.Id}</Id>
    <Content>{e.MessageBefore.Content}</Content>
</OriginalMessage>
<AfterMessage>
    <Id>{e.Message.Id}</Id>
    <Content>{e.Message.Content}</Content>
</AfterMessage>
<Author>
    <Id>{authorId}</Id>
    <Username>{username}</Username>
    <Discriminator>{discriminator}</Discriminator>
</Author>
<Channel>
    <Id>{channelId}</Id>
    <Name>{channelName}</Name>
</Channel>";

        var path = $"./data/messages/{e.Channel.Id}/deleted";
        Directory.CreateDirectory( path );

        var file = File.Create( $"{path}/{e.Message.Id}.xml" );
        var buffer = Encoding.Default.GetBytes( str );
        file.Write( buffer, 0, buffer.Length );
        file.Close();
        
        await using var fs = new FileStream( $"{path}/{e.Message.Id}.xml", FileMode.Open, FileAccess.Read );

        await FloridaStateRoleplay.GetChannel( Config.Current.MessageLogChannel )
            .SendMessageAsync(
                new DiscordMessageBuilder()
                    .WithContent(
                        $"__**Message Edited**__" )
                    .WithFiles( new Dictionary<string, Stream> { { $"{e.Message.Id}.xml", fs } } )
            );
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

        var content = e.Message.Content.ToLower();

        if ( content.Contains( "http://" ) || content.Contains( "https://" ) )
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
