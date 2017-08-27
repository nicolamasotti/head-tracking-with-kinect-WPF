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
using System.Net;
using System.Net.Sockets;

namespace Head_tracking_WPF_application
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        BodyFrameReader bfr;
        Body[] bodies = new Body[6];

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPAddress IP = IPAddress.Loopback;
        IPEndPoint ep;

        public MainWindow()
        {
            InitializeComponent();

            ep = new IPEndPoint(IP, 9999);

            this.Loaded += MainWindow_Loaded;
        }
        
        void MainWindow_Loaded(object sender, RoutedEventArgs args)
        {          
            //throw new NotImplementedException();
            console.Text = "Your head coordinates are:";
            sensor = KinectSensor.GetDefault();
            sensor.Open();
            bfr = sensor.BodyFrameSource.OpenReader();
            bfr.FrameArrived += bfr_FrameArrived;
        }

        void bfr_FrameArrived(object sender, BodyFrameArrivedEventArgs args)
        {
            //throw new NotImplementedException();
            using(BodyFrame bf = args.FrameReference.AcquireFrame())
            {
                if (bf != null)
                {
                    bf.GetAndRefreshBodyData(bodies);
                    foreach (Body body in bodies)
	                {
                        if (body.IsTracked)
                        {
                            Joint headJoint = body.Joints[JointType.Head];
                            if (headJoint.TrackingState == TrackingState.Tracked)
                            {
                                Single X = headJoint.Position.X;
                                Single Y = headJoint.Position.Y;
                                Single Z = headJoint.Position.Z;
   
                                string x = X.ToString("0.00000000");
                                string y = Y.ToString("0.00000000");
                                string z = Z.ToString("0.00000000");

                                console.Text = String.Format("Your head coordinates are: \n X: {0} \n Y: {1} \n Z: {2} \n", x, y, z);

                                string message = x + "\n" + y + "\n" + z;
                                byte[] stream = Encoding.UTF8.GetBytes(message);
                                socket.SendTo(stream, ep);
                            }
                        }
	                }
                }
            }
        }
    }
}
