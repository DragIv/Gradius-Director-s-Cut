using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace game
{
    public partial class Form1 : Form
    {
        BufferedGraphicsContext context;
        BufferedGraphics b_graphics;
        public static Graphics graphics;
        public static Rectangle field;
        public static string root;
        public static List<GameObject> gameObjects = new List<GameObject>();
        public static List<Star> stars = new List<Star>();
        public static Player player;
        public static Hud hud;
        public static GameStates current_game_state;
        public static Random rand;
        public static AudioPlayer audio_player = new AudioPlayer();
        public DateTime timer = DateTime.Now;
        public enum GameStates { Playing, Win, Lose };
        public int maximum_stars = 60;

        CancellationTokenSource token;


        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            Cursor.Hide();

            Size = SystemInformation.PrimaryMonitorSize;
            panel1.Size = SystemInformation.PrimaryMonitorSize;

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(panel1.Width + 1, panel1.Height + 1);
            b_graphics = context.Allocate(panel1.CreateGraphics(),
                 new Rectangle(0, 0, panel1.Width, panel1.Height));
            graphics = b_graphics.Graphics;

            root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            field = new Rectangle(0, 0, panel1.Width, panel1.Height);
            player = new Player(new PointF(100, 320), new Size(104, 48), 3);
            hud = new Hud();
            current_game_state = GameStates.Playing;
            rand = new Random();
            gameObjects.Add(player);

            for (int i = 0; i < maximum_stars; i++)
            {
                stars.Add(new Star(new Point(rand.Next(field.Width), rand.Next(field.Height)), new Size(3, 3), rand.Next(3, 8)));
            }
            audio_player.Play(rand.Next(audio_player.music_num.Count));
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                timer = DateTime.Now;
                if (token != null) token.Cancel();
                gameObjects.Clear();
                player = new Player(new PointF(100, 320), new Size(104, 48), 3);
                gameObjects.Add(player);
                hud = new Hud();
                current_game_state = GameStates.Playing;
            }
            if (player != null)
            {
                if (e.KeyCode == Keys.Z)
                {
                    player.is_shooting = true;
                }
                if (e.KeyCode == Keys.Up)
                {
                    player.direction_buttons[MovableObject.Directions.Up] = true;
                }
                if (e.KeyCode == Keys.Down)
                {
                    player.direction_buttons[MovableObject.Directions.Down] = true;
                }
                if (e.KeyCode == Keys.Right)
                {
                    player.direction_buttons[MovableObject.Directions.Right] = true;
                }
                if (e.KeyCode == Keys.Left)
                {
                    player.direction_buttons[MovableObject.Directions.Left] = true;
                }
                player.ChangeDirection();
            }

        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (player != null)
            {
                if (e.KeyCode == Keys.Z)
                {
                    player.is_shooting = false;
                }
                if (e.KeyCode == Keys.Space)
                {
                    player.PowerUp();
                }
                if (e.KeyCode == Keys.Up)
                {
                    player.direction_buttons[Player.Directions.Up] = false;
                }
                if (e.KeyCode == Keys.Down)
                {
                    player.direction_buttons[Player.Directions.Down] = false;
                }
                if (e.KeyCode == Keys.Right)
                {
                    player.direction_buttons[Player.Directions.Right] = false;
                }
                if (e.KeyCode == Keys.Left)
                {
                    player.direction_buttons[Player.Directions.Left] = false;
                }
            }
            player.ChangeDirection();
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'p' || e.KeyChar == 'P')
            {
                timerAction.Enabled = !timerAction.Enabled;
                timerSpawnEnemy.Enabled = !timerSpawnEnemy.Enabled;
                timerDraw.Enabled = !timerDraw.Enabled;
            }
        }
        private void timerDraw_Tick(object sender, EventArgs e)
        {
            graphics.Clear(Color.Black);
            foreach (Star star in stars)
            {
                star.Draw();
            }
            foreach (GameObject obj in gameObjects)
            {
                obj.Draw();

            }
            hud.DrawPowerups();
            bool has_Enemies = false;
            foreach (GameObject obj in gameObjects)
            {
                if (obj is Enemy) has_Enemies = true;
            }
            if (current_game_state == GameStates.Win && !has_Enemies)
            {
                hud.ShowMessage("you win!");
            }
            else if (current_game_state == GameStates.Lose)
            {
                hud.ShowMessage("you lose! press r to retry");
            }
            b_graphics.Render();
        }
        private void timerAction_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i] is MovableObject)
                {
                    try
                    {
                        (gameObjects[i] as MovableObject).Move();
                        (gameObjects[i] as MovableObject).collider.Update();
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].collider.CheckCollision();
            }
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].Move();
            }
            if (player.is_shooting) player.Shoot();
            int gameObjects_Count = gameObjects.Count;
            for (int i = 0; i < gameObjects_Count; i++)
            {
                if (gameObjects[i] is Enemy)
                {
                    (gameObjects[i] as Enemy).Shoot();
                }
            }
            if (stars.Count < maximum_stars) stars.Add(new Star(new Point(field.Width, rand.Next(field.Height)), new Size(3, 3), rand.Next(3, 8)));

            if (DateTime.Now - timer > TimeSpan.FromSeconds(300))
            {
                current_game_state = GameStates.Win;
                timerSpawnEnemy.Enabled = false;
            }
        }

        private void timerSpawnEnemy_Tick(object sender, EventArgs e)
        {
            token = new CancellationTokenSource();
            spawnEnemyAsync(rand.Next(120), token);

            if (DateTime.Now - timer < TimeSpan.FromSeconds(25))
            {
                timerSpawnEnemy.Interval = rand.Next(3000, 5000);
            }
            else if (DateTime.Now - timer < TimeSpan.FromSeconds(60))
            {
                timerSpawnEnemy.Interval = rand.Next(2000, 2500);
            }
            else if (DateTime.Now - timer < TimeSpan.FromSeconds(80))
            {
                timerSpawnEnemy.Interval = rand.Next(500, 1500);
            }
            else if (DateTime.Now - timer < TimeSpan.FromSeconds(100))
            {
                timerSpawnEnemy.Interval = rand.Next(500, 1500);
            }
            else if (DateTime.Now - timer < TimeSpan.FromSeconds(200))
            {
                timerSpawnEnemy.Interval = rand.Next(500, 1000);
            }
        }
        async private void spawnEnemyAsync(int enemy_type, CancellationTokenSource token)
        {
            float c_Y = rand.Next(field.Height - 64);
            if (enemy_type < 25)
            {
                for (int i = 0; i < rand.Next(4, 11); i++)
                {
                    gameObjects.Add(new Enemy(new PointF(panel1.Width, c_Y), new Size(55, 50), 5, 100, "Sphere"));
                    await Task.Delay(250);
                    if (token.IsCancellationRequested) return;
                }
            }
            if (enemy_type >= 25 && enemy_type < 65)
            {
                for (int i = 0; i < rand.Next(4, 11); i++)
                {
                    gameObjects.Add(new Enemy(new PointF(panel1.Width, c_Y), new Size(60, 60), 5, 100, "Spinner"));
                    await Task.Delay(250);
                    if (token.IsCancellationRequested) return;
                }
            }
            if (enemy_type >= 65 && enemy_type < 90)
            {
                for (int i = 0; i < rand.Next(2, 4); i++)
                {
                    gameObjects.Add(new Enemy(new PointF(panel1.Width, c_Y), new Size(56, 64), 3, 150, "Gunboat", true));
                    await Task.Delay(600);
                    if (token.IsCancellationRequested) return;
                }
            }
            if (enemy_type >= 90 && enemy_type <= 120)
            {
                for (int i = 0; i < rand.Next(3, 6); i++)
                {
                    gameObjects.Add(new Enemy(new PointF(panel1.Width, c_Y), new Size(56, 64), 6, 100, "Jumpy", false));
                    await Task.Delay(600);
                    if (token.IsCancellationRequested) return;
                }
            }
        }
    }
}