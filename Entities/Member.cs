using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Emzi0767.Utilities;
using Newtonsoft.Json;

namespace FloridaStateRoleplay.Discord.Entities;

public class Member
{
    private static Dictionary<ulong, Member> _members = new();
    
    public ulong Id { get; set; }
    public Birthday? Birthday { get; set; }
    
    public List<Punishment> Punishments { get; set; } = new();

    public ulong Experience { get; set; } = 0ul;

    public uint Level
    {
        get
        {
            if ( Experience == 0 ) return 0;
            
            for ( uint level = 0; level < Convert.ToUInt32( Config.Current.Ranks.Length ); level++ )
            {
                ulong xpNeeded = Config.Current.Ranks[level] / 2;

                if ( Experience <= xpNeeded )
                {
                    return level - 1;
                }
            }

            return 0;
        }
    }

    [JsonIgnore] private DiscordMember _discordMember;
    
    [JsonIgnore]
    public DiscordMember DiscordMember
    {
        get
        {
            if ( _discordMember is null )
                _discordMember = Program.FloridaStateRoleplay.GetMemberAsync( Id ).GetAwaiter().GetResult();

            return _discordMember;
        }
    }
    
    [JsonIgnore] public string Mention => DiscordMember?.Mention ?? $"<@{Id}>";
    
    [JsonIgnore] public bool Muted => 
        DiscordMember?.Roles.Contains( Program.FloridaStateRoleplay.GetRole( Program.MutedRoleId ) ) ?? false;

    [JsonIgnore] public DateTime NextXpDrop = DateTime.MinValue;
    
    public static async Task<Member> FromId( ulong id )
    {
        if ( _members.ContainsKey( id ) )
            return _members[id];

        var member = new Member
        {
            Id = id
        };

        _members[id] = member;
        return member;
    }

    public async Task Unmute( Member staff, string reason )
    {
        if ( DiscordMember is null ) return;

        var lastMute = Punishments.FirstOrDefault( x => x.Type == PunishmentType.Mute && x.Expired == false );
        if ( lastMute is null ) return;

        lastMute.Expired = true;
        lastMute.RevokeReason = reason;
        lastMute.RevokerId = staff.Id;
        
        await DiscordMember.RevokeRoleAsync( Program.FloridaStateRoleplay.GetRole( Program.MutedRoleId ) );
        await SaveAsync();
    }

    public async Task Unban( Member staff, string reason )
    {
        var lastBan = Punishments.FirstOrDefault( x => x.Type == PunishmentType.Ban && x.Expired == false );
        if ( lastBan is null ) return;

        lastBan.Expired = true;
        lastBan.RevokeReason = reason;
        lastBan.RevokerId = staff.Id;
        
        await Program.FloridaStateRoleplay.UnbanMemberAsync( Id );
        await SaveAsync();
    }

    public async Task Mute( Member staff, TimeAway length, string reason )
    {
        Punishments.Add( new Punishment
        {
            Reason = reason,
            Type = PunishmentType.Mute,
            InvokedAt = DateTime.Now,
            ExpiresAt = DateTime.Now + (TimeSpan) length,
            StaffId = staff.Id,
            TargetId = Id
        } );

        var discordMember = await Program.FloridaStateRoleplay.GetMemberAsync( Id );
        await discordMember.GrantRoleAsync( Program.FloridaStateRoleplay.GetRole( Program.MutedRoleId ) );
        await SaveAsync();
    }
    
    public async Task Kick( Member staff, string reason )
    {
        Punishments.Add( new Punishment
        {
            Reason = reason,
            Type = PunishmentType.Kick,
            InvokedAt = DateTime.Now,
            StaffId = staff.Id,
            TargetId = Id
        } );

        var discordMember = await Program.FloridaStateRoleplay.GetMemberAsync( Id );
        await discordMember.RemoveAsync( reason );
        await SaveAsync();
    }

    public async Task Ban( Member staff, TimeAway length, string reason )
    {
        Punishments.Add( new Punishment
        {
            Reason = reason,
            Type = PunishmentType.Ban,
            InvokedAt = DateTime.Now,
            ExpiresAt = DateTime.Now + length.ToTimeSpan(),
            StaffId = staff.Id,
            TargetId = Id
        } );
        
        await Program.FloridaStateRoleplay.BanMemberAsync( Id, reason: reason );
        await SaveAsync();
    }

    public async Task Warn( Member staff, string reason )
    {
        Punishments.Add( new Punishment
        {
            Reason = reason,
            Type = PunishmentType.Warn,
            InvokedAt = DateTime.Now,
            StaffId = staff.Id,
            TargetId = Id
        });

        await SaveAsync();
    }
    
    public static IEnumerable<Member> All
    {
        get
        {
            var members = new List<Member>();
            foreach ( ( ulong id, var member ) in _members )
                members.Add( member );

            return members;
        }
    }

    public static async Task Initialize()
    {
        const string path = "./data/members.json";

        string json = await File.ReadAllTextAsync( path );
        _members = JsonConvert.DeserializeObject<Dictionary<ulong, Member>>( json ) ?? new Dictionary<ulong, Member>();
    }

    public async Task SaveAsync()
    {
        await SaveAllAsync();
    }

    public static async Task SaveAllAsync()
    {
        const string path = "./data/members.json";

        string json = JsonConvert.SerializeObject( _members );
        await File.WriteAllTextAsync( path, json );
    }

    public static async Task<Member> FromUser( DiscordUser? user )
    {
        if ( user is null ) return null;
        return await FromId( user.Id );
    }

    public async Task AddExperienceAsync( DiscordGuild guild, DiscordUser user, bool sendMessage = true, float modifier = 1f )
    {
        uint beforeLevel = Level;
        ulong beforeXp = Experience;

        Experience += Convert.ToUInt64( new Random().Next( 0, 31 ) * modifier );
        Console.WriteLine( $"Updating {DiscordMember?.DisplayName ?? user.Username }'s xp. {beforeXp} => {Experience}" );

        if ( beforeLevel != 0 && sendMessage )
            if ( beforeLevel != Level )
                await guild.GetChannel( Config.Current.LevelUpChannelId )
                    .SendMessageAsync( $"{Mention} has leveled up to level {Level}!" );

        NextXpDrop = DateTime.Now + TimeSpan.FromMinutes( 0.5 );
    }
}