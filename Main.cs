using System.Threading.Tasks;
using BTD_Mod_Helper.Api.ModOptions;
using FillMapWithMonkeys;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using Il2CppInterop.Runtime;
using UnityEngine;
using Vector2 = Il2CppAssets.Scripts.Simulation.SMath.Vector2;
using Vector3 = Il2CppAssets.Scripts.Simulation.SMath.Vector3;

[assembly:
    MelonInfo(typeof(FillMapWithMonkeys.Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace FillMapWithMonkeys;

[HarmonyPatch]
public class Main : BloonsTD6Mod
{
    private static MelonLogger.Instance Logger;

    public override void OnInitialize()
    {
        Logger = LoggerInstance;
    }

    private static readonly ModSettingHotkey FillMapWithMonkeys = new(KeyCode.BackQuote, HotkeyModifier.Ctrl);

    /// <inheritdoc />
    public override void OnUpdate()
    {
        if (FillMapWithMonkeys.JustPressed())
        {
            FillMap();
        }
    }


    private static void FillMap()
    {
        Logger.Msg("tower placed: " + ShopMenu.instance.selectedButton.TowerModel.name);

        var towerModel = ShopMenu.instance.selectedButton.TowerModel.Duplicate();
        towerModel.skinName = "";

        for (int x = 0; x < Constants.worldWidth; x++)
        {
            for (int y = 0; y < Constants.worldHeight; y++)
            {
                var position = new Vector3(Constants.worldXMin + x, Constants.worldZMin + y);

                var areaAtPoint = InGame.instance.GetMap().GetAreaAtPoint(position.ToVector2());
                if (!InGame.instance.GetUnityToSimulation().Simulation.Map.CanPlace(position.ToVector2(), towerModel))
                {
                    continue;
                }

                InGame.instance.GetTowerManager().CreateTower(towerModel, position,
                    InGame.instance.bridge.MyPlayerNumber,
                    areaAtPoint?.GetAreaID() ?? ObjectId.FromData(1),
                    ObjectId.FromData(4294967295), null, false, false, 0, false);
            }
        }
    }
}