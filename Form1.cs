using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ZedGraph;
using System.IO;
using System.Globalization;
using System.Numerics;
using GPIBlibrary;


namespace compression_tester
{
    public partial class Form1 : Form
    {
        GPIB bus = new GPIB();
        int addr8901, addrgen1, addrgen2;
        float G = 1.11f;
        double iffreq, lofreq, rffreq;
        float startlevel, stoplevel,lolevel;
        double[] tempi, tempc;
        double[] MEM1c, MEM2c, MEM3c, MEM4c, MEM5c, MEM1i, MEM2i, MEM3i, MEM4i, MEM5i;
        Boolean MEM1 = false, MEM2 = false, MEM3 = false, MEM4 = false, MEM5 = false, tempdata = true;
        int n;
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;


        public Form1()
        {
            InitializeComponent();
            List<String> tList = bus.portlist();
           comboBox1.Items.Add("Select COM port...");
            comboBox1.Items.AddRange(tList.ToArray());
            comboBox1.SelectedIndex = 0;
            comboBox2.Items.Add("+");
            comboBox2.Items.Add("-");
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
           bool con =  bus.start(comboBox1.Text, 1000);
           try
           {
               addr8901 = Convert.ToInt16(textBox1.Text);
               addrgen2 = Convert.ToInt16(textBox2.Text);
               addrgen1 = Convert.ToInt16(textBox3.Text);
           }
           catch (Exception ex) { textboxadd(ex.ToString()); }
           if (con == false) { textboxadd("Connection failed!"); }
           bus.eoi(1); // enable EOI
           

            // do other stuff?

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                rffreq = double.Parse(textBox6.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                startlevel = float.Parse(textBox7.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                stoplevel = float.Parse(textBox8.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex) { textboxadd(ex.ToString()); }
            n = (int)(stoplevel - startlevel);
            textBox9.Text = n.ToString();

            if (addrgen1 != 0) {
                try
                {
                    lofreq = double.Parse(textBox4.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    lolevel = float.Parse(textBox5.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex) { }
            bus.write(addrgen1, "SQ");
            bus.write(addrgen1,"FR"+lofreq.ToString(nfi)+"MZ");
            bus.write(addrgen1, "AP" + lolevel.ToString(nfi) + "DM");
            }

            bus.write(addrgen2, "SQ");
            bus.write(addr8901, "CL");
            bus.write(addrgen2, "FR" + rffreq.ToString(nfi) + "MZ");
            bus.write(addrgen2, "S1AM30PC");
            bus.write(addrgen2, "AP" + startlevel.ToString(nfi) + "DM");
            bus.write(addr8901, "CLAU6.1SPD1H0LOP0C0R0M1T0");

            if (comboBox2.Text == "+")
            {
                iffreq = rffreq + lofreq;
            }
            else {
                iffreq = Math.Abs(rffreq - lofreq);
            }
            bus.write(addr8901, "F1" + iffreq.ToString(nfi) + "MZ");


           // after this the analyzer should be configured for use.  

            textboxadd("IF frequency: " + iffreq.ToString() + "MHz");
            textboxadd( "SET EXTERNAL SQUARE WAVE MODULATION TO 30% AM");

        
        }

        private void textboxadd(string indata) {
            textBox10.Text = textBox10.Text + Environment.NewLine + indata;
        }

        private void plot() {

            zedGraphControl1.GraphPane.CurveList.Clear();
            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.XAxis.Type = AxisType.Linear;
            //  myPane.XAxis.MinorGrid.IsVisible = false;
            myPane.XAxis.Scale.Mag = 0;
            zedGraphControl1.AxisChange();
            
            if (tempdata == true) { PointPairList spl1 = new PointPairList(tempi, tempc); LineItem myCurve1 = myPane.AddCurve("TEMP", spl1, Color.Blue, SymbolType.None); myCurve1.Line.Width = 2.0F; }
            if (MEM1 == true) { PointPairList spl2 = new PointPairList(MEM1i, MEM1c); LineItem myCurve2 = myPane.AddCurve("MEM1", spl2, Color.Red, SymbolType.None); myCurve2.Line.Width = 2.0F; }
            if (MEM2 == true) { PointPairList spl3 = new PointPairList(MEM2i, MEM2c); LineItem myCurve3 = myPane.AddCurve("MEM2", spl3, Color.Green, SymbolType.None); myCurve3.Line.Width = 3.0F; }
            if (MEM3 == true) { PointPairList spl4 = new PointPairList(MEM3i, MEM3c); LineItem myCurve4 = myPane.AddCurve("MEM3", spl4, Color.Magenta, SymbolType.None); myCurve4.Line.Width = 3.0F; }
            if (MEM4 == true) { PointPairList spl5 = new PointPairList(MEM4i, MEM4c); LineItem myCurve5 = myPane.AddCurve("MEM4", spl5, Color.Yellow, SymbolType.None); myCurve5.Line.Width = 3.0F; }
            if (MEM5 == true) { PointPairList spl6 = new PointPairList(MEM5i, MEM5c); LineItem myCurve6 = myPane.AddCurve("MEM5", spl6, Color.Cyan, SymbolType.None); myCurve6.Line.Width = 3.0F; }
             
            myPane.Title.Text = "COMPRESSION TEST";
            myPane.YAxis.Title.Text = "Compression dB";
            myPane.XAxis.Title.Text = "Peak Input Level";
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            // I add all three functions just to be sure it refeshes the plot.   
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            double R=0;

            // program sets 8901 to D3 if variable 2 is set?


            for (int i = 0; i <= 2; i++) {
                string temp = bus.writeread(addr8901, "T3"); //Red command in orginal program
                try
                {
                    R = R + double.Parse(temp, System.Globalization.NumberStyles.Float);
                }
                catch (Exception ex) { textboxadd(ex.ToString()); }

                textboxadd("Reference " + temp.ToString());
            }
            textboxadd("R value: " + R.ToString());
            R = (R/3)*0.01f; // convert from % into decimals
            int m = 0;

            for (float i = startlevel; i < stoplevel; i = i + 1.0f)
            {
                tempc = new double[n];
                tempi = new double[n];    
                bus.write(addrgen2, "AP" + (i).ToString(nfi) + "DM"); // may need to figure out the needed formatting here?  (lolevel + n)
                string temp = bus.writeread(addr8901,"T3");
                double b = 0.01f * double.Parse(temp,System.Globalization.NumberStyles.Float);
                double clin = ((G - R) * (G + b)) / ((G + R) * (G - b));
                double C;
                if (m > 6) {
                   C = 20 * Math.Log10(clin) + tempc[m-6] ; // need to check that this returns what it should
                }else{
                    C = 20*Math.Log10(clin);
                }

                double J = i + 20 * Math.Log10(1 + (R / G));

                tempi[m] = J;
                tempc[m] = C;
                textboxadd(i.ToString()+ "  " + J.ToString() + "    " + C.ToString() +"    " + temp);
                m++;
            }
            textboxadd("Measured data in TEMP");
            plot();
        }

       

        private void button5_Click(object sender, EventArgs e)
        {
            // copy routine
            switch (comboBox3.Text)
            {
                case "MEM1":
                   MEM1c = new double[n]; MEM1i = new double[n]; MEM1c = tempc; MEM1i = tempi;
                    break;
                case "MEM2":
                   MEM2c = new double[n]; MEM2i = new double[n]; MEM2c = tempc; MEM2i = tempi;
                    break;
                case "MEM3":
                    MEM3c = new double[n]; MEM3i = new double[n]; MEM3c = tempc; MEM3i = tempi;
                    break;
                case "MEM4":
                    MEM4c = new double[n]; MEM4i = new double[n]; MEM4c = tempc; MEM4i = tempi;
                    break;
                case "MEM5":
                   MEM5c = new double[n]; MEM5i = new double[n]; MEM5c = tempc; MEM5i = tempi;
                    break;
                default:
                    break;
           }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            switch (comboBox3.Text)
            {
                case "MEM1":
                    MEM1 = !MEM1;
                    break;
                case "MEM2":
                    MEM2 = !MEM2;
                    break;
                case "MEM3":
                    MEM3 = !MEM3;
                    break;
                case "MEM4":
                    MEM4 = !MEM4;
                    break;
                case "MEM5":
                    MEM5 = !MEM5;
                    break;
                case "TEMP":
                    tempdata = !tempdata;
                    break;
                default:
                    break;
            }
            plot();
        }

      
    }
}
