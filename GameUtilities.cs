using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using static Character.HumanAccessory;

using AC.Scene;
using Actor = AC.User.ActorData;

namespace IllusionMods;

/// <summary>
/// TODO move to a separate API dll
/// </summary>
internal static class GameUtilities
{
    public const string GameProcessName = "Aicomi";

    /// <summary>
    /// True in character maker, both in main menu and in-game maker.
    /// </summary>
    public static bool InsideMaker => CharacterCreation.HumanCustom.Initialized;

    /// <summary>
    /// True if an H Scene is currently playing.
    /// </summary>
    public static bool InsideHScene => H.HScene.IsActive();

    /// <summary>
    /// True if an ADV or Touch scene is playing
    /// </summary>
    public static bool InsideCommunication
    {
        get
        {
            var es = ExploreScene.Instance;
            return es != null
                && es.CommunicationUI != null
                && es.CommunicationUI.isActiveAndEnabled
                && es.CommunicationUI._targets.Count > 0
                && !InsideHScene;
        }
    }

    /// <summary>
    /// Get a display name of the character. Only use in interface, not for keeping track of the character.
    /// If <paramref name="translated"/> is true and AutoTranslator is active, try to get a translated version of the name in current language. Otherwise, return the original name.
    /// </summary>
    //public static string GetCharaName(this Actor chara, bool translated)
    //{
    //    var fullname = chara?.charFile?.Parameter.GetCharaName(translated);
    //    if (!string.IsNullOrEmpty(fullname))
    //    {
    //        if (translated)
    //        {
    //            TranslationHelper.TryTranslate(fullname, out var translatedName);
    //            if (!string.IsNullOrEmpty(translatedName))
    //                return translatedName;
    //        }
    //        return fullname;
    //    }
    //    return chara?.chaCtrl?.name ?? chara?.ToString();
    //}

    /// <summary>
    /// Get a display name of the character. Only use in interface, not for keeping track of the character.
    /// If <paramref name="translated"/> is true and AutoTranslator is active, try to get a translated version of the name in current language. Otherwise, return the original name.
    /// </summary>
    public static string GetCharaName(this HumanDataParameter param, bool translated)
    {
        var fullname = param?.fullname;
        if (!string.IsNullOrEmpty(fullname))
        {
            //if (translated)
            //{
            //    TranslationHelper.TryTranslate(fullname, out var translatedName);
            //    if (!string.IsNullOrEmpty(translatedName))
            //        return translatedName;
            //}
            return fullname;
        }
        return "";
    }


    /// <summary>
    /// Get Humans involved in the current scene.
    /// </summary>
    public static IEnumerable<Character.Human> GetCurrentHumans()
    {
        if (InsideMaker)
        {
            var maker = CharacterCreation.HumanCustom.Instance;
            if (maker != null)
            {
                return new[] { maker.Human };
            }
        }
        else if (InsideHScene)
        {
            var HActors = H.HScene.Instance.GetActors(H.HScene.ActorType.All);

            var ha = HActors.Select(ha => ha.Human).Where(ha => ha != null).ToArray();
            if (ha != null)
            {
                return ha;
            }
        }
        else if (InsideCommunication)
        {
            var dictAllHumans = GetHumansDictionary();
            if (dictAllHumans != null)
            {
                var h = ExploreScene.Instance?.CommunicationUI?._targets
                    .AsManagedEnumerable().Where(x => x?.BaseData != null)
                    .Select(x =>
                    {
                        dictAllHumans.TryGetValue(x.BaseData, out var sa);
                        return sa;
                    })
                    .Where(x => x != null).ToArray();
                if (h != null)
                {
                    return h;
                }
            }
        }
        return Enumerable.Empty<Human>();
    }

    // IDHI: Human information as found under AC.Scene.Explore.Actor
    // Dictionary<AC.User.ActorData, AC.Scene.Explore.Actor.Params.Chara>
    public static Dictionary<Actor, Human> GetHumansDictionary()
    {
        var sa = ExploreScene.Instance?._actors.ToArray();
        var sah = sa.Select(x => x).Where(x => x != null)
            .ToDictionary(x => x.BaseData, x => x.Params.Chara);
        if (sah != null)
        {
            return sah;
        }
        return null;
    }

    // IDHI: For Aicomi HideCatetory is a flag H = 1, Bath = 2, Overnight = 4

    public static void SetHiddenAccessoryState(this Human self, HideCategory category, bool show, bool invertH = false)
    {
        var flags = new HideCategoryFlag[] { HideCategoryFlag.H, HideCategoryFlag.Bath, HideCategoryFlag.Overnight };

        SetAccessoryStateHidden(self, flags[(int)category], show, invertH);
    }

    public static void SetAccessoryStateHidden(this Human self, HideCategoryFlag category, bool show, bool invertH = false)
    {
        if (self == null)
        {
            return;
        }

        if (invertH && category != HideCategoryFlag.H)
        {
            invertH = false;
        }

        var accessories = self.coorde._nowCoordinate.Accessory.parts;

        for (var slot = 0; slot < accessories.Length; slot++)
        {
            var acc = accessories[slot];
            var categorySet = acc.hideCategory.IsHideCategory(category);
            if (invertH)
            {
                categorySet = !categorySet;
            }

            if (categorySet)
            {
                self.acs.SetAccessoryState(slot, show);
            }
        }
    }

    #region HideCategory
    public enum HideCategoryFlag
    {
        None = 0,
        H = 1,
        Bath = 2,
        Overnight = 4
    }

    private static readonly HideCategoryFlag[] flags =
        new HideCategoryFlag[] { HideCategoryFlag.H, HideCategoryFlag.Bath, HideCategoryFlag.Overnight };

    public static bool IsHideCategory(this int self, HideCategory category)
        => IsHideCategory(self, flags[(int)category]);

    internal static bool IsHideCategory(this int self, HideCategoryFlag category)
        => (self & (int)category) == (int)category;
    #endregion
}
