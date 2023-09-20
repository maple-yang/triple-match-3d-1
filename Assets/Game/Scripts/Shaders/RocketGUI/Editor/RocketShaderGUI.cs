using Content.Shaders.UniversalShader.RocketGUI;
using UnityEditor;
using UnityEngine;

public class RocketShaderGUI : ShaderGUI
{
    bool specularGroup = false;
    bool gradientGroup = false;
    bool fresnelGroup = false;
    bool otherGroup = false;
    bool additionalLitGroup = false;
    bool additionalUnlitGroup = false;
    
    float fresnelStartValue;
    float fresnelEndValue;
    float shadowStartValue;
    float shadowEndValue;

    private Vector2 animationTextureTiling;
    private Vector2 animationTexturePass;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material targetMat = materialEditor.target as Material;

        Light(materialEditor, properties);
        Base(materialEditor, properties);
        Gradient(materialEditor,targetMat, properties);
        Specular(materialEditor, targetMat, properties);
        Fresnel(materialEditor, targetMat, properties);
        AdditionalLitTexture(materialEditor, targetMat, properties);
        AdditionalUnlitTexture(materialEditor, targetMat, properties);
        EditorGUILayout.Space();
        ShowOtherParameters(materialEditor, properties);
        //Animation(materialEditor, targetMat, properties);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        materialEditor.RenderQueueField();
        materialEditor.EnableInstancingField();
        materialEditor.DoubleSidedGIField();
    }

    private void ShowOtherParameters(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        otherGroup = EditorGUILayout.BeginFoldoutHeaderGroup(otherGroup, "Other parameters",RocketGUIStyle.OrangeInteractiveFoldout);
        
        //TODO Will change with the next patch
        for (int i = otherGroup ? 34 : properties.Length; i < properties.Length - 10; i++)
        { 
            materialEditor.ShaderProperty(properties[i], properties[i].displayName);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void Animation(MaterialEditor materialEditor, Material targetMat, MaterialProperty[] properties)
    {
        MaterialProperty m_animationTextureTiling = FindProperty("AnimationTextureTiling", properties);
        MaterialProperty m_animationTexturePass = FindProperty("AnimationTexturePass", properties);
        MaterialProperty m_animationTextureRotate = FindProperty("AnimationTextureRotate", properties);
        MaterialProperty m_animationTextureSpeed = FindProperty("AnimationTextureSpeed", properties);

        animationTextureTiling = m_animationTextureTiling.vectorValue;
        animationTexturePass = m_animationTexturePass.vectorValue;
        
        materialEditor.ShaderProperty(m_animationTextureSpeed, m_animationTextureSpeed.displayName);
        materialEditor.ShaderProperty(m_animationTextureRotate, m_animationTextureRotate.displayName);
        m_animationTextureTiling.vectorValue = EditorGUILayout.Vector2Field(m_animationTextureTiling.displayName, animationTextureTiling);
        m_animationTexturePass.vectorValue = EditorGUILayout.Vector2Field(m_animationTexturePass.displayName, animationTexturePass);
    }

    private void Gradient(MaterialEditor materialEditor, Material targetMat, MaterialProperty[] properties)
    {
        bool useGradient = targetMat.IsKeywordEnabled("USE_GRADIENT");
        gradientGroup = EditorGUILayout.BeginFoldoutHeaderGroup(useGradient, "HeightGradient", RocketGUIStyle.OrangeInteractiveFoldout);
        if (gradientGroup)
        {
            targetMat.EnableKeyword("USE_GRADIENT");
            MaterialProperty _startColoeGradient = FindProperty("StartColorGradient", properties);
            MaterialProperty _endColorGradient = FindProperty("EndColorGradient", properties);
            MaterialProperty _startZoneGradient = FindProperty("StartZoneGradient", properties);
            MaterialProperty _endZoneGradient = FindProperty("EndZoneGradient", properties);
            MaterialProperty _powerGradient = FindProperty("PowerGradient", properties);
            materialEditor.ShaderProperty(_startColoeGradient, _startColoeGradient.displayName);
            materialEditor.ShaderProperty(_endColorGradient, _endColorGradient.displayName);
            materialEditor.ShaderProperty(_startZoneGradient, _startZoneGradient.displayName);
            materialEditor.ShaderProperty(_endZoneGradient, _endZoneGradient.displayName);
            materialEditor.ShaderProperty(_powerGradient, _powerGradient.displayName);
        }
        else
        {
            targetMat.DisableKeyword("USE_GRADIENT");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void Light(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        EditorGUILayout.LabelField("Light", RocketGUIStyle.OrangeBoldHeader);
        MaterialProperty _intensityUnlitColor = FindProperty("IntensityUnlitColor", properties);
        MaterialProperty _unlitColor = FindProperty("UnlitColor", properties);
        MaterialProperty _frontColor = FindProperty("_FrontColor", properties);
        MaterialProperty _shadowColor = FindProperty("ShadowColor", properties);
        MaterialProperty _shadowSmoothstepTo = FindProperty("ShadowSmoothstepTo", properties);
        MaterialProperty _shadowSmoothstepFrom = FindProperty("ShadowSmoothstepFrom", properties);
        MaterialProperty _smoothness = FindProperty("Smoothness", properties);

        shadowStartValue = _shadowSmoothstepTo.floatValue;
        shadowEndValue = _shadowSmoothstepFrom.floatValue;
        
        materialEditor.ShaderProperty(_intensityUnlitColor, _intensityUnlitColor.displayName);
        materialEditor.ShaderProperty(_unlitColor, _unlitColor.displayName);
        materialEditor.ShaderProperty(_frontColor, _frontColor.displayName);
        materialEditor.ShaderProperty(_shadowColor, _shadowColor.displayName);
        materialEditor.ShaderProperty(_smoothness, _smoothness.displayName);
        EditorGUILayout.MinMaxSlider("Shadow Scattering",ref shadowEndValue, ref shadowStartValue, -3, 3);
        EditorGUILayout.Space();
        
        _shadowSmoothstepTo.floatValue = shadowStartValue;
        _shadowSmoothstepFrom.floatValue = shadowEndValue;
    }
    
    private void Base(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        EditorGUILayout.LabelField("Base", RocketGUIStyle.OrangeBoldHeader);
        MaterialProperty _baseMap = FindProperty("_BaseMap", properties,true);
        MaterialProperty _baseColor = FindProperty("_BaseColor", properties);
        MaterialProperty _normalMap = FindProperty("NormalMap", properties);
        
        GUIContent content = new GUIContent(_baseMap.displayName, _baseMap.textureValue, "Albedo");
        materialEditor.TexturePropertySingleLine(content, _baseMap, _baseColor);

        GUIContent contentNormalMap = new GUIContent(_normalMap.displayName, _normalMap.textureValue, "NormalMapTexture");
        materialEditor.TexturePropertySingleLine(contentNormalMap, _normalMap);
        
        EditorGUI.indentLevel++;
        materialEditor.TextureScaleOffsetProperty(_baseMap);
        EditorGUI.indentLevel--;
    }

    private void Specular(MaterialEditor materialEditor, Material targetMat, MaterialProperty[] properties)
    {
        bool useSpecular = targetMat.IsKeywordEnabled("USE_SPECULAR");
        specularGroup = EditorGUILayout.BeginFoldoutHeaderGroup(useSpecular, "Specular", RocketGUIStyle.OrangeInteractiveFoldout);
        if (specularGroup)
        {
            targetMat.EnableKeyword("USE_SPECULAR");
            MaterialProperty _specularColor = FindProperty("SpecularColor", properties);
            MaterialProperty _specularValue = FindProperty("SpecularValue", properties);
            MaterialProperty _smoothnessSpecular = FindProperty("SmoothnessSpecular", properties);
            
            materialEditor.ShaderProperty(_specularColor, _specularColor.displayName);
            materialEditor.ShaderProperty(_specularValue, _specularValue.displayName);
            materialEditor.ShaderProperty(_smoothnessSpecular, _smoothnessSpecular.displayName);
            EditorGUILayout.Space();
        }
        else
        {
            targetMat.DisableKeyword("USE_SPECULAR");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void AdditionalLitTexture(MaterialEditor materialEditor, Material targetMat, MaterialProperty[] properties)
    {
        bool useAdditionalLitTexture = targetMat.IsKeywordEnabled("USE_ADDITIONAL_LIT_TEXTURE");
        bool useMoveAdditionalLitTexture = targetMat.IsKeywordEnabled("USE_MOVE_ADDITIONAL_LIT_TEXTURE");
        
        additionalLitGroup = EditorGUILayout.BeginFoldoutHeaderGroup(useAdditionalLitTexture, "AdditionalLitTexture", RocketGUIStyle.OrangeInteractiveFoldout);
        if (additionalLitGroup)
        {
            targetMat.EnableKeyword("USE_ADDITIONAL_LIT_TEXTURE");
            MaterialProperty _addLitTexture = FindProperty("AddLitTexture", properties);
            MaterialProperty _colorAddLitTexture = FindProperty("ColorAddLitTexture", properties);
            
            GUIContent content = new GUIContent(_addLitTexture.displayName, _addLitTexture.textureValue, "BaseTexture");
            materialEditor.TexturePropertySingleLine(content, _addLitTexture, _colorAddLitTexture);
        
            EditorGUI.indentLevel++;
            materialEditor.TextureScaleOffsetProperty(_addLitTexture);
            EditorGUI.indentLevel--;
            
            MaterialProperty m_animationTextureRotate = FindProperty("AdditionalLitTextureRotate", properties);
            materialEditor.ShaderProperty(m_animationTextureRotate, "Rotate");

            
            EditorGUILayout.Space();
            
            if (EditorGUILayout.Toggle("Use Move Texture",useMoveAdditionalLitTexture))
            {
                targetMat.EnableKeyword("USE_MOVE_ADDITIONAL_LIT_TEXTURE");
                
                MaterialProperty m_animationTextureSpeed = FindProperty("AdditionalLitTextureSpeed", properties);
                MaterialProperty animationTextureRotateMove = FindProperty("AdditionalLitTextureRotateMove", properties);
                
                materialEditor.ShaderProperty(animationTextureRotateMove, "Rotate move");
                materialEditor.ShaderProperty(m_animationTextureSpeed, "Speed");
            }
            else
            {
                targetMat.DisableKeyword("USE_MOVE_ADDITIONAL_LIT_TEXTURE");
            }
        }
        else
        {
            targetMat.DisableKeyword("USE_ADDITIONAL_LIT_TEXTURE");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void AdditionalUnlitTexture(MaterialEditor materialEditor, Material targetMat, MaterialProperty[] properties)
    {
        bool useAdditionalLitTexture = targetMat.IsKeywordEnabled("USE_ADDITIONAL_UNLIT_TEXTURE");
        bool useMoveAdditionalLitTexture = targetMat.IsKeywordEnabled("USE_MOVE_ADDITIONAL_UNLIT_TEXTURE");

        additionalLitGroup = EditorGUILayout.BeginFoldoutHeaderGroup(useAdditionalLitTexture, "AdditionalUnlitTexture", RocketGUIStyle.OrangeInteractiveFoldout);
        if (additionalLitGroup)
        {
            targetMat.EnableKeyword("USE_ADDITIONAL_UNLIT_TEXTURE");
            MaterialProperty _addUnlitTexture = FindProperty("AddUnlitTexture", properties);
            MaterialProperty _colorAddUnlitTexture = FindProperty("ColorAddUnlitTexture", properties);
            
            GUIContent content = new GUIContent(_addUnlitTexture.displayName, _addUnlitTexture.textureValue, "BaseTexture");
            materialEditor.TexturePropertySingleLine(content, _addUnlitTexture, _colorAddUnlitTexture);
        
            EditorGUI.indentLevel++;
            materialEditor.TextureScaleOffsetProperty(_addUnlitTexture);
            EditorGUI.indentLevel--;
            
            MaterialProperty animationTextureRotate = FindProperty("AdditionalUnlitTextureRotate", properties);
            materialEditor.ShaderProperty(animationTextureRotate, "Rotate");

            EditorGUILayout.Space();
            
            if (EditorGUILayout.Toggle("Use Move Texture",useMoveAdditionalLitTexture))
            {
                targetMat.EnableKeyword("USE_MOVE_ADDITIONAL_UNLIT_TEXTURE");
                
                MaterialProperty animationTextureSpeed = FindProperty("AdditionalUnlitTextureSpeed", properties);
                MaterialProperty animationTextureRotateMove = FindProperty("AdditionalUnlitTextureRotateMove", properties);
                
                materialEditor.ShaderProperty(animationTextureRotateMove, "Rotate move");
                materialEditor.ShaderProperty(animationTextureSpeed, "Speed");
            }
            else
            {
                targetMat.DisableKeyword("USE_MOVE_ADDITIONAL_UNLIT_TEXTURE");
            }
        }
        else
        {
            targetMat.DisableKeyword("USE_ADDITIONAL_UNLIT_TEXTURE");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void Fresnel(MaterialEditor materialEditor, Material targetMat, MaterialProperty[] properties)
    {
        bool useFresnel = targetMat.IsKeywordEnabled("USE_FRESNEL");
        fresnelGroup = EditorGUILayout.BeginFoldoutHeaderGroup(useFresnel, "Fresnel",RocketGUIStyle.OrangeInteractiveFoldout);
        if (fresnelGroup)
        {
            targetMat.EnableKeyword("USE_FRESNEL");
            MaterialProperty _fresnelColor = FindProperty("FresnelEffectColor", properties);
            MaterialProperty _fresnelStart = FindProperty("FresnelStart", properties);
            MaterialProperty _fresnelEnd = FindProperty("FresnelEnd", properties);
            fresnelStartValue = _fresnelStart.floatValue;
            fresnelEndValue = _fresnelEnd.floatValue;
            
            materialEditor.ShaderProperty(_fresnelColor, _fresnelColor.displayName);
            EditorGUILayout.MinMaxSlider("Fresnel Scattering",ref fresnelStartValue, ref fresnelEndValue, 0, 5);
            
            _fresnelStart.floatValue = fresnelStartValue;
            _fresnelEnd.floatValue = fresnelEndValue;
            EditorGUILayout.Space();
        }
        else
        {
            targetMat.DisableKeyword("USE_FRESNEL");
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
