using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CS352_LabProject
{
    public partial class Form1 : Form
    {
        int index, head, direction, totalMov, start, end;
        bool flagAllDone = false;

        List<int> queue = new List<int>();
        List<int> movements = new List<int>();

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBoxDisk_TextChanged(object sender, EventArgs e)
        {
            string diskAlgo = comboBoxDisk.Text;
            index = comboBoxDisk.FindString(diskAlgo);

            /* Display the Direction ComboBox */
            if(index > 1)
            {
                lbDir.Visible = true;
                comboBoxDir.Visible = true;
            }
            else
            {
                lbDir.Visible = false;
                comboBoxDir.Visible = false;
            }

        }

        private void comboBoxDir_TextChanged(object sender, EventArgs e)
        {
            /* Larger => 0 
               Smaller => 1
            */
            string algoDir = comboBoxDir.Text;
            direction = comboBoxDir.FindString(algoDir);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            /* Check if any textbox is empty */
            flagAllDone = false;
            totalMov = 0;
            movements.Clear();

            if (comboBoxDisk.Text != "" &&
                textBoxQueue.Text != "" &&
                textBoxHead.Text != "" &&
                tbStart.Text != "" &&
                tbEnd.Text != "")
            {
                flagAllDone = true;
            }

            if (index > 1 && comboBoxDir.Text == "")
            {
                flagAllDone = false;
            }

            /* start processing */
            if (flagAllDone)
            {
                /* get textBoxs values */
                head = Int32.Parse(textBoxHead.Text);
                start = Int32.Parse(tbStart.Text);
                end = Int32.Parse(tbEnd.Text);
                string diskQueue = textBoxQueue.Text;
                queue = diskQueue.Split(' ').Select(Int32.Parse).ToList();

                if (index == 0) { FCFS(); }
                if (index == 1) { SSTF(); }
                if (index == 2) { SCAN(); }
                if (index == 3) { CSCAN(); }
                if (index == 4) { LOOK(); }
                if (index == 5) { CLOOK(); }

                lbResult.Text = totalMov + "";

                plot();
            } 
            else
            {
                MessageBox.Show("The field cannot be empty. Please enter a value");
            }
        }

        /* IMPORTANT NOTE 
         * u only can use the following global vars (queue - head - direction - totalMov)
         * u can't edit any thing outside your function
         */
        private void FCFS()
        {
            int d, cur_track, headTmp = head;
            movements.Add(headTmp);
            for (int i = 0; i < queue.Count(); i++)
            {
                cur_track = queue[i];
                d = Math.Abs(cur_track - headTmp);
                totalMov += d;
                headTmp = cur_track;
                movements.Add(headTmp);
            }
        }

        private void SSTF()
        {
            int minVal, distance, pos = 0, headTmp = head;
            movements.Add(head);

            for (int i = 0; i < queue.Count(); i++)
            {
                minVal = 999;
                
                for (int j = 0; j < queue.Count(); j++)
                {
                    if (queue[j] != -1)
                    {
                        distance = Math.Abs(headTmp - queue[j]);
                        if (minVal >= distance)
                        {
                            minVal = distance;
                            pos = j;
                        }
                    }
                }

                totalMov += Math.Abs(headTmp - queue[pos]);
                headTmp = queue[pos];
                queue[pos] = -1;
                movements.Add(headTmp);
            }
        }

        private void SCAN()
        {
            List<int> queueTmp = queue.ToList();
            queueTmp.Add(head);
            queueTmp.Add(start);
            queueTmp.Add(end);
            queueTmp.Sort();

            int pos = queueTmp.IndexOf(head);

            if (direction == 0)
            {
                
                for (int i = pos; i < queueTmp.Count(); i++)
                {
                    movements.Add(queueTmp[i]);
                }
                for (int i = pos - 1; i >= 1; i--)
                {
                    movements.Add(queueTmp[i]);
                }

                totalMov = Math.Abs(head - queueTmp[queueTmp.Count()-1]) + Math.Abs(queueTmp[queueTmp.Count()-1] - queueTmp[1]);
            }
            else
            {
                for (int i = pos; i >= 0; i--)
                {
                    movements.Add(queueTmp[i]);
                }

                for (int i = pos + 1; i < queueTmp.Count() - 1; i++)
                {
                    movements.Add(queueTmp[i]);
                }
                totalMov = Math.Abs(head - queueTmp[0]) + Math.Abs(queueTmp[0] - queueTmp[queueTmp.Count() - 2]);
            }
        }

        private void CSCAN()
        {

            if (direction == 0)
            {
                int seek_count = 0;
                int distance, cur_track;

                List<int> left = new List<int>();
                List<int> right = new List<int>();
                List<int> seek_sequence = new List<int>();
                left.Add(0);
                right.Add(end);

                for (int i = 0; i < queue.Count; i++)
                {
                    if (queue[i] < head)
                        left.Add(queue[i]);
                    if (queue[i] > head)
                        right.Add(queue[i]);
                }
                left.Sort();
                right.Sort();
                for (int i = 0; i < right.Count; i++)
                {
                    cur_track = right[i];
                    seek_sequence.Add(cur_track);
                    distance = Math.Abs(cur_track - head);
                    seek_count += distance;
                    head = cur_track;
                }

                head = 0;
                seek_count += (end);


                for (int i = 0; i < left.Count; i++)
                {
                    cur_track = left[i];
                    seek_sequence.Add(cur_track);
                    distance = Math.Abs(cur_track - head);
                    seek_count += distance;
                    head = cur_track;
                }

                totalMov = seek_count;
                for (int i = 0; i < seek_sequence.Count; i++)
                {
                    movements.Add(seek_sequence[i]);
                }
            }
            else
            {
                int seek_coun = 0;
                int distanc, curent_track;

                List<int> left = new List<int>();
                List<int> right = new List<int>();
                List<int> seek_sequence = new List<int>();
                left.Add(0);
                right.Add(end);
                for (int i = 0; i < queue.Count; i++)
                {
                    if (queue[i] < head)
                        left.Add(queue[i]);
                    if (queue[i] > head)
                        right.Add(queue[i]);
                }

                left.Sort();
                right.Sort();

                for (int i = 0; i < left.Count; i++)
                {
                    curent_track = left[i];
                    seek_sequence.Add(curent_track);
                    distanc = Math.Abs(curent_track - head);
                    seek_coun += distanc;
                    head = curent_track;
                }
                head = end;
                seek_coun += head;


                for (int i = right.Count - 1; i > 0; i--)
                {
                    curent_track = right[i];
                    seek_sequence.Add(curent_track);
                    distanc = Math.Abs(curent_track - head);
                    seek_coun += distanc;
                    head = curent_track;
                }
                totalMov = seek_coun;
                for (int i = 0; i < seek_sequence.Count; i++)
                {
                    movements.Add(seek_sequence[i]);
                }
            }
        }


        private void LOOK()
        {
            int distance, cur_track, headTmp = head;
            List<int> left = new List<int>();
            List<int> right = new List<int>();
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i] < head)
                    left.Add(queue[i]);
                if (queue[i] > head)
                    right.Add(queue[i]);
            }
            left.Sort();
            right.Sort();
            movements.Add(headTmp);
            int run = 2;
            while (run-- > 0)
            {
                if (direction == 1)
                {
                    for (int i = left.Count - 1; i >= 0; i--)
                    {
                        cur_track = left[i];
                        movements.Add(cur_track);
                        distance = Math.Abs(cur_track - headTmp);
                        totalMov += distance;
                        headTmp = cur_track;
                    }
                    direction = 0;
                }
                else if (direction == 0)
                {
                    for (int i = 0; i < right.Count; i++)
                    {
                        cur_track = right[i];
                        movements.Add(cur_track);
                        distance = Math.Abs(cur_track - headTmp);
                        totalMov += distance;
                        headTmp = cur_track;
                    }
                    direction = 1;
                }
            }
        }
    private void CLOOK()
        {
            int distance, cur_track, headClook = head;
            List<int> left = new List<int>();
            List<int> right = new List<int>();
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i] < head)
                    left.Add(queue[i]);
                if (queue[i] > head)
                    right.Add(queue[i]);
            }
            left.Sort();
            right.Sort();
            movements.Add(headClook);
            int loop = 2;
            while (loop-- > 0)
            {
                if (direction == 1)
                {
                    for (int i = 0; i < left.Count; i++)
                    {
                        cur_track = left[i];
                        movements.Add(cur_track);
                        distance = Math.Abs(cur_track - headClook);
                        totalMov += distance;
                        headClook = cur_track;
                    }
                    direction = 0;
                }
                else if (direction == 0)
                {
                    for (int i = 0; i < right.Count; i++)
                    {
                        cur_track = right[i];
                        movements.Add(cur_track);
                        distance = Math.Abs(cur_track - headClook);
                        totalMov += distance;
                        headClook = cur_track;

                    }
                    direction = 1;
                }
            }
        }

        private void plot()
        {
            chart1.Series["Series1"].Points.Clear();
            List<int> numbers = new List<int>();
            int j = movements.Count();
            for(int i=0;i<movements.Count();i++)
            {
                chart1.Series["Series1"].Points.AddXY(movements[i],j);
                j--;
            }
        }
    }
}
