using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Character;
using IllusionMods;
using RuntimeUnityEditor.Core.Utils;
using ACExporter.Utils;
using UnityEngine;

namespace ACExporter
{
    [BepInPlugin(GUID, "Aicomi Exporter", Version)]
    [BepInProcess(GameUtilities.GameProcessName)]
    public class ACExporterPlugin : BasePlugin
    {
        public const string Version = "1.0.0.0";
        public const string GUID = "ACExporter";

        internal static ManualLogSource Logger { get; private set; }
        private static ACExporterPlugin Instance { get; set; }

        private const float Margin = 5f;
        private const float WindowWidth = 125f;

        private static readonly GUILayoutOption[] _NoLayoutOptions = Array.Empty<GUILayoutOption>();
        private static readonly List<GUIContent[][]> _AccessoryButtonContentCache = new();

        private readonly List<IStateToggleButton> _buttons = new();
        private const int CoordCount = 3; //todo make this not hardcoded
        //private readonly CoordButton[] _coordButtons = new CoordButton[CoordCount];

        private Rect _windowRect;
        private Vector2 _accessorySlotsScrollPos = Vector2.zero;

        private Human[] _visibleCharas = Array.Empty<Human>();
        public static Human selectedChara;
        private readonly ImguiComboBox _charaDropdown = new();
        private GUIContent[] _visibleCharasContents = Array.Empty<GUIContent>();
        private GUIContent _selectedCharaContent;
        private PmxBuilder _pmxBuilder;
        public static string basePath = "Export_PMX";
        private SVSExporterMenuComponent component;

        private ConfigEntry<Color> BackgroundColor { get; set; }
        private float _currentBackgroundAlpha;

        private ConfigEntry<bool> ShowCoordinateButtons { get; set; }
        private ConfigEntry<bool> ShowMainSub { get; set; }
        private ConfigEntry<KeyboardShortcut> Keybind { get; set; }

