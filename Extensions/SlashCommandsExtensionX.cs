using DSharpPlus.SlashCommands;

namespace FloridaStateRoleplay.Discord.Extensions;

public static class SlashCommandsExtensionX
{
    public static void RegisterCommands<T1>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
    }

    public static void RegisterCommands<T1, T2>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
    }

    public static void RegisterCommands<T1, T2, T3>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
        where T3 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
        commands.RegisterCommands<T3>( id );
    }

    public static void RegisterCommands<T1, T2, T3, T4>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
        where T3 : ApplicationCommandModule
        where T4 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
        commands.RegisterCommands<T3>( id );
        commands.RegisterCommands<T4>( id );
    }

    public static void RegisterCommands<T1, T2, T3, T4, T5>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
        where T3 : ApplicationCommandModule
        where T4 : ApplicationCommandModule
        where T5 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
        commands.RegisterCommands<T3>( id );
        commands.RegisterCommands<T4>( id );
        commands.RegisterCommands<T5>( id );
    }
}