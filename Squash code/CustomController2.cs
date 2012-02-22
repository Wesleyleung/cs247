//CS247 Project 3: Gesture Controller
//Controller #2
//User can scroll through and select multiple targets from the screen
//Scroll by moving right foot forwards/backwards
//Select by swiping left hand from left to right
//Unselect by swiping right hand from right to left
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
    class CustomController2 : SkeletonController
    {
        
        private MainWindow window;
        int defaultTarget = 3;
        int footBackCounter = 5;
        int footFrontCounter = 5;

        public CustomController2(MainWindow win) : base(win)
        {
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
            Joint footRight = skeleton.Joints[JointID.FootRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint footLeft = skeleton.Joints[JointID.FootLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            double rFootDepth = footRight.Position.Z;
            double lFootDepth = footLeft.Position.Z;
            double distBetweenFeet = rFootDepth - lFootDepth;
            double lHandHipDist = leftHand.Position.X - hipCenter.Position.X;
            double rHandHipDist = rightHand.Position.X - hipCenter.Position.X;

            if (lHandHipDist > 0)
            {
              targets[defaultTarget].setTargetHighlighted();
            }
            if (rHandHipDist < 0)
            {
                targets[defaultTarget].setTargetUnselected();
            }
            if (distBetweenFeet > 0.15)
            {
                footBackCounter++;
                double rLeanThreshold = 30 - (distBetweenFeet - 0.15) * 75;
                if (rLeanThreshold < 7)
                {
                    rLeanThreshold = 7;
                }
                if (footBackCounter > (int)rLeanThreshold)
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
                    footBackCounter = 0;
                }
            }

            if (distBetweenFeet < -0.15)
            {
                footFrontCounter++;
                double lLeanThreshold = 30 - (Math.Abs(0.3 + distBetweenFeet)) * 75;
                if (lLeanThreshold < 7)
                {
                    lLeanThreshold = 7;
                }
                if (footFrontCounter > (int)lLeanThreshold)
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
                    footFrontCounter = 0;
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
            targets[1].setTargetPosition(23, 366);
            targets[2].setTargetPosition(23, 280);
            targets[3].setTargetPosition(23, 197);
            targets[4].setTargetPosition(23, 112);
            targets[5].setTargetPosition(23, 23);
            window.selected1.SetValue(Canvas.LeftProperty, 10.0);
            window.selected1.SetValue(Canvas.TopProperty, 353.0);
            window.selected2.SetValue(Canvas.LeftProperty, 10.0);
            window.selected2.SetValue(Canvas.TopProperty, 267.0);
            window.selected3.SetValue(Canvas.LeftProperty, 10.0);
            window.selected3.SetValue(Canvas.TopProperty, 184.0);
            window.selected4.SetValue(Canvas.LeftProperty, 10.0);
            window.selected4.SetValue(Canvas.TopProperty, 99.0);
            window.selected5.SetValue(Canvas.LeftProperty, 10.0);
            window.selected5.SetValue(Canvas.TopProperty, 10.0);
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