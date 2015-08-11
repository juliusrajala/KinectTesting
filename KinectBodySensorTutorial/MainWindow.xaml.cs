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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KinectSensor sensor;
        MultiSourceFrameReader reader;
        IList<Body> bodies;

        public MainWindow()
        {
            InitializeComponent();
            InitKinect();
        }

        void InitKinect()
        {
            sensor = KinectSensor.GetDefault();
            if (sensor != null)
            {
                sensor.Open();
            }
            reader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color |
                                                        FrameSourceTypes.Depth |
                                                        FrameSourceTypes.Infrared |
                                                        FrameSourceTypes.Body);
            reader.MultiSourceFrameArrived += reader_MultiSourceFrameArrived;
        }

        void reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            
            var reference = e.FrameReference.AcquireFrame();

            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                //TODO: Do something with frame
                if (frame != null)
                {
                    camera.Source = frame.ToBitMap();
                    //canvas.Width = camera.ActualWidth;
                    //canvas.Height = camera.ActualHeight;
                }
            }

            using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                //Do something with frame
            }

            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            {
                //Do something with frame
            }

            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                //Do something with frame
                if (frame != null)
                {
                    DoThingsWithBodyFrame(frame);
                }
            }
        }

        void DoThingsWithBodyFrame(BodyFrame frame)
        {
            canvas.Children.Clear();

            bodies = new Body[frame.BodyFrameSource.BodyCount];
            
            frame.GetAndRefreshBodyData(bodies);

            foreach (Body body in bodies)
            {
                if (body != null)
                {
                    //Console.WriteLine("Body not null");
                    //canvas.DrawBody(body);

                    if (body.IsTracked)
                    {
                        Joint[] joints = {
                                             body.Joints[JointType.ThumbLeft],
                                             body.Joints[JointType.ThumbRight],
                                             body.Joints[JointType.HandLeft],
                                             body.Joints[JointType.HandRight]
                                         };
                        foreach (Joint joint in joints)
                        {
                            CameraSpacePoint jointPosition = joint.Position;

                            Point point = new Point();

                            if (joint.TrackingState == TrackingState.Tracked)
                            {
                                ColorSpacePoint colorPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);
                                point.X = float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X;
                                point.Y = float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                            }

                            Ellipse ellipse = new Ellipse
                            {
                                Fill = Brushes.IndianRed,
                                Width = 15,
                                Height = 15
                            };

                            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

                            canvas.Children.Add(ellipse);
                        }
                    }
                    #region oldTest
                    //if (body.IsTracked)
                    //{
                        
                    //    Joint handRight = body.Joints[JointType.HandRight];
                    //    CameraSpacePoint camHandRight = handRight.Position;
                    //    ColorSpacePoint hrPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(camHandRight);
                        
                    //    Joint thumbRight = body.Joints[JointType.ThumbRight];
                    //    CameraSpacePoint camThumbRight = thumbRight.Position;
                    //    ColorSpacePoint trPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(camThumbRight);

                    //    Joint handLeft = body.Joints[JointType.HandLeft];
                    //    CameraSpacePoint camHandLeft = handLeft.Position;
                    //    ColorSpacePoint hlPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(camHandLeft);

                    //    Joint thumbLeft = body.Joints[JointType.ThumbLeft];
                    //    CameraSpacePoint camThumbLeft = thumbLeft.Position;
                    //    ColorSpacePoint tlPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(camThumbLeft);
                    //    if (handLeft.TrackingState != TrackingState.Tracked) return;
                    //    if (handRight.TrackingState != TrackingState.Tracked) return;
                    //    if (thumbLeft.TrackingState != TrackingState.Tracked) return;
                    //    if (thumbRight.TrackingState != TrackingState.Tracked) return;
                    //    canvas.DrawPoint(hrPoint, 0);
                    //    canvas.DrawPoint(trPoint, 1);
                    //    canvas.DrawPoint(hlPoint, 0);
                    //    canvas.DrawPoint(tlPoint, 1);

                    //}
                    #endregion
                }
            }
        }    
    }
}
