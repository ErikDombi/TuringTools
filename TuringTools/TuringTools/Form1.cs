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
        int RGBDecimalCount = 1;
        int CurrentPointsRectSizeXY = 6; //Preferably an even number

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
                for (int i = 0; i < pictureBox1.Width / boxSize; i++)
                {
                    g.DrawLine(new Pen(Color.DarkGray), new Point(i * boxSize, 0), new Point(i * boxSize, pictureBox1.Height));
                }
                for (int j = 0; j < pictureBox1.Height / boxSize; j++)
                {
                    g.DrawLine(new Pen(Color.DarkGray), new Point(0, j * boxSize), new Point(pictureBox1.Width, j * boxSize));
                }
            }
            pictureBox1.BackgroundImage = b;
            _color = pictureBox3.BackColor;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Draw(new MouseEventArgs(MouseButtons.XButton1, 1, e.X, e.Y, 0));
            int xRnd = (int)Math.Round((double)e.X / boxSize) * boxSize;
            int yRnd = (int)Math.Round((double)e.Y / boxSize) * boxSize;
            pictureBox2.Left = xRnd - pictureBox2.Width / 2;
            pictureBox2.Top = yRnd - pictureBox2.Height / 2;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            Draw(e);
        }

        public void Draw(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (currentPoints.Count > 1)
                    allPoints.Add(currentPoints.ToArray());
                currentPoints = new List<Point>();
            }

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.None || e.Button == MouseButtons.Right || e.Button == MouseButtons.XButton1)
            {
                int xRnd = pictureBox2.Left + pictureBox2.Width / 2;
                int yRnd = pictureBox2.Top + pictureBox2.Height / 2;

                if (e.Button == MouseButtons.Left)
                    currentPoints.Add(new Point(xRnd, yRnd));


                Bitmap b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (Graphics g = Graphics.FromImage(b))
                {

                    g.DrawLine(new Pen(Color.Black, 2), new Point(xRnd, 0), new Point(xRnd, pictureBox1.Height));
                    g.DrawLine(new Pen(Color.Black, 2), new Point(0, yRnd), new Point(pictureBox1.Width, yRnd));


                    if (currentPoints.Count > 1)
                    {
                        for (int i = 0; i < currentPoints.Count - 1; i++)
                        {
                            g.DrawLine(new Pen(_color, penSize), currentPoints[i], currentPoints[i + 1]);
                            g.FillRectangle(new SolidBrush(Color.Black), currentPoints[i].X - CurrentPointsRectSizeXY / 2, currentPoints[i].Y - CurrentPointsRectSizeXY / 2, CurrentPointsRectSizeXY, CurrentPointsRectSizeXY);
                        }
                    }
                    foreach (var pointArray in allPoints)
                    {
                        if (pointArray.Length > 1)
                            for (int i = 0; i < pointArray.Length - 1; i++)
                            {
                                g.DrawLine(new Pen(_color, penSize), pointArray[i], pointArray[i + 1]);
                            }
                    }
                    if (currentPoints.Count > 0)
                    {
                        g.FillRectangle(new SolidBrush(Color.Black), currentPoints[currentPoints.Count - 1].X - CurrentPointsRectSizeXY / 2, currentPoints[currentPoints.Count - 1].Y - CurrentPointsRectSizeXY / 2, CurrentPointsRectSizeXY, CurrentPointsRectSizeXY);
                        g.DrawString($"({xRnd - currentPoints.LastOrDefault().X}, {yRnd - currentPoints.LastOrDefault().Y})", new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular), new SolidBrush(Color.Black), new Point(xRnd, yRnd));
                    }
                }

                pictureBox1.Image = b;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && colorDialog1.Color != _color)
            {
                _color = colorDialog1.Color;
                pictureBox3.BackColor = _color;
                Draw(new MouseEventArgs(MouseButtons.None, 1, MousePosition.X, MousePosition.Y, 0));
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Draw(e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            penSize = trackBar1.Value;
            CurrentPointsRectSizeXY = trackBar1.Value + 2;
            Draw(new MouseEventArgs(MouseButtons.None, 1, MousePosition.X, MousePosition.Y, 0));
        }

        public void GenerateCode()
        {
            //DOING THIS MAY BREAK BEING ABLE TO CONTINUE A LINE AFTER GENERATING. LOOK FOR A FIX (Probably simple)
            if (currentPoints.Count > 1)
                allPoints.Add(currentPoints.ToArray());
            currentPoints = new List<Point>();

            Point TopLeft = new Point(allPoints.Select(x => x.Select(c => c.X).Min()).Min(), allPoints.Select(x => x.Select(c => c.Y).Min()).Min());
            Point BotRight = new Point(allPoints.Select(x => x.Select(c => c.X).Max()).Max(), allPoints.Select(x => x.Select(c => c.Y).Max()).Max());
            Size bmpSize = new Size(BotRight.X - TopLeft.X, BotRight.Y - TopLeft.Y);
            int yEffector = SystemInformation.BorderSize.Height;
            string code = "";

            var R = Math.Round((byte)(_color.R) * 0.392156862745098 / 100, RGBDecimalCount);
            var G = Math.Round((byte)(_color.G) * 0.392156862745098 / 100, RGBDecimalCount);
            var B = Math.Round((byte)(_color.B) * 0.392156862745098 / 100, RGBDecimalCount);
            var OR = (byte)(_color.R) * 0.392156862745098 / 100;
            var OG = (byte)(_color.G) * 0.392156862745098 / 100;
            var OB = (byte)(_color.B) * 0.392156862745098 / 100;
            Console.WriteLine($"Exporting Colour (Rounding Values)...\n   R: {_color.R} -> {OR} -> {R}\n   G: {_color.G} -> {OG} -> {G}\n   B: {_color.B} -> {OB} -> {B}");

            code += $"setscreen(\"graphics:{bmpSize.Width + penSize};{bmpSize.Height + penSize * 2};nobuttonbar\")\n\n";
            code += $"var imgColor : int := RGB.AddColor({R}, {G}, {B})\n";
            if (checkBox1.Checked)
                code += "var imgX : int := 0\n";
            if (checkBox2.Checked)
                code += $"var imgY : int := 0\n";


            //Completely arbitury, as if currentPoints contains more that 1 point, it will be converted to AllPoints
            //for (int i = 0; i < currentPoints.Count - 1; i++)
            //{
            //    int x1 = (currentPoints[i].X - TopLeft.X);
            //    int y1 = bmpSize.Height - (currentPoints[i].Y - TopLeft.Y)- yEffector;
            //    int x2 = (currentPoints[i + 1].X - TopLeft.X);
            //    int y2 = bmpSize.Height - (currentPoints[i + 1].Y - TopLeft.Y) - yEffector;
            //    if(checkBox1.Checked && checkBox2.Checked)
            //    {
            //        code += $"Draw.ThickLine (imgX + {x1 + penSize / 2}, imgY + {y1 + penSize / 2}, imgX + {x2 + penSize / 2}, imgY + {y2}, {penSize + penSize / 2}, imgColor)\n";
            //    }
            //    else if (checkBox1.Checked)
            //    {
            //        code += $"Draw.ThickLine (imgX + {x1 + penSize / 2}, {y1 + penSize / 2}, imgX + {x2 + penSize / 2}, {y2 + penSize / 2}, {penSize}, imgColor)\n";
            //    }
            //    else if (checkBox2.Checked)
            //    {
            //        code += $"Draw.ThickLine ({x1 + penSize / 2}, imgY + {y1 + penSize / 2}, {x2 + penSize / 2}, imgY + {y2 + penSize / 2}, {penSize}, imgColor)\n";
            //    }
            //    else
            //    {
            //        code += $"Draw.ThickLine ({x1 + penSize / 2}, {y1 + penSize / 2}, {x2 + penSize / 2}, {y2 + penSize / 2}, {penSize}, imgColor)\n";
            //    }  
            //}

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
                            code += $"Draw.ThickLine (imgX + {x1 + penSize / 2}, imgY + {y1 + penSize / 2}, imgX + {x2 + penSize / 2}, imgY + {y2 + penSize / 2}, {penSize}, imgColor)\n";
                        }
                        else if (checkBox1.Checked)
                        {
                            code += $"Draw.ThickLine (imgX + {x1 + penSize / 2}, {y1 + penSize / 2}, imgX + {x2 + penSize / 2}, {y2 + penSize / 2}, {penSize}, imgColor)\n";
                        }
                        else if (checkBox2.Checked)
                        {
                            code += $"Draw.ThickLine ({x1 + penSize / 2}, imgY + {y1 + penSize / 2}, {x2 + penSize / 2}, imgY + {y2 + penSize / 2}, {penSize}, imgColor)\n";
                        }
                        else
                        {
                            code += $"Draw.ThickLine ({x1 + penSize / 2}, {y1 + penSize / 2}, {x2 + penSize / 2}, {y2 + penSize / 2}, {penSize}, imgColor)\n";
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
