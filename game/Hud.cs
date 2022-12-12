using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace game
{
    public class Hud
    {
        public int powerup_height = 70;
        string[] images_path = new string[6];
        private string sprites_path;
        private int powerup_width = 0;
        private Dictionary<string, int> powerup_to_number = new Dictionary<string, int>()
        {
            {"speed", 1},
            {"missile", 2},
            {"double", 3},
            {"laser", 4},
            {"option", 5},
            {"mark", 6},
        };
        private Dictionary<char, int> char_to_num = new Dictionary<char, int>()
        {
            {'a', 1},
            {'b', 2},
            {'c', 3},
            {'d', 4},
            {'e', 5},
            {'f', 6},
            {'g', 7},
            {'i', 8},
            {'k', 9},
            {'l', 10},
            {'m', 11},
            {'n', 12},
            {'o', 13},
            {'p', 14},
            {'r', 15},
            {'s', 16},
            {'t', 17},
            {'u', 18},
            {'y', 19},
            {'w', 20},
            {'!', 21},
            {' ', 22},
        };
        List<Image> alphabet = new List<Image>();

        public Hud()
        {
            sprites_path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\assets\\sprites\\Hud";
            string[] files = Directory.GetFiles(sprites_path + "\\powerup_inactive");
            foreach (string file in files)
            {
                string[] path = file.Split('\\');
                if (path[path.Length - 1].Split('.')[0] != "empty")
                {
                    images_path[powerup_to_number[path[path.Length - 1].Split('.')[0]] - 1] = file;
                }
            }
            foreach (string file in images_path)
            {
                powerup_width += Image.FromFile(file).Width + 50;
            }
            Image image = Image.FromFile(sprites_path + "\\Alphabet\\Eng.png");
            int parts = image.Width / char_to_num.Count;
            for (int i = 0; i < image.Width; i += parts)
            {
                Bitmap bmpImage = new Bitmap(image);
                Rectangle selection = new Rectangle(i, 0, parts, image.Height);
                alphabet.Add(bmpImage.Clone(selection, bmpImage.PixelFormat));
                bmpImage.Dispose();
            }
        }
        public void DrawPowerups()
        {
            for (int i = 0; i < images_path.Length; i++)
            {
                string temp = images_path[i];
                string[] image_path = temp.Split('\\');
                switch (image_path[image_path.Length - 1].Split('.')[0])
                {
                    case "speed":
                        if (Form1.player.speed >= 16)
                        {
                            image_path[image_path.Length - 1] = "empty.png";
                        }
                        break;
                    case "missile":
                        if (Form1.player.missiles)
                        {
                            image_path[image_path.Length - 1] = "empty.png";
                        }
                        break;
                    case "double":
                        if (Form1.player.currentShootingStyle == Player.ShootingStyle.Tripple)
                        {
                            image_path[image_path.Length - 1] = "empty.png";
                        }
                        break;
                    case "laser":
                        if (Form1.player.currentShootingStyle == Player.ShootingStyle.Laser)
                        {
                            image_path[image_path.Length - 1] = "empty.png";
                        }
                        break;
                    case "option":
                        if (Form1.player.options[0] != null && Form1.player.options[1] != null)
                        {
                            image_path[image_path.Length - 1] = "empty.png";
                        }
                        break;
                    case "mark":
                        if (Form1.player.shield != null && Form1.player.shield.HP == 2)
                        {
                            image_path[image_path.Length - 1] = "empty.png";
                        }
                        break;
                }
                temp = sprites_path + "\\powerup_inactive\\" + image_path[image_path.Length - 1];
                if (i + 1 == Form1.player.powerUps)
                {
                    temp = sprites_path + "\\powerup_active\\" + image_path[image_path.Length - 1];
                }
                Image image = Image.FromFile(temp);
                Form1.graphics.DrawImage(image, new Point((Form1.field.Width - powerup_width) / 2 + (image.Width + 50) * i, Form1.field.Height - powerup_height));
            }
        }
        public void ShowMessage(string message)
        {
            for (int j = 1; j <= message.Length; j++)
            {
                PointF p = new PointF(Form1.field.Width / 2 - (28 * message.Length / 2), Form1.field.Height / 2 - 28);
                Form1.graphics.DrawImage(alphabet[char_to_num[message[j - 1]] - 1], new PointF(p.X + 28 * j, p.Y));
            }
        }
    }
}