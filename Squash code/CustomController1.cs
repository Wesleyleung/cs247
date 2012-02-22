//CS247 Project 3: Gesture Controller
//Controller #1
//User can scroll through and select multiple targets from the screen
//Scroll by leaning left/right
//Select and unselect by putting elbows up and hands close together
//By Parth Bhakta, Paul Lee, Wesley Leung, Chamal Samaranayake, and Pak Man Yuen
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

namespace SkeletalTracking
{
    class CustomController1 : SkeletonController
    {
        private MainWindow window;
        int defaultTarget = 3;
        int rightLeanCounter = 5;
        int leftLeanCounter = 5;
        int elbowCounter = 0;
        bool elbowsUp = false;
        
        public CustomController1(MainWindow win) : base(win){
            window = win;
        }
        /*
        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            //Scale the joints to the size of the window
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftElbow = skeleton.Joints[JointID.ElbowLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightElbow = skeleton.Joints[JointID.ElbowRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint head = skeleton.Joints[JointID.Head].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint hipCenter = skeleton.Joints[JointID.HipCenter].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint kneeLeft = skeleton.Joints[JointID.KneeLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint kneeRight = skeleton.Joints[JointID.KneeRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            double rightLeanAngle = calculateAngle(head, hipCenter, kneeRight);
            double leftLeanAngle = calculateAngle(head, hipCenter, kneeLeft);
            double rightElbowAngle = calculateAngle(rightShoulder, rightElbow, rightHand);
            double leftElbowAngle = calculateAngle(leftShoulder, leftElbow, leftHand);

            double handsSeperationX = Math.Abs(leftHand.Position.X - rightHand.Position.X);
            double handsSeperationY = Math.Abs(leftHand.Position.Y - rightHand.Position.Y);
            double shoulderSeperation = Math.Abs(leftShoulder.Position.X - rightHand.Position.X);
            if (rightElbowAngle < 1.0 && leftElbowAngle < 1.0 && handsSeperationX < shoulderSeperation && handsSeperationY < 30)
            {
                elbowCounter++;
                if (elbowCounter > 5 && !elbowsUp)
                {
                    elbowsUp = true;
                    if (targets[defaultTarget].isHighlighted())
                    {
                        targets[defaultTarget].setTargetUnselected();
                    }
                    else
                    {
                        targets[defaultTarget].setTargetHighlighted();
                    }
                    elbowCounter = 0;
                }
                leftLeanCounter = 0;
                rightLeanCounter = 0;
            }
            else
            {
                elbowsUp = false;
                elbowCounter = 0;
            }

            if (rightLeanAngle < 2.77)
            {
                rightLeanCounter++;
                double rLeanThreshold = 22 - (2.77 - rightLeanAngle) * 30;
                if (rLeanThreshold < 4)
                {
                    rLeanThreshold = 4;
                }
                if (rightLeanCounter > (int)rLeanThreshold)
                {
                    switch (defaultTarget)
                    {
                        case 1:
                            window.selected1.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 2:
                            window.selected2.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 3:
                            window.selected3.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 4:
                            window.selected4.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 5:
                            window.selected5.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        default:
                            break;
                    }
                    int nextTarget = (defaultTarget % 5) + 1;
                    defaultTarget = nextTarget;
                    switch (defaultTarget)
                    {
                        case 1:
                            window.selected1.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 2:
                            window.selected2.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 3:
                            window.selected3.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 4:
                            window.selected4.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 5:
                            window.selected5.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        default:
                            break;
                    }
                    rightLeanCounter = 0;
                }
            }

            if (leftLeanAngle < 2.77)
            {
                leftLeanCounter++;
                double lLeanThreshold = 22 - (2.77 - leftLeanAngle) * 30;
                if (lLeanThreshold < 4)
                {
                    lLeanThreshold = 4;
                }
                if (leftLeanCounter > (int)lLeanThreshold)
                {
                    switch (defaultTarget)
                    {
                        case 1:
                            window.selected1.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 2:
                            window.selected2.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 3:
                            window.selected3.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 4:
                            window.selected4.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        case 5:
                            window.selected5.Fill = new SolidColorBrush(Colors.Transparent);
                            break;
                        default:
                            break;
                    }
                    int nextTarget = defaultTarget - 1;
                    if (nextTarget == 0)
                    {
                        nextTarget = 5;
                    }
                    defaultTarget = nextTarget;
                    switch (defaultTarget)
                    {
                        case 1:
                            window.selected1.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 2:
                            window.selected2.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 3:
                            window.selected3.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 4:
                            window.selected4.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        case 5:
                            window.selected5.Fill = new SolidColorBrush(Colors.Green);
                            break;
                        default:
                            break;
                    }
                    leftLeanCounter = 0;
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

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            targets[1].setTargetPosition(23, 220);
            targets[2].setTargetPosition(111, 96);
            targets[3].setTargetPosition(262, 33);
            targets[4].setTargetPosition(409, 96);
            targets[5].setTargetPosition(505, 220);
            window.selected1.SetValue(Canvas.LeftProperty, 10.0);
            window.selected1.SetValue(Canvas.TopProperty, 207.0);
            window.selected2.SetValue(Canvas.LeftProperty, 98.0);
            window.selected2.SetValue(Canvas.TopProperty, 84.0);
            window.selected3.SetValue(Canvas.LeftProperty, 249.0);
            window.selected3.SetValue(Canvas.TopProperty, 20.0);
            window.selected4.SetValue(Canvas.LeftProperty, 396.0);
            window.selected4.SetValue(Canvas.TopProperty, 84.0);
            window.selected5.SetValue(Canvas.LeftProperty, 492.0);
            window.selected5.SetValue(Canvas.TopProperty, 207.0);
            window.selected1.Fill = new SolidColorBrush(Colors.Transparent);
            window.selected2.Fill = new SolidColorBrush(Colors.Transparent);
            window.selected3.Fill = new SolidColorBrush(Colors.Green);
            window.selected4.Fill = new SolidColorBrush(Colors.Transparent);
            window.selected5.Fill = new SolidColorBrush(Colors.Transparent);
            defaultTarget = 3;
        }
         */
    }
}
