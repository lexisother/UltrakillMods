namespace DebugMode

open BepInEx
open HarmonyLib
open DebugMode.PluginInfo

[<HarmonyPatch(typeof<UnityEngine.Debug>)>]
type Patch() =
    [<HarmonyPatch("isDebugBuild", MethodType.Getter)>]
    [<HarmonyPostfix>]
    static member get_isDebugBuild(__result: byref<bool>) =
        __result <- true
        ()

[<BepInPlugin(pluginId, pluginName, pluginVersion)>]
type Plugin() =
    inherit BaseUnityPlugin()

    member this.Awake() =
        Harmony(pluginId).PatchAll()