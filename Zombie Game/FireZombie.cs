using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zombie_Game
{
    class FireZombie : Zombie
    {
        Random randNum = new Random();
        //public string direction;
        //public int attackLeft;
        //public int attackTop;
        private PictureBox attack = new PictureBox();
        private Timer attackTimer = new Timer();


        public FireZombie()
        {
            health = 150;
            speed = 5;
            attackDamage = 1;
        }
        public override PictureBox Spawn()
        {
            PictureBox fireZombie = new PictureBox();
            fireZombie.Tag = "fireZombie";
            fireZombie.Image = Properties.Resources.FireZombieLeft;
            fireZombie.Left = randNum.Next(0, 1000);
            fireZombie.Top = randNum.Next(0, 1000);
            fireZombie.SizeMode = PictureBoxSizeMode.AutoSize;
            FireZombieList.Add(fireZombie);
            return fireZombie;
        }
        //public void MakeAttack(Form form)
        //{
        //    attack.BackColor = Color.DarkRed;
        //    attack.Size = new Size(7, 7);
        //    attack.Tag = "bullet";
        //    attack.Left = attackLeft;
        //    attack.Top = attackTop;
        //    attack.BringToFront();
        //    form.Controls.Add(attack);
        //    attackTimer.Interval = attackSpeed;
        //    attackTimer.Tick += new EventHandler(AttackTimerEvent);
        //    attackTimer.Start();
        //}
        //private void AttackTimerEvent(object sender, EventArgs e)
        //{
        //    if (direction == "left")
        //    {
        //        attack.Left -= 10;
        //    }
        //    if (direction == "right")
        //    {
        //        attack.Left += 10;
        //    }
        //    if (attack.Left < 10 || attack.Left > 950)
        //    {
        //        attackTimer.Stop();
        //        attackTimer.Dispose();
        //        attack.Dispose();
        //        attackTimer = null;
        //        attack = null;
        //    }
        //}
        public override int ZombieAttack()
        {
            return attackDamage;
        }
        public override int getSpeed()
        {
            return speed;
        }
        public override void resetHealth()
        {
            health = 150;
        }
        public override int getHealth()
        {
            return health;
        }
        public override void setSpeed(int x)
        {
            speed += x;
        }
        public override void resetSpeed()
        {
            speed = 5;
        }
    }
}
