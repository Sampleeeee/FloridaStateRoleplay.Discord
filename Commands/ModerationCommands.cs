using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FloridaStateRoleplay.Discord.Entities;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Commands;

[SlashCommandGroup( "mod", "Moderation Commands" )]
public class ModerationCommands : ApplicationCommandModule
{
    [SlashCommand( "kick", "Kick a member of the server" )]
    private async Task KickCommandAsync( InteractionContext ctx, [Option( "target", "The user you would like to kick." )] DiscordUser user, [Option( "reason", "The reason for your kick." )] string reason )
    {
        var staff = await Member.FromId( ctx.Member.Id );
        var target = await Member.FromId( user.Id );

        async Task InsufficientPermissions()
        {
            await ctx.RespondAsEphemeralAsync( "You do not have permission to use this command." );

            var builder = new DiscordEmbedBuilder
            {
                Title = "Insufficient Permissions",
                Description =
                    $"{ctx.Member.Mention} tried to run command '/kick' on <@{target.DiscordMember.Id}> but does not have the proper permissions."
            };

            await Program.StaffModLogAsync( builder );
        }
        
        if ( target.DiscordMember is null )
        {
            await ctx.RespondAsEphemeralAsync( "We could not find this user!" );
            return;
        }
        
        if ( !ctx.Member.IsStaff() )
        {
            await InsufficientPermissions();
            return;
        }

        if ( !ctx.Member.CanKick( target.DiscordMember ) )
        {
            await InsufficientPermissions();
            return;
        }

        await ctx.RespondAsync( $"{target.Mention} was kicked by {staff.Mention} for reason: {reason}" );
        
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

        await target.Kick( staff, reason );
    }
}