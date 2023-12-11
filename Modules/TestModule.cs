using Discord.Commands;

namespace DiscordBot.Modules;

public class TestModule : ModuleBase
{
    [Command("echo")]
    public async Task SayAsync([Remainder] string echo)
    {
        await ReplyAsync(echo);
    }
}