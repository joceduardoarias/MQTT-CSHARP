using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Application2
{
    public partial class Form1 : Form
    {
        MqttClient mqttClient;
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Manejador del evento Load para el Form1. Este método inicializa el cliente MQTT,
        /// se suscribe al tópico "Application1/Message" y realiza la conexión.
        /// </summary>
        /// <param name="sender">El objeto que originó el evento.</param>
        /// <param name="e">Los argumentos del evento.</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    // Inicializa el cliente MQTT
                    InitializeMqttClient();

                    // Suscribe al tópico
                    SubscribeToTopic();

                    // Conecta el cliente MQTT
                    ConnectMqttClient();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        /// <summary>
        /// Inicializa el cliente MQTT para la comunicación. Este método se encarga de crear
        /// una nueva instancia del cliente MQTT y de suscribirse al evento para recibir mensajes publicados.
        /// </summary>
        private void InitializeMqttClient()
        {
            if (mqttClient == null)
            {
                const string MqttServerIp = "localhost";
                mqttClient = new MqttClient(MqttServerIp);
                mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
            }
        }

        /// <summary>
        /// Suscribe al tópico "Application1/Message" con un nivel de calidad de servicio de al menos una vez.
        /// </summary>
        private void SubscribeToTopic()
        {
            if (mqttClient != null)
            {
                mqttClient.Subscribe(new string[] { "Application2/Message" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            }
        }

        /// <summary>
        /// Conecta el cliente MQTT al servidor si aún no está conectado.
        /// </summary>
        private void ConnectMqttClient()
        {
            if (mqttClient != null && !mqttClient.IsConnected)
            {
                mqttClient.Connect("Application1");
            }
        }

        /// <summary>
        /// Manejador del evento MqttMsgPublishReceived para el cliente MQTT.
        /// Decodifica el mensaje recibido y lo añade al listBox1.
        /// </summary>
        /// <param name="sender">El objeto que originó el evento.</param>
        /// <param name="e">Los argumentos del evento.</param>
        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Message);

                // Validación del mensaje aquí (si es necesario)

                listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(message)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Manejador del evento Click para el botón1.
        /// Publica un mensaje al tópico "Application1/Message" a través del cliente MQTT.
        /// </summary>
        /// <param name="sender">El objeto que originó el evento.</param>
        /// <param name="e">Los argumentos del evento.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    if (mqttClient != null && mqttClient.IsConnected)
                    {
                        // Validación del contenido del mensaje aquí, si es necesario
                        mqttClient.Publish("Application1/Message", Encoding.UTF8.GetBytes(textBox1.Text));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

    }
}
