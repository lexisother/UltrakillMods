namespace FixScenesCommand

open BepInEx
open HarmonyLib
open System
open System.Collections.Generic
open System.Text.RegularExpressions
open GameConsole
open GameConsole.Commands
open plog.Models
open FixScenesCommand.PluginInfo

// God forgive what I'm about to do.
// Solely here to share state between two completely separate patches.
// Sorry, F# nutcases that read this.
module Globals =
    let mutable SceneNames = List<String>()

[<HarmonyPatch(typeof<Scenes>)>]
type FixScenesCommand() =
    [<HarmonyPatch("Execute")>]
    [<HarmonyPrefix>]
    static member Execute(__instance: Scenes, con: Console) =
        if con.CheatBlocker() then
            false
        else
            __instance.Log.Info("<b>Available Scenes:</b>", Array.Empty<Tag>())
            for scene in Globals.SceneNames do
                __instance.Log.Info(scene, Array.Empty<Tag>())
            false

[<HarmonyPatch(typeof<GetMissionName>)>]
type LevelLister() =
    [<HarmonyPatch("GetSceneName")>]
    [<HarmonyTranspiler>]
    static member getSceneName(instructions: IEnumerable<CodeInstruction>) =
        let codes = List<CodeInstruction>(instructions)

        for ins in instructions do
            Regex.Match(ins.ToString(), """ldstr "(.*?)" \[Label[0-9]*?\]""")
            |> _.Groups[1].Value
            |> fun x ->
                if not(String.IsNullOrEmpty(x)) then
                    x |> Globals.SceneNames.Add

        codes

[<BepInPlugin(pluginId, pluginName, pluginVersion)>]
type Plugin() =
    inherit BaseUnityPlugin()

    member this.Awake() =
        Harmony(pluginId).PatchAll()