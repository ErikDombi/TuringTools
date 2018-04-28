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
        List<Point> drawPoints = new List<Point>();
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

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            //Cursor.Hide();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            //Cursor.Show();
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            //Cursor.Hide();
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            //Cursor.Hide();
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            Draw(e);
        }

        public void Draw(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int xRnd = (int)Math.Round((double)(MousePosition.X - this.Left) / boxSize) * boxSize;
                int yRnd = (int)Math.Round((double)(MousePosition.Y - this.Top) / boxSize) * boxSize;
                drawPoints.Add(new Point(xRnd, yRnd));

                Bitmap b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                if (drawPoints.Count > 1)
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        for (int i = 0; i < drawPoints.Count - 1; i++)
                        {
                            g.DrawLine(new Pen(_color, penSize), drawPoints[i], drawPoints[i + 1]);
                        }
                    }

                pictureBox1.Image = b;

                Point TopLeft = new Point(drawPoints.Select(x => x.X).Min(), drawPoints.Select(x => x.Y).Min());
                Console.WriteLine("Current most TopLeft point in the shape is: " + TopLeft.ToString());
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
            Point TopLeft = new Point(drawPoints.Select(x => x.X).Min(), drawPoints.Select(x => x.Y).Min());
            Point BotRight = new Point(drawPoints.Select(x => x.X).Max(), drawPoints.Select(x => x.Y).Max());
            Size bmpSize = new Size(BotRight.X - TopLeft.X, BotRight.Y - TopLeft.Y);
            int yEffector = SystemInformation.BorderSize.Height;

            string code = "";
            code += $"import GUI\nsetscreen(\"graphics:{bmpSize.Width};{bmpSize.Height};nobuttonbar\")\n";
            if (checkBox1.Checked)
                code += "var imgX : int := 0\n";
            if (checkBox2.Checked)
                code += $"var imgY : int := 0\n";
            for (int i = 0; i < drawPoints.Count - 1; i++)
            {
                int x1 = (drawPoints[i].X - TopLeft.X);
                int y1 = bmpSize.Height - (drawPoints[i].Y - TopLeft.Y)- yEffector;
                int x2 = (drawPoints[i + 1].X - TopLeft.X);
                int y2 = bmpSize.Height - (drawPoints[i + 1].Y - TopLeft.Y) - yEffector;
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
                code = code.Replace("+ -", "- ");
                code = code.Replace(" + 0", "");
            }

            Clipboard.SetText(code);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateCode();
        }
    }
}
