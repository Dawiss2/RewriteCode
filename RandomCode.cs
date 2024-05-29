using BepInEx;
using VampireCommandFramework;
using UnityEngine;
using ProjectM;
using Bloodstone.API;
using Unity.Entities;
using Stunlock.Core;
using ProjectM.Scripting;
using System.Collections.Generic;
using System.Linq;

[BepInPlugin("com.yourname.vrising.rewritecode", "Rewrite Code Challenge", "1.0.0")]
public class RandomCode : MonoBehaviour
{
    private static System.Random random = new System.Random();
    private static string currentCode;
    private static bool codeActive = false;
    private int codeLength = 15;
    private float intervalInSeconds = 2700f;

    Dictionary<PrefabGUID, int> rewardsList = new Dictionary<PrefabGUID, int>
        {
            { new PrefabGUID(-651878258), 1 }, //1 Onyx Tear
            { new PrefabGUID(-1397591435), 5 }, //5 Reinforced Plank
            { new PrefabGUID(-1190647720), 2 }, //2 Power Core
            { new PrefabGUID(2116142390), 5 }, //5 Radium Alloy
            { new PrefabGUID(780044299), 50 }, //50 Research Paper
            { new PrefabGUID(2085163661), 100 }, //100 Research Schematics
            { new PrefabGUID(2065714452), 75 }, //75 Research Scrolls
            { new PrefabGUID(702067317), 5 }, //5 Silk
            { new PrefabGUID(-2130812821), 10 }, //10 Spectral Dust
            { new PrefabGUID(834864259), 200 }, //200 Tech Scrap
            { new PrefabGUID(1252507075), 15 }, //15 Whetstone
            { new PrefabGUID(-762000259), 10}, //10 Dark Silver
            { new PrefabGUID(-1461326411), 1}, // 1 Golem 
            { new PrefabGUID(-1021407417), 5}, // Major Explosive Box
            { new PrefabGUID(1779299585), 5}, // Minor Explosive Box
            { new PrefabGUID(429052660), 10}, // Blood Rose Potion
            { new PrefabGUID(880699252), 30}, // Sulphur
            { new PrefabGUID(-11246506), 30}, // Silkworm
            { new PrefabGUID(-164367832), 50}, // Ghost Shroom
            { new PrefabGUID(444400639), 20}, // Cotton Yarn
            { new PrefabGUID(-1823614190), 5}, // Sludge-filled Canister
            { new PrefabGUID(-1233716303), 15}, // Glass
            { new PrefabGUID(2106123809), 10}, // Ghost Yarn
            { new PrefabGUID(2103989354), 300}, // Stygian Shard
            { new PrefabGUID(576389135), 300}, // Greater Stygian Shard
            { new PrefabGUID(1005440012), 10}, // Scourgestone
            { new PrefabGUID(-608131642), 30}, // Grave Dust
            { new PrefabGUID(88009216), 300}, // Sacred Grapes
            { new PrefabGUID(-437611596), 10}, // Empty Glass Bottle
            { new PrefabGUID(541321301), 2}, // Wrangler's Potion
            { new PrefabGUID(-1568756102), 2}, // Potion of Rage
            { new PrefabGUID(1510182325), 2}, // Witch Potion
            { new PrefabGUID(-2102469163), 2}, // Vampiric Brew
            { new PrefabGUID(-2139183850), 2}, // Garlic Resistance Potion
            { new PrefabGUID(639992282), 2}, // Holy Resistance Flask
            { new PrefabGUID(2107622409), 2}, // Silver Resistance Potion
            { new PrefabGUID(970650569), 2}, // Fire Resistance Brew
            { new PrefabGUID(-38051433), 2}, // Minor Sun Resistance Brew
            { new PrefabGUID(-1886460367), 5}, // Bat Leather
            { new PrefabGUID(-412448857), 20}, // Charged Battery
            { new PrefabGUID(1270271716), 40}, // Depleted Battery
            { new PrefabGUID(-1458997116), 5} // Shadow Weave
        };

