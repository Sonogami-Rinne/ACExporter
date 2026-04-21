using UnityEngine;

namespace ACExporter
{
    public interface IStateToggleButton
    {
        GUIContent Content { get; }
        void OnClick();
    }
}