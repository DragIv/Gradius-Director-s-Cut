using System;
using System.Drawing;

namespace game
{
    public class Collider
    {
        RectangleF bounding_box;
        public GameObject parent;
        public bool exists = true;
        public Collider(PointF location, Size size, GameObject parent)
        {
            bounding_box = new RectangleF(location, size);
            this.parent = parent;
        }
        public void CheckCollision()
        {
            Random random = new Random();
            if (!exists)
            {
                if (parent.animation.current_state != Animation.AnimationStates.Explosion)
                    Form1.gameObjects.Remove(parent);
                return;
            }
            for (int i = 0; i < Form1.gameObjects.Count; i++)
            {
                if (!Form1.gameObjects[i].collider.exists)
                {
                    if (Form1.gameObjects[i].animation.current_state != Animation.AnimationStates.Explosion)
                    {
                        Form1.gameObjects.RemoveAt(i);
                        i--;
                    }
                    continue;
                }
                if (Form1.gameObjects[i].collider.bounding_box.IntersectsWith(bounding_box) && parent != Form1.gameObjects[i])
                {
                    if (parent is Bullet && (parent as Bullet).parent is Player && Form1.gameObjects[i] is Enemy)
                    {
                        (Form1.gameObjects[i] as Enemy).TakeDamage((parent as Bullet).name);
                        if ((Form1.gameObjects[i] as Enemy).HP <= 0)
                        {
                            Form1.gameObjects[i].collider.exists = false;
                            Form1.gameObjects[i].animation.changeState("Explosion");
                            Form1.audio_player.Play("enemyexplosion");
                            if (Form1.rand.Next(100) < 9) Form1.gameObjects.Add(new PowerUp(Form1.gameObjects[i].location, Form1.gameObjects[i].size, 8));
                        }
                        Form1.gameObjects.Remove(parent);
                        return;
                    }
                    if (parent is Enemy && Form1.gameObjects[i] is Player ||
                        parent is Bullet && (parent as Bullet).parent is Enemy && Form1.gameObjects[i] is Player)
                    {
                        (Form1.gameObjects[i] as Player).TakeDamage();
                        if (!(Form1.gameObjects[i] as Player).is_alive)
                        {
                            Form1.gameObjects[i].collider.exists = false;
                            Form1.gameObjects[i].animation.changeState("Explosion");
                        };
                        Form1.gameObjects.Remove(parent);
                        break;
                    }
                    if (parent is PowerUp && Form1.gameObjects[i] is Player)
                    {
                        Form1.gameObjects.Remove(parent);
                        Form1.player.PowerInc();
                    }
                }
            }
        }
        public void Update()
        {
            bounding_box = new RectangleF(parent.location, parent.size);
        }
    }
}