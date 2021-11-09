using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Sandbox;
using System;

namespace EnhancedUI.Utils
{
    /// <summary>
    /// This exists as a alternative to Keen's audio system if you want to play custom sounds without the limitations of Keen's audio system.
    /// </summary>
    internal class AudioPlayer
    {
        FadeInOutSampleProvider ?fade;
        private string playlist;

        public AudioPlayer(string startingPlaylist)
        {
            playlist = startingPlaylist;
        }

        public void Play(bool shouldLoop, bool shouldFadein, double? fadeDurationInMilliseconds = null)
        {
            if (shouldFadein != false && fadeDurationInMilliseconds == null)
            {
                throw new Exception("[EnhancedUI Audio System]: Cannot play audio because shouldFadein was set to true but fadeDurationInMilliseconds was not passed into EnhancedUI.Utils.AudioPlayer.Play(bool shouldLoop, bool shouldFadein, double? fadeDurationInMilliseconds = null).");
            }
            var audio = new AudioFileReader(playlist);
            audio.Volume = MySandboxGame.Config.MusicVolume;
            if (shouldLoop == true && shouldFadein != true)
            {
                LoopStream loop = new LoopStream(audio);
                var waveOutDevice = new WaveOutEvent();
                waveOutDevice.Init(loop);
                waveOutDevice.Play();
                return;
            }
            else if (shouldFadein == true && shouldLoop != true && fadeDurationInMilliseconds != null)
            {
                WaveToSampleProvider waveToSampleProvider = new WaveToSampleProvider(audio);
                fade = new FadeInOutSampleProvider(waveToSampleProvider, true);
                fade.BeginFadeIn((double)fadeDurationInMilliseconds);
                var waveOutDevice = new WaveOutEvent();
                waveOutDevice.Init(fade);
                waveOutDevice.Play();
                return;
            }
            else if (shouldLoop == true && shouldFadein == true && fadeDurationInMilliseconds != null)
            {
                LoopStream loop = new LoopStream(audio);
                WaveToSampleProvider waveToSampleProvider = new WaveToSampleProvider(loop);
                fade = new FadeInOutSampleProvider(waveToSampleProvider, true);
                fade.BeginFadeIn((double)fadeDurationInMilliseconds);
                var waveOutDevice = new WaveOutEvent();
                waveOutDevice.Init(fade);
                waveOutDevice.Play();
                return;
            }
            else
            {
                var waveOutDevice = new WaveOutEvent();
                waveOutDevice.Init(audio);
                waveOutDevice.Play();
            }
        }

        public void FadeOut()
        {
            if (fade != null)
            {
                fade.BeginFadeOut(5000);
            }
        }
    }
}
