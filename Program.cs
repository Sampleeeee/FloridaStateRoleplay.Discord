using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Commands;
using FloridaStateRoleplay.Discord.Entities;
using FloridaStateRoleplay.Discord.Extensions;
using Newtonsoft.Json;

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
        Discord = new DiscordClient( new DiscordConfiguration
        {
            Token = Config.Current.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        } );

        FloridaStateRoleplay = await Discord.GetGuildAsync( GuildId );
        await Member.Initialize();

        var commands = Discord.UseSlashCommands();
        commands.RegisterCommands<UtilityCommands, InformationCommands, ModerationCommands>( 917912577768566835 );

        var timer = new System.Timers.Timer
        {
            Interval = 2 * 60 * 1000,
            AutoReset = true
        };

        timer.Elapsed += ( _, _ ) => UpdateMembers().GetAwaiter().GetResult();
        timer.Start();
        
        Discord.GuildMemberAdded += OnMemberAdded;
        Discord.GuildMemberRemoved += OnMemberRemoved;
        Discord.MessageCreated += OnMessageCreated;

        await Discord.ConnectAsync();
        await Task.Delay( -1 );
    }

    private async Task OnMessageCreated( DiscordClient sender, MessageCreateEventArgs e )
    {
        await HandleLevels( e );
    }

    private async Task HandleLevels( MessageCreateEventArgs e )
    {
        if ( e.Author.IsBot ) return;
        
        var member = await Member.FromId( e.Author.Id );
        
        if ( member is null ) return;
        if ( member.NextXpDrop >= DateTime.Now ) return;

        await member.AddExperienceAsync( e.Guild, e.Author );
        await member.Save();
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

        await Member.SaveAll();
    }
}