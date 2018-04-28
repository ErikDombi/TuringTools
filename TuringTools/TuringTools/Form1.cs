using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TuringTools
{
    public partial class Form1 : Form
    {
        int boxSize = 20;
        List<Point> currentPoints = new List<Point>();
        List<Point[]> allPoints = new List<Point[]>();
        Color _color = Color.Red;
        int penSize = 3;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                for(int i = 0; i < pictureBox1.Width / boxSize; i++)
                {
                    g.DrawLine(new Pen(Color.Black), new Point(i * boxSize, 0), new Point(i * boxSize, pictureBox1.Height));
                }
                for (int j = 0; j < pictureBox1.Height / boxSize; j++)
                {
                    g.DrawLine(new Pen(Color.Black), new Point(0, j * boxSize), new Point(pictureBox1.Width, j * boxSize));
                }
            }
            pictureBox1.BackgroundImage = b;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int xRnd = (int)Math.Round((double)e.X / boxSize) * boxSize;
            int yRnd = (int)Math.Round((double)e.Y / boxSize) * boxSize;
            Console.WriteLine(new Point(xRnd, yRnd).ToString());
            pictureBox2.Left = xRnd - pictureBox2.Width / 2;
            pictureBox2.Top = yRnd - pictureBox2.Height / 2;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            Draw(e);
        }

        public void Draw(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Using this method does not seem to produce the same result as before when subtracting this.left. Odd.
                //int xRnd = (int)Math.Round((double)(MousePosition.X - this.Left) / boxSize) * boxSize;
                //int yRnd = (int)Math.Round((double)(MousePosition.Y - this.Top) / boxSize) * boxSize;
                int xRnd = pictureBox2.Left + pictureBox2.Width / 2;
                int yRnd = pictureBox2.Top + pictureBox2.Height / 2;

                currentPoints.Add(new Point(xRnd, yRnd));

                Bitmap b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    if(currentPoints.Count > 1)
                    for (int i = 0; i < currentPoints.Count - 1; i++)
                    {
                        g.DrawLine(new Pen(_color, penSize), currentPoints[i], currentPoints[i + 1]);
                    }
                    foreach (var pointArray in allPoints)
                    {
                        if(pointArray.Length > 1)
                        for (int i = 0; i < pointArray.Length - 1; i++)
                        {
                            g.DrawLine(new Pen(_color, penSize), pointArray[i], pointArray[i + 1]);
                        }
                    }
                }

                pictureBox1.Image = b;

                Point TopLeft = new Point(currentPoints.Select(x => x.X).Min(), currentPoints.Select(x => x.Y).Min());
                Console.WriteLine("Current most TopLeft point in the shape is: " + TopLeft.ToString());
            }else if(e.Button == MouseButtons.Right)
            {
                if (currentPoints.Count > 1)
                    allPoints.Add(currentPoints.ToArray());
                currentPoints = new List<Point>();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            if (colorDialog1.Color != null)
            {
                _color = colorDialog1.Color;
                pictureBox3.BackColor = _color;

            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Draw(e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            penSize = trackBar1.Value;
        }

        public void GenerateCode()
        {
            if(currentPoints.Count > 1)
                allPoints.Add(currentPoints.ToArray());
            currentPoints = new List<Point>();

            Point TopLeft = new Point(allPoints.Select(x => x.Select(c => c.X).Min()).Min(), allPoints.Select(x => x.Select(c => c.Y).Min()).Min());
            Point BotRight = new Point(allPoints.Select(x => x.Select(c => c.X).Max()).Max(), allPoints.Select(x => x.Select(c => c.Y).Max()).Max());

            //Point TopLeft = new Point(currentPoints.Select(x => x.X).Min(), currentPoints.Select(x => x.Y).Min());
            //Point BotRight = new Point(currentPoints.Select(x => x.X).Max(), currentPoints.Select(x => x.Y).Max());
            Size bmpSize = new Size(BotRight.X - TopLeft.X, BotRight.Y - TopLeft.Y);
            int yEffector = SystemInformation.BorderSize.Height;

            string code = "";
            code += $"import GUI\nsetscreen(\"graphics:{bmpSize.Width};{bmpSize.Height};nobuttonbar\")\n";
            if (checkBox1.Checked)
                code += "var imgX : int := 0\n";
            if (checkBox2.Checked)
                code += $"var imgY : int := 0\n";
            for (int i = 0; i < currentPoints.Count - 1; i++)
            {
                int x1 = (currentPoints[i].X - TopLeft.X);
                int y1 = bmpSize.Height - (currentPoints[i].Y - TopLeft.Y)- yEffector;
                int x2 = (currentPoints[i + 1].X - TopLeft.X);
                int y2 = bmpSize.Height - (currentPoints[i + 1].Y - TopLeft.Y) - yEffector;
                if(checkBox1.Checked && checkBox2.Checked)
                {
                    code += $"drawline (imgX + {x1}, imgY + {y1}, imgX + {x2}, imgY + {y2}, 153)\n";
                }
                else if (checkBox1.Checked)
                {
                    code += $"drawline (imgX + {x1}, {y1}, imgX + {x2}, {y2}, 153)\n";
                }
                else if (checkBox2.Checked)
                {
                    code += $"drawline ({x1}, imgY + {y1}, {x2}, imgY + {y2}, 153)\n";
                }
                else
                {
                    code += $"drawline ({x1}, {y1}, {x2}, {y2}, 153)\n";
                }
                
            }

            foreach (var pointArray in allPoints)
            {
                if (pointArray.Length > 1)
                    for (int i = 0; i < pointArray.Length - 1; i++)
                    {
                        int x1 = (pointArray[i].X - TopLeft.X);
                        int y1 = bmpSize.Height - (pointArray[i].Y - TopLeft.Y) - yEffector;
                        int x2 = (pointArray[i + 1].X - TopLeft.X);
                        int y2 = bmpSize.Height - (pointArray[i + 1].Y - TopLeft.Y) - yEffector;
                        if (checkBox1.Checked && checkBox2.Checked)
                        {
                            code += $"drawline (imgX + {x1}, imgY + {y1}, imgX + {x2}, imgY + {y2}, 153)\n";
                        }
                        else if (checkBox1.Checked)
                        {
                            code += $"drawline (imgX + {x1}, {y1}, imgX + {x2}, {y2}, 153)\n";
                        }
                        else if (checkBox2.Checked)
                        {
                            code += $"drawline ({x1}, imgY + {y1}, {x2}, imgY + {y2}, 153)\n";
                        }
                        else
                        {
                            code += $"drawline ({x1}, {y1}, {x2}, {y2}, 153)\n";
                        }
                    }
            }

            code = code.Replace("+ -", "- ");
            code = code.Replace(" + 0", "");
            Clipboard.SetText(code);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateCode();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        bool moving = false;
        Point FormO;
        Point MouseO;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            FormO = Location;
            MouseO = MousePosition;
            move();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        public async void move()
        {
            while (moving)
            {
                this.Left = MousePosition.X - (MouseO.X - FormO.X);
                this.Top = MousePosition.Y - (MouseO.Y - FormO.Y);
                await Task.Delay(1);
            }
        }

        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            FormO = Location;
            MouseO = MousePosition;
            move();
        }

        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }
    }
}
