namespace FrostweepGames.Plugins.SpeechRepeater
{
    public interface IVoiceDetectionManager
    {
        bool CheckVoice(byte[] data);
    }
}