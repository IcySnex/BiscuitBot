using BiscuitBot.Models;
using BiscuitBot.Services;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace BiscuitBot.Commands;

public class UserModule(
	InviteService inviteService) : ApplicationCommandModule<ApplicationCommandContext>
{
	[SlashCommand("whois", "Shows information about a user")]
	public InteractionMessageProperties Whois(
		[SlashCommandParameter(Description = "The user to get info of")]
		User? user = null)
	{
		user ??= Context.User;

		MemberData? data = inviteService.GetMemberData(
			Context.Guild!.Id,
			user.Id);

		List<EmbedFieldProperties> fields =
		[
			new()
			{
				Name = "Joined Discord",
				Value = $"<t:{user.CreatedAt.ToUnixTimeSeconds()}:R>",
				Inline = true
			}
		];

		if (user is GuildUser guildUser && guildUser.JoinedAt.HasValue)
		{
			fields.Add(new()
			{
				Name = "Joined Server",
				Value = $"<t:{guildUser.JoinedAt.Value.ToUnixTimeSeconds()}:R>",
				Inline = true
			});
		}

		if (data is not null)
		{
			string inviter = data.InviterId.HasValue
				? (data.InviterIsBot ? data.InviterName ?? "Unknown Bot" : $"<@{data.InviterId}>")
				: data.InviterName ?? "Unknown";

			fields.Add(new()
			{
				Name = "Invited By",
				Value = inviter,
				Inline = true
			});
		}
		else
		{
			fields.Add(new()
			{
				Name = "Invited By",
				Value = "Unknown",
				Inline = true
			});
		}

		EmbedProperties embed = new()
		{
			Title = $"{user.Username} ({user.Id})",
			Thumbnail = new(user.GetAvatarUrl()?.ToString(1024) ?? user.DefaultAvatarUrl.ToString()),
			Color = new(0xfce9d2),
			Fields = fields
		};

		return new()
		{
			Embeds =
			[
				embed
			],
			Flags = MessageFlags.Ephemeral
		};
	}
}
