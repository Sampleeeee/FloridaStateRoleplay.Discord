using DSharpPlus.Entities;

namespace FloridaStateRoleplay.Discord.Extensions;

public static class DiscordGuildX
{
    public static async Task<DiscordMember?> GetMemberOrNullAsync( this DiscordGuild guild, ulong id )
    {
        DiscordMember? member = null;

        try
        {
            member = await guild.GetMemberAsync( id );
        }
        catch
        {
            // ignored
        }

        return member;
    }
}