using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ElectricField__SaticalCharges
{
    public partial class FormEF_StaticCharge : Form
    {
        #region Fileds
        ElectricField EF;
        double scale = 50;
        double scaleR = 60;
        Font fontCharge = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular);
        //bool isCachedCharge;
        int nChachedCharge;
        Point ptCursorOld;
        Random rand = new Random();
        #endregion

        #region Methods
        void Reset()
        {
            nChachedCharge = -1;

            EF = new ElectricField(scale);
            EF.Add(new Charge(10, 7, -1));
            EF.Add(new Charge(12, 7, -1));
            EF.Add(new Charge(10, 9, -1));
            EF.Add(new Charge(12, 9, -1));
            EF.Add(new Charge(11, 8, +3));
         

            EF.CreateGraphic();
            GC.Collect();
        }

        bool CreateNewCharge(int mouseX, int mouseY, double q)
        {
            double dx, dy, r = scaleR * EF.a; 
            foreach (Charge charge in EF.charges)
            {
                dx = mouseX - charge.x * scale;
                dy = mouseY - charge.y * scale;
                

                if (dx * dx + dy * dy < r * r)
                    return false;
            }
            Charge c = new Charge(mouseX / scale, mouseY / scale, q);
            EF.Add(c);
            EF.CreateGraphic();
            GC.Collect();
            return true;
        }
        #endregion


        //-----------------------------------------------------------------------------
        public FormEF_StaticCharge()
        {
            InitializeComponent();
         
            ResizeRedraw = true;
            //textBox_q.Parent = pictureBox1;
            //textBox_q.Visible = false;
            Reset();
     

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);

            g.DrawPath(new Pen(Color.Blue), EF.gPath);
            foreach (Charge charge in EF.charges)
            {
                int size = (int)(scaleR * EF.a);
                Rectangle rect = new Rectangle((int)(scale * charge.x) - size, (int)(scale * charge.y) - size, 2 * size, 2 * size);
                double q = charge.q;
                Color color = (q < 0) ? Color.Green : Color.Red;
                g.FillEllipse(new SolidBrush(color), rect);

                string s = "";
                if (q > 0)
                    s += "+";
                s += q.ToString();
                //s = s.Substring(1);
                SizeF si = g.MeasureString(s, fontCharge);
                int added = 5;
                if (si.Width > rect.Width) added = 0;
                
                g.DrawString(s, fontCharge, new SolidBrush(Color.White), rect.X + added, rect.Y + 8);
                
            }


        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int ptScreenX = e.X;
            int ptScreenY = e.Y;
            double x, y, dx, dy;
            double r = EF.a * scaleR;
            
            if (e.Button == MouseButtons.Left)
            {
                if (ModifierKeys == Keys.Control)
                {
                    double q = (rand.Next(2) == 0) ? -1 : 1;
                    if (CreateNewCharge(ptScreenX, ptScreenY, q))
                        pictureBox1.Invalidate();
                    return;
                }
                
                for (int i = 0, count = EF.charges.Count; i < count; i++)
                {

                    x = EF.charges[i].x * scale;
                    y = EF.charges[i].y * scale;
                    dx = ptScreenX - x;
                    dy = ptScreenY - y;
                    if (dx * dx + dy * dy <= r * r)
                    {
                        nChachedCharge = i;
                        ptCursorOld = new Point(e.X, e.Y);
                        return;
                    }
                }
                
               
            }
           
            else if (e.Button == MouseButtons.Right)
            {
                for (int i = 0, count = EF.charges.Count; i < count; i++)
                {
                    x = EF.charges[i].x * scale;
                    y = EF.charges[i].y * scale;
                    dx = ptScreenX - x;
                    dy = ptScreenY - y;
                    if (dx * dx + dy * dy < r * r)
                    {
                        EF.charges.RemoveAt(i);
                        EF.CreateGraphic();
                        pictureBox1.Invalidate();
                        return;
                    }
                }
            }
            nChachedCharge = -1;



        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (nChachedCharge >= 0 && e.Button == MouseButtons.Left)
            {
                int dx =  (e.X - ptCursorOld.X);
                int dy =  (e.Y - ptCursorOld.Y);
                double x = EF.charges[nChachedCharge].x; x += (dx / scale);
                double y = EF.charges[nChachedCharge].y; y += (dy / scale);
                EF.charges[nChachedCharge].x = x;
                EF.charges[nChachedCharge].y = y;

                //Cursor.Position = pictureBox1.PointToScreen(new Point(e.X + dx, e.Y + dy));
                ptCursorOld = new Point(e.X, e.Y);
                EF.CreateGraphic();

                pictureBox1.Invalidate();


            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            nChachedCharge = -1;
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            double x, y, dx, dy, r = EF.a * scaleR;
            for (int i = 0, count = EF.charges.Count; i < count; i++)
            {
                x = EF.charges[i].x * scale;
                y = EF.charges[i].y * scale;
                dx = e.X - x;
                dy = e.Y - y;
                if (dx * dx + dy * dy <= r * r)
                {
                    FormChangeCharge form = new FormChangeCharge();
                    form.textBox_q.Text = EF.charges[i].q.ToString();
                    form.Location = pictureBox1.PointToScreen(new Point(e.X, e.Y));
                    form.ShowDialog();
                    if (form.isChanged)
                    {
                        double q = double.Parse(form.textBox_q.Text);
                        EF.charges[i].q = q;
                        EF.CreateGraphic();
                        pictureBox1.Invalidate();
                        return;
                    }
                   
                   
                }
            }
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            string text = "Designed by Hor Dashti (h.dashti2@gmail.com)";
            MessageBox.Show(text);
        }




    }
}