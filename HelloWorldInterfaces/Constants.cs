using System.Collections.Immutable;

namespace HelloWorldInterfaces;

public static class Constants
{
    public const string StreamProviderName = "NumberProviderStream";
    public const string StreamNamespace    = "NumberStreamNamespace";

    public static readonly ImmutableList<string> SayHelloNames;

    static Constants()
    {
        // Note: "Emma" appears twice in the list. SayHello method processes one after the other.
        SayHelloNames = new [] { "Emma", "Emma", "Liam", "Olivia", "Noah", "Ava", "Sophia" }.ToImmutableList();
    }

    public static string GetRandomName()
    {
        return SayHelloNames[Random.Shared.Next(SayHelloNames.Count)];
    }
}
