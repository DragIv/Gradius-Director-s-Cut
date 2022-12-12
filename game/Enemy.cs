using System;
using System.Drawing;

namespace game
{
    public class Enemy : MovableObject
    {
        public string enemy_name;
        public int HP = 100;
        public bool can_shoot;
        TimeSpan shoot_delay;
        DateTime last_shoot_bullet = DateTime.Now;
        float distance_traveled = 1000;

        public Enemy(PointF location, Size size, float speed, int HP, string enemy_name, bool can_shoot = false) : base(location, size, speed, enemy_name)
        {
            moving = true;
            this.HP = HP;
            this.can_shoot = can_shoot;
            if (can_shoot)
                shoot_delay = new TimeSpan(0, 0, 0, 2);
        }
        public override void Move()
        {
            if (moving)
            {
                PointF loc;
                switch (name)
                {
                    case "Spinner":
                        if (!direction_buttons[Directions.Left])
                        {
                            direction_buttons[Directions.Left] = true;
                            ChangeDirection();
                        }
                        loc = new PointF(location.X + vector_speed.X, location.Y - (float)Math.Cos(0.5 * ToRadians(location.X)) * 5);
                        distance_traveled += Math.Abs(vector_speed.Y);
                        break;
                    case "Gunboat":
                        if (!direction_buttons[Directions.Left])
                        {
                            direction_buttons[Directions.Left] = true;
                            ChangeDirection();
                        }
                        distance_traveled += Math.Abs(vector_speed.X);
                        if (distance_traveled > 230)
                        {
                            distance_traveled = 0;
                            if (direction_buttons[Directions.Up])
                            {
                                direction_buttons[Directions.Up] = false;
                                direction_buttons[Directions.Down] = true;
                            }
                            else if (direction_buttons[Directions.Down])
                            {
                                direction_buttons[Directions.Up] = true;
                                direction_buttons[Directions.Down] = false;
                            }
                            else if (location.Y > Form1.field.Height / 2)
                                direction_buttons[Directions.Up] = true;
                            else direction_buttons[Directions.Down] = true;
                            ChangeDirection();
                        }
                        loc = new PointF(location.X + vector_speed.X, location.Y - vector_speed.Y);
                        break;
                    case "Jumpy":
                        if (angle == 0)
                        {
                            angle = Form1.rand.Next(225, 246);
                            angle = Form1.rand.Next(2) == 1 ? angle - 90 : angle;
                            UpdateVectorSpeed();
                        }
                        if (location.Y + size.Height > Form1.field.Height || location.Y < 0)
                        {
                            angle = (360 - angle);
                            UpdateVectorSpeed();
                        }
                        loc = new PointF(location.X + vector_speed.X, location.Y - vector_speed.Y);
                        break;
                    default:
                        loc = new PointF(location.X + vector_speed.X, location.Y - vector_speed.Y);
                        break;
                }
                location = loc;
            }
            if (location.X + size.Width < 0 || location.X > Form1.field.Width)
            {
                Form1.gameObjects.Remove(this);
                moving = false;
            }
        }
        public void TakeDamage(string bullet_type)
        {
            int dmg;
            switch (bullet_type)
            {
                case "Bullet":
                    dmg = 25;
                    break;
                case "Laser":
                    dmg = 50;
                    break;
                case "UpperBullet":
                    dmg = 25;
                    break;
                case "Missiles":
                    dmg = 100;
                    break;
                default:
                    dmg = 25;
                    break;
            }
            HP -= dmg;
        }
        public void Shoot()
        {
            if (DateTime.Now - last_shoot_bullet > shoot_delay && can_shoot)
            {
                Form1.gameObjects.Add(new Bullet(new PointF(location.X, location.Y + size.Height / 2), new Size(24, 24), 6, "EnemyBullet", this));
                last_shoot_bullet = DateTime.Now;
            }
        }
    }
}