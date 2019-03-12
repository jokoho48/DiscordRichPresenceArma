class CfgPatches
{
    class JK_DiscordRichtPresence
    {
        name = "JK_DiscordRichtPresence";
        author = "joko // Jonas";
    };
};
class CfgFunctions
{
    class JK_DiscordRichtPresence
    {
        class DiscordRichtPresence
        {
            file = "JK_DiscordRichtPresence";
            class postInit {
                postInit = 1;
            };
            class preInit {
                preInit = 1;
            };
            class preStart {
                preStart = 1;
            };
        };
    };
};
