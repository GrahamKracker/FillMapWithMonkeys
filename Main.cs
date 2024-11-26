using System.Threading.Tasks;
using BTD_Mod_Helper.Api.ModOptions;
using FillMapWithMonkeys;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
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

    private static readonly ModSettingHotkey FillMapWithMonkeys = new(KeyCode.F, HotkeyModifier.Ctrl);

    /// <inheritdoc />
    public override void OnUpdate()
    {
        if (FillMapWithMonkeys.JustPressed())
        {
            ResetMapPositions();
            FillMap();
        }
    }

    /// <inheritdoc />
    public override void OnRestart()
    {
        ResetMapPositions();
    }

    /// <inheritdoc />
    public override void OnMainMenu()
    {
        ResetMapPositions();
    }

    private static void ResetMapPositions()
    {
        for (var i = 0; i < Constants.worldHeight; i++)
        {
            for (var j = 0; j < Constants.worldWidth; j++)
            {
                MapPositions[i][j] = false;
            }
        }
    }

    private static readonly bool[][] MapPositions = new bool[(int)Constants.worldHeight][];

    public Main()
    {
        for (var i = 0; i < Constants.worldHeight; i++)
        {
            MapPositions[i] = new bool[(int)Constants.worldWidth];
        }
    }

    private static void FillMap()
    {
        Logger.Msg("tower placed: " + ShopMenu.instance.selectedButton.TowerModel.name);

        var towerModel = ShopMenu.instance.selectedButton.TowerModel.Duplicate();
        towerModel.skinName = "";

        for (int x = 0; x < Constants.worldHeight; x++)
        {
            for (int y = 0; y < Constants.worldWidth; y++)
            {
                var position = new Vector3(Constants.worldXMin + x, Constants.worldZMin + y);
                if (MapPositions[x][y])
                    continue;

                var areaAtPoint = InGame.instance.GetMap().GetAreaAtPoint(position.ToVector2());
                if (areaAtPoint != null && !towerModel.IsPlaceableInAreaType(areaAtPoint.areaModel.type))
                {
                    continue;
                }

                InGame.instance.GetTowerManager().CreateTower(towerModel, position,
                    InGame.instance.bridge.MyPlayerNumber,
                    areaAtPoint?.GetAreaID() ?? ObjectId.FromData(1),
                    ObjectId.FromData(4294967295), null, false, false, 0, false);

                var towerRadius = towerModel.radius;
                //fill in the map positions that the tower is taking up
                for (int i = 0; i < towerRadius; i++)
                {
                    for (int j = 0; j < towerRadius; j++)
                    {
                        if (x + i < Constants.worldHeight && y + j < Constants.worldWidth)
                        {
                            MapPositions[x + i][y + j] = true;
                        }
                    }
                }
            }
        }
    }
}