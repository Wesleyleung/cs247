//CS247 Gesture Controller
//TODO: change green circles to expand the thing.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using System.IO;
using System.Windows.Media.Imaging;
using System.Timers;

namespace SkeletalTracking
{
    class SkeletonController
    {
        private MainWindow window;
        public SkeletonController(MainWindow win)
        {
            window = win;
        }

        private Boolean leftHandDominant = false;
        private Boolean rightHandDominant = false;
        private int shirtYPos;
        private int shirtXPos;
        private int shirtYRange;
        private int shirtXRange;
        private Boolean shirtYPosSet = false;
        private Boolean shirtColorDetected = false;

        private Boolean player1TrainingComplete = false;
        private Boolean player2TrainingComplete = false;
        private int player1R;
        private int player1G;
        private int player1B; 
        private int player2R;
        private int player2G;
        private int player2B;
        private Boolean player1IsRighty = true;
        private Boolean player2IsRighty = true;
        private Boolean stopRecording = false;

        //This function will be implemented by you in the subclass files provided.
        //A simple example of highlighting targets when hovered over has been provided below

        //Note: targets is a dictionary that allows you to retrieve the corresponding target on screen
        //and manipulate its state and position, as well as hide/show it (see class defn. below).
        //It is indexed from 1, thus you can retrieve an individual target with the expression
        //targets[3], which would retrieve the target labeled "3" on screen.
        public virtual void processSkeletonFrame(SkeletonData skeleton)
        {
            /*Example implementation*/
            //Scale the joints to the size of the window
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint head = skeleton.Joints[JointID.Head].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint shoulderCenter = skeleton.Joints[JointID.ShoulderCenter].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint hipCenter = skeleton.Joints[JointID.HipCenter].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint shoulderRight = skeleton.Joints[JointID.ShoulderRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint FootLeft = skeleton.Joints[JointID.FootLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            
            Boolean inBall = (Math.Abs(FootLeft.Position.Y -head.Position.Y) < 40)? true: false;
            
            if (leftHand.Position.Y > head.Position.Y && rightHand.Position.Y > head.Position.Y && !player2TrainingComplete)
            {
                leftHandDominant = false;
                rightHandDominant = false;
                shirtYPosSet = false;
                shirtColorDetected = false;
            }

            if (leftHand.Position.Y < head.Position.Y && !leftHandDominant && !rightHandDominant && !inBall)
            {
                leftHandDominant = true;
                Console.WriteLine("leftHandSelected");
            }
            else if(rightHand.Position.Y < head.Position.Y && !leftHandDominant && !rightHandDominant && !inBall){
                rightHandDominant = true;
                Console.WriteLine("rightHandSelected");
            }
            if (rightHandDominant || leftHandDominant)
            {
                if (!shirtYPosSet)
                {
                    shirtYPos = (int)((shoulderCenter.Position.Y + hipCenter.Position.Y) / 2.0);
                    shirtXPos = (int)((shoulderCenter.Position.X + hipCenter.Position.X) / 2.0);
                    shirtYRange = (int)(Math.Abs((hipCenter.Position.Y - shoulderCenter.Position.Y)) / 8.0);
                    shirtXRange = (int)(Math.Abs((shoulderRight.Position.X - shoulderCenter.Position.X)) / 8.0);
                    shirtYPosSet = true;
                    Console.WriteLine("ShirtYPos" + shirtYPos);
                    Console.WriteLine("ShirtXPos" + shirtXPos);
                    Console.WriteLine("ShirtYRange" + shirtYRange);
                    Console.WriteLine("ShirtXRange" + shirtXRange);
                }
            }
        }

        private Boolean go = true;
        private int range = 20;
        //Must be divisor of 640 and 480
        private int sampleRate = 5;
        private List<int> player1PositionX = new List<int>();
        private List<int> player1PositionY = new List<int>();
        private List<int> player2PositionX = new List<int>();
        private List<int> player2PositionY = new List<int>();

        private void trackPlayers(ImageFrame img, int playerNum)
        {
            int playerR = 0;
            int playerG = 0;
            int playerB = 0;
            if(playerNum==1){
                playerR = player1R;
                playerG = player1G;
                playerB = player1B;
            }
            if(playerNum==2){
                playerR = player2R;
                playerG = player2G;
                playerB = player2B;
            }
            
            if (go)
            {
                PlanarImage image = img.Image;
                int offset = 0;
                int[] shirtColorArray = new int[(640 * 480)/(sampleRate*sampleRate)];
                int row = -1;
                int maxCount = 0;
                //If this is too slow we can go to +=5
                for (int y = 0; y < 480/sampleRate; y++)
                {

                    //Console.Write("R:" + y);
                    //Console.Write(" ");
                    int count = 0;
                    for (int x = 0; x < 640/sampleRate; x++)
                    {
                        
                        if (image.Bits[offset + 2] + range> playerR && image.Bits[offset + 2] - range < playerR && image.Bits[offset + 1] + range > playerG && image.Bits[offset + 1] - range < playerG && image.Bits[offset] + range > playerB && image.Bits[offset] - range < playerB)
                        {
                            shirtColorArray[y * (640/sampleRate) + x] = 1;
                            count++;
                        }
                        else
                        {
                            shirtColorArray[y * (640/sampleRate) + x] = 0;
                        }
                        offset += 4*sampleRate;
                        //Console.Write(shirtColorArray[y * (640 / sampleRate) + x]);
                    }

                    if(count > maxCount) {
                        maxCount = count;
                        row = y;
                    }
                    //Console.WriteLine("");
                    offset += 4*(640*(sampleRate-1));
                }
                int newCount = 0;
                int col = -1;
                if (row >= 0)
                {
                    for (int i = 0; i < 640 / sampleRate; i++)
                    {
                        if (shirtColorArray[row * (640 / sampleRate) + i] == 1)
                        {
                            newCount++;
                        }
                        if (newCount == maxCount / 2)
                        {
                            col = i;
                            break;
                        }
                    }
                }
                //Console.WriteLine("max row:" + row);
                //Console.WriteLine("max col:" + col);
                double xpixel = (double)(col * sampleRate);
                double ypixel = (double)(row * sampleRate);

                //Console.WriteLine("xpixel:" + xpixel);
                //Console.WriteLine("ypixel:" + ypixel);
                if (playerNum == 1)
                {
                    player1PositionX.Add((int)xpixel);
                    player1PositionY.Add((int)ypixel);
                    window.ellipse3.SetValue(Canvas.LeftProperty, xpixel);
                    window.ellipse3.SetValue(Canvas.TopProperty, ypixel);
                }
                if (playerNum == 2)
                {
                    player2PositionX.Add((int)xpixel);
                    player2PositionY.Add((int)ypixel);
                    window.ellipse4.SetValue(Canvas.LeftProperty, xpixel);
                    window.ellipse4.SetValue(Canvas.TopProperty, ypixel);
                }
                        
                //window.ellipse3.SetValue(Canvas.LeftProperty, 300.0);
                //window.ellipse3.SetValue(Canvas.TopProperty, ypixel);
                //go = false;
            }
        }
        
        public virtual void stopRecordingClicked()
        {
            
            if (!stopRecording)
            {
                stopRecording = true;
                window.image1.Opacity = 0.0;
                window.qrcode.Opacity = 1.0;
                window.ellipse1.Opacity = 0.0;
                window.ellipse2.Opacity = 0.0;
                window.ellipse3.Opacity = 0.0;
                window.ellipse4.Opacity = 0.0;

                //turnOffSkeleton();

                
                
                System.IO.StreamWriter file1 = new System.IO.StreamWriter( "C:\\kinectimgs\\playa1.txt");
                for (int i = 0; i < player1PositionX.Count; i++)
                {
                    file1.WriteLine(player1PositionX[i] + "," + player1PositionY[i]);
                    //window.ellipse3.SetValue(Canvas.LeftProperty, i*5.0);
                    //Console.WriteLine("1X:" + player1PositionX[i]);
                    //window.ellipse3.SetValue(Canvas.TopProperty, i*5.0);
                    //Console.WriteLine("1Y:" + player1PositionY[i]);
                    
                    //window.ellipse4.SetValue(Canvas.LeftProperty, (double)(player1PositionX[i]));
                    //Console.WriteLine("2X:" + player2PositionX[i]);
                    //window.ellipse4.SetValue(Canvas.TopProperty, (double)(player2PositionY[i]));
                    //Console.WriteLine("2Y:" + player2PositionY[i]);
                    /*
                    for (int j = 0; j < 10; j++)
                    {
                        Console.WriteLine("Waiting.." + i);
                    }
                     */
                }
                file1.Close();

                System.IO.StreamWriter file2 = new System.IO.StreamWriter( "C:\\kinectimgs\\playa2.txt");
                for (int i = 0; i < player2PositionX.Count; i++)
                {
                    file2.WriteLine(player2PositionX[i] + "," + player2PositionY[i]);
                }
                file2.Close();
            }
        }

        private void turnOffSkeleton()
        {


        }

        public virtual void imageCallback(ImageFrame img)
        {
            if (player2TrainingComplete && !stopRecording)
            {
                trackPlayers(img, 1);
                trackPlayers(img, 2);
            }
            
            
            
            if (!player2TrainingComplete && shirtYPosSet && !shirtColorDetected && shirtXRange>0 && shirtYRange>0)
            {
                PlanarImage image = img.Image;
                int[] rArray = new int[(2 * shirtYRange) * (2 * shirtXRange)];
                int[] gArray = new int[(2 * shirtYRange) * (2 * shirtXRange)];
                int[] bArray = new int[(2 * shirtYRange) * (2 * shirtXRange)];
                int i = 0;
                for (int yRect = shirtYPos - shirtYRange; yRect < (shirtYPos + shirtYRange); yRect++)
                {
                    for (int xRect = shirtXPos - shirtXRange; xRect < (shirtXPos + shirtXRange); xRect++)
                    {
                        rArray[i] = image.Bits[((yRect * 640 + xRect) * 4) + 2];
                        gArray[i] = image.Bits[((yRect * 640 + xRect) * 4) + 1];
                        bArray[i] = image.Bits[((yRect * 640 + xRect) * 4)];
                        i++;
                    }
                }
                Array.Sort(rArray);
                Array.Sort(gArray);
                Array.Sort(bArray);
                int rMedian = rArray[rArray.Length/2];
                int gMedian = gArray[gArray.Length/2];
                int bMedian = bArray[bArray.Length/2];
                shirtColorDetected = true;
                if (!player1TrainingComplete)
                {
                    player1R = rMedian;
                    player1G = gMedian;
                    player1B = bMedian;
                    player1IsRighty = rightHandDominant;
                    window.ellipse1.Fill = new SolidColorBrush(Color.FromRgb((byte)rMedian, (byte)gMedian, (byte)bMedian));
                    Console.WriteLine("R:" + rMedian + " G:" + gMedian + " B:" + bMedian);
                    player1TrainingComplete = true;
                }
                else
                {
                    player2R = rMedian;
                    player2G = gMedian;
                    player2B = bMedian;
                    player2IsRighty = rightHandDominant;
                    player2TrainingComplete = true;
                    window.ellipse2.Fill = new SolidColorBrush(Color.FromRgb((byte)rMedian, (byte)gMedian, (byte)bMedian));
                    Console.WriteLine("R:" + rMedian + " G:" + gMedian + " B:" + bMedian);
                }
            }







        }

        private double calculateAngle(Joint joint1, Joint joint2, Joint joint3)
        {
            double x1x2 = joint1.Position.X - joint2.Position.X;
            double y1y2 = joint1.Position.Y - joint2.Position.Y;
            double x3x2 = joint3.Position.X - joint2.Position.X;
            double y3y2 = joint3.Position.Y - joint2.Position.Y;
            return Math.Acos((x1x2 * x3x2 + y1y2 * y3y2) / (Math.Sqrt(x1x2 * x1x2 + y1y2 * y1y2) * Math.Sqrt(x3x2 * x3x2 + y3y2 * y3y2)));    
        }


        //This is called when the controller becomes active. This allows you to place your targets and do any 
        //initialization that you don't want to repeat with each new skeleton frame. You may also 
        //directly move the targets in the MainWindow.xaml file to achieve the same initial repositioning.
        public virtual void controllerActivated(){
            
        }

        //The default value that gets passed to MaxSkeletonX and MaxSkeletonY in the Coding4Fun Joint.ScaleTo function is 1.5f
        //This function will change that so that your scaling in processSkeletonFrame aligns with the scaling done when we
        //position the ellipses in the MainWindow.xaml.cs file.
        public void adjustScale(float f)
        {
            window.k_xMaxJointScale = f;
            window.k_yMaxJointScale = f;
        }

    }
}
