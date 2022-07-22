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
    
    public static void RegisterCommands<T1, T2, T3, T4, T5, T6>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
        where T3 : ApplicationCommandModule
        where T4 : ApplicationCommandModule
        where T5 : ApplicationCommandModule
        where T6 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
        commands.RegisterCommands<T3>( id );
        commands.RegisterCommands<T4>( id );
        commands.RegisterCommands<T5>( id );
        commands.RegisterCommands<T6>( id );
    }
    
    public static void RegisterCommands<T1, T2, T3, T4, T5, T6, T7>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
        where T3 : ApplicationCommandModule
        where T4 : ApplicationCommandModule
        where T5 : ApplicationCommandModule
        where T6 : ApplicationCommandModule
        where T7 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
        commands.RegisterCommands<T3>( id );
        commands.RegisterCommands<T4>( id );
        commands.RegisterCommands<T5>( id );
        commands.RegisterCommands<T6>( id );
        commands.RegisterCommands<T7>( id );
    }
    
    public static void RegisterCommands<T1, T2, T3, T4, T5, T6, T7, T8>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
        where T3 : ApplicationCommandModule
        where T4 : ApplicationCommandModule
        where T5 : ApplicationCommandModule
        where T6 : ApplicationCommandModule
        where T7 : ApplicationCommandModule
        where T8 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
        commands.RegisterCommands<T3>( id );
        commands.RegisterCommands<T4>( id );
        commands.RegisterCommands<T5>( id );
        commands.RegisterCommands<T6>( id );
        commands.RegisterCommands<T7>( id );
        commands.RegisterCommands<T8>( id );
    }
    
    public static void RegisterCommands<T1, T2, T3, T4, T5, T6, T7, T8, T9>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
        where T3 : ApplicationCommandModule
        where T4 : ApplicationCommandModule
        where T5 : ApplicationCommandModule
        where T6 : ApplicationCommandModule
        where T7 : ApplicationCommandModule
        where T8 : ApplicationCommandModule
        where T9 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
        commands.RegisterCommands<T3>( id );
        commands.RegisterCommands<T4>( id );
        commands.RegisterCommands<T5>( id );
        commands.RegisterCommands<T6>( id );
        commands.RegisterCommands<T7>( id );
        commands.RegisterCommands<T8>( id );
        commands.RegisterCommands<T9>( id );
    }
    
    public static void RegisterCommands<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( this SlashCommandsExtension commands, ulong id )
        where T1 : ApplicationCommandModule
        where T2 : ApplicationCommandModule
        where T3 : ApplicationCommandModule
        where T4 : ApplicationCommandModule
        where T5 : ApplicationCommandModule
        where T6 : ApplicationCommandModule
        where T7 : ApplicationCommandModule
        where T8 : ApplicationCommandModule
        where T9 : ApplicationCommandModule
        where T10 : ApplicationCommandModule
    {
        commands.RegisterCommands<T1>( id );
        commands.RegisterCommands<T2>( id );
        commands.RegisterCommands<T3>( id );
        commands.RegisterCommands<T4>( id );
        commands.RegisterCommands<T5>( id );
        commands.RegisterCommands<T6>( id );
        commands.RegisterCommands<T7>( id );
        commands.RegisterCommands<T8>( id );
        commands.RegisterCommands<T9>( id );
        commands.RegisterCommands<T10>( id );
    }
}