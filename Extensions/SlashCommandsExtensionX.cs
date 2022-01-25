using DSharpPlus.SlashCommands;

namespace FloridaStateRoleplay.Discord.Extensions;

public static class SlashCommandsExtensionX
{
    public static void RegisterCommands<t1, t2, t3>( this SlashCommandsExtension commands, ulong id )
        where t1 : ApplicationCommandModule
        where t2 : ApplicationCommandModule
        where t3 : ApplicationCommandModule
    {
        commands.RegisterCommands<t1>( id );
        commands.RegisterCommands<t2>( id );
        commands.RegisterCommands<t3>( id );
    }
}