    Dictionary<string, string> itemNames = new Dictionary<string, string>
        {
            { "Item_Ingredient_OnyxTear", "Onyx Tear" },
            { "Item_Ingredient_ReinforcedPlank", "Reinforced Plank" },
            { "Item_Ingredient_PowerCore", "Power Core" },
            { "Item_Ingredient_RadiumAlloy", "Radium Alloy" },
            { "Item_Ingredient_Research_Paper", "Research Paper" },
            { "Item_Ingredient_Research_Schematic", "Research Schematic" },
            { "Item_Ingredient_Research_Scroll", "Research Scroll" },
            { "Item_Ingredient_Silk", "Silk" },
            { "Item_Ingredient_Spectraldust", "Spectral Dust" },
            { "Item_Ingredient_TechScrap", "Tech Scrap" },
            { "Item_Ingredient_Whetstone", "Whetstone" },
            { "Item_Ingredient_Mineral_DarkSilverBar", "Dark Silver Bar"},
            { "Item_Building_Siege_Golem_T02", "Siege Golem Stone"},
            { "Item_Building_Explosives_T02", "Major Explosive Box"},
            { "Item_Building_Explosives_T01", "Minor Explosive Box"},
            { "Item_Consumable_HealingPotion_T02", "Blood Rose Potion"},
            { "Item_Ingredient_Mineral_Sulfur", "Sulphur"},
            { "Item_Ingredient_Silkworm", "Silkworm"},
            { "Item_Ingredient_Plant_GhostShroom", "Ghost Shroom"},
            { "Item_Ingredient_CottonYarn", "Cotton Yarn"},
            { "Item_Consumable_Canister_ToxicSludge", "Sludge-filled Canister"},
            { "Item_Ingredient_Glass", "Glass"},
            { "Item_Ingredient_GhostYarn", "Ghost Yarn"},
            { "Item_NetherShard_T01", "Stygian Shard"},
            { "Item_NetherShard_T02", "Greater Stygian Shard"},
            { "Item_Ingredient_Scourgestone", "Scourgestone"},
            { "Item_Ingredient_Gravedust", "Grave Dust"},
            { "Item_Ingredient_Plant_SacredGrapes", "Sacred Grapes"},
            { "Item_Consumable_EmptyBottle", "Empty Glass Bottle"},
            { "Item_Consumable_WranglersPotion_T01", "Wrangler's Potion"},
            { "Item_Consumable_PhysicalPowerPotion_T02", "Potion of Rage"},
            { "Item_Consumable_SpellPowerPotion_T02", "Witch Potion"},
            { "Item_Consumable_SpellLeechPotion_T01", "Vampiric Brew"},
            { "Item_Consumable_GarlicResistancePotion_T02", "Garlic Resistance Potion"},
            { "Item_Consumable_HolyResistancePotion_T02", "Holy Resistance Flask"},
            { "Item_Consumable_SilverResistancePotion_T02", "Silver Resistance Potion"},
            { "Item_Consumable_FireResistancePotion_T01", "Fire Resistance Brew"},
            { "Item_Consumable_SunResistancePotion_T01", "Minor Sun Resistance Brew"},
            { "Item_Ingredient_BatLeather", "Bat Leather"},
            { "Item_Ingredient_BatteryCharged", "Charged Battery"},
            { "Item_Ingredient_Battery", "Depleted Battery"},
            { "Item_Ingredient_ShadowWeave", "Shadow Weave"}
        };
    public void Start()
    {
        InvokeRepeating(nameof(SendRandomCode), intervalInSeconds, intervalInSeconds);
    }
    private void SendRandomCode()
    {
        currentCode = GenerateRandomCode(codeLength);
        codeActive = true;
        SendChatMessage($"<color=orange>Use command <color=red>.code {currentCode}<color=orange> as first player to claim the reward!");
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] stringChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        return new string(stringChars);
    }

    private static void SendChatMessage(string message)
    {
        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, message);
    }

    private void RewardPlayer(Entity target, string playerName)
    {

        int reward = random.Next(rewardsList.Count);
        var prefabSys = VWorld.Server.GetExistingSystemManaged<PrefabCollectionSystem>();
        prefabSys.PrefabGuidToNameDictionary.TryGetValue(rewardsList.Keys.ElementAt(reward), out var itemprefabname);
        itemNames.TryGetValue(itemprefabname, out string itemname);
        SendChatMessage($"<color=orange>Player {playerName} correctly rewrote the code and won <color=red>[{rewardsList.Values.ElementAt(reward)}x {itemname}]<color=orange>!");
        AddItemToInventory(target, rewardsList.Keys.ElementAt(reward), rewardsList.Values.ElementAt(reward));
    }
    public static Entity AddItemToInventory(Entity recipient, PrefabGUID guid, int amount)
    {
        ServerGameManager serverGameManager = VWorld.Server.GetExistingSystemManaged<ServerScriptMapper>()._ServerGameManager;
        var inventoryResponse = serverGameManager.TryAddInventoryItem(recipient, guid, amount);
        return inventoryResponse.NewEntity;
        return new Entity();
    }
    [Command("newcode", description: "Starts a new code event", adminOnly: true)]
    public void NewCode(ChatCommandContext caller)
    {
        SendRandomCode();
    }
    [Command("testitem", description: "", adminOnly: true)]
    public void TestItem(ChatCommandContext caller, int guid)
    {
        AddItemToInventory(caller.Event.SenderCharacterEntity, new PrefabGUID(guid), 1);
    }
    [Command("code", description: "Submit the code to win a reward", adminOnly: false)]
    public void Code(ChatCommandContext caller, string args)
    {
        if (args.Length == 0)
        {
            caller.Reply("Usage: .code <code>");
            return;
        }

        string submittedCode = args;
        if (codeActive)
        {
            if (submittedCode.Equals(currentCode, System.StringComparison.OrdinalIgnoreCase))
            {
                codeActive = false;
                RewardPlayer(caller.Event.SenderCharacterEntity, caller.Name);
            }
            else
            {
                caller.Reply("Incorrect code, try again!");
            }
        }
        else
        {
            caller.Reply("Code event isn't active right now.");
        }
    }
}