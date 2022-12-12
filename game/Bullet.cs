using System.Drawing;

namespace game
{
    public class Bullet : MovableObject
    {
        public GameObject parent;
        public Bullet(PointF location, Size size, float speed, string name, GameObject parent) : base(location, size, speed, name)
        {
            this.parent = parent;
            this.name = name;
            if (parent is Player)
            {
                direction_buttons[Directions.Right] = true;
                if (Form1.audio_player.sounds.ContainsKey(name.ToLower()))
                {
                    Form1.audio_player.Play(name.ToLower());
                }
                if (name == "UpperBullet")
                {
                    direction_buttons[Directions.Up] = true;
                }
                ChangeDirection();
                if (name == "Missiles")
                {
                    angle = 285;
                    animation.changeState("Downwards");
                    UpdateVectorSpeed();
                    //Form1.audio_player.Play("missile");
                }
            }
            else if (parent is Enemy)
            {
                angle = getAngleBetweenEntities(Form1.player.location);
                if (angle >= 0 && angle <= 180)
                {
                    animation.changeState("Upwards");
                }
                else if (angle > 180 && angle <= 360)
                {
                    animation.changeState("Downwards");
                }
                UpdateVectorSpeed();
            }
            moving = true;
        }
        public override void Move()
        {

            base.Move();
            if (location.X < 0 || location.X > Form1.field.Width)
            {
                Form1.gameObjects.Remove(this);
                moving = false;
            }
            if (name == "Missiles" && location.Y >= Form1.field.Height - size.Height)
            {
                angle = 0;
                animation.changeState("Forward");
                UpdateVectorSpeed();
            }
        }
    }
}