# 🍪 BiscuitBot

A sweet and simple Discord bot to manage welcomes, goodbyes, and auto-roles with a touch of sweetness.

## Features

- **Warm Welcomes**: Greet new members with a cute message and track who invited them!
- **Gentle Goodbyes**: Keep track of when members leave the jar.
- **Auto-Roles**: Automatically give new friends a role when they join.

---

## Commands

All commands are Slash Commands (`/`) and require **Administrator** permissions.

### Welcome
- `/welcome enable` - Turn on welcome messages.
- `/welcome disable` - Turn off welcome messages.
- `/welcome set [channel]` - Pick where the welcomes go!
- `/welcome status` - See if welcomes are active and where.

### Leave
- `/leave enable` - Turn on leave messages.
- `/leave disable` - Turn off leave messages.
- `/leave set [channel]` - Pick where the goodbyes go!
- `/leave status` - See if goodbyes are active and where.

### Auto-Role
- `/auto-role enable` - Turn on auto-roles.
- `/auto-role disable` - Turn off auto-roles.
- `/auto-role set [role]` - Pick the role to give new friends!
- `/auto-role status` - Check the current auto-role settings.

---

## Getting Started

1. **Invite the bot** to your server with `Administrator` permissions.
2. **Setup your channels**:
   - Use `/welcome set` to choose your welcome channel.
   - Use `/leave set` to choose your leave channel.
3. **Setup your roles**:
   - Use `/auto-role set` to pick a starting role.
4. **Enable features**:
   - `/welcome enable`
   - `/leave enable`
   - `/auto-role enable`

---

## Self-Hosting

Want to bake your own BiscuitBot?

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A Discord Bot Token from the [Developer Portal](https://discord.com/developers/applications)

### Setup
1. Clone the repo.
2. Rename `appsettings.example.json` to `appsettings.json`.
3. Add your `Token` to `appsettings.json`.
4. Enable the **Server Members Intent** in the Discord Developer Portal.
5. Run the bot:
   ```bash
   dotnet run --project BiscuitBot
   ```