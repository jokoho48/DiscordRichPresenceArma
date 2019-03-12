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

if !(isNull player) then {
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
