using DSharpPlus.Entities;

namespace FloridaStateRoleplay.Discord.Extensions;

public static class DiscordMemberX
{
    public static bool IsGuest( this DiscordMember member )
    {
        return member.Roles.Contains( member.Guild.GetRole( 917912577768566839 ) );
    }

    public static bool IsStaff( this DiscordMember member )
    {
        return member.Roles.Contains( member.Guild.GetRole( 917912578087338043 ) );
    }

    public static bool IsManagemenet( this DiscordMember member )
    {
        return member.Roles.Contains( member.Guild.GetRole( 917912578213154911 ) );
    }

    public static bool CanKick( this DiscordMember member, DiscordMember target )
    {
        return member.Hierarchy > target.Hierarchy;
    }

    public static bool CanBan( this DiscordMember member, DiscordMember target )
    {
        return member.Hierarchy > target.Hierarchy;
    }

    public static bool CanMute( this DiscordMember member, DiscordMember target )
    {
        return member.Hierarchy > target.Hierarchy;
    }

    public static bool CanWarn( this DiscordMember member, DiscordMember target )
    {
        return true;
    }

    public static bool CanUnmute( this DiscordMember member, DiscordMember target )
    {
        return true;
    }

    public static bool CanUnban( this DiscordMember member, ulong id )
    {
        return true;
    }
}