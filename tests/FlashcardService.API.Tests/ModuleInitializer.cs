using System.Runtime.CompilerServices;

namespace FlashcardService.API.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        UseProjectRelativeDirectory("snapshots");
    }
}