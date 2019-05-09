using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System;
using GlmNet;
using System.Media;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Graphics
{
    public partial class GraphicsForm : Form
    {
        Renderer renderer = new Renderer();
        Thread MainLoopThread;
        public SoundPlayer Music;
        float deltaTime;
        int ImageCounter = 0;
        public GraphicsForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();
            SoundPlayer Music = new SoundPlayer("relaxingPiano.wav");
            MoveCursor();

           
            initialize();
            deltaTime = 0.005f;
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();
            Music.Play();


        }
        void initialize()
        {
            renderer.Initialize();   
        }
        void MainLoop()
        {
            
            while (true)
            {
                renderer.Draw();
                renderer.Update(deltaTime);

                simpleOpenGlControl1.Refresh();
                
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            renderer.Draw();
            renderer.Update(deltaTime);
        }




        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            float speed = 1000f;
            vec3 camPos = renderer.cam.GetCameraPosition();
            vec3 camDir = renderer.cam.GetLookDirection();
            vec3 camUp = renderer.cam.GetUPVector();


            if (camPos.x > -850000f && camPos.x < 850000f && camPos.y < 850000f && camPos.z > -850000f && camPos.z < 850000f)
            {

                if (e.KeyChar == 'a')
                    renderer.cam.Strafe(-speed);
                if (e.KeyChar == 'd')
                    renderer.cam.Strafe(speed);
                if (e.KeyChar == 's')
                    renderer.cam.Walk(-speed);
                if (e.KeyChar == 'w')
                    renderer.cam.Walk(speed);

                int x = (int)renderer.cam.mPosition.x;
                int z = (int)renderer.cam.mPosition.z;

                if (renderer.cam.mPosition.x % 1000 != 0)
                    x = ((int)(renderer.cam.mPosition.x / 1000) + 1) * 1000;
                if (renderer.cam.mPosition.z % 1000 != 0)
                    z = ((int)(renderer.cam.mPosition.z / 1000) + 1) * 1000;

                Tuple<int, int> XZTuple = new Tuple<int, int>(x, z);
                if (renderer.HeightsDict.ContainsKey(XZTuple))
                {
                    renderer.cam.mPosition.y = renderer.HeightsDict[XZTuple] + 60000;
                }

             
                if (e.KeyChar == 'f')
                {
                    Bitmap controlBitMap = new Bitmap(simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);
                    System.Drawing.Graphics.FromImage(controlBitMap).CopyFromScreen(PointToScreen(simpleOpenGlControl1.Location), new Point(0, 0), simpleOpenGlControl1.Size);
                    controlBitMap.Save("MyPanelImage"+ImageCounter+".png", ImageFormat.Png);
                    MessageBox.Show("Image has been saved successfully");
                    ++ImageCounter;
                
                }

                if (e.KeyChar == 't')
                {
                    simpleOpenGlControl1.KeyPress -= simpleOpenGlControl1_KeyPress;


                    Random point = new Random();
                    List<Tuple<int, int>> tourPoints = new List<Tuple<int, int>>();
                    for (int i = 0; i < 6; ++i)
                    { 
                        int X = point.Next(-500000,900000);
                        int Z = point.Next(-500000, 900000);
                        if (X % 1000 != 0)
                            X= ((int)(X / 1000) + 1) * 1000;
                        if (Z % 1000 != 0)
                            Z = ((int)(Z / 1000) + 1) * 1000;
                       // MessageBox.Show(X.ToString() + " " + Z.ToString());
                        Tuple<int, int> XZTUPLE = new Tuple<int, int>(X, Z);
                        tourPoints.Add(XZTUPLE);
                    }
                    for (int i = 0; i < tourPoints.Count - 1; ++i)
                    {
                        if (i == 0)
                        {

                            renderer.cam.mPosition.x = tourPoints[0].Item1;
                            renderer.cam.mPosition.z = tourPoints[0].Item2;
                            renderer.cam.mPosition.y = 100000;
                        }

                        while (true)
                        {
                            if (renderer.cam.mPosition.x == tourPoints[i + 1].Item1 && renderer.cam.mPosition.z == tourPoints[i + 1].Item2)
                                break;





                            if (tourPoints[i + 1].Item1 < renderer.cam.mPosition.x)
                            {
                                renderer.cam.mPosition.x -=1000;
                            }

                            if (tourPoints[i + 1].Item1 > renderer.cam.mPosition.x)
                            {
                                renderer.cam.mPosition.x +=1000;
                            }
                            
                            
                           /* Tuple<int, int> XZTUPLE = new Tuple<int, int>(((int)(renderer.cam.mPosition.x / 1000) + 1) * 1000, ((int)(renderer.cam.mPosition.z / 1000) + 1) * 1000);
                            if (renderer.HeightsDict.ContainsKey(XZTUPLE))
                            {
                                renderer.cam.mPosition.y = renderer.HeightsDict[XZTUPLE] + 100000;
                            }*/

                            if (tourPoints[i + 1].Item2 < renderer.cam.mPosition.z)
                            {

                                renderer.cam.mPosition.z -= 1000;
                            }

                            if (tourPoints[i + 1].Item2 > renderer.cam.mPosition.z)
                            {
                                renderer.cam.mPosition.z += 1000;
                            }
                         /*   Tuple<int, int> XZTUPLe = new Tuple<int, int>(((int)(renderer.cam.mPosition.x / 1000) + 1) * 1000, ((int)(renderer.cam.mPosition.z / 1000) + 1) * 1000);
                            if (renderer.HeightsDict.ContainsKey(XZTUPLe))
                            {
                                renderer.cam.mPosition.y = renderer.HeightsDict[XZTUPLe] + 100000;
                            }*/
                            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
                            timer1.Interval = 50;
                            timer1.Enabled = true;
                            timer1.Start();
                            timer1.Tick += (s, t) =>
                            {
                                timer1.Enabled = false;
                                timer1.Stop();
                            };
                            while (timer1.Enabled)
                            {
                                Application.DoEvents();
                            }

                        }


                    }
                   

                    simpleOpenGlControl1.KeyPress += simpleOpenGlControl1_KeyPress;


                }

                
            }
            else
            {
                if (renderer.cam.mPosition.x <= -850000f)
                {

                    renderer.cam.mPosition.x = -850000f + 1;
                }

                if (renderer.cam.mPosition.x >= 850000f)
                {
                    renderer.cam.mPosition.x = 850000f - 1; 
                }




                if (renderer.cam.mPosition.z <= -850000f)
                {
                    renderer.cam.mPosition.z = -850000f + 1;
                }

                if (renderer.cam.mPosition.z >= 850000f)
                {
                    renderer.cam.mPosition.z = 850000f - 1;
                }

            }
            if (e.KeyChar == Convert.ToChar(Keys.Escape))
                   Application.Exit();
        }

        float prevX, prevY;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;

          


            float speed = 0.022f;
            float delta = e.X - prevX;
            if (delta > 2)
            {
                renderer.cam.Yaw(-speed);

            }

            else if (delta < -2)
            { 
                renderer.cam.Yaw(speed);
            }

            delta = e.Y - prevY;
            if (delta > 2)
            {
                renderer.cam.Pitch(-speed);
            
            }

            else if (delta < -2)
            {
                renderer.cam.Pitch(speed);
            }
              
            MoveCursor();

        }

      

        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width/2+p.X, simpleOpenGlControl1.Size.Height/2+p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X+simpleOpenGlControl1.Size.Width/2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }
    }
}
