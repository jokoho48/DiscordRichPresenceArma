#include "macros.hpp"
/*
 * Author: joko // Jonas
 * PostInit for Discord Rich Presence
 *
 * Arguments:
 * None
 *
 * Return Value:
 * None
 */

if (hasInterface) then {
    if (isText(missionConfigFile >> "JK_DiscordRichPresence" >> "CustomAppID")) then {
        "DiscordRichPresenceArma" callExtension ["customAppId", getText(missionConfigFile >> "JK_DiscordRichPresence" >> "CustomAppID")];
    };
    "DiscordRichPresenceArma" callExtension ["serverStartTimeUpdate", str(serverTime)];

    "DiscordRichPresenceArma" callExtension ["presenceUpdate", briefingName, getText (configFile >> "CfgWorlds" >> worldName >> "description"), serverName, worldName];

    addMissionEventHandler ["Ended", {
        params ["_endType"];
        "DiscordRichPresenceArma" callExtension "end";
    }];
    [] spawn {
        waitUntil {!isNull(call BIS_fnc_displayMission)};
        (call BIS_fnc_displayMission) displayAddEventhandler ["Unload",{
            "DiscordRichPresenceArma" callExtension "end";
        }];
    };
};
