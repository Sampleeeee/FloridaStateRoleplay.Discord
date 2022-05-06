using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FloridaStateRoleplay.Discord.Commands;

public class UtilityCommands : ApplicationCommandModule
{
    public static DiscordInteractionResponseBuilder GetBugReportModal()
    {
        var title = new TextInputComponent(
            label: "Thread Title",
            customId: "br-title",
            placeholder: "Bug report thread title",
            required: true,
            style: TextInputStyle.Short
        );

        var description = new TextInputComponent(
            label: "Describe the bug",
            customId: "br-desc",
            placeholder: "A clear and concise description of what the bug is.",
            required: true,
            style: TextInputStyle.Paragraph
        );

        var steps = new TextInputComponent(
            label: "Steps to reproduce",
            customId: "br-steps",
            placeholder: "Steps to reproduce the behavior:\n1. Go to '...'\n2. Click on '...'\n4. See error",
            required: true,
            style: TextInputStyle.Paragraph
        );

        var behavior = new TextInputComponent(
            label: "Expected Behavior",
            customId: "br-behavior",
            placeholder: "A clear and concise description of what you expected to happen.",
            required: true,
            style: TextInputStyle.Paragraph
        );

        var screenshots = new TextInputComponent(
            label: "Screenshots",
            customId: "br-screenshots",
            placeholder: "If applicable, add screenshots to help explain your problem.",
            required: false,
            style: TextInputStyle.Paragraph
        );

        var response = new DiscordInteractionResponseBuilder()
            .WithTitle( "Bug Report" )
            .WithCustomId( "bug-report" )
            .AddComponents( title )
            .AddComponents( description )
            .AddComponents( steps )
            .AddComponents( behavior )
            .AddComponents( screenshots );

        return response;
    }
    
    [SlashCommand( "bug", "Submit a new bug report." )]
    public async Task BugCommand( InteractionContext ctx )
    {
        if ( ctx.Member.Roles.Contains( ctx.Guild.GetRole( 917912577768566839 ) ) ) return;

        var response = GetBugReportModal();
        await ctx.Interaction.CreateResponseAsync( InteractionResponseType.Modal, response );
    }

    public static DiscordInteractionResponseBuilder GetFeatureRequestModal()
    {
        var title = new TextInputComponent(
            label: "Thread Title",
            customId: "fr-title",
            placeholder: "Feature request thread title",
            required: true,
            style: TextInputStyle.Short
        );

        var description = new TextInputComponent(
            label: "Quick Description",
            customId: "fr-desc",
            placeholder: "Please provide a clear and concise description of your feature request.",
            required: true,
            style: TextInputStyle.Paragraph
        );

        var context = new TextInputComponent(
            label: "Additional Context",
            customId: "fr-context",
            placeholder: "Please provide any other context or screenshots about the feature request here.",
            required: true,
            style: TextInputStyle.Paragraph
        );

        var links = new TextInputComponent(
            label: "Links",
            customId: "fr-links",
            placeholder: "Please provide any links that may be relevant to the feature request.",
            required: false,
            style: TextInputStyle.Paragraph
        );

        var response = new DiscordInteractionResponseBuilder()
            .WithTitle( "Feature Request" )
            .WithCustomId( "feature-request" )
            .AddComponents( title )
            .AddComponents( description )
            .AddComponents( context )
            .AddComponents( links );
        
        return response;
    }
    
    [SlashCommand( "suggestion", "Submit a new suggestion" )]
    public async Task SuggestionCommand( InteractionContext ctx )
    {
        if ( ctx.Member.Roles.Contains( ctx.Guild.GetRole( 917912577768566839 ) ) ) return;

        var response = GetFeatureRequestModal();
        await ctx.Interaction.CreateResponseAsync( InteractionResponseType.Modal, response );
    }
    
}