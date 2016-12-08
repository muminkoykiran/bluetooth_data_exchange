using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace Csharp_Android_Arasi_Bluetooth_Veri_Alisverisi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SerialPort serialport = new SerialPort();

        public delegate void AddDataDelegate(String myString);
        public AddDataDelegate myDelegate;

        private void btnBringPort_Click(object sender, EventArgs e)
        {
            /* Açık Serial Portları Combobox içerisine yerleştiriyoruz 
            ve varsayılan olarak ilk portnu seçiyoruz */
            foreach (string port in SerialPort.GetPortNames())
            {
                cmbSerialPort.Items.Add(port);
            }
            cmbSerialPort.SelectedIndex = 0;

            /* Tüm olabilecek iletişim hızlarını combobox içerisine yerleştiriyoruz 
            ve uygun olan varsayılan olarak 9600 seçiyoruz */
            cmbBaudRate.Items.Add("1200");
            cmbBaudRate.Items.Add("2400");
            cmbBaudRate.Items.Add("4800");
            cmbBaudRate.Items.Add("9600");
            cmbBaudRate.Items.Add("19200");
            cmbBaudRate.Items.Add("38400");
            cmbBaudRate.Items.Add("57600");
            cmbBaudRate.Items.Add("115200");
            cmbBaudRate.Items.Add("230400");
            cmbBaudRate.SelectedIndex = 3;

            //Stop bit ve Party değerlerini combobox içerisine alıp varsılan uygun değerleri seçiyoruz
            cmbStopBits.DataSource = Enum.GetValues(typeof(StopBits));
            cmbStopBits.SelectedIndex = 0;
            cmbParity.DataSource = Enum.GetValues(typeof(Parity));
            cmbStopBits.SelectedIndex = 1;

            //Data Bit değerlerin combobox içerisine ekliyoruz ve varsayılan olarak 8bit seçiyoruz
            cmbDataBits.Items.Add("4");
            cmbDataBits.Items.Add("5");
            cmbDataBits.Items.Add("6");
            cmbDataBits.Items.Add("7");
            cmbDataBits.Items.Add("8");
            cmbDataBits.SelectedIndex = 4;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (serialport.IsOpen)
            {
                serialport.Close();
                btnConnect.Text = "Bağlan";

                txtSend.Enabled = false;
                txtReceived.Enabled = false;
                btnSendData.Enabled = false;
                btnClear.Enabled = false;
            }
            else if(!serialport.IsOpen)
            {
                //Gerekli Olan Ayarları Tanımlıyoruz
                serialport.PortName = cmbSerialPort.SelectedItem.ToString();
                serialport.BaudRate = Convert.ToInt16(cmbBaudRate.SelectedItem);
                serialport.StopBits = (StopBits)cmbStopBits.SelectedItem;
                serialport.Parity = (Parity)cmbParity.SelectedItem;
                serialport.DataBits = Convert.ToInt16(cmbDataBits.SelectedItem);

                try
                {
                    //Serial Port bağlantısını açıyoruz
                    serialport.Open();
                    btnConnect.Text = "Bağlantıyı Kes";
                    myDelegate = new AddDataDelegate(AddDataMethod);
                    serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                    txtSend.Enabled = true;
                    txtReceived.Enabled = true;
                    btnSendData.Enabled = true;
                    btnClear.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("Bağlantı Kurulmadı: \n" + serialport.PortName.ToString() + " " + serialport.BaudRate.ToString());
                    return;
                };
            }
        }
        public void AddDataMethod(String myString)
        {
            txtReceived.AppendText(myString);
        }
        private void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string s = sp.ReadExisting();
            txtReceived.Invoke(myDelegate, new Object[] { s });
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceived.Text = "";
        }
        private void btnSendData_Click(object sender, EventArgs e)
        {
            serialport.WriteLine(txtSend.Text);
            txtSend.Text = "";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            txtSend.Enabled = false;
            txtReceived.Enabled = false;
            btnSendData.Enabled = false;
            btnClear.Enabled = false;
        }
    }
}
