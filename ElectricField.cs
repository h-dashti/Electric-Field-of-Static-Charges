using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ElectricField__SaticalCharges
{
    public class ElectricField
    {
        public List<Charge> charges = new List<Charge>();
        public GraphicsPath gPath = new GraphicsPath();
        public double qTotal;

        public double E_Min = 0.01;
        public int lpc = 8;                // Lines Per Charges
        public const double twoPI = 2 * Math.PI;
        public double a = 0.2;
        public double maxL = 4;
        double scale;
        public int nAdded = 20;

        public ElectricField(double scale)
        {
            this.scale = scale;
        }

        public void CreateGraphic()
        {
            gPath.Reset();
            int N = charges.Count;
            int sign = Math.Sign(qTotal);
            if (sign == 0) sign = 1;
            double dsSmall = 0.01 * sign;
            double dsBig = sign;
           
            double r = 0;

            for (int i = 0; i < N; i++)
            {
                double q = charges[i].q;
                //sign = Math.Sign(q);
                //dsSmall = sign * 0.01;
                //dsBig = sign;

                if (q * sign > 0)
                {
                    double dTheta = twoPI / (lpc * Math.Abs(q));
                    for (double theta = 0; theta <= twoPI; theta += dTheta)
                    {
                        double xLine = charges[i].x + a * Math.Cos(theta);
                        double yLine = charges[i].y + a * Math.Sin(theta);

                        double dsLenght = 0;
                        bool stopPlot = false;
                        int counterPoints = nAdded;
                        do
                        {
                            double Ex = 0, Ey = 0;
                            for (int j = 0; j < N; j++)
                            {
                                double dx = xLine - charges[j].x;
                                double dy = yLine - charges[j].y;

                                r = Math.Sqrt(dx * dx + dy * dy);
                                if (r > 0.9 * a)
                                {
                                    double E0 = charges[j].q / (r * r * r);
                                    Ex += E0 * dx;
                                    Ey += E0 * dy;
                                }
                                else
                                    stopPlot = true;

                            }// End for j

                            double E = Math.Sqrt(Ex * Ex + Ey * Ey);
                            double dsTemp = (E > E_Min || r < 20) ? dsSmall : dsBig;
                            dsLenght += Math.Abs(dsTemp);
                            if (E < E_Min)
                                stopPlot = true;

                            xLine += dsTemp * Ex / E;
                            yLine += dsTemp * Ey / E;

                            if (counterPoints == nAdded)
                            {
                                Point pt = new Point((int)(scale * xLine), (int)(scale * yLine));
                                gPath.AddLine(pt, pt);
                                counterPoints = 0;
                            }
                            counterPoints++;

                        } while (!stopPlot);
                        gPath.StartFigure();
                    }// End For theta

                }// End IF
            }// End for i

        }

        public void Add(Charge c)
        {
            //if (c.q == 0) return;
            charges.Add(c);
            qTotal += c.q;
        }



    }



    //***************************************************************
    //***************************************************************
    public class Charge
    {
        public double x, y, q;
        public Charge(double x, double y, double q)
        {
            this.x = x;
            this.y = y;
            this.q = q;
        }
    }
}
