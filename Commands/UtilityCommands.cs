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

        var context = new TextInputComponent(
            label: "Additional Context",
            customId: "fr-screenshots",
            placeholder: "Add any other context about the problem here.",
            required: false,
            style: TextInputStyle.Paragraph
        );

        var response = new DiscordInteractionResponseBuilder()
            .WithTitle( "Bug Report" )
            .WithCustomId( "bug-report" )
            .AddComponents( title )
            .AddComponents( steps )
            .AddComponents( behavior )
            .AddComponents( screenshots )
            .AddComponents( context );

        return response;
    }
    
    [SlashCommand( "bug", "Submit a new bug report." )]
    public async Task BugCommand( InteractionContext ctx )
    {
        if ( ctx.Member.Roles.Contains( ctx.Guild.GetRole( 917912577768566839 ) ) ) return;

        var response = GetBugReportModal();
        await ctx.Interaction.CreateResponseAsync( InteractionResponseType.Modal, response );

        async Task OnSubmit( DiscordClient sender, ModalSubmitEventArgs e )
        {
            if ( e.Interaction.Data.CustomId != response.CustomId ) return;

            var channel = ctx.Guild.GetChannel( 917912579660197955 );

            try
            {
                var button =
                    new DiscordButtonComponent( ButtonStyle.Primary, "create_bug_report", "Create Bug Report" );
                
                var builder = new DiscordMessageBuilder()
                    .WithContent(
                        $"__**Describe the bug**__\n{e.Values["br-desc"]}\n\n__**To Reproduce**__\n{e.Values["br-steps"]}\n\n__**Expected Behavior**__\n{e.Values["br-behavior"]}\n__**Screenshots**__\n{e.Values["br-screenshots"]}\n__**Additional Context**__\n{e.Values["br-context"]}\n\nSubmitted by: <@{ctx.Member.Id}>" )
                    .AddComponents( button );

                var message = await channel.SendMessageAsync( builder );
                await message.CreateThreadAsync( e.Values["br-title"], AutoArchiveDuration.Week );
            }
            catch
            {
                // ignored
            }


            ctx.Client.ModalSubmitted -= OnSubmit;
        }

        ctx.Client.ModalSubmitted += OnSubmit;
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

        async Task OnSubmit( DiscordClient sender, ModalSubmitEventArgs e )
        {
            if ( e.Interaction.Data.CustomId != response.CustomId ) return;

            var channel = ctx.Guild.GetChannel( 917912579660197956 );

            try
            {
                var button = new DiscordButtonComponent( ButtonStyle.Primary, "create_feature_request",
                    "Create Feature Request" );
                
                var builder = new DiscordMessageBuilder()
                    .WithContent( $"__**Quick Description**__\n{e.Values["fr-desc"]}\n\n__**Additional Context**__\n{e.Values["fr-context"]}\n\n__**Links**__\n{e.Values["fr-links"]}\n\nSubmitted by: <@{ctx.Member.Id}>" )
                    .AddComponents( button );

                var message = await channel.SendMessageAsync( builder );
                await message.CreateThreadAsync( e.Values["fr-title"], AutoArchiveDuration.Week );
            }
            catch
            {
                // ignored
            }
            
            
            ctx.Client.ModalSubmitted -= OnSubmit;
        }

        ctx.Client.ModalSubmitted += OnSubmit;
    }
    
}