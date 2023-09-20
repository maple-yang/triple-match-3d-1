using UnityEditor;
using UnityEngine;

namespace Content.Shaders.UniversalShader.RocketGUI
{
    public static class RocketGUIStyle
    {
        private static Color m_orangeColor = new Color(1f, 0.58f, 0f);
        
        static GUIStyle _OrangeBoldLabel;
        public static GUIStyle OrangeBoldLabel
        {
            get
            {
                if(_OrangeBoldLabel == null)
                {
                    var color = m_orangeColor;
                    _OrangeBoldLabel = new GUIStyle(EditorStyles.label);
                    _OrangeBoldLabel.normal.textColor = color;
                    _OrangeBoldLabel.active.textColor = color;
                    _OrangeBoldLabel.focused.textColor = color;
                    _OrangeBoldLabel.hover.textColor = color;
                    _OrangeBoldLabel.fontStyle = FontStyle.Bold;
                }
                return _OrangeBoldLabel;
            }
        }
        
        static GUIStyle _OrangeBoldHeader;
        public static GUIStyle OrangeBoldHeader
        {
            get
            {
                if(_OrangeBoldHeader == null)
                {
                    _OrangeBoldHeader = new GUIStyle(OrangeBoldLabel);
                    _OrangeBoldHeader.fontSize = 14;
                }
                return _OrangeBoldHeader;
            }
        }
        
        static GUIStyle _OrangeInteractiveFoldout;
        public static GUIStyle OrangeInteractiveFoldout
        {
            get
            {
                if (_OrangeInteractiveFoldout == null)
                {
                    _OrangeInteractiveFoldout = new GUIStyle(EditorStyles.foldoutHeader)
                    {
                        fontStyle = FontStyle.Bold,
                    };

                    _OrangeInteractiveFoldout.fontSize = 14;
                    _OrangeInteractiveFoldout.normal.textColor = Color.gray;
                    _OrangeInteractiveFoldout.active.textColor = m_orangeColor;
                    _OrangeInteractiveFoldout.focused.textColor = Color.gray;
                    _OrangeInteractiveFoldout.hover.textColor = m_orangeColor;
                    _OrangeInteractiveFoldout.onNormal.textColor = m_orangeColor;
                    _OrangeInteractiveFoldout.onActive.textColor = m_orangeColor;
                    _OrangeInteractiveFoldout.onFocused.textColor = m_orangeColor;
                    _OrangeInteractiveFoldout.onHover.textColor = m_orangeColor;
                }
                return _OrangeInteractiveFoldout;
            }
        }
    }
}
