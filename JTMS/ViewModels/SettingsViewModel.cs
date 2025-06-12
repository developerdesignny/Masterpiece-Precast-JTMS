using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Data;
using JTMS.Dialogs;
using JTMS.Helpers;
using JTMS.Models;
using JTMS.ViewModels.DialogsVM;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Timers;

namespace JTMS.ViewModels
{
    class SettingsViewModel : ObservableObject
    {
        private string _connectionString = string.IsNullOrEmpty(Properties.Settings.Default.ConnectionString) ? "Server=127.0.0.1;Database=jtms;Uid=root;Pwd=pass;SslMode=None;AllowPublicKeyRetrieval=True" : Properties.Settings.Default.ConnectionString;
        public string ConnectionString
        {
            get => _connectionString; set
            {
                SetProperty(ref _connectionString, value);
            }
        }

        private string _dbVersion = string.IsNullOrEmpty(Properties.Settings.Default.DatabaseVersion) ? "8.4.0" : Properties.Settings.Default.DatabaseVersion;
        public string DBVersion
        {
            get => _dbVersion; set
            {
                SetProperty(ref _dbVersion, value);
            }
        }

        private string _port1 = Properties.Settings.Default.Port1;
        public string Port1
        {
            get => _port1; set
            {
                SetProperty(ref _port1, value);
                if (port1.IsOpen == true)
                    port1?.Close();
                setPort1();
                Properties.Settings.Default.Port1 = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _port2 = Properties.Settings.Default.Port2;
        public string Port2
        {
            get => _port2; set
            {
                SetProperty(ref _port2, value);
                if (port2.IsOpen == true)
                    port2?.Close();
                setPort2();
                Properties.Settings.Default.Port2 = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _port3 = Properties.Settings.Default.Port3;
        public string Port3
        {
            get => _port3; set
            {
                SetProperty(ref _port3, value);
                if (port3.IsOpen == true)
                    port3?.Close();
                setPort3();
                Properties.Settings.Default.Port3 = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _port4 = Properties.Settings.Default.Port4;
        public string Port4
        {
            get => _port4; set
            {
                SetProperty(ref _port4, value);
                if (port4.IsOpen == true)
                    port4?.Close();
                setPort4();
                Properties.Settings.Default.Port4 = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _port4_2 = Properties.Settings.Default.Port4_2;
        public string Port4_2
        {
            get => _port4_2; set
            {
                SetProperty(ref _port4_2, value);
                if (port4_2.IsOpen == true)
                    port4_2?.Close();
                setPort4_2();
                Properties.Settings.Default.Port4_2 = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _port5 = Properties.Settings.Default.Port5;
        public string Port5
        {
            get => _port5; set
            {
                SetProperty(ref _port5, value);
                if (port5.IsOpen == true)
                    port5?.Close();
                setPort5();
                Properties.Settings.Default.Port5 = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _port6 = Properties.Settings.Default.Port6;
        public string Port6
        {
            get => _port6; set
            {
                SetProperty(ref _port6, value);
                if (port6.IsOpen == true)
                    port6?.Close();
                setPort6();
                Properties.Settings.Default.Port6 = value;
                Properties.Settings.Default.Save();
            }
        }
        //

        private bool _portConn1 = false;
        public bool PortConn1
        {
            get => _portConn1; set
            {
                SetProperty(ref _portConn1, value);
            }
        }

        private bool _portConn2 = false;
        public bool PortConn2
        {
            get => _portConn2; set
            {
                SetProperty(ref _portConn2, value);
            }
        }

        private bool _portConn3 = false;
        public bool PortConn3
        {
            get => _portConn3; set
            {
                SetProperty(ref _portConn3, value);
            }
        }

        private bool _portConn4 = false;
        public bool PortConn4
        {
            get => _portConn4; set
            {
                SetProperty(ref _portConn4, value);
            }
        }

        private bool _portConn4_2 = false;
        public bool PortConn4_2
        {
            get => _portConn4_2; set
            {
                SetProperty(ref _portConn4_2, value);
            }
        }

        private bool _portConn5 = false;
        public bool PortConn5
        {
            get => _portConn5; set
            {
                SetProperty(ref _portConn5, value);
            }
        }

        private bool _portConn6 = false;
        public bool PortConn6
        {
            get => _portConn6; set
            {
                SetProperty(ref _portConn6, value);
            }
        }

        //
        private ObservableCollection<string> _serialPorts = new ObservableCollection<string>();
        public ObservableCollection<string> SerialPorts { get => _serialPorts; set => SetProperty(ref _serialPorts, value); }

        public RelayCommand SaveSettingsCommand { get; set; }
        public RelayCommand MigrateCMD { get; set; }
        public IRelayCommand DisconnectCMD { get; set; }

        private SerialPort port1;
        private SerialPort port2;
        private SerialPort port3;
        private SerialPort port4;
        private SerialPort port4_2;
        private SerialPort port5;
        private SerialPort port6;

        private System.Timers.Timer notificationTimer;

        public Action<string, ScanMode> SDataRecieved;

        public SettingsViewModel()
        {
            SaveSettingsCommand = new RelayCommand(saveSettings);
            MigrateCMD = new RelayCommand(migrateDB);
            DisconnectCMD = new RelayCommand<string>(port => disconnectPort(port));

            initPort(ref port1);
            initPort(ref port2);
            initPort(ref port3);
            initPort(ref port4);
            initPort(ref port4_2);
            initPort(ref port5);
            initPort(ref port6);

            port1.DataReceived += Port1_DataReceived;
            port2.DataReceived += Port2_DataReceived;
            port3.DataReceived += Port3_DataReceived;
            port4.DataReceived += Port4_DataReceived;
            port4_2.DataReceived += Port4_2_DataReceived;
            port5.DataReceived += Port5_DataReceived;
            port6.DataReceived += Port6_DataReceived;

            var ports = GetAvailablePorts();
            SerialPorts = new ObservableCollection<string>(ports);

            notificationTimer = new(3000); // 3 sec
            notificationTimer.Elapsed += NotificationTimer_Elapsed;
            notificationTimer.AutoReset = true;
            notificationTimer.Enabled = true;
        }

        private void disconnectPort(string port)
        {
            if (port == "Port1")
            {
                if (port1.IsOpen == true)
                    port1.Close();
                Port1 = string.Empty;
            }
            else if (port == "Port2")
            {
                if (port2.IsOpen == true)
                    port2.Close();
                Port2 = string.Empty;
            }
            else if (port == "Port3")
            {
                if (port3.IsOpen == true)
                    port3.Close();
                Port3 = string.Empty;
            }
            else if (port == "Port4")
            {
                if (port4.IsOpen == true)
                    port4.Close();
                Port4 = string.Empty;
            }
            else if (port == "Port4_2")
            {
                if (port4_2.IsOpen == true)
                    port4_2.Close();
                Port4_2 = string.Empty;
            }
            else if (port == "Port5")
            {
                if (port5.IsOpen == true)
                    port5.Close();
                Port5 = string.Empty;
            }
            else if (port == "Port6")
            {
                if (port6.IsOpen == true)
                    port6.Close();
                Port6 = string.Empty;
            }
        }

        private void Port4_2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                string str = string.Empty;
                while (port4_2.BytesToRead > 0)
                    str += port4_2.ReadExisting();
                SDataRecieved?.Invoke(str, ScanMode.Process4);
            });
        }

        private void NotificationTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            setPort1();
            setPort2();
            setPort3();
            setPort4();
            setPort4_2();
            setPort5();
            setPort6();
        }//

        private void setPort1()
        {
            if (string.IsNullOrEmpty(Port1))
                PortConn1 = false;
            else if (port1.IsOpen == false)
            {
                try
                {
                    port1.PortName = Port1;
                    port1.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    PortConn1 = false;
                }

                catch (InvalidOperationException args)
                {
                    PortConn1 = false;
                }

                catch (Exception args)
                {
                    PortConn1 = false;
                }
            }
            else
                PortConn1 = true;
        }//

        private void setPort2()
        {

            if (string.IsNullOrEmpty(Port2))
                PortConn2 = false;
            else if (port2.IsOpen == false)
            {
                try
                {
                    port2.PortName = Port2;
                    port2.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    PortConn2 = false;
                }

                catch (InvalidOperationException args)
                {
                    PortConn2 = false;
                }

                catch (Exception args)
                {
                    PortConn2 = false;
                }
            }
            else
                PortConn2 = true;
        }//

        private void setPort3()
        {
            if (string.IsNullOrEmpty(Port3))
                PortConn3 = false;
            else if (port3.IsOpen == false)
            {
                try
                {
                    port3.PortName = Port3;
                    port3.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    PortConn3 = false;
                }

                catch (InvalidOperationException args)
                {
                    PortConn3 = false;
                }

                catch (Exception args)
                {
                    PortConn3 = false;
                }
            }
            else
                PortConn3 = true;
        }//

        private void setPort4()
        {
            if (string.IsNullOrEmpty(Port4))
                PortConn4 = false;
            else if (port4.IsOpen == false)
            {
                try
                {
                    port4.PortName = Port4;
                    port4.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    PortConn4 = false;
                }

                catch (InvalidOperationException args)
                {
                    PortConn4 = false;
                }

                catch (Exception args)
                {
                    PortConn4 = false;
                }
            }
            else
                PortConn4 = true;
        }//

        private void setPort4_2()
        {
            if (string.IsNullOrEmpty(Port4_2))
                PortConn4_2 = false;
            else if (port4_2.IsOpen == false)
            {
                try
                {
                    port4_2.PortName = Port4_2;
                    port4_2.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    PortConn4_2 = false;
                }

                catch (InvalidOperationException args)
                {
                    PortConn4_2 = false;
                }

                catch (Exception args)
                {
                    PortConn4_2 = false;
                }
            }
            else
                PortConn4_2 = true;
        }//

        private void setPort5()
        {
            if (string.IsNullOrEmpty(Port5))
                PortConn5 = false;
            else if (port5.IsOpen == false)
            {
                try
                {
                    port5.PortName = Port5;
                    port5.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    PortConn5 = false;
                }

                catch (InvalidOperationException args)
                {
                    PortConn5 = false;
                }

                catch (Exception args)
                {
                    PortConn5 = false;
                }
            }
            else
                PortConn5 = true;
        }//

        private void setPort6()
        {
            if (string.IsNullOrEmpty(Port6))
                PortConn6 = false;
            else if (port6.IsOpen == false)
            {
                try
                {
                    port6.PortName = Port6;
                    port6.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    PortConn6 = false;
                }

                catch (InvalidOperationException args)
                {
                    PortConn6 = false;
                }

                catch (Exception args)
                {
                    PortConn6 = false;
                }
            }
            else
                PortConn6 = true;
        }//

        public List<string> GetAvailablePorts()
        {
            var ports = new List<string>();
            SerialPort.GetPortNames().ToList<string>().ForEach((str) =>
            {
                if (!string.IsNullOrEmpty(str) && ports.Contains(str) == false)
                    ports.Add(str);
            });
            return ports;
        }

        private void Port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                string str = string.Empty;
                while (port1.BytesToRead > 0)
                    str += port1.ReadExisting();
                SDataRecieved?.Invoke(str, ScanMode.Process1);
            });
        }
        private void Port2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                string str = string.Empty;
                while (port2.BytesToRead > 0)
                    str += port2.ReadExisting();
                SDataRecieved?.Invoke(str, ScanMode.Process2);
            });
        }
        private void Port3_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                string str = string.Empty;
                while (port3.BytesToRead > 0)
                    str += port3.ReadExisting();
                SDataRecieved?.Invoke(str, ScanMode.Process3);
            });
        }
        private void Port4_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                string str = string.Empty;
                while (port4.BytesToRead > 0)
                    str += port4.ReadExisting();
                SDataRecieved?.Invoke(str, ScanMode.Process4);
            });
        }
        private void Port5_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                string str = string.Empty;
                while (port5.BytesToRead > 0)
                    str += port5.ReadExisting();
                SDataRecieved?.Invoke(str, ScanMode.Process5);
            });
        }
        private void Port6_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                string str = string.Empty;
                while (port6.BytesToRead > 0)
                    str += port6.ReadExisting();
                SDataRecieved?.Invoke(str, ScanMode.Process6);
            });
        }

        private void initPort(ref SerialPort port)
        {
            port = new SerialPort();
            port.BaudRate = 9600;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.ReadTimeout = 10000;
            port.WriteTimeout = 10000;
        }

        private async void migrateDB()
        {
            try
            {
                var context = new JTMSContext();
                context.Database.Migrate();
                await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Database migrations applied" }, "RootDialogHost");
            }
            catch (Exception ex) { new MessageWin("error", ex.Message); }
        }

        private async void saveSettings()
        {
            Properties.Settings.Default.ConnectionString = ConnectionString;
            Properties.Settings.Default.DatabaseVersion = DBVersion;
            Properties.Settings.Default.Save();
            WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Settings-Updated" });
            await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Settings Saved" }, "RootDialogHost");
        }
    }
}
