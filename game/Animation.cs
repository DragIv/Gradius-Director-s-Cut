using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace game
{
    public class Animation
    {
        public enum AnimationStates { Forward, Upwards, Downwards, Explosion };
        Dictionary<AnimationStates, List<Image>> sprites = new Dictionary<AnimationStates, List<Image>>();
        Dictionary<string, AnimationStates> sprites_string = new Dictionary<string, AnimationStates>()
        {
            {"Forward",  AnimationStates.Forward},
            {"Upwards", AnimationStates.Upwards},
            {"Downwards",  AnimationStates.Downwards},
            {"Explosion",  AnimationStates.Explosion},
        };
        Dictionary<string, int> frames_count = new Dictionary<string, int>()
        {
            {"Forward", 1},
            {"Upwards", 1},
            {"Downwards", 1},
            {"Explosion", 1},
        };
        public AnimationStates current_state = AnimationStates.Forward;
        public GameObject parent;
        public int frame_number = 0;
        private string sprites_path;
        private TimeSpan frame_duration;
        private DateTime last_frame_change;

        public Animation(TimeSpan frame_duration, GameObject parent)
        {
            last_frame_change = DateTime.Now;
            this.frame_duration = frame_duration;
            this.parent = parent;
            string[] class_name = parent.ToString().Split('.');
            sprites_path = Form1.root + "\\assets\\sprites\\" + class_name[class_name.Length - 1] + "\\" + parent.name;
            StreamReader sr = new StreamReader(sprites_path + "\\frames_info.txt");
            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(' ');
                frames_count[line[0]] = int.Parse(line[1]);
            }
            string[] files;
            files = Directory.GetFiles(sprites_path);

            foreach (string file in files)
            {
                string[] state = file.Split('\\');
                if (state[state.Length - 1].Split('.')[1] == "txt") continue;
                Image image = Image.FromFile(file);
                List<Image> images = new List<Image>();
                int parts = image.Width / frames_count[state[state.Length - 1].Split('.')[0]];
                for (int i = 0; i < image.Width; i += parts)
                {
                    Bitmap bmpImage = new Bitmap(image);
                    Rectangle selection = new Rectangle(i, 0, parts, image.Height);
                    images.Add(bmpImage.Clone(selection, bmpImage.PixelFormat));
                    bmpImage.Dispose();
                }
                sprites[sprites_string[state[state.Length - 1].Split('.')[0]]] = images;
            }
        }
        public void changeState(string state)
        {
            if (sprites.ContainsKey(sprites_string[state]) && current_state != sprites_string[state])
            {
                current_state = sprites_string[state];
                frame_number = 0;
            }
        }
        public Image GetCurrentSprite()
        {
            if (DateTime.Now - last_frame_change >= frame_duration)
            {
                if (sprites[current_state].Count - 1 > frame_number)
                    frame_number++;
                else
                {
                    if (current_state == AnimationStates.Explosion)
                    {
                        current_state = AnimationStates.Forward;
                        return sprites[AnimationStates.Explosion][frame_number - 1];
                    }
                    frame_number = 0;
                }
                last_frame_change = DateTime.Now;
            }
            return sprites[current_state][frame_number];
        }
    }
}