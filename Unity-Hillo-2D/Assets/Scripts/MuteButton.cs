using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] string _volumeType;
    [SerializeField] Image _img;
    [SerializeField] Sprite _unmutedIcon;
    [SerializeField] Sprite _mutedIcon;
    float volume = 0;
    bool _muted = false;
    void Start()
    {
        mixer.GetFloat(_volumeType, out volume);
    }
    public void Mute()
    {
        _muted = !_muted;
        print(_volumeType);
        if (_muted == true)
        {
            mixer.SetFloat(_volumeType, -80);
            _img.sprite = _mutedIcon;
        }
        else
        {
            _img.sprite = _unmutedIcon;
            mixer.SetFloat(_volumeType, volume);
        }
    }
    
}