        public override void Load()
        {
            Logger = Log;
            Instance = this;

            // todo remove whenever bepinex il2cpp gets it
            var colorConverter = new TypeConverter
            {
                ConvertToString = (obj, type) => ColorUtility.ToHtmlStringRGBA((Color)obj),
                ConvertToObject = (str, type) =>
                {
                    if (!ColorUtility.TryParseHtmlString("#" + str.Trim('#', ' '), out var c))
                        throw new FormatException("Invalid color string, expected hex #RRGGBBAA");
                    return c;
                }
            };
            TomlTypeConverter.AddConverter(typeof(Color), colorConverter);

            BackgroundColor = Config.Bind("General", "Background Color", new Color(0, 0, 0, 0.5f), "Tint of the background color of the clothing state menu (subtle change). When mouse cursor hovers over the menu, transparency is forced to 1.");

            Keybind = Config.Bind("General", "Toggle clothing state menu", new KeyboardShortcut(UnityEngine.KeyCode.Tab, UnityEngine.KeyCode.LeftControl), "Keyboard shortcut to toggle the clothing state menu on and off.\nCan be used outside of character maker in some cases - works for males in H scenes (the male has to be visible for the menu to appear) and in some conversations with girls.");
            //ShowCoordinateButtons = Config.Bind("Options", "Show coordinate change buttons in Character Maker", false, "Adds buttons to the menu that allow quickly switching between clothing sets. Same as using the clothing dropdown.\nThe buttons are always shown outside of character maker.");
            //ShowMainSub = Config.Bind("Options", "Show S/H in accessory list", true, "Show in the toggle list whether an accessory is set to Show (S) or Hide (H) in H scenes.");

            component = AddComponent<SVSExporterMenuComponent>();

            if (!System.IO.File.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        private sealed class SVSExporterMenuComponent : MonoBehaviour
        {
            private void Update() => Instance.Update();
            private void OnGUI() => Instance.OnGUI();
        }

        private bool _showInterface;
        private bool ShowInterface
        {
            get => _showInterface;
            set
            {
                if (value)
                {
                    RefreshCharacterList();
                    _showInterface = selectedChara != null;
                }
                else
                {
                    _showInterface = true;
                }
            }
        }

        private bool RefreshCharacterList()
        {
            _visibleCharas = GameUtilities.GetCurrentHumans().ToArray();
            if (selectedChara == null || !_visibleCharas.Contains(selectedChara))
                selectedChara = _visibleCharas.FirstOrDefault();

            _visibleCharasContents = _visibleCharas.Select(x => new GUIContent(x.fileParam.GetCharaName(true))).ToArray();
            var anyChara = selectedChara != null;
            if (anyChara)
            {
                _selectedCharaContent = _visibleCharasContents[Array.IndexOf(_visibleCharas, selectedChara)];
                SetupInterface();
            }
            else
            {
                _selectedCharaContent = null;
                _showInterface = false;
            }
            return anyChara;
        }


        private void Update()
        {
            if (Keybind.Value.IsDown())
                ShowInterface = !ShowInterface;

            if (_showInterface)
            {
                if (!selectedChara?.transform)
                {
                    if (!RefreshCharacterList())
                        return;
                }

                var mouseHover = _windowRect.Contains(new Vector2(UnityEngine.Input.mousePosition.x, Screen.height - UnityEngine.Input.mousePosition.y));
                _currentBackgroundAlpha = Mathf.Lerp(_currentBackgroundAlpha, mouseHover ? 1f : BackgroundColor.Value.a, Time.deltaTime * 10f);
            }
            else
            {
                _currentBackgroundAlpha = 0;
            }
        }

        private void OnGUI()
        {
            if (!ShowInterface)
                return;

            if (!GameUtilities.InsideMaker)
            {
                return;
            }

            var backgroundColor = BackgroundColor.Value;
            backgroundColor.a = _currentBackgroundAlpha;

            GUI.backgroundColor = backgroundColor;

            // To allow mouse draging the skin has to have solid background, box works fine
            var style = _currentBackgroundAlpha == 0 ? GUI.skin.label : GUI.skin.box;
            _windowRect = GUILayout.Window(90876311, _windowRect, (GUI.WindowFunction)WindowFunc, GUIContent.none, style, _NoLayoutOptions);

            void WindowFunc(int id)
            {
                GUI.backgroundColor = backgroundColor;

                // Space for mouse dragging
                GUILayout.Space(6);

                _charaDropdown.Show(_selectedCharaContent, () => _visibleCharasContents, i =>
                {
                    selectedChara = _visibleCharas[i];
                    RefreshCharacterList();
                }, (int)_windowRect.yMax);

                foreach (var clothButton in _buttons)
                {
                    if (clothButton == null)
                    {
                        GUILayout.Space(7);
                    }
                    else
                    {
                        if (GUILayout.Button(clothButton.Content, _NoLayoutOptions))
                            clothButton.OnClick();
                        GUILayout.Space(-5);
                    }
                }
                GUILayout.Space(5);

                var w = _windowRect.width;
                _windowRect = IMGUIUtils.DragResizeEat(id, _windowRect);
                // Only resize width
                _windowRect.width = w;

                _charaDropdown.DrawDropdownIfOpen();
            }
        }

        private void SetupInterface()
        {
            //const float coordWidth = 25f;
            //const float coordHeight = 20f;

            _buttons.Clear();

            // Let the window auto-size and keep the position while outside maker
            _windowRect = new Rect(x: _windowRect.x != 0 ? _windowRect.x : Screen.width / 2 - 20,
                                   y: _windowRect.y != 0 ? _windowRect.y : 20,
                                   width: WindowWidth,
                                   height: 100);

            _buttons.Add(new ActionButton("Export", () =>
            {
                //PmxBuilder.Test();
                _pmxBuilder = new PmxBuilder();
                component.StartCoroutine(_pmxBuilder.BuildStart().WrapToIl2Cpp());

            }));
            _buttons.Add(new ActionButton("ExportAllOutfits", () =>
            {
                _pmxBuilder = new PmxBuilder();
                _pmxBuilder.exportAllOutfits = true;
                component.StartCoroutine(_pmxBuilder.BuildStart().WrapToIl2Cpp());

            }));
            _buttons.Add(new ActionButton("ExportWithMainCamera", () =>
            {
                _pmxBuilder = new PmxBuilder();
                _pmxBuilder.exportWithMainCamera = true;
                component.StartCoroutine(_pmxBuilder.BuildStart().WrapToIl2Cpp());

            }));
        }
    }
}