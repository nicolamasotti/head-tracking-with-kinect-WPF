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
        //Body[] bodies = new Body[6];

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
            text_block_1.Text = "HEAD COORDINATES";
            text_block_2.Text = String.Format("Sending data to IP {0}", IP);
            sensor = KinectSensor.GetDefault();
            sensor.Open();
            bfr = sensor.BodyFrameSource.OpenReader();
            bfr.FrameArrived += bfr_FrameArrived;
        }

        void bfr_FrameArrived(object sender, BodyFrameArrivedEventArgs args)
        {
            using(BodyFrame bf = args.FrameReference.AcquireFrame())
            {
                if (bf != null)
                {
                    Body[] bodies = new Body[bf.BodyCount];
                    bf.GetAndRefreshBodyData(bodies);
                    //inside the "bodies" array, search those which are tracked and, amongst them, take the first or default one
                    Body body = bodies.Where(b => b.IsTracked).FirstOrDefault();

                    //bf.GetAndRefreshBodyData(bodies);
                    //foreach (Body body in bodies)
	                //{
                        //if (body.IsTracked)
                        //{
                    if (body != null)
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

                            text_block_1.Text = String.Format("HEAD COORDINATES: \n X: {0} \n Y: {1} \n Z: {2} \n", x, y, z);

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
