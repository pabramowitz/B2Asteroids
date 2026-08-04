namespace B2Asteroids
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    ///    Summary description for Form1.
    /// </summary>
    public partial class Asteroids : Form
    {
        private const double PI_DIV_180 = 0.017453293;
        private const int MAX_TRIG_TERMS = 10;
        private const int BULLET_MAX = 10;
        private const double BULLET_STEP = 7.0;
        private const double BULLET_TOTAL = 300;
        private const int MAX_ASTEROIDS = 16;
        private const double SIZE_BIG = 40.0;
        private const double SIZE_MED = SIZE_BIG / 2.0;
        private const double SIZE_SMALL = SIZE_MED / 2.0;
        private const double SQRT_2 = 1.4142136;
        private const int UFO_DELAY = 300;
        private const int UFO_SPEED = 4;
        private const int UFO_SHOOT_DELAY = 16;
        private const int UFO_BULLET_MAX = 8;
        private const int UFO_PTS = 8;
        private const int PLANE_PTS = 12;

        SolidBrush planeBrush, blackBrush, bulletBrush, asteroidBrush, textBrush, ufoBrush;
        Pen ufoPen, bulletPen;
        Font textFont;
        Graphics gc, winGc;
        Point[] planeShape;
        Point[] planeLoc;
        Point[] asteroidBig;
        Point[] asteroidMed;
        Point[] asteroidSmall;
        Point[] ufoShape;
        Point[] ufoLoc;
        int[] bulletX;
        int[] bulletY;
        int[] bulletDist;
        int[] bulletHead;
        double[] asteroidX;
        double[] asteroidY;
        int[] asteroidSize;
        int[] asteroidHeading;
        int[] ufoBulletX;
        int[] ufoBulletY;
        int[] ufoBulletHead;
        Boolean turnLeft, turnRight, moveForward, gameFinished, bShield;
        double sinX, cosX, playerX, playerY, playerHead, playerSpeed, speedX, speedY;
        int maxDiv4, maxDiv2, numDestroyed, score, lives;
        int ufoX, ufoY, ufoCount, ufoHead, ufoBulletCount;
        int ufoExplosionX, ufoExplosionY, ufoExplosionRange;
        int asteroidScore, shieldRemaining;
        Random rr;
        double asteroidSpeed;
        Bitmap theBitmap;

        private Timer timer1;

        public Asteroids()
        {
            InitializeComponent();
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeWindow()
        {
            this.timer1 = new Timer(this.components);
            //@this.TrayHeight = 90;
            //@this.TrayLargeIcon = false;
            //@this.TrayAutoArrange = true;
            //@timer1.SetLocation (new System.Drawing.Point (7, 7));
            timer1.Interval = 25;
            timer1.Tick += new System.EventHandler(this.TimerTick);
            this.Text = "B2 Asteroids";
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 16);
            this.ClientSize = new System.Drawing.Size(492, 473);
            this.KeyDown += new KeyEventHandler(this.Asteroids_KeyDown);
            this.Resize += new System.EventHandler(this.Asteroids_Resize);
            this.KeyUp += new KeyEventHandler(this.Asteroids_KeyUp);
            this.Paint += new PaintEventHandler(this.Asteroids_Paint);
            this.Layout += new LayoutEventHandler(this.Asteroids_Layout);
        }

        protected void Asteroids_Resize(object sender, System.EventArgs e)
        {
            theBitmap = new Bitmap(this.Width, this.Height);
            gc = Graphics.FromImage(theBitmap);
        }

        protected void Asteroids_Paint(object sender, PaintEventArgs e)
        {
            DrawGameboard();
        }

        protected void Asteroids_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                turnLeft = false;
            else if (e.KeyCode == Keys.Right)
                turnRight = false;
            else if (e.KeyCode == Keys.Up)
                moveForward = false;
            else if (e.KeyCode == Keys.Down)
            {
                bShield = false;
            }
        }

        protected void Asteroids_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                turnLeft = true;
            else if (e.KeyCode == Keys.Right)
                turnRight = true;
            else if (e.KeyCode == Keys.Up)
                moveForward = true;
            else if (e.KeyCode == Keys.Down && shieldRemaining >= 0)
                bShield = true;

            //Player shoots
            else if (e.KeyCode == Keys.Space && bShield == false)
            {
                int i;
                bool bFound = false;

                PSinCos(playerHead);

                for (i = 0; i < BULLET_MAX && bFound == false; i++)
                {
                    if (bulletX[i] == -1)
                    {
                        bulletX[i] = (int)(playerX + 10.0 * sinX);
                        bulletY[i] = (int)(playerY - 10.0 * cosX);
                        bulletHead[i] = (int)playerHead;
                        bulletDist[i] = 0;
                        bFound = true;
                    }
                }
            }

            //Restart game
            else if (e.KeyCode == Keys.F5)
            {
                StartGame();
            }
        }

        protected void Asteroids_Layout(object sender, LayoutEventArgs e)
        {
            InitializeWindow();

            //Init variables
            winGc = Graphics.FromHwnd(this.Handle);
            theBitmap = new Bitmap(this.Width, this.Height);
            gc = Graphics.FromImage(theBitmap);

            blackBrush = new SolidBrush(Color.Black);
            planeBrush = new SolidBrush(Color.Gray);
            bulletBrush = new SolidBrush(Color.White);
            asteroidBrush = new SolidBrush(Color.Brown);
            textBrush = new SolidBrush(Color.White);
            ufoBrush = new SolidBrush(Color.LightGreen);
            ufoPen = new Pen(ufoBrush, 3);
            bulletPen = new Pen(bulletBrush, 2);

            textFont = new Font("Arial", 12);

            rr = new Random();

            maxDiv2 = MAX_ASTEROIDS / 2;
            maxDiv4 = MAX_ASTEROIDS / 4;

            planeShape = new Point[PLANE_PTS];
            planeShape[0] = new Point(0, 4);
            planeShape[1] = new Point(2, 2);
            planeShape[2] = new Point(3, 4);
            planeShape[3] = new Point(5, 2);
            planeShape[4] = new Point(10, 5);
            planeShape[5] = new Point(13, 2);
            planeShape[6] = new Point(0, -12);
            planeShape[7] = new Point(-13, 2);
            planeShape[8] = new Point(-10, 5);
            planeShape[9] = new Point(-5, 2);
            planeShape[10] = new Point(-3, 4);
            planeShape[11] = new Point(-2, 2);

            asteroidBig = new Point[8];
            asteroidBig[0] = new Point(0, (int)-SIZE_BIG);
            asteroidBig[1] = new Point((int)(-SIZE_BIG / SQRT_2), (int)(-SIZE_BIG / SQRT_2));
            asteroidBig[2] = new Point((int)(-SIZE_BIG), 0);
            asteroidBig[3] = new Point((int)(-SIZE_BIG / SQRT_2), (int)(SIZE_BIG / SQRT_2));
            asteroidBig[4] = new Point(0, (int)(SIZE_BIG));
            asteroidBig[5] = new Point((int)(SIZE_BIG / SQRT_2), (int)(SIZE_BIG / SQRT_2));
            asteroidBig[6] = new Point((int)(SIZE_BIG), 0);
            asteroidBig[7] = new Point((int)(SIZE_BIG / SQRT_2), (int)(-SIZE_BIG / SQRT_2));

            asteroidMed = new Point[8];
            asteroidMed[0] = new Point(0, (int)(-SIZE_MED));
            asteroidMed[1] = new Point((int)(-SIZE_MED / SQRT_2), (int)(-SIZE_MED / SQRT_2));
            asteroidMed[2] = new Point((int)(-SIZE_MED), 0);
            asteroidMed[3] = new Point((int)(-SIZE_MED / SQRT_2), (int)(SIZE_MED / SQRT_2));
            asteroidMed[4] = new Point(0, (int)(SIZE_MED));
            asteroidMed[5] = new Point((int)(SIZE_MED / SQRT_2), (int)(SIZE_MED / SQRT_2));
            asteroidMed[6] = new Point((int)(SIZE_MED), 0);
            asteroidMed[7] = new Point((int)(SIZE_MED / SQRT_2), (int)(-SIZE_MED / SQRT_2));

            asteroidSmall = new Point[8];
            asteroidSmall[0] = new Point(0, (int)(-SIZE_SMALL));
            asteroidSmall[1] = new Point((int)(-SIZE_SMALL / SQRT_2), (int)(-SIZE_SMALL / SQRT_2));
            asteroidSmall[2] = new Point((int)(-SIZE_SMALL), 0);
            asteroidSmall[3] = new Point((int)(-SIZE_SMALL / SQRT_2), (int)(SIZE_SMALL / SQRT_2));
            asteroidSmall[4] = new Point(0, (int)(SIZE_SMALL));
            asteroidSmall[5] = new Point((int)(SIZE_SMALL / SQRT_2), (int)(SIZE_SMALL / SQRT_2));
            asteroidSmall[6] = new Point((int)(SIZE_SMALL), 0);
            asteroidSmall[7] = new Point((int)(SIZE_SMALL / SQRT_2), (int)(-SIZE_SMALL / SQRT_2));

            ufoShape = new Point[UFO_PTS];
            ufoShape[0] = new Point(14, 0);
            ufoShape[1] = new Point(6, 0);
            ufoShape[2] = new Point(5, -7);
            ufoShape[3] = new Point(-4, -7);
            ufoShape[4] = new Point(-6, 0);
            ufoShape[5] = new Point(-14, 0);
            ufoShape[6] = new Point(-11, 8);
            ufoShape[7] = new Point(11, 8);

            planeLoc = new Point[PLANE_PTS];
            ufoLoc = new Point[UFO_PTS];
            bulletX = new int[BULLET_MAX];
            bulletY = new int[BULLET_MAX];
            bulletDist = new int[BULLET_MAX];
            bulletHead = new int[BULLET_MAX];
            asteroidX = new double[MAX_ASTEROIDS];
            asteroidY = new double[MAX_ASTEROIDS];
            asteroidSize = new int[MAX_ASTEROIDS];
            asteroidHeading = new int[MAX_ASTEROIDS];
            ufoBulletX = new int[UFO_BULLET_MAX];
            ufoBulletY = new int[UFO_BULLET_MAX];
            ufoBulletHead = new int[UFO_BULLET_MAX];

            turnLeft = false;
            turnRight = false;
            moveForward = false;

            StartGame();
        }

        protected void TimerTick(object sender, System.EventArgs e)
        {
            ufoCount++;
            if (ufoCount == UFO_DELAY)
            {
                ufoCount = 0;
                GenerateUfo();
            }

            MoveAll();
            TestUfoShoot();
            UpdateShield();
            CheckHit();
            DrawGameboard();

            if (numDestroyed == MAX_ASTEROIDS + maxDiv2 + maxDiv4)
                NextLevel();
        }

        public void UpdateShield()
        {
            if (bShield == true)
            {
                shieldRemaining -= 1;
                if (shieldRemaining <= 0)
                {
                    bShield = false;
                    shieldRemaining = 0;
                }
            }
        }

        private void StartGame()
        {
            int i;

            score = 0;
            lives = 3;
            shieldRemaining = 100;
            ufoCount = 0;
            ufoBulletCount = 0;
            ufoExplosionRange = -1;
            gameFinished = false;
            asteroidSpeed = 1.5;
            asteroidScore = 0;
            NextLevel();
            ufoX = -1;

            for (i = 0; i < UFO_BULLET_MAX; i++)
                ufoBulletX[i] = -1;
            timer1.Enabled = true;
        }

        private void ResetPlayer()
        {
            playerX = this.Width / 2;
            playerY = this.Height / 2;
            playerHead = 0;
            speedX = 0;
            speedY = 0;
        }

        private void NextLevel()
        {
            int i;

            asteroidSpeed += 0.5;
            asteroidScore++;

            ResetPlayer();
            numDestroyed = 0;

            for (i = 0; i < BULLET_MAX; i++)
                bulletX[i] = -1;

            for (i = 0; i < MAX_ASTEROIDS; i++)
                asteroidX[i] = -1;

            for (i = 0; i < maxDiv4; i++)
            {
                asteroidX[i] = 0;
                asteroidY[i] = 0;
                asteroidSize[i] = (int)SIZE_BIG;
                asteroidHeading[i] = rr.Next(0, 360);
            }
        }

        private void TestUfoShoot()
        {
            if (ufoX != -1)
            {
                ufoBulletCount++;
                if (ufoBulletCount == UFO_SHOOT_DELAY)
                {
                    ufoBulletCount = 0;
                    UfoShoot();
                }
            }
        }

        private void UfoShoot()
        {
            int i;

            for (i = 0; i < UFO_BULLET_MAX; i++)
            {
                if (ufoBulletX[i] == -1)
                {
                    ufoBulletX[i] = ufoX;
                    ufoBulletY[i] = ufoY;
                    ufoBulletHead[i] = rr.Next(360);

                    break;
                }
            }
        }

        private void DrawGameboard()
        {
            DrawMap();
            DrawAsteroids();
            if (ufoX != -1)
                DrawUfo();
            DrawExplosion();
            DrawBullets();
            DrawPlayer();
            DrawUfoBullets();
            DrawScore();

            //End game if
            if (gameFinished == true)
            {
                timer1.Enabled = false;

                Point p = new()
                {
                    X = 100,
                    Y = 100
                };
                gc.DrawString("Game Over - Hit 'F5' to restart", textFont, textBrush, p);
            }

            //Copy bitmap to screen
            winGc.DrawImage(theBitmap, 0, 0);
        }

        private void GenerateUfo()
        {
            int dir, head;

            dir = rr.Next(4);
            head = rr.Next(80) - 39;

            if (dir == 0)
            {
                ufoX = 0;
                ufoY = rr.Next(this.Height) / 2 + this.Height / 4;
                ufoHead = 90 + head;
            }
            else if (dir == 1)
            {
                ufoX = this.Width;
                ufoY = rr.Next(this.Height) / 2 + this.Height / 4;
                ufoHead = 270 + head;
            }
            else if (dir == 2)
            {
                ufoX = rr.Next(this.Width) / 2 + this.Width / 4;
                ufoY = 0;
                ufoHead = 180 + head;
            }
            else if (dir == 3)
            {
                ufoX = rr.Next(this.Width) / 2 + this.Width / 4;
                ufoY = this.Height;
                ufoHead = head;
                if (ufoHead < 0)
                    ufoHead += 360;
            }
        }

        private void DrawUfo()
        {
            int i;

            for (i = 0; i < UFO_PTS; i++)
            {
                ufoLoc[i].X = ufoX + ufoShape[i].X;
                ufoLoc[i].Y = ufoY + ufoShape[i].Y;
            }
            gc.FillPolygon(ufoBrush, ufoLoc);
        }

        private void DrawExplosion()
        {
            if (ufoExplosionRange > 0)
            {
                gc.DrawEllipse(ufoPen,
                              ufoExplosionX - ufoExplosionRange,
                              ufoExplosionY - ufoExplosionRange,
                              2 * ufoExplosionRange, 2 * ufoExplosionRange);
                ufoExplosionRange += 10;
                if (ufoExplosionRange >= 100)
                    ufoExplosionRange = -1;
            }
        }

        private void DrawScore()
        {
            String scoreStr = "Score: " + score;
            Point p = new()
            {
                X = 10,
                Y = 10
            };
            gc.DrawString(scoreStr, textFont, textBrush, p);

            scoreStr = "Shield: " + shieldRemaining + "%";
            p = new Point
            {
                X = 10,
                Y = 30
            };
            gc.DrawString(scoreStr, textFont, textBrush, p);

            scoreStr = "Lives: " + lives;
            p = new Point
            {
                X = this.Width - 100,
                Y = 10
            };
            gc.DrawString(scoreStr, textFont, textBrush, p);
        }

        private void DrawAsteroids()
        {
            int i;

            for (i = 0; i < MAX_ASTEROIDS; i++)
            {
                if (asteroidX[i] != -1)
                {
                     gc.FillEllipse(asteroidBrush,
                                   (int)asteroidX[i] - asteroidSize[i],
                                   (int)asteroidY[i] - asteroidSize[i],
                                   2 * asteroidSize[i], 2 * asteroidSize[i]);
                }
            }
        }

        private void DrawBullets()
        {
            int i;

            for (i = 0; i < BULLET_MAX; i++)
            {
                if (bulletX[i] != -1)
                    gc.FillRectangle(bulletBrush, bulletX[i] - 1, bulletY[i] - 1, 3, 3);
            }
        }

        private void DrawUfoBullets()
        {
            int i;

            for (i = 0; i < UFO_BULLET_MAX; i++)
            {
                if (ufoBulletX[i] != -1)
                    gc.FillRectangle(ufoBrush, ufoBulletX[i] - 1, ufoBulletY[i] - 1, 3, 3);
            }

        }

        private void DrawMap()
        {
            gc.FillRectangle(blackBrush, 0, 0, this.Width, this.Height);
        }

        private void DrawPlayer()
        {
            //The commented out code also works and uses region instead of polygon
            //      Region r;
            //      GraphicsPath gp;
            int i;

            PSinCos(playerHead);

            for (i = 0; i < PLANE_PTS; i++)
                planeLoc[i] = new Point(
                      (int)(playerX + planeShape[i].X * cosX - planeShape[i].Y * sinX),
                      (int)(playerY + planeShape[i].X * sinX + planeShape[i].Y * cosX));

            //      gp = new GraphicsPath(planeLoc, graphicType12);
            //      r = new Region(gp);
            //      gc.FillRegion(planeBrush, r); 
            gc.FillPolygon(planeBrush, planeLoc);

            if (bShield == true)
                gc.DrawEllipse(bulletPen,
                              (int)(playerX - 20),
                              (int)(playerY - 20),
                              40, 40);

        }

        private void DecreaseLives()
        {
            lives--;
            if (lives < 0)
            {
                lives = 0;
            }
            if (lives == 0)
            {
                gameFinished = true;
            }
            else
            {
                shieldRemaining = 100;
                ResetPlayer();
            }
        }

        private void MoveAll()
        {
            int i;

            //Player turn left
            if (turnLeft == true)
            {
                playerHead -= 7;
                if (playerHead < 0)
                    playerHead += 360;
            }

            //Player turn right
            if (turnRight == true)
            {
                playerHead += 7;
                if (playerHead >= 360)
                    playerHead -= 360;
            }

            //Player accelerate
            if (moveForward == true)
            {
                PSinCos(playerHead);
                speedX += 0.2 * sinX;
                speedY -= 0.2 * cosX;
                playerSpeed = Math.Sqrt(speedX * speedX + speedY * speedY);
                if (playerSpeed > 5)
                {
                    double t1;
                    t1 = 5.0 / playerSpeed;
                    speedX *= t1;
                    speedY *= t1;
                }
            }

            //Move player
            playerX += speedX;
            playerY += speedY;

            if (playerX < 0)
                playerX += this.Width;
            else if (playerX > this.Width)
                playerX -= this.Width;

            if (playerY < 0)
                playerY += this.Height;
            else if (playerY > this.Height)
                playerY -= this.Height;


            //Move bullets
            for (i = 0; i < BULLET_MAX; i++)
            {
                if (bulletX[i] != -1)
                {
                    PSinCos(bulletHead[i]);
                    bulletX[i] += (int)(BULLET_STEP * sinX);
                    bulletY[i] -= (int)(BULLET_STEP * cosX);

                    if (bulletX[i] < 0)
                        bulletX[i] += this.Width;
                    else if (bulletX[i] > this.Width)
                        bulletX[i] -= this.Width;
                    if (bulletY[i] < 0)
                        bulletY[i] += this.Height;
                    else if (bulletY[i] > this.Height)
                        bulletY[i] -= this.Height;

                    bulletDist[i] += (int)BULLET_STEP;
                    if (bulletDist[i] > BULLET_TOTAL)
                        bulletX[i] = -1;
                }
            }

            //Move Asteroids
            for (i = 0; i < MAX_ASTEROIDS; i++)
            {
                if (asteroidX[i] != -1)
                {
                    PSinCos(asteroidHeading[i]);
                    asteroidX[i] += (asteroidSpeed * sinX);
                    asteroidY[i] -= (asteroidSpeed * cosX);

                    if (asteroidX[i] < 0)
                        asteroidX[i] += this.Width;
                    else if (asteroidX[i] > this.Width)
                        asteroidX[i] -= this.Width;
                    if (asteroidY[i] < 0)
                        asteroidY[i] += this.Height;
                    else if (asteroidY[i] > this.Height)
                        asteroidY[i] -= this.Height;
                }
            }

            //Move bullets
            for (i = 0; i < UFO_BULLET_MAX; i++)
            {
                if (ufoBulletX[i] != -1)
                {
                    PSinCos(ufoBulletHead[i]);
                    ufoBulletX[i] = (int)(ufoBulletX[i] + BULLET_STEP * sinX);
                    ufoBulletY[i] = (int)(ufoBulletY[i] - BULLET_STEP * cosX);
                    if (ufoBulletX[i] < 0 || ufoBulletX[i] > this.Width ||
                        ufoBulletY[i] < 0 || ufoBulletY[i] > this.Height)
                        ufoBulletX[i] = -1;
                }
            }

            //Move ufo
            if (ufoX != -1)
            {
                PSinCos(ufoHead);
                ufoX += (int)(UFO_SPEED * sinX);
                ufoY -= (int)(UFO_SPEED * cosX);

                if (ufoX < 0 || ufoX > this.Width ||
                    ufoY < 0 || ufoY > this.Height)
                    ufoX = -1;
            }

        }

        private void PSinCos(double angle)
        {
            double sinTerm, cosTerm, numerator, denominator;
            int k;

            angle *= PI_DIV_180;
            sinTerm = angle;
            cosTerm = 1.0;
            sinX = sinTerm;
            cosX = cosTerm;
            numerator = angle * angle;

            for (k = 1; k <= MAX_TRIG_TERMS; k++)
            {
                denominator = 4 * k * k;
                sinTerm = -sinTerm * numerator / (denominator + 2 * k);
                cosTerm = -cosTerm * numerator / (denominator - 2 * k);
                sinX += sinTerm;
                cosX += cosTerm;
            }
        }

        private void CheckHit()
        {
            int i, j;
            double dx, dy, dist;
            bool bFound;
            Region r, r2;
            Rectangle rr;
            GraphicsPath gp;

            //Check if player shot anything
            for (i = 0; i < BULLET_MAX; i++)
            {
                if (bulletX[i] != -1)
                {
                    bFound = false;

                    //Check if an asteroid shot
                    for (j = 0; j < MAX_ASTEROIDS && bFound == false; j++)
                    {
                        if (asteroidX[j] != -1)
                        {
                            dx = asteroidX[j] - bulletX[i];
                            dy = asteroidY[j] - bulletY[i];
                            dist = dx * dx + dy * dy;
                            if (dist <= asteroidSize[j] * asteroidSize[j])
                            {
                                bulletX[i] = -1;
                                numDestroyed++;

                                //Split asteroid if it is large
                                if (asteroidSize[j] == (int)SIZE_BIG)
                                {
                                    asteroidSize[j] = (int)(SIZE_MED);
                                    asteroidHeading[j] = asteroidHeading[j] - 90;
                                    if (asteroidHeading[j] < 0)
                                        asteroidHeading[j] += 360;
                                    asteroidSize[j + maxDiv2] = (int)(SIZE_MED);
                                    asteroidX[j + maxDiv2] = asteroidX[j];
                                    asteroidY[j + maxDiv2] = asteroidY[j];
                                    asteroidHeading[j + maxDiv2] = asteroidHeading[j] + 180;
                                    if (asteroidHeading[j + maxDiv2] > 360)
                                        asteroidHeading[j + maxDiv2] -= 360;
                                    score += asteroidScore;
                                }

                                //Split asteroid if it is medium
                                else if (asteroidSize[j] == (int)SIZE_MED)
                                {
                                    asteroidSize[j] = (int)(SIZE_SMALL);
                                    asteroidHeading[j] = asteroidHeading[j] - 90;
                                    if (asteroidHeading[j] < 0)
                                        asteroidHeading[j] += 360;
                                    asteroidSize[j + maxDiv4] = (int)(SIZE_SMALL);
                                    asteroidX[j + maxDiv4] = asteroidX[j];
                                    asteroidY[j + maxDiv4] = asteroidY[j];
                                    asteroidHeading[j + maxDiv4] = asteroidHeading[j] + 180;
                                    if (asteroidHeading[j + maxDiv4] > 360)
                                        asteroidHeading[j + maxDiv4] -= 360;
                                    score += (asteroidScore + 1);
                                }

                                //Remove asteroid if it is small
                                else
                                {
                                    asteroidX[j] = -1;
                                    score += (asteroidScore + 2);
                                }
                            }
                        }
                    }
                }
            }

            //Check if ufo and player collided
            if (ufoX != -1 && bShield == false)
            {
                gp = new GraphicsPath();
                gp.AddPolygon(ufoLoc);
                r = new Region(gp);
                gp = new GraphicsPath();
                gp.AddPolygon(planeLoc);
                r2 = new Region(gp);
                r.Intersect(r2);
                if (!r.IsEmpty(gc))
                {
                    DecreaseLives();
                }
            }

            //Check if player ran into ufo explosion
            if (ufoExplosionRange > 0 && bShield == false)
            {
                bool pointBelow = false;
                bool pointAbove = false;
                for (i = 0; i < PLANE_PTS; i++)
                {
                    dx = ufoExplosionX - planeLoc[i].X;
                    dy = ufoExplosionY - planeLoc[i].Y;
                    dist = dx * dx + dy * dy;
                    if (dist <= ufoExplosionRange * ufoExplosionRange)
                        pointBelow = true;
                    else
                        pointAbove = true;
                }

                if (pointAbove && pointBelow)
                {
                    DecreaseLives();
                }
            }

            //Check if player ran into asteroid
            for (i = 0; i < PLANE_PTS && bShield == false; i++)
            {
                for (j = 0; j < MAX_ASTEROIDS; j++)
                {
                    if (asteroidX[j] != -1)
                    {
                        dx = asteroidX[j] - planeLoc[i].X;
                        dy = asteroidY[j] - planeLoc[i].Y;
                        dist = dx * dx + dy * dy;
                        if (dist <= asteroidSize[j] * asteroidSize[j])
                        {
                            DecreaseLives();
                        }
                    }
                }
            }

            //Check if ufo ran into asteroid
            if (ufoX != -1)
            {
                for (i = 0; i < UFO_PTS && ufoX != -1; i++)
                {
                    for (j = 0; j < MAX_ASTEROIDS && ufoX != -1; j++)
                    {
                        if (asteroidX[j] != -1)
                        {
                            dx = asteroidX[j] - ufoLoc[i].X;
                            dy = asteroidY[j] - ufoLoc[i].Y;
                            dist = dx * dx + dy * dy;
                            if (dist < asteroidSize[j] * asteroidSize[j])
                            {
                                ufoExplosionRange = 10;
                                ufoExplosionX = ufoX;
                                ufoExplosionY = ufoY;
                                ufoX = -1;
                            }
                        }
                    }
                }
            }

            //Check if ufo shot an asteroid
            for (i = 0; i < UFO_BULLET_MAX; i++)
            {
                if (ufoBulletX[i] != -1)
                {
                    for (j = 0; j < MAX_ASTEROIDS; j++)
                    {
                        if (asteroidX[j] != -1)
                        {
                            dx = asteroidX[j] - ufoBulletX[i];
                            dy = asteroidY[j] - ufoBulletY[i];
                            dist = dx * dx + dy * dy;
                            if (dist < asteroidSize[j] * asteroidSize[j])
                                ufoBulletX[i] = -1;
                        }
                    }
                }
            }

            //check if ufo shot player
            gp = new GraphicsPath();
            gp.AddPolygon(planeLoc);
            for (i = 0; i < UFO_BULLET_MAX && bShield == false; i++)
            {
                if (ufoBulletX[i] != -1)
                {
                    rr = new Rectangle(ufoBulletX[i] - 1, ufoBulletY[i] - 1, 3, 3);
                    r2 = new Region(rr);
                    r = new Region(gp);
                    r.Intersect(r2);
                    if (!r.IsEmpty(gc))
                    {
                        DecreaseLives();
                    }
                }
            }

            //check if player shot ufo
            bool bHit = false;
            if (ufoX != -1)
            {
                gp = new GraphicsPath();
                gp.AddPolygon(ufoLoc);

                for (i = 0; i < BULLET_MAX && bHit == false; i++)
                {
                    if (bulletX[i] != -1)
                    {
                        r = new Region(gp);
                        rr = new Rectangle(bulletX[i] - 1, bulletY[i] - 1, 3, 3);
                        r2 = new Region(rr);
                        r.Intersect(r2);
                        if (!r.IsEmpty(gc))
                        {
                            ufoExplosionRange = 10;
                            ufoExplosionX = ufoX;
                            ufoExplosionY = ufoY;
                            ufoX = -1;
                            bulletX[i] = -1;
                            score += 25;
                            bHit = true;
                        }
                    }
                }
            }

        }
    }
}