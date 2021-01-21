using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;

namespace _4211711003_UAS
{
    public partial class Form1 : Form
    {
        //Global Variabel
        Bitmap sourceImage = null;
        Bitmap srcImage = null;
        Bitmap HSLImage = null, RGBImage = null, YCbCrImage = null;

        //Space antara min Trackbar dan max Trackbar
        int TRACK_SPACE = 2;

        //HSL trackbar variable
        int Hmin, Hmax;
        float Smin, Smax, Lmin, Lmax;

        //RGB trackbar variable
        int Rmin, Rmax, Gmin, Gmax, Bmin, Bmax;

        //Global variable for camera initialization
        public FilterInfoCollection USB_Webcams = null;
        public VideoCaptureDevice videoCap = null;

        //YCbCr trackbar variable
        float Ymin, Ymax, Cbmin, Cbmax, Crmin, Crmax;

        public Form1()
        {
            InitializeComponent();

            //trackbar init
            trackBarInit();
            trackBarReset(true, true, true);
            trackBarDisable();
            labelReset(true, true, true);

            //fullscreen display
            WindowState = FormWindowState.Maximized;
        }

        //For Form1
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //Enumerate video devices
                USB_Webcams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (USB_Webcams.Count == 0)
                {
                    throw new ApplicationException();
                }

                //add all device to comboBox
                foreach (FilterInfo videoCaptureDevice in USB_Webcams)
                {
                    comboBoxCamSel.Items.Add(videoCaptureDevice.Name);
                }

            }

            catch (ApplicationException)
            {
                comboBoxCamSel.Items.Add("No local capture devices");
                comboBoxCamSel.Enabled = false;
                buttonStart.Enabled = false;
            }

