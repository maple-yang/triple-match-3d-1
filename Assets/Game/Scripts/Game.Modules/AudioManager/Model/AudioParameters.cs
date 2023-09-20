using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules.AudioManager.Model
{
    [CreateAssetMenu(fileName = nameof(AudioParameters), menuName = "Configs/Modules/"+nameof(AudioParameters), order = 0)]

    public class AudioParameters : ScriptableObject
    {
        [field: SerializeField]
        public AudioSourceComponent AudioSourceComponentReference { get; private set; }

        [field: SerializeField] 
        public List<AudioClipParameter> MusicParameters { get; private set; }
        
        [field: SerializeField] 
        public List<AudioClipParameter> SoundParameters { get; private set; }
    }

    [Serializable]
    public class AudioClipParameter
    {
        public string AudioName;
        public AudioClip AudioClip;
        [Range(0, 1)]
        public float Volume = 1;
    }
}