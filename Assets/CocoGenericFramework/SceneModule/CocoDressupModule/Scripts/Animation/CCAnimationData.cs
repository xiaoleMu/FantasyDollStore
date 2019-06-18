using UnityEngine;
using System.Collections;
using CocoPlay;
#if COCO_FAKE
using CocoAudioID = CocoPlay.Fake.CocoAudioID;


#else
using CocoAudioID = Game.CocoAudioID;

#endif

public class CCAnimationData
{
    public CCAnimationData()
    {
    }

    public CCAnimationData(string _AnimName, string _audio = "", WrapMode pWrapMode = WrapMode.Once)
    {
        AnimName = _AnimName;
        Audio = _audio;
        mWrapMode = pWrapMode;
    }

    public CCAnimationData(string _AnimName, CocoAudioID audioID, WrapMode pWrapMode = WrapMode.Once)
    {
        string _audio = CocoRoot.GetInstance<ICocoAudioData>().GetAudioName(audioID);

        AnimName = _AnimName;
        Audio = _audio;
        mWrapMode = pWrapMode;
    }

    public CCAnimationData(string _AnimName, string audioname, WrapMode pWrapMode, float time, string path)
    {
        AnimName = _AnimName;
        Audio = audioname;
        mWrapMode = pWrapMode;
        normalizedTime = time;
        AssetsPath = path;
    }

    public CCAnimationData Clone()
    {
        var CCAnimationData = new CCAnimationData();
        CCAnimationData.AnimName = this.AnimName;
        CCAnimationData.Audio = this.Audio;
        CCAnimationData.mWrapMode = this.mWrapMode;
        CCAnimationData.normalizedTime = this.normalizedTime;
        CCAnimationData.AssetsPath = this.AssetsPath;
        CCAnimationData.Key = this.Key;
        return CCAnimationData;
    }

    public string AnimName;
    public string Audio;
    public WrapMode mWrapMode;
    public float normalizedTime = 0.0f;
    public string AssetsPath;
    public string Key;
    public bool isProp;
}