            comboBoxCamSel.SelectedIndex = 0;
        }

        //For Form 1 Closed
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (videoCap != null && videoCap.IsRunning)
                videoCap.Stop();
        }

        //For Start Button
        private void buttonStart_Click(object sender, EventArgs e)
        {
            //buttonOpen.Enabled = false;
            trackBarDisable();

            trackBarReset(true, true, true);
            labelReset(true, true, true);


            pictureBox2.Image = srcImage;
            pictureBox3.Image = srcImage;


            if (USB_Webcams.Count != 0)
            {
                startCapture();
                trackBarEnable();
                buttonStart.Enabled = false;
                buttonOpen.Enabled = false;
                trackBarDisable();
            }
        }

        //For Stop Capturing
        private void stopCapture()
        {
            videoCap.Stop();
        }

        //For Start Capturing
        private void startCapture()
        {
            videoCap = new VideoCaptureDevice(USB_Webcams[comboBoxCamSel.SelectedIndex].MonikerString);
            videoCap.NewFrame += new NewFrameEventHandler(newFrame);
            videoCap.Start();
        }

        //For Setting Camera
        private void buttonSetting_Click(object sender, EventArgs e)
        {
            if ((videoCap != null))
            {
                try
                {
                    ((VideoCaptureDevice)videoCap).DisplayPropertyPage(this.Handle);
                }

                catch (NotSupportedException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

            }
        }

        //For New Frame
        private void newFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap capImage = (Bitmap)eventArgs.Frame.Clone();
            sourceImage = (Bitmap)capImage.Clone();
            pictureBox1.Image = capImage;

            if (capImage != null)
            {

                if (radioButton1.Checked)
                {
                    RGBFiltering(sourceImage);
                    objectTrackingCam(sourceImage);
                }

                else if (radioButton2.Checked)
                {
                    HSLFiltering(sourceImage);
                    objectTrackingCam(sourceImage);
                }

                else if (radioButton3.Checked)
                {
                    YCbCrFiltering(sourceImage);
                    objectTrackingCam(sourceImage);
                }
            }
        }

        //For Button Open File Dialog
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            trackBarDisable();
        }

        //For Open File Dialog
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            sourceImage = (Bitmap)Bitmap.FromFile(openFileDialog1.FileName);

            pictureBox1.Image = sourceImage;
            pictureBox2.Image = sourceImage;
            //objectTracking(sourceImage);

            trackBarReset(true, true, true);
            labelReset(true, true, true);
            trackBarEnable();
        }

        //For RGB Rb
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

            trackBarReset(true, true, true);
            labelReset(true, true, true);

            if (buttonOpen.Enabled)
            {
                pictureBox2.Image = sourceImage;
                pictureBox3.Image = sourceImage;

            }
            else
            {
                pictureBox2.Image = srcImage;
                pictureBox3.Image = srcImage;
            }

            //RGB trackbar Enable
            trackBarRmax.Enabled = true;
            trackBarRmin.Enabled = true;

            trackBarGmax.Enabled = true;
            trackBarGmin.Enabled = true;

            trackBarBmax.Enabled = true;
            trackBarBmin.Enabled = true;

            //YCbCr trackbar disable
            trackBarYmax.Enabled = false;
            trackBarYmin.Enabled = false;

            trackBarCbmax.Enabled = false;
            trackBarCbmin.Enabled = false;

            trackBarCrmax.Enabled = false;
            trackBarCrmin.Enabled = false;

            //HSL tracbar disable
            trackBarHmax.Enabled = false;
            trackBarHmin.Enabled = false;

            trackBarSmax.Enabled = false;
            trackBarSmin.Enabled = false;

            trackBarLmax.Enabled = false;
            trackBarLmin.Enabled = false;
        }

        //For HSL Rb
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            trackBarReset(true, true, true);
            labelReset(true, true, true);

            if (buttonOpen.Enabled)
            {
                pictureBox2.Image = sourceImage;
                pictureBox3.Image = sourceImage;
            }
            else
            {
                pictureBox2.Image = srcImage;
                pictureBox3.Image = srcImage;
            }

            //HSL tracbar Enable
            trackBarHmax.Enabled = true;
            trackBarHmin.Enabled = true;

            trackBarSmax.Enabled = true;
            trackBarSmin.Enabled = true;

            trackBarLmax.Enabled = true;
            trackBarLmin.Enabled = true;

            //RGB trackbar disable
            trackBarRmax.Enabled = false;
            trackBarRmin.Enabled = false;

            trackBarGmax.Enabled = false;
            trackBarGmin.Enabled = false;

            trackBarBmax.Enabled = false;
            trackBarBmin.Enabled = false;

            //YCbCr trackbar disable
            trackBarYmax.Enabled = false;
            trackBarYmin.Enabled = false;

            trackBarCbmax.Enabled = false;
            trackBarCbmin.Enabled = false;

            trackBarCrmax.Enabled = false;
            trackBarCrmin.Enabled = false;
        }

        //For YCbCr Rb
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            trackBarReset(true, true, true);
            labelReset(true, true, true);

            if (buttonOpen.Enabled)
            {
                pictureBox2.Image = sourceImage;
                pictureBox3.Image = sourceImage;
            }
            else
            {
                pictureBox2.Image = srcImage;
                pictureBox3.Image = srcImage;
            }

            //YCbCr trackbar disable
            trackBarYmax.Enabled = true;
            trackBarYmin.Enabled = true;

            trackBarCbmax.Enabled = true;
            trackBarCbmin.Enabled = true;

            trackBarCrmax.Enabled = true;
            trackBarCrmin.Enabled = true;

            //RGB trackbar disable
            trackBarRmax.Enabled = false;
            trackBarRmin.Enabled = false;

            trackBarGmax.Enabled = false;
            trackBarGmin.Enabled = false;

            trackBarBmax.Enabled = false;
            trackBarBmin.Enabled = false;

            //HSL tracbar disable
            trackBarHmax.Enabled = false;
            trackBarHmin.Enabled = false;

            trackBarSmax.Enabled = false;
            trackBarSmin.Enabled = false;

            trackBarLmax.Enabled = false;
            trackBarLmin.Enabled = false;

        }

        //For Reset Button
        private void buttonReset_Click(object sender, EventArgs e)
        {
            trackBarReset(true, true, true);
            labelReset(true, true, true);
            
            if (buttonOpen.Enabled)
            {
                pictureBox2.Image = sourceImage;
                pictureBox3.Image = sourceImage;
            }
            else
            {
                pictureBox2.Image = srcImage;
                pictureBox3.Image = srcImage;
            }
        }

        //For Camera Stop Button
        private void buttonCamStop_Click(object sender, EventArgs e)
        {
            stopCapture();
            buttonStart.Enabled = true;
            buttonOpen.Enabled = true;
        }

        //For HSL Filtering
        private void HSLFiltering(Bitmap srcImage)
        {
            //Create filter
            HSLFiltering filter = new HSLFiltering();

            filter.Hue = new IntRange(Hmin, Hmax);
            filter.Saturation = new Range(Smin, Smax);
            filter.Luminance = new Range(Lmin, Lmax);

            //Apply the filter
            HSLImage = filter.Apply(sourceImage);
            pictureBox2.Image = HSLImage;
        }

        //For RGB Filtering
        private void RGBFiltering(Bitmap srcImage)
        {
            //Create filter
            ColorFiltering filter = new ColorFiltering();

            //Set color ranges to keep
            filter.Red = new IntRange(Rmin, Rmax);
            filter.Green = new IntRange(Gmin, Gmax);
            filter.Blue = new IntRange(Bmin, Bmax);

            //Apply the filter
            RGBImage = filter.Apply(sourceImage);
            pictureBox2.Image = RGBImage;
        }

        //For YCbCr Filtering
        private void YCbCrFiltering(Bitmap srcImage)
        {
            // create filter
            YCbCrFiltering filter = new YCbCrFiltering();

            // set color ranges to keep
            filter.Y = new Range(Ymin, Ymax);
            filter.Cb = new Range(Cbmin, Cbmax);
            filter.Cr = new Range(Crmin, Crmax);

            YCbCrImage = filter.Apply(sourceImage);

            //draw the picture
            pictureBox2.Image = YCbCrImage;
        }

        //For Tracking Object From Camera
        private void objectTrackingCam(Bitmap srcImage)
        {

            //Copy detected image to the new one
            if (radioButton1.Checked)
            {

                Bitmap newImage = (Bitmap)RGBImage.Clone();

                //Blob counter on the detected image
                BlobCounter bc = new BlobCounter();
                bc.MinHeight = 5;
                bc.MinWidth = 5;
                bc.FilterBlobs = true;
                bc.ObjectsOrder = ObjectsOrder.Area;
                bc.ProcessImage(newImage);
                Rectangle[] rects = bc.GetObjectsRectangles();
                foreach (Rectangle recs in rects)
                {
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0]; //= recs;
                        Graphics graph = Graphics.FromImage(srcImage);

                        using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 2))
                        {
                            graph.DrawRectangle(pen, objectRect);
                        }

                        graph.Dispose();
                    }
                }

                //Draw tracked object on picture box
                pictureBox3.Image = srcImage;

            }

            else if (radioButton2.Checked)
            {

                Bitmap newImage = (Bitmap)HSLImage.Clone();

                //Blob counter on the detected image
                BlobCounter bc = new BlobCounter();
                bc.MinHeight = 5;
                bc.MinWidth = 5;
                bc.FilterBlobs = true;
                bc.ObjectsOrder = ObjectsOrder.Area;
                bc.ProcessImage(newImage);
                Rectangle[] rects = bc.GetObjectsRectangles();
                foreach (Rectangle recs in rects)
                {
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0]; //= recs;
                        Graphics graph = Graphics.FromImage(srcImage);

                        using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 2))
                        {
                            graph.DrawRectangle(pen, objectRect);
                        }

                        graph.Dispose();
                    }
                }

                //Draw tracked object on picture box
                pictureBox3.Image = srcImage;
            }

            else if (radioButton3.Checked)
            {

                Bitmap newImage = (Bitmap)YCbCrImage.Clone();

                //Blob counter on the detected image
                BlobCounter bc = new BlobCounter();
                bc.MinHeight = 5;
                bc.MinWidth = 5;
                bc.FilterBlobs = true;
                bc.ObjectsOrder = ObjectsOrder.Area;
                bc.ProcessImage(newImage);
                Rectangle[] rects = bc.GetObjectsRectangles();
                foreach (Rectangle recs in rects)
                {
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0]; //= recs;
                        Graphics graph = Graphics.FromImage(srcImage);

                        using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 2))
                        {
                            graph.DrawRectangle(pen, objectRect);
                        }

                        graph.Dispose();
                    }
                }

                //Draw tracked object on picture box
                pictureBox3.Image = srcImage;
            }
        }

        //For Tracking Object From Image
        private void ObjectTracking1(Bitmap sourceImage)
        {
            if (radioButton1.Checked)
            {
                Bitmap tempImage = new Bitmap(sourceImage);
                pictureBox3.Image = tempImage;

                BlobCounter bc = new BlobCounter();
                bc.MinHeight = 5;
                bc.MinWidth = 5;
                bc.FilterBlobs = true;
                bc.ObjectsOrder = ObjectsOrder.Area;

                bc.ProcessImage(RGBImage);
                Rectangle[] rects = bc.GetObjectsRectangles();
                foreach (Rectangle recs in rects)
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0]; //= recs;
                        Graphics graph = Graphics.FromImage(tempImage);
                        using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 2))
                        {
                            graph.DrawRectangle(pen, objectRect);
                        }
                        graph.Dispose();
                    }
            }

            else if (radioButton2.Checked)
            {
                Bitmap tempImage = new Bitmap(sourceImage);
                pictureBox3.Image = tempImage;

                BlobCounter bc = new BlobCounter();
                bc.MinHeight = 5;
                bc.MinWidth = 5;
                bc.FilterBlobs = true;
                bc.ObjectsOrder = ObjectsOrder.Area;

                bc.ProcessImage(HSLImage);
                Rectangle[] rects = bc.GetObjectsRectangles();
                foreach (Rectangle recs in rects)
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0]; //= recs;
                        Graphics graph = Graphics.FromImage(tempImage);
                        using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 2))
                        {
                            graph.DrawRectangle(pen, objectRect);
                        }
                        graph.Dispose();
                    }
            }

            else if (radioButton3.Checked)
            {
                Bitmap tempImage = new Bitmap(sourceImage);
                pictureBox3.Image = tempImage;

                BlobCounter bc = new BlobCounter();
                bc.MinHeight = 5;
                bc.MinWidth = 5;
                bc.FilterBlobs = true;
                bc.ObjectsOrder = ObjectsOrder.Area;

                bc.ProcessImage(YCbCrImage);
                Rectangle[] rects = bc.GetObjectsRectangles();
                foreach (Rectangle recs in rects)
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0]; //= recs;
                        Graphics graph = Graphics.FromImage(tempImage);
                        using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 2))
                        {
                            graph.DrawRectangle(pen, objectRect);
                        }
                        graph.Dispose();
                    }
            }
        }

        //For Rmin Trackbar
        private void trackBarRmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarRmax.Value - trackBarRmin.Value <= TRACK_SPACE)
                trackBarRmin.Value = trackBarRmax.Value - TRACK_SPACE;

            Rmin = trackBarRmin.Value;
            labelRmin.Text = string.Format("RMin : {0}", Rmin);

            if (buttonOpen.Enabled)
            {
                RGBFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Rmax Trackbar
        private void trackBarRmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarRmax.Value - trackBarRmin.Value <= TRACK_SPACE)
                trackBarRmax.Value = trackBarRmin.Value + TRACK_SPACE;

            Rmax = trackBarRmax.Value;
            labelRmax.Text = string.Format("RMax : {0}", Rmax);

            if (buttonOpen.Enabled)
            {
                RGBFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Gmin Trackbar
        private void trackBarGmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarGmax.Value - trackBarGmin.Value <= TRACK_SPACE)
                trackBarGmin.Value = trackBarGmax.Value - TRACK_SPACE;

            Gmin = trackBarGmin.Value;
            labelGmin.Text = string.Format("Gmin : {0}", Gmin);

            if (buttonOpen.Enabled)
            {
                RGBFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Gmax Trackbar
        private void trackBarGmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarGmax.Value - trackBarGmin.Value <= TRACK_SPACE)
                trackBarGmax.Value = trackBarGmin.Value + TRACK_SPACE;

            Gmax = trackBarGmax.Value;
            labelGmax.Text = string.Format("GMax : {0}", Gmax);

            if (buttonOpen.Enabled)
            {
                RGBFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Bmin Trackbar
        private void trackBarBmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarBmax.Value - trackBarBmin.Value <= TRACK_SPACE)
                trackBarBmin.Value = trackBarBmax.Value - TRACK_SPACE;

            Bmin = trackBarBmin.Value;
            labelBmin.Text = string.Format("Bmin : {0}", Bmin);

            if (buttonOpen.Enabled)
            {
                RGBFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }
        
        //For Bmax Trackbar
        private void trackBarBmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarBmax.Value - trackBarBmin.Value <= TRACK_SPACE)
                trackBarBmax.Value = trackBarBmin.Value + TRACK_SPACE;

            Bmax = trackBarBmax.Value;
            labelBmax.Text = string.Format("Bmax : {0}", Bmax);
            if (buttonOpen.Enabled)
            {
                RGBFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Hmin Trackbar
        private void trackBarHmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarHmax.Value - trackBarHmin.Value <= TRACK_SPACE)
                trackBarHmin.Value = trackBarHmax.Value - TRACK_SPACE;

            Hmin = trackBarHmin.Value;
            labelHmin.Text = string.Format("HueMin : {0}", Hmin);

            if (buttonOpen.Enabled)
            {
                HSLFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Hmax Trackbar
        private void trackBarHmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarHmax.Value - trackBarHmin.Value <= TRACK_SPACE)
                trackBarHmax.Value = trackBarHmin.Value + TRACK_SPACE;

            Hmax = trackBarHmax.Value;
            labelHmax.Text = string.Format("HueMax : {0}", Hmax);

            if (buttonOpen.Enabled)
            {
                HSLFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Smin Trackbar
        private void trackBarSmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarSmax.Value - trackBarSmin.Value <= TRACK_SPACE)
                trackBarSmin.Value = trackBarSmax.Value - TRACK_SPACE;

            Smin = (float)trackBarSmin.Value / 100;
            labelSmin.Text = string.Format("SMin : {0}", Smin);

            if (buttonOpen.Enabled)
            {
                HSLFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Smax Trackbar
        private void trackBarSmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarSmax.Value - trackBarSmin.Value <= TRACK_SPACE)
                trackBarSmax.Value = trackBarSmin.Value + TRACK_SPACE;

            Smax = (float)trackBarSmax.Value / 100;
            labelSmax.Text = string.Format("SMax : {0}", Smax);

            if (buttonOpen.Enabled)
            {
                HSLFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }

        }

        //For Lmin Trackbar
        private void trackBarLmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarLmax.Value - trackBarLmin.Value <= TRACK_SPACE)
                trackBarLmin.Value = trackBarLmax.Value - TRACK_SPACE;

            Lmin = (float)trackBarLmin.Value / 100;
            labelLmin.Text = string.Format("LMin : {0}", Lmin);

            if (buttonOpen.Enabled)
            {
                HSLFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Lmax Trackbar
        private void trackBarLmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarLmax.Value - trackBarLmin.Value <= TRACK_SPACE)
                trackBarLmax.Value = trackBarLmin.Value + TRACK_SPACE;

            Lmax = (float)trackBarLmax.Value / 100;
            labelLmax.Text = string.Format("LMax : {0}", Lmax);

            if (buttonOpen.Enabled)
            {
                HSLFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }

        }

        //For Ymin Trackbar
        private void trackBarYmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarYmax.Value - trackBarYmin.Value <= TRACK_SPACE)
                trackBarYmin.Value = trackBarYmax.Value - TRACK_SPACE;

            Ymin = (float)trackBarYmin.Value / 100;
            labelYmin.Text = string.Format("Ymin : {0}", Ymin);

            if (buttonOpen.Enabled)
            {
                YCbCrFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Ymax Trackbar
        private void trackBarYmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarYmax.Value - trackBarYmin.Value <= TRACK_SPACE)
                trackBarYmax.Value = trackBarYmin.Value + TRACK_SPACE;

            Ymax = (float)trackBarYmax.Value / 100;
            labelYmax.Text = string.Format("Ymax : {0}", Ymax);

            if (buttonOpen.Enabled)
            {
                YCbCrFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Cbmin Trackbar
        private void trackBarCbmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarCbmax.Value - trackBarCbmin.Value <= TRACK_SPACE)
                trackBarCbmin.Value = trackBarCbmax.Value - TRACK_SPACE;

            Cbmin = (float)trackBarCbmin.Value / 100;
            labelCbmin.Text = string.Format("Cbmin : {0}", Cbmin);

            if (buttonOpen.Enabled)
            {
                YCbCrFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Cbmax Trackbar
        private void trackBarCbmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarCbmax.Value - trackBarCbmin.Value <= TRACK_SPACE)
                trackBarCbmax.Value = trackBarCbmin.Value + TRACK_SPACE;

            Cbmax = (float)trackBarCbmax.Value / 100;
            labelCbmax.Text = string.Format("Cbmax : {0}", Cbmax);

            if (buttonOpen.Enabled)
            {
                YCbCrFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Crmin Trackbar
        private void trackBarCrmin_Scroll(object sender, EventArgs e)
        {
            if (trackBarCrmax.Value - trackBarCrmin.Value <= TRACK_SPACE)
                trackBarCrmin.Value = trackBarCrmax.Value - TRACK_SPACE;

            Crmin = (float)trackBarCrmin.Value / 100;
            labelCrmin.Text = string.Format("Crmin : {0}", Crmin);

            if (buttonOpen.Enabled)
            {
                YCbCrFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //For Crmax Trackbar
        private void trackBarCrmax_Scroll(object sender, EventArgs e)
        {
            if (trackBarCrmax.Value - trackBarCrmin.Value <= TRACK_SPACE)
                trackBarCrmax.Value = trackBarCrmin.Value + TRACK_SPACE;

            Crmax = (float)trackBarCrmax.Value / 100;
            labelCrmax.Text = string.Format("Crmax : {0}", Crmax);

            if (buttonOpen.Enabled)
            {
                YCbCrFiltering(sourceImage);
                ObjectTracking1(sourceImage);
            }
        }

        //Trackbar Initialization
        private void trackBarInit()
        {
            trackBarYmax.Maximum = 100;
            trackBarYmin.Maximum = 100;

            trackBarCbmax.Maximum = 50;
            trackBarCbmax.Minimum = -50;
            trackBarCbmin.Maximum = 50;
            trackBarCbmin.Minimum = -50;

            trackBarCrmax.Maximum = 50;
            trackBarCrmax.Minimum = -50;
            trackBarCrmin.Maximum = 50;
            trackBarCrmin.Minimum = -50;

            trackBarHmin.Maximum = 360;
            trackBarHmax.Maximum = 360;

            trackBarSmax.Maximum = 100;
            trackBarSmin.Maximum = 100;

            trackBarLmax.Maximum = 100;
            trackBarLmin.Maximum = 100;

            trackBarRmax.Maximum = 255;
            trackBarRmin.Maximum = 255;

            trackBarGmax.Maximum = 255;
            trackBarGmin.Maximum = 255;

            trackBarBmax.Maximum = 255;
            trackBarBmin.Maximum = 255;
        }

        //Trackbar Reseting
        private void trackBarReset(bool RGB, Boolean HSL, bool YCbCr)
        {
            //RGB trackbar reset
            if (RGB)
            {
                trackBarRmax.Value = 255;
                trackBarRmin.Value = 0;

                trackBarGmax.Value = 255;
                trackBarGmin.Value = 0;

                trackBarBmax.Value = 255;
                trackBarBmin.Value = 0;

                Rmin = 0; Rmax = 255;
                Gmin = 0; Gmax = 255;
                Bmin = 0; Bmax = 255;
            }

            //YCbCr trackbar reset
            if (YCbCr)
            {
                trackBarYmax.Value = 100;
                trackBarYmin.Value = 0;

                trackBarCbmax.Value = 50;
                trackBarCbmin.Value = -50;

                trackBarCrmax.Value = 50;
                trackBarCrmin.Value = -50;

                Ymin = (float)trackBarYmin.Value / 100;
                Ymax = (float)trackBarYmax.Value / 100;
                Crmin = (float)trackBarCrmin.Value / 100;
                Crmax = (float)trackBarCrmax.Value / 100;
                Cbmin = (float)trackBarCbmin.Value / 100;
                Cbmax = (float)trackBarCbmax.Value / 100;
            }

            //HSL tracbar reset
            if (HSL)
            {
                trackBarHmax.Value = 360;
                trackBarHmin.Value = 0;

                trackBarSmax.Value = 100;
                trackBarSmin.Value = 0;

                trackBarLmax.Value = 100;
                trackBarLmin.Value = 0;

                Hmin = trackBarHmin.Value;
                Hmax = trackBarHmax.Value;

                Smin = (float)trackBarSmin.Value / 100;
                Smax = (float)trackBarSmax.Value / 100;

                Lmin = (float)trackBarLmin.Value / 100;
                Lmax = (float)trackBarLmax.Value / 100;
            }
        }

        //Trackbar Disable
        private void trackBarDisable()
        {
            //HSL tracbar disable
            trackBarHmax.Enabled = false;
            trackBarHmin.Enabled = false;

            trackBarSmax.Enabled = false;
            trackBarSmin.Enabled = false;

            trackBarLmax.Enabled = false;
            trackBarLmin.Enabled = false;

            //RGB trackbar disable
            trackBarRmax.Enabled = false;
            trackBarRmin.Enabled = false;

            trackBarGmax.Enabled = false;
            trackBarGmin.Enabled = false;

            trackBarBmax.Enabled = false;
            trackBarBmin.Enabled = false;

            //YCbCr trackbar disable
            trackBarYmax.Enabled = false;
            trackBarYmin.Enabled = false;

            trackBarCbmax.Enabled = false;
            trackBarCbmin.Enabled = false;

            trackBarCrmax.Enabled = false;
            trackBarCrmin.Enabled = false;

            //button reset disable
            //buttonReset.Enabled = false;
        }

        //Trackbar Enable
        private void trackBarEnable()
        {
            //HSL tracbar Enable
            trackBarHmax.Enabled = true;
            trackBarHmin.Enabled = true;

            trackBarSmax.Enabled = true;
            trackBarSmin.Enabled = true;

            trackBarLmax.Enabled = true;
            trackBarLmin.Enabled = true;

            //RGB trackbar Enable
            trackBarRmax.Enabled = true;
            trackBarRmin.Enabled = true;

            trackBarGmax.Enabled = true;
            trackBarGmin.Enabled = true;

            trackBarBmax.Enabled = true;
            trackBarBmin.Enabled = true;

            //YCbCr trackbar disable
            trackBarYmax.Enabled = true;
            trackBarYmin.Enabled = true;

            trackBarCbmax.Enabled = true;
            trackBarCbmin.Enabled = true;

            trackBarCrmax.Enabled = true;
            trackBarCrmin.Enabled = true;

            //buttonReset.Enabled = false;
        }

        //Reseting Label
        private void labelReset(bool RGB, bool HSL, bool YCbCr)
        {
            //HSL label reset
            if (HSL)
            {
                labelHmax.Text = string.Format("HueMax : {0}", trackBarHmax.Value);
                labelHmin.Text = string.Format("HueMin : {0}", trackBarHmin.Value);

                labelSmin.Text = string.Format("SatMin : {0}", (float)trackBarSmin.Value / 100);
                labelSmax.Text = string.Format("SatMax : {0}", (float)trackBarSmax.Value / 100);

                labelLmax.Text = string.Format("LumMax : {0}", (float)trackBarLmax.Value / 100);
                labelLmin.Text = string.Format("LumMin : {0}", (float)trackBarLmin.Value / 100);
            }

            //RGB label reset
            if (RGB)
            {
                labelRmax.Text = string.Format("RMax : {0}", trackBarRmax.Value);
                labelRmin.Text = string.Format("RMin : {0}", trackBarRmin.Value);

                labelGmax.Text = string.Format("GMax : {0}", trackBarGmax.Value);
                labelGmin.Text = string.Format("Gmin : {0}", trackBarGmin.Value);

                labelBmax.Text = string.Format("Bmax : {0}", trackBarBmax.Value);
                labelBmin.Text = string.Format("Bmin : {0}", trackBarBmin.Value);
            }
            //YCbCr label reset
            if (YCbCr)
            {
                labelYmax.Text = string.Format("Ymax : {0}", (float)trackBarYmax.Value / 100);
                labelYmin.Text = string.Format("Ymin : {0}", (float)trackBarYmin.Value / 100);

                labelCbmax.Text = string.Format("Cbmax : {0}", (float)trackBarCbmax.Value / 100);
                labelCbmin.Text = string.Format("Cbmin : {0}", (float)trackBarCbmin.Value / 100);

                labelCrmax.Text = string.Format("Crmax : {0}", (float)trackBarCrmax.Value / 100);
                labelCrmin.Text = string.Format("Crmin : {0}", (float)trackBarCrmin.Value / 100);
            }
        }

        //For Exit Button
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
