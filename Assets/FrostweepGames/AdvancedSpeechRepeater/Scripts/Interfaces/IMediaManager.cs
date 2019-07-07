using System;
using UnityEngine;

namespace FrostweepGames.Plugins.SpeechRepeater
{
    public interface IMediaManager
    {
        event Action StartedListeningEvent;
        event Action<AudioClip> FinishedListeningEvent;
        event Action ListeningFailedEvent;

        event Action EndAudioPlayingEvent;

        event Action BeginTalkigEvent;
        event Action<AudioClip> EndTalkigEvent;

        bool IsCanWork { get; set; }
        bool IsListening { get; set; }

        void StopListening();
        void StartListening();
    }
}