using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace KinectBodySensorTutorial
{
    public static class Extensions
    {
        #region drawing
        public static void DrawPoint(this Canvas canvas, Joint joint)
        {
            if (joint.TrackingState == TrackingState.NotTracked) return;
            joint = joint.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            Ellipse ellipse;

            if (joint.JointType == JointType.Head)
            {
                ellipse = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = new SolidColorBrush(Colors.LightBlue)
                };
            }
            else if (joint.JointType == JointType.ThumbLeft || joint.JointType == JointType.ThumbRight)
            {
                ellipse = new Ellipse
                {
                    Width = 15,
                    Height = 15,
                    Fill = new SolidColorBrush(Colors.Indigo)
                };
            }
            else
            {
                ellipse = new Ellipse
                {
                    Width = 15,
                    Height = 15,
                    Fill = new SolidColorBrush(Colors.IndianRed)
                };
            }
            
            

            Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);

        }

        public static void DrawPoint(this Canvas canvas, ColorSpacePoint point, int type)
        {
            
            Ellipse ellipse;
            switch (type)
            {
                case 1:
                    ellipse = new Ellipse{
                        Width = 15,
                        Height = 15,
                        Fill = new SolidColorBrush(Colors.Indigo)
                    };
                    break;
                default:
                    ellipse = new Ellipse
                    {
                        Width = 15,
                        Height = 15,
                        Fill = new SolidColorBrush(Colors.IndianRed)
                    };
                    break;
            }

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);

        }


        public static void DrawBody(this Canvas canvas, Body body)
        {
            if (body.IsTracked)
            {
                Console.WriteLine("Body is tracked.");


                //Draw joints
                foreach (Joint joint in body.Joints.Values)
                {
                    canvas.DrawPoint(joint);
                }

                canvas.DrawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck]);
                canvas.DrawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder]);
                canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft]);
                canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight]);
                canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid]);
                canvas.DrawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]);
                canvas.DrawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]);
                canvas.DrawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]);
                canvas.DrawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]);
                canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft]);
                canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight]);
                canvas.DrawLine(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft]);
                canvas.DrawLine(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight]);
                canvas.DrawLine(body.Joints[JointType.HandTipLeft], body.Joints[JointType.ThumbLeft]);
                canvas.DrawLine(body.Joints[JointType.HandTipRight], body.Joints[JointType.ThumbRight]);
                canvas.DrawLine(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase]);
                canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]);
                canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight]);
                canvas.DrawLine(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft]);
                canvas.DrawLine(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight]);
                canvas.DrawLine(body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]);
                canvas.DrawLine(body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]);
                canvas.DrawLine(body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft]);
                canvas.DrawLine(body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight]);
            }
        }

        public static void DrawLine(this Canvas canvas, Joint first, Joint second)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            first = first.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);
            second = second.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            Line line = new Line
            {
                X1 = first.Position.X,
                Y1 = first.Position.Y,
                X2 = second.Position.X,
                Y2 = second.Position.Y,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(Colors.Black)
            };

            canvas.Children.Add(line);
        }
        #endregion

        #region Body

        public static Joint ScaleTo(this Joint joint, double width, double height, float skeletonMaxX, float skeletonMaxY)
        {
            joint.Position = new CameraSpacePoint
            {
                X = Scale(width, skeletonMaxX, joint.Position.X),
                Y = Scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            return joint;
        }

        public static Joint ScaleTo(this Joint joint, double width, double height)
        {
            return ScaleTo(joint, width, height, 1.0f, 1.0f);
        }

        private static float Scale(double maxPixel, double maxSkeleton, float position)
        {
            float value = (float)((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));

            if (value > maxPixel)
            {
                return (float)maxPixel;
            }

            if (value < 0)
            {
                return 0;
            }

            return value;
        }

        #endregion
        #region Camera

        public static ImageSource ToBitMap(this ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((PixelFormats.Bgr32.BitsPerPixel + 7) / 8)];
            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }
            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }
        #endregion
    }

}