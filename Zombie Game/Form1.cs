using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Threading;

using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;//!!!!!!
using System.Runtime.Serialization.Formatters.Binary;

namespace Zombie_Game
{
    public partial class ZombieGame : Form
    {
        Player player = new Player();
        Zombie zombie = new Zombie();
        Zombie fireZombie = new FireZombie();
        Zombie tankZombie = new TankZombie();
        Random randNum = new Random();
        SoundPlayer GunShot = new SoundPlayer("Gunshot.wav");
        SoundPlayer Reload = new SoundPlayer("Reload.wav");

        private List<Scoreboard> listScores = new List<Scoreboard>();

        bool gameOver = true;
        bool isFireZombieSpawned = false;
        bool isTankZombieSpawned = false;
        bool isHealthPackSpawned = false;
        bool TankSpeedMagic = false;
        string name;
        int selectedIndex = -1;
        int kills = 0;
        int highScore = 0;
        public ZombieGame()
        {
            InitializeComponent();
            //RestartGame();
        }
        private void MainTimerEvent(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(110, 110, 110);

            if (player.getHealth() <= 55)
            {
                healthBar.ForeColor = Color.Orange;
            }
            if (player.getHealth() <= 30)
            {
                if (isHealthPackSpawned == false)
                {
                    DropHealthPack();
                    isHealthPackSpawned = true;
                }
            }
            if (player.getHealth() <= 20)
            {
                healthBar.ForeColor = Color.Red;
            }

            if (player.getHealth() > 1)
            {
                healthBar.Value = player.getHealth();
            }

            else
            {
                GameOver();
            }
            ammoLabel.Text = "Ammo: " + player.GetAmmo();
            killsLabel.Text = "Kills: " + kills;

            if (isTankZombieSpawned == true && TankSpeedMagic == false)
            {
                zombie.setSpeed(2);
                fireZombie.setSpeed(2);
                TankSpeedMagic = true;
            }
            if (player.GetLeft() == true && playerImage.Left > 0)
            {
                playerImage.Left -= player.getSpeed();
            }
            if (player.GetRight() == true && playerImage.Left + playerImage.Width < 945)
            {
                playerImage.Left += player.getSpeed();
            }
            if (player.GetUp() == true && playerImage.Top > 0)
            {
                playerImage.Top -= player.getSpeed();
            }
            if (player.GetDown() == true && playerImage.Top + 70 < this.ClientSize.Height)
            {
                playerImage.Top += player.getSpeed();
            }
            foreach (Control x in this.Controls)
            {
                if (kills % 10 == 0 && !isFireZombieSpawned && kills != 0)
                {
                    this.Controls.Add(fireZombie.Spawn());
                    playerImage.BringToFront();
                    isFireZombieSpawned = true;
                }
                if (x is PictureBox && (string)x.Tag == "fireZombie")
                {

                    if (playerImage.Bounds.IntersectsWith(x.Bounds))
                    {
                        player.setHealth(fireZombie.ZombieAttack());
                    }


                    if (x.Left > playerImage.Left)
                    {
                        x.Left -= fireZombie.getSpeed();
                        ((PictureBox)x).Image = Properties.Resources.FireZombieLeft;
                        fireZombie.direction = "left";
                    }
                    if (x.Left < playerImage.Left)
                    {
                        x.Left += fireZombie.getSpeed();
                        ((PictureBox)x).Image = Properties.Resources.FireZombieRight;
                        fireZombie.direction = "right";
                    }
                    if (x.Top > playerImage.Top)
                    {
                        x.Top -= fireZombie.getSpeed();
                    }
                    if (x.Top < playerImage.Top)
                    {
                        x.Top += fireZombie.getSpeed();
                    }
                    foreach (Control j in this.Controls)
                    {
                        if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "fireZombie")
                        {
                            if (x.Bounds.IntersectsWith(j.Bounds))
                            {
                                fireZombie.setHealth(player.GetDamage());
                                this.Controls.Remove(j);
                                ((PictureBox)j).Dispose();
                                if (fireZombie.getHealth() < 1)
                                {
                                    kills++;
                                    this.Controls.Remove(j);
                                    ((PictureBox)j).Dispose();
                                    this.Controls.Remove(x);
                                    ((PictureBox)x).Dispose();
                                    fireZombie.FireZombieList.Remove(((PictureBox)x));
                                    fireZombie.resetHealth();
                                    isFireZombieSpawned = false;
                                }
                            }
                        }
                    }
                }
                if (kills % 15 == 0 && !isTankZombieSpawned && kills != 0)
                {
                    this.Controls.Add(tankZombie.Spawn());
                    playerImage.BringToFront();
                    isTankZombieSpawned = true;
                }
                if (x is PictureBox && (string)x.Tag == "tankZombie")
                {

                    if (playerImage.Bounds.IntersectsWith(x.Bounds))
                    {
                        player.setHealth(tankZombie.ZombieAttack());
                    }


                    if (x.Left > playerImage.Left)
                    {
                        x.Left -= tankZombie.getSpeed();
                        ((PictureBox)x).Image = Properties.Resources.TankZombieLeft;
                    }
                    if (x.Left < playerImage.Left)
                    {
                        x.Left += tankZombie.getSpeed();
                        ((PictureBox)x).Image = Properties.Resources.TankZombieRight;
                    }
                    if (x.Top > playerImage.Top)
                    {
                        x.Top -= tankZombie.getSpeed();
                    }
                    if (x.Top < playerImage.Top)
                    {
                        x.Top += tankZombie.getSpeed();
                    }
                    foreach (Control j in this.Controls)
                    {
                        if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "tankZombie")
                        {
                            if (x.Bounds.IntersectsWith(j.Bounds))
                            {
                                tankZombie.setHealth(player.GetDamage());
                                this.Controls.Remove(j);
                                ((PictureBox)j).Dispose();
                                if (tankZombie.getHealth() < 1)
                                {
                                    kills++;
                                    this.Controls.Remove(j);
                                    ((PictureBox)j).Dispose();
                                    this.Controls.Remove(x);
                                    ((PictureBox)x).Dispose();
                                    tankZombie.TankZombieList.Remove(((PictureBox)x));
                                    tankZombie.resetHealth();
                                    isTankZombieSpawned = false;
                                    TankSpeedMagic = false;
                                    zombie.setSpeed(-1);
                                    fireZombie.setSpeed(-1);
                                }
                            }
                        }
                    }
                }
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (playerImage.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        player.SetAmmo();
                        Reload.Play();
                    }
                }
                if (x is PictureBox && (string)x.Tag == "healthPack")
                {
                    if (playerImage.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        player.SetNewHealth(100);
                        isHealthPackSpawned = false;
                        healthBar.ForeColor = Color.LimeGreen;
                    }
                }
                if (x is PictureBox && (string)x.Tag == "zombie")
                {

                    if (playerImage.Bounds.IntersectsWith(x.Bounds))
                    {
                        player.setHealth(zombie.ZombieAttack());
                    }


                    if (x.Left > playerImage.Left)
                    {
                        x.Left -= zombie.getSpeed();
                        ((PictureBox)x).Image = Properties.Resources.zombieLeft;
                    }
                    if (x.Left < playerImage.Left)
                    {
                        x.Left += zombie.getSpeed();
                        ((PictureBox)x).Image = Properties.Resources.zombieRight;
                    }
                    if (x.Top > playerImage.Top)
                    {
                        x.Top -= zombie.getSpeed();
                    }
                    if (x.Top < playerImage.Top)
                    {
                        x.Top += zombie.getSpeed();
                    }

                    foreach (Control j in this.Controls)
                    {
                        if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                        {
                            if (x.Bounds.IntersectsWith(j.Bounds))
                            {
                                zombie.setHealth(player.GetDamage());
                                if (zombie.getHealth() < 1)
                                {
                                    kills++;
                                    this.Controls.Remove(j);
                                    ((PictureBox)j).Dispose();
                                    this.Controls.Remove(x);
                                    ((PictureBox)x).Dispose();
                                    zombie.zombiesList.Remove(((PictureBox)x));
                                    SpawnZombies();
                                    zombie.resetHealth();
                                }
                            }
                        }
                    }
                }
            }
        }
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver == true)
            {
                return;
            }
            if (gameOver == false)
            {
                if (e.KeyCode == Keys.Left)
                {
                    player.Movement("left");
                    playerImage.Image = Properties.Resources.newLeft;
                }

                if (e.KeyCode == Keys.Right)
                {
                    player.Movement("right");
                    playerImage.Image = Properties.Resources.newRight;
                }

                if (e.KeyCode == Keys.Up)
                {
                    player.Movement("up");
                }

                if (e.KeyCode == Keys.Down)
                {
                    player.Movement("down");
                }
            }

        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                player.StopMovement("left");
            }

            if (e.KeyCode == Keys.Right)
            {
                player.StopMovement("right");
            }

            if (e.KeyCode == Keys.Up)
            {
                player.StopMovement("up");
            }

            if (e.KeyCode == Keys.Down)
            {
                player.StopMovement("down");
            }
            if (e.KeyCode == Keys.Space && player.GetAmmo() > 0 && gameOver == false)
            {
                player.SetAmmo(1);
                ShootBullet(player.facing);
                GunShot.Play();

                if (player.GetAmmo() < 1)
                {
                    DropAmmo();
                    DropAmmo();
                }
            }
            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }
        }
        private void ShootBullet(string direction)
        {
            Player attack = new Player();
            attack.direction = direction;
            attack.attackLeft = playerImage.Left + (playerImage.Width / 2);
            attack.attackTop = playerImage.Top + (playerImage.Height / 2);
            attack.MakeAttack(this);
        }
        //private void FireZombieShoot(string direction)
        //{
        //    FireZombie first = new FireZombie();
        //    fireZombie.direction = direction;
        //    fireZombie.attackLeft = randNum.Next(0, 1000);
        //    fireZombie.attackTop = randNum.Next(0, 1000);
        //    fireZombie.MakeAttack(this);
        //}
        private void SpawnZombies()
        {
            this.Controls.Add(zombie.Spawn());
            playerImage.BringToFront();
        }
        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = randNum.Next(10, 950 - ammo.Width);
            ammo.Top = randNum.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";
            this.Controls.Add(ammo);
            ammo.BringToFront();
            playerImage.BringToFront();
        }
        //private void FireZombieAttack()
        //{
        //    PictureBox trap = new PictureBox();
        //    trap.Image = Properties.Resources.DeathStep;
        //    trap.SizeMode = PictureBoxSizeMode.AutoSize;
        //    trap.Left = randNum.Next(10, 950 - trap.Width);
        //    trap.Top = randNum.Next(60, this.ClientSize.Height - trap.Height);
        //    trap.Tag = "trap";
        //    this.Controls.Add(trap);
        //    trap.BringToFront();
        //    playerImage.BringToFront();
        //}
        private void DropHealthPack()
        {
            PictureBox healthPack = new PictureBox();
            healthPack.Image = Properties.Resources.Health_Pack;
            healthPack.SizeMode = PictureBoxSizeMode.AutoSize;
            healthPack.Left = randNum.Next(10, 950 - healthPack.Width);
            healthPack.Top = randNum.Next(60, this.ClientSize.Height - healthPack.Height);
            healthPack.Tag = "healthPack";
            this.Controls.Add(healthPack);
            healthPack.BringToFront();
            playerImage.BringToFront();
        }
        private void RestartGame()
        {
            StartButton.Enabled = false;
            SaveButton.Enabled = false;
            LoadButton.Enabled = false;
            ScoreBoardGrid.Enabled = false;
            NameTextBox.Enabled = false;
            DeleteFromScorboardButton.Enabled = false;
            UpdateNameButton.Enabled = false;
            playerImage.Location = new Point(443, 338);
            playerImage.Image = Properties.Resources.newRight;

            healthBar.ForeColor = Color.LimeGreen;

            foreach (PictureBox i in zombie.zombiesList)
            {
                this.Controls.Remove(i);
            }
            for (int i = 0; i < 2; i++)
            {
                foreach (Control x in this.Controls)
                {
                    if (x is PictureBox && (string)x.Tag == "ammo")
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();

                    }
                }
            }
            foreach (Control j in this.Controls)
            {
                if (j is PictureBox && (string)j.Tag == "healthPack")
                {
                    this.Controls.Remove(j);

                }
            }
            //foreach (Control j in this.Controls)
            //{
            //    if (j is PictureBox && (string)j.Tag == "DeathStep")
            //    {
            //        this.Controls.Remove(j);

            //    }
            //}
            foreach (Control j in this.Controls)
            {
                if (j is PictureBox && (string)j.Tag == "fireZombie")
                {
                    this.Controls.Remove(j);
                }
            }
            foreach (Control j in this.Controls)
            {
                if (j is PictureBox && (string)j.Tag == "tankZombie")
                {
                    this.Controls.Remove(j);
                }
            }

            zombie.zombiesList.Clear();
            zombie.FireZombieList.Clear();
            zombie.TankZombieList.Clear();

            for (int i = 0; i < 3; i++)
            {
                SpawnZombies();
            }

            player.SetUp(false);
            player.SetDown(false);
            player.SetRight(false);
            player.SetDown(false);

            zombie.resetSpeed();
            fireZombie.resetSpeed();

            gameOver = false;
            isFireZombieSpawned = false;
            isTankZombieSpawned = false;
            isHealthPackSpawned = false;
            TankSpeedMagic = false;

            player.SetNewHealth(100);
            kills = 0;
            player.SetAmmo(10.0); ;
            gameTimer.Start();
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            name = NameTextBox.Text;
        }

        private void DataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedIndex = ScoreBoardGrid.CurrentRow.Index;
        }

        private void UpdateNameB(object sender, EventArgs e)
        {
            if (selectedIndex == -1 || listScores.Count == 0)
            {
                string message = "Please press on a player to Update The name\n";
                string title = "update name error";
                MessageBox.Show(message, title);
            }
            else
            {
                listScores[selectedIndex].SetPlayerName(name);
                ScoreBoardGrid[0, selectedIndex].Value = name;
            }
        }

        private void DeleteFromScoreboardButton(object sender, EventArgs e)
        {
            if (selectedIndex == -1 || listScores.Count == 0)
            {
                string message = "Please press on a player to delete first\n";
                string title = "delete error";
                MessageBox.Show(message, title);
            }
            else
            {
                listScores.RemoveAt(selectedIndex);
                ScoreBoardGrid.Rows.RemoveAt(selectedIndex);
            }
        }

        private void SaveScoreBoard(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();// + "..\\myModels";
            saveFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    //!!!!
                    formatter.Serialize(stream, listScores);
                }
            }
        }

        private void LoadScoreButton(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();// + "..\\myModels";
            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream stream = File.Open(openFileDialog1.FileName, FileMode.Open);
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                //!!!!
                listScores = (List<Scoreboard>)binaryFormatter.Deserialize(stream);
            }
            updatedatagridupload(listScores);
        }
        private void updatedatagridupload(List<Scoreboard> listScores)
        {
            for (int i = 0; i < listScores.Count; i++)
            {
                ScoreBoardGrid.Rows.Add(listScores[i].getplayerName(), listScores[i].getplayerScore());
            }
        }
        private void GameOver()
        {
            gameOver = true;
            StartButton.Enabled = true;
            SaveButton.Enabled = true;
            LoadButton.Enabled = true;
            ScoreBoardGrid.Enabled = true;
            NameTextBox.Enabled = true;
            DeleteFromScorboardButton.Enabled = true;
            UpdateNameButton.Enabled = true;
            isHealthPackSpawned = false;
            listScores.Add(new Scoreboard(kills, name));
            ScoreBoardGrid.Rows.Add(name, kills);
            if (kills > highScore)
            {
                highScore = kills;
                txtHighScore.Text = "High Score: " + Environment.NewLine + highScore;
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
            playerImage.Image = Properties.Resources.newDead;
            healthBar.ForeColor = Color.LightGray;
            gameTimer.Stop();
        }
    }
}