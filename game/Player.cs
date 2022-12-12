using System;
using System.Drawing;

namespace game
{
    public class Player : MovableObject
    {
        public bool is_alive = true;
        public bool is_shooting = false;
        TimeSpan shoot_delay;
        DateTime last_shoot_bullet;
        DateTime last_shoot_missile;
        public int HP = 1;
        public int powerUps = 0;
        public enum ShootingStyle { Bullet, Laser, Tripple };
        public ShootingStyle currentShootingStyle;
        public bool missiles = false;
        public Option[] options = new Option[2];
        public Shield shield = null;
        public Player(PointF location, Size size, float speed) : base(location, size, speed)
        {
            last_shoot_bullet = DateTime.Now;
            last_shoot_missile = DateTime.Now;
            shoot_delay = new TimeSpan(0, 0, 0, 0, 100);
            currentShootingStyle = ShootingStyle.Bullet;
        }

        public override void Draw()
        {
            foreach (Option option in options)
                if (option != null) Form1.graphics.DrawImage(option.animation.GetCurrentSprite(), option.location);
            if (shield != null) Form1.graphics.DrawImage(shield.animation.GetCurrentSprite(), shield.location);
            base.Draw();
        }

        public override void Move()
        {
            if (is_alive)
            {
                base.Move();
                foreach (Option option in options)
                    if (option != null)
                    {
                        option.Move();
                        option.collider.Update();
                    }
                if (shield != null)
                {
                    shield.UpdateLocation();
                }
            }
        }

        public void Shoot()
        {
            if (DateTime.Now - last_shoot_bullet > shoot_delay && is_alive)
            {
                switch (currentShootingStyle)
                {
                    case ShootingStyle.Bullet:
                        {
                            Form1.gameObjects.Add(new Bullet(new PointF(location.X + size.Width, location.Y + size.Height / 2), new Size(40, 15), speed + 12, "Bullet", this));

                        }
                        foreach (Option option in options)
                            if (option != null) Form1.gameObjects.Add(new Bullet(new PointF(option.location.X + option.size.Width, option.location.Y + option.size.Height / 2), new Size(40, 15), option.speed + 12, "Bullet", this));
                        break;
                    case ShootingStyle.Laser:
                        Form1.gameObjects.Add(new Bullet(new PointF(location.X + size.Width, location.Y + size.Height / 2), new Size(40, 15), speed + 12, "Laser", this));
                        foreach (Option option in options)
                            if (option != null) Form1.gameObjects.Add(new Bullet(new PointF(option.location.X + option.size.Width, option.location.Y + option.size.Height / 2), new Size(40, 15), option.speed + 12, "Laser", this));
                        break;
                    case ShootingStyle.Tripple:
                        Form1.gameObjects.Add(new Bullet(new PointF(location.X + size.Width, location.Y + size.Height / 2), new Size(40, 15), speed + 12, "Bullet", this));
                        Form1.gameObjects.Add(new Bullet(new PointF(location.X + size.Width, location.Y + size.Height / 2 - 10), new Size(40, 15), speed + 12, "UpperBullet", this));
                        foreach (Option option in options)
                        {
                            if (option != null)
                            {
                                Form1.gameObjects.Add(new Bullet(new PointF(option.location.X + option.size.Width, option.location.Y + option.size.Height / 2), new Size(40, 15), option.speed + 12, "Bullet", this));
                                Form1.gameObjects.Add(new Bullet(new PointF(option.location.X + option.size.Width, option.location.Y + option.size.Height / 2 - 10), new Size(40, 15), option.speed + 12, "UpperBullet", this));
                            }
                        }
                        break;
                }
                last_shoot_bullet = DateTime.Now;
            }
            if (missiles && DateTime.Now - last_shoot_missile > (new TimeSpan(0, 0, 0, 0, shoot_delay.Milliseconds * 5)) && is_alive)
            {
                Form1.gameObjects.Add(new Bullet(new PointF(location.X + size.Width / 2, location.Y + size.Height), new Size(30, 24), 8, "Missiles", this));
                foreach (Option option in options)
                    if (option != null) Form1.gameObjects.Add(new Bullet(new PointF(option.location.X + option.size.Width / 2, option.location.Y + option.size.Height), new Size(30, 24), 8, "Missiles", this));
                last_shoot_missile = DateTime.Now;
            }
        }
        public void TakeDamage()
        {
            if (shield != null)
            {
                shield.TakeDamage();
            }
            else
            {
                HP -= 1;
                if (HP <= 0)
                {
                    is_alive = false;
                    Form1.current_game_state = Form1.GameStates.Lose;
                    Form1.audio_player.Stop();
                    Form1.audio_player.Play("gameover");
                }
            }
        }

        public void PowerUp()
        {
            switch (powerUps)
            {
                case 0:
                    break;
                case 1:
                    if (speed < 16)
                    {
                        speed += 2;
                        foreach (Option obj in options)
                        {
                            if (obj != null) obj.speed += 2;
                        }
                        powerUps = 0;
                    }
                    break;
                case 2:
                    if (missiles == false)
                    {
                        missiles = true;
                        powerUps = 0;
                    }
                    break;
                case 3:
                    if (currentShootingStyle != ShootingStyle.Tripple)
                    {
                        currentShootingStyle = ShootingStyle.Tripple;
                        powerUps = 0;
                    }
                    break;
                case 4:
                    if (currentShootingStyle != ShootingStyle.Laser)
                    {
                        currentShootingStyle = ShootingStyle.Laser;
                        powerUps = 0;
                    }
                    break;
                case 5:
                    if (options[0] == null)
                    {
                        options[0] = new Option(location, new Size(64, 40), speed, this);
                        powerUps = 0;
                    }
                    else if (options[1] == null)
                    {
                        options[1] = new Option(location, new Size(64, 40), speed, options[0]);
                        powerUps = 0;
                    }
                    break;
                case 6:
                    if (shield == null || shield.HP < 2)
                    {
                        shield = new Shield(new PointF(location.X + size.Width, location.Y), size, this, 2);
                        powerUps = 0;
                    }
                    break;
            }
        }

        internal void PowerInc()
        {
            if (powerUps < 6)
            {
                Form1.audio_player.Play("powerup");
                powerUps++;
            }
        }
    }
}