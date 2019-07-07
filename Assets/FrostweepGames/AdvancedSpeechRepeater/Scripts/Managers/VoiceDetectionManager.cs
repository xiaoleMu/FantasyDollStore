namespace FrostweepGames.Plugins.SpeechRepeater
{
    public class VoiceDetectionManager : IService, IVoiceDetectionManager
    {
        private double _threshold;

        public void Init()
        {

        }

        public void Dispose()
        {

        }

        public void Update()
        {

        }

        public bool CheckVoice(byte[] data)
        {
            _threshold = AdvancedSpeechRepeater.Instance.config.voiceDetectionThreshold;

            return ProcessData(data);
        }


        private bool ProcessData(byte[] data)
        {
            bool detected = false;
            double sumTwo = 0;
            double tempValue;

            for (int index = 0; index < data.Length; index += 2)
            {
                tempValue = (short)((data[index + 1] << 8) | data[index + 0]);

                tempValue /= Constants.AUDIO_DETECT_RATIO;

                sumTwo += tempValue * tempValue;

                if (tempValue > _threshold)
                    detected = true;
            }

            sumTwo /= (data.Length / 2);

            if (detected || sumTwo > _threshold)
                return true;
            else
                return false;
        }
    }
}