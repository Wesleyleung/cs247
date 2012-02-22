//Consulted code by Travis Feirtag www.therobotgeek.net/articles/WPF_Scrolling_Content.aspx


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using System.IO;
using System.Xml; 

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //StackPanel imgPanel = this.imgStackPanel;

            List<Image> imgList = getImages(@"C:\pics");
            foreach (Image img in imgList)
            {
                //imgPanel.Children.Add(img);
            }

            obj = new FaceRestAPI("3728359974280cbfe98c90e4c4cc7e1d", "a7057564ad99fd57cabe3f286a5a1f63", "", true, "xml", "", "");
        }


         //This code is from therobotgeek.net
        private List<Image> getImages(string path)
        {
            List<Image> imgList = new List<Image>();
            string strFilePath = "";
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if(file.Extension.ToLower().Equals(".jpg")){ 
                    strFilePath = file.FullName;
                    Image cur = new Image();
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(strFilePath, UriKind.Absolute);
                    bmp.EndInit();
                    cur.Height = 140;
                    cur.Stretch = Stretch.Fill;
                    cur.Source = bmp;
                    cur.Margin = new Thickness(10);
                    imgList.Add(cur);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}-{1}", ex.Message, strFilePath));
            }
            return imgList;
        }

        //Kinect Runtime
        Runtime nui;

        //Targets and skeleton controller
        SkeletonController exampleController;
        CustomController1 yourController1;
        CustomController2 yourController2;

        //Holds the currently active controller
        SkeletonController currentController;

        //Scaling constants
        public float k_xMaxJointScale = 1.5f;
        public float k_yMaxJointScale = 1.5f;

        int num = 0;
        FaceRestAPI obj;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupKinect();
            exampleController = new SkeletonController(this);
            yourController1 = new CustomController1(this);
            yourController2 = new CustomController2(this);
            currentController = exampleController;
            currentController.controllerActivated();
        }

        private void SetupKinect()
        {
            if (Runtime.Kinects.Count == 0)
            {
                this.Title = "No Kinect connected"; 
            }
            else
            {
                //use first Kinect
                nui = Runtime.Kinects[0];

                //Initialize to do skeletal tracking
                nui.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor | RuntimeOptions.UseDepthAndPlayerIndex);

                //add event to receive skeleton data
                nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

                //add event to receive video data
                nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);

                //to experiment, toggle TransformSmooth between true & false and play with parameters            
                nui.SkeletonEngine.TransformSmooth = true;
                TransformSmoothParameters parameters = new TransformSmoothParameters();
                // parameters used to smooth the skeleton data
                parameters.Smoothing = 0.3f;
                parameters.Correction = 0.3f;
                parameters.Prediction = 0.4f;
                parameters.JitterRadius = 0.7f;
                parameters.MaxDeviationRadius = 0.2f;
                nui.SkeletonEngine.SmoothParameters = parameters;

                //Open the video stream
                nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                
                //Force video to the background
                Canvas.SetZIndex(image1, -10000);
            }
        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            num++;
            //Automagically create BitmapSource for Video
            BitmapSource img = e.ImageFrame.ToBitmapSource();
            currentController.imageCallback(e.ImageFrame);
            image1.Source = img;
            
            
            List<string> urls = new List<string>();
            if (true)
            {
                string filename = "C:\\kinectimgs\\" + num + ".jpg";
                using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(img));
                    encoder.QualityLevel = 100;
                    encoder.Save(fileStream);
                }
                //Console.WriteLine(filename);
           
                /*
                FaceRestAPI.FaceAPI objFaces = obj.faces_detect(urls, filename, null, null);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(objFaces.rawData);

                XmlNodeList xnList = xml.SelectNodes(@"response/photos/photo/tags/tag/attributes/mood");
                foreach (XmlNode xn in xnList)
                {
                    string mood = xn["value"].InnerText;
                    string confidence = xn["confidence"].InnerText;
                    Console.WriteLine(mood);
                    Console.WriteLine(confidence);
                }
                 */
            }
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            
            SkeletonFrame allSkeletons = e.SkeletonFrame;

            //get the first tracked skeleton
            SkeletonData skeleton = (from s in allSkeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();


            if(skeleton != null)
            {
                //set positions on our joints of interest (already defined as Ellipse objects in the xaml)
                SetEllipsePosition(headEllipse, skeleton.Joints[JointID.Head]);
                SetEllipsePosition(leftEllipse, skeleton.Joints[JointID.HandLeft]);
                SetEllipsePosition(rightEllipse, skeleton.Joints[JointID.HandRight]);
                SetEllipsePosition(shoulderCenter, skeleton.Joints[JointID.ShoulderCenter]);
                SetEllipsePosition(shoulderRight, skeleton.Joints[JointID.ShoulderRight]);
                SetEllipsePosition(shoulderLeft, skeleton.Joints[JointID.ShoulderLeft]);
                SetEllipsePosition(ankleRight, skeleton.Joints[JointID.AnkleRight]);
                SetEllipsePosition(ankleLeft, skeleton.Joints[JointID.AnkleLeft]);
                SetEllipsePosition(footLeft, skeleton.Joints[JointID.FootLeft]);
                SetEllipsePosition(footRight, skeleton.Joints[JointID.FootRight]);
                SetEllipsePosition(wristLeft, skeleton.Joints[JointID.WristLeft]);
                SetEllipsePosition(wristRight, skeleton.Joints[JointID.WristRight]);
                SetEllipsePosition(elbowLeft, skeleton.Joints[JointID.ElbowLeft]);
                SetEllipsePosition(elbowRight, skeleton.Joints[JointID.ElbowRight]);
                SetEllipsePosition(ankleLeft, skeleton.Joints[JointID.AnkleLeft]);
                SetEllipsePosition(footLeft, skeleton.Joints[JointID.FootLeft]);
                SetEllipsePosition(footRight, skeleton.Joints[JointID.FootRight]);
                SetEllipsePosition(wristLeft, skeleton.Joints[JointID.WristLeft]);
                SetEllipsePosition(wristRight, skeleton.Joints[JointID.WristRight]);
                SetEllipsePosition(kneeLeft, skeleton.Joints[JointID.KneeLeft]);
                SetEllipsePosition(kneeRight, skeleton.Joints[JointID.KneeRight]);
                SetEllipsePosition(hipCenter, skeleton.Joints[JointID.HipCenter]);
                SetEllipsePosition(hipLeft, skeleton.Joints[JointID.HipLeft]);
                SetEllipsePosition(hipRight, skeleton.Joints[JointID.HipRight]);
                currentController.processSkeletonFrame(skeleton);

            }
        }

        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {    
            var scaledJoint = joint.ScaleTo(640, 480, k_xMaxJointScale, k_yMaxJointScale);

            Canvas.SetLeft(ellipse, scaledJoint.Position.X - (double)ellipse.GetValue(Canvas.WidthProperty) / 2 );
            Canvas.SetTop(ellipse, scaledJoint.Position.Y - (double)ellipse.GetValue(Canvas.WidthProperty) / 2);
            Canvas.SetZIndex(ellipse, (int) -Math.Floor(scaledJoint.Position.Z*100));
            if (joint.ID == JointID.HandLeft || joint.ID == JointID.HandRight)
            {   
                byte val = (byte)(Math.Floor((joint.Position.Z - 0.8)* 255 / 2));
                ellipse.Fill = new SolidColorBrush(Color.FromRgb(val, val, val));
            }
        }



        private void Window_Closed(object sender, EventArgs e)
        {
            //Cleanup
            nui.Uninitialize();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                currentController = exampleController;
                controllerText.Content = "Example Controller";
                currentController.controllerActivated();
            }

            if (e.Key == Key.D2)
            {
                currentController = yourController1;
                controllerText.Content = "Controller 1";
                currentController.controllerActivated();
            }

            if (e.Key == Key.D3)
            {
                currentController = yourController2;
                controllerText.Content = "Controller 2";
                currentController.controllerActivated();

            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Clicked");
            currentController.stopRecordingClicked();
        }
    }


}
