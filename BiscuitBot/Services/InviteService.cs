using System.Collections.Concurrent;
using System.Text.Json;
using BiscuitBot.Models;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Rest;

namespace BiscuitBot.Services;

public class InviteService(
	ILogger<InviteService> logger,
	RestClient restClient)
{
	const string MemberDataPath = "member_data.json";
	
	readonly ConcurrentDictionary<ulong, Dictionary<string, int>> inviteCache = new();
	readonly ConcurrentDictionary<ulong, int> vanityCache = new();
	readonly ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, MemberData>> memberCache = LoadMemberData(logger);
	readonly ConcurrentDictionary<ulong, int> memberCounts = new();
	readonly SemaphoreSlim locker = new(1, 1);

	
	static ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, MemberData>> LoadMemberData(
		ILogger logger)
	{
		if (!File.Exists(MemberDataPath))
			return [];

		try
		{
			logger.LogInformation("Reading member data from {Path}", MemberDataPath);
			string json = File.ReadAllText(MemberDataPath);
			
			Dictionary<string, Dictionary<string, MemberData>>? rawData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, MemberData>>>(json);
			
			if (rawData is null)
				return [];

			ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, MemberData>> result = [];
			foreach (KeyValuePair<string, Dictionary<string, MemberData>> guildPair in rawData)
			{
				if (ulong.TryParse(guildPair.Key, out ulong guildId))
				{
					ConcurrentDictionary<ulong, MemberData> members = [];
					foreach (KeyValuePair<string, MemberData> memberPair in guildPair.Value)
					{
						if (ulong.TryParse(memberPair.Key, out ulong userId))
							members[userId] = memberPair.Value;
					}
					result[guildId] = members;
				}
			}
			
			return result;
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to load member data from {Path}", MemberDataPath);
			return [];
		}
	}

	void SaveMemberData()
	{
		try
		{
			Dictionary<string, Dictionary<string, MemberData>> rawData = [];
			foreach (KeyValuePair<ulong, ConcurrentDictionary<ulong, MemberData>> guildPair in memberCache)
			{
				Dictionary<string, MemberData> members = [];
				foreach (KeyValuePair<ulong, MemberData> memberPair in guildPair.Value)
					members[memberPair.Key.ToString()] = memberPair.Value;
				
				rawData[guildPair.Key.ToString()] = members;
			}

			string json = JsonSerializer.Serialize(rawData);
			File.WriteAllText(MemberDataPath, json);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to save member data to {Path}", MemberDataPath);
		}
	}
	
	void CleanupStaleData()
	{
		bool changed = false;

		foreach (KeyValuePair<ulong, ConcurrentDictionary<ulong, MemberData>> guildPair in memberCache)
		{
			if (guildPair.Value.IsEmpty)
			{
				if (memberCache.TryRemove(guildPair.Key, out _))
					changed = true;
			}
		}

		if (changed)
		{
			logger.LogInformation("Cleaned up empty guild tracking data");
			SaveMemberData();
		}
	}

	
	public async Task InitializeAsync(
		ulong guildId,
		int memberCount)
	{
		memberCounts[guildId] = memberCount;
		
		await locker.WaitAsync();
		try
		{
			await UpdateCacheAsync(guildId);
			CleanupStaleData();
		}
		finally
		{
			locker.Release();
		}
	}
	
	public async Task UpdateCacheAsync(
		ulong guildId,
		IEnumerable<RestInvite>? invites = null)
	{
		logger.LogInformation("Fetching invites for guild {GuildId}", guildId);
		invites ??= await restClient.GetGuildInvitesAsync(guildId);
			
		Dictionary<string, int> cache = [];
		foreach (RestInvite invite in invites)
			cache[invite.Code] = invite.Uses ?? 0;

		inviteCache[guildId] = cache;

		try
		{
			GuildVanityInvite vanity = await restClient.GetGuildVanityInviteAsync(guildId);
			vanityCache[guildId] = vanity.Uses;
		}
		catch
		{
			// Ignore
		}
	}

	
	public async Task<User?> GetInviterAsync(
		ulong guildId)
	{
		await locker.WaitAsync();
		try
		{
			logger.LogInformation("Fetching invites for guild {GuildId}", guildId);
			IEnumerable<RestInvite> invites = await restClient.GetGuildInvitesAsync(guildId);
			
			if (!inviteCache.TryGetValue(guildId, out Dictionary<string, int>? cachedInvites))
			{
				await UpdateCacheAsync(guildId, invites);
				return null;
			}

			User? inviter = null;
			Dictionary<string, int> newCache = [];

			foreach (RestInvite invite in invites)
			{
				if (cachedInvites.TryGetValue(invite.Code, out int uses) && invite.Uses > uses)
				{
					inviter = invite.Inviter;
					logger.LogInformation("Found inviter {InviterId} via code {Code} in guild {GuildId}", inviter?.Id, invite.Code, guildId);
				}

				newCache[invite.Code] = invite.Uses ?? 0;
			}

			inviteCache[guildId] = newCache;

			if (inviter is not null)
				return inviter;

			try
			{
				GuildVanityInvite vanity = await restClient.GetGuildVanityInviteAsync(guildId);
				
				if (vanityCache.TryGetValue(guildId, out int vanityUses) && vanity.Uses > vanityUses)
				{
					logger.LogInformation("Member joined guild {GuildId} via vanity URL", guildId);
					vanityCache[guildId] = vanity.Uses;
					return null; 
				}
				vanityCache[guildId] = vanity.Uses;
			}
			catch
			{
				// Ignore
			}

			return null;
		}
		finally
		{
			locker.Release();
		}
	}

	
	public void AddInvite(
		ulong guildId,
		string code,
		int uses)
	{
		if (inviteCache.TryGetValue(guildId, out Dictionary<string, int>? cache))
			cache[code] = uses;
	}

	public void RemoveInvite(
		ulong guildId,
		string code)
	{
		if (inviteCache.TryGetValue(guildId, out Dictionary<string, int>? cache))
			cache.Remove(code);
	}
	
	
	public void TrackMember(
		ulong guildId,
		ulong userId,
		bool inviterIsBot,
		ulong? inviterId,
		string? inviterName,
		DateTimeOffset joinedAt)
	{
		ConcurrentDictionary<ulong, MemberData> guildMembers = memberCache.GetOrAdd(
			guildId,
			_ => []);
		
		guildMembers[userId] = new() { InviterIsBot = inviterIsBot, InviterId = inviterId, InviterName = inviterName, JoinedAt = joinedAt };
		SaveMemberData();
	}

	public void ForgetMember(
		ulong guildId,
		ulong userId)
	{
		if (memberCache.TryGetValue(guildId, out ConcurrentDictionary<ulong, MemberData>? guildMembers))
		{
			if (guildMembers.TryRemove(userId, out _))
				SaveMemberData();
		}

		memberCounts.AddOrUpdate(guildId, 0, (_, count) => Math.Max(0, count - 1));
	}

	public MemberData? GetMemberData(
		ulong guildId,
		ulong userId)
	{
		if (memberCache.TryGetValue(guildId, out ConcurrentDictionary<ulong, MemberData>? guildMembers) && 
			guildMembers.TryGetValue(userId, out MemberData? data))
			return data;

		return null;
	}

	public int GetAndIncrementMemberCount(
		ulong guildId,
		int fallbackCount)
	{
		return memberCounts.AddOrUpdate(
			guildId,
			_ => fallbackCount + 1,
			(_, count) => count + 1);
	}
}
