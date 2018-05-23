using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace iTraceVS
{
    public class reticle : Form
    {
        Pen crossPen = new Pen(Color.Red, 3);
        private int MAX_NUM_POINTS = 15;
        private int totalX;
        private int totalY;
        private List<int> xPoints = new List<int>();
        private List<int> yPoints = new List<int>();
        private bool display;
        private System.Windows.Forms.Timer timer;
        private Point newPos;

        public reticle() {
            Hide();
            totalX = 0;
            totalY = 0;
            display = false;
            newPos = new Point(0, 0);
            timer = new System.Windows.Forms.Timer() { Interval = 20, Enabled = true };
            timer.Tick += new EventHandler(timerTick);

            TopMost = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.LightGreen;
            TransparencyKey = Color.LightGreen;

            Width = 60;
            Height = 60;

            Paint += new PaintEventHandler(reticleFormPaint);
        }

        //Use timer to update reticle posistion because Location cannot be directly set from within the worker thread
        void timerTick(object sender, EventArgs e) {
            Location = newPos;
        }

        void reticleFormPaint(object sender, PaintEventArgs e) {
            e.Graphics.DrawEllipse(crossPen, (Width - 15) / 2, (Height - 15) / 2, 15, 15);
        }

        public void toDraw(bool draw) {
            display = draw;
            if (display)
                Show();
            else
                Hide();
        }

        public void updateReticle(int x, int y) {
            //No reason to do anything if it can't be seen...
            if (!display)
                return;

            // Invalid screen coordinates...
            if (x < 0 || y < 0)
                return;
            
            //Sum up all the x and y we have seen
            totalX += x;
            totalY += y;

            //Add them to the list of values we have seen
            xPoints.Insert(0, x);
            yPoints.Insert(0, y);

            /*
             * If we have enough points (MAX_NUM_POINTS) for desired smoothness
             * then we take an average of the points and move the reticle.
             *
             * To save re-totaling the points, we then just remove the oldest value from the
             * total and the lists (back) so the next time the function is called we only
             * evaluate the last MAX_NUM_POINTS.
             */

            if (xPoints.Count == MAX_NUM_POINTS) {
                double avgX = totalX / MAX_NUM_POINTS;
                double avgY = totalY / MAX_NUM_POINTS;

                newPos = new Point(Convert.ToInt32(avgX), Convert.ToInt32(avgY));
                newPos.Offset(-(Width / 2), -(Height / 2));

                //Remove oldest points from totals and lists
                totalX -= xPoints.Last();
                totalY -= yPoints.Last();
                xPoints.RemoveAt(xPoints.Count - 1);
                yPoints.RemoveAt(yPoints.Count - 1);
            }
        }
    }
    
}
