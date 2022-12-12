using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace game
{
    public class AudioPlayer
    {
        public Dictionary<string, sound_pair> sounds = new Dictionary<string, sound_pair>();
        public Dictionary<int, string> music_num = new Dictionary<int, string>();

        public AudioPlayer()
        {
            DirectoryInfo current_folder = new DirectoryInfo(Directory.GetCurrentDirectory());
            string path = current_folder.Parent.Parent.FullName + @"\assets\sounds\";
            foreach (var file_name in Directory.GetFiles(path + @"music"))
            {
                sounds.Add(Path.GetFileNameWithoutExtension(file_name), new sound_pair(file_name));
                music_num.Add(music_num.Count, Path.GetFileNameWithoutExtension(file_name));
            }
            foreach (var file_name in Directory.GetFiles(path + @"sfx"))
            {
                sounds.Add(Path.GetFileNameWithoutExtension(file_name), new sound_pair(file_name));
            }
        }
        public void Play(string name, bool loop = false)
        {
            sounds[name].loop = loop;
            if (sounds[name].outputDevice.PlaybackState == PlaybackState.Playing)
            {
                sounds[name].outputDevice.Stop();
            }
            sounds[name].outputDevice.Play();
        }
        public void Play(int i, bool loop = false)
        {
            Play(music_num[i], loop);
        }
        public void Stop()
        {
            foreach (var sound in sounds)
            {
                sound.Value.outputDevice.Stop();
            }
        }
    }
    public class sound_pair
    {
        public WaveOutEvent outputDevice = new WaveOutEvent();
        public AudioFileReader file;
        public bool loop = false;
        public sound_pair(string file_name)
        {
            file = new AudioFileReader(file_name);
            outputDevice.PlaybackStopped += OnPlaybackStopped;
            outputDevice.Init(file);
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            file.CurrentTime = new TimeSpan(0);
            if (loop)
            {
                outputDevice.Play();
            }
        }
    }
}