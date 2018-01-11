using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using HidSharp;

namespace VoidBattery
{
    public partial class MainForm : Form
    {
        public PacketHandler Packets { get; }

        public MainForm()
        {
            InitializeComponent();

            btnExit.Click += (_, __) => Application.Exit();

            Packets = new PacketHandler();
            InitWorker();
        }

        #region UI updating

        public delegate void UpdateBatteryDelegate(int battery);

        public delegate void UpdateErrorDelegate(string error);

        public void UpdateBattery(int battery)
        {
            if (lblBattery.InvokeRequired)
            {
                lblBattery.BeginInvoke(new UpdateBatteryDelegate(UpdateBattery), battery);
                return;
            }

            lblBattery.Text = $"{battery}%";
        }

        public void UpdateError(string error)
        {
            if (lblError.InvokeRequired)
            {
                lblError.BeginInvoke(new UpdateErrorDelegate(UpdateError), error);
                return;
            }

            Debug.WriteLine($"Error: {error}");
            lblError.Text = $"Error: {error}";
        }

        #endregion

        #region Background worker

        private BackgroundWorker _worker;
        private int _vendorId = 0x1b1c;
        private int _productId = 0xa14;

        private void InitWorker()
        {
            _worker?.Dispose();

            _worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _worker.DoWork += DoWork;
            _worker.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (_worker.CancellationPending)
                e.Cancel = true;

            HidDevice headset = DeviceList.Local.GetHidDeviceOrNull(_vendorId, _productId);

            if (headset == null)
            {
                UpdateError("device not found");
                return;
            }

            using (HidStream stream = headset.Open())
            {
                stream.ReadTimeout = int.MaxValue;  // i hate this so much

                while (true)
                {
                    byte[] buf;

                    try
                    {
                        buf = stream.Read();
                    }
                    catch (TimeoutException)
                    {
                        Debug.WriteLine("Timed out. How the fuck?");
                        continue;
                    }

                    Debug.WriteLine($"Received packet: length {buf.Length}, payload {BitConverter.ToString(buf)}");

                    if (buf.Length != 5)
                        continue;

                    // indicators
                    if (buf[0] != 0x64 || buf[4] != 0x01)
                        continue;

                    int value = buf[2] & 0x7F;
                    Debug.WriteLine($"Registering new value: {value}");

                    Packets.RegisterPacket(value);
                    UpdateBattery(Packets.CurrentLevel);
                }
            }
        }


        #endregion
    }
}
