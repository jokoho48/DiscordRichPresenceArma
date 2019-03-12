#include "macros.hpp"
/*
 * Author: joko // Jonas
 * PreInit for Discord Rich Presence
 *
 * Arguments:
 * None
 *
 * Return Value:
 * None
 */
JK_DiscordRichPresence_SplitString = toString [1];

#ifdef ISDEV
addMissionEventHandler ["EachFrame", {
    private _result = "DiscordRichPresenceArma" callExtension "readlogs";
    {
        if !(_x isEqualTo "") then {
            diag_log _x;
        };
        nil
    } count _result splitString JK_DiscordRichPresence_SplitString;
}];
#endif
