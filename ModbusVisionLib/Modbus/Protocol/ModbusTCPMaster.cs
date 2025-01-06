
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using System.ComponentModel;
using System.Collections;
using ModbusVisionLib.Modbus.IO;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;

namespace ModbusVisionLib.Modbus.Protocol
{
 
	
	public class ModbusTCPMaster : Dictionary<string, Signal>
	{
		#region Properties public

		/// <summary>
		/// Liste des signaux d'entrées
		/// </summary>
		public ThreadedBindingList<SignalInput> Inputs { get; set; }

		/// <summary>
		/// Liste des signaux de sorties
		/// </summary>
		public ThreadedBindingList<Signal> Outputs { get; set; }


		/// <summary>
		/// Adresse IP du PLC
		/// </summary>
		public string IPadressPlc { get; set; }


		/// <summary>
		/// Port de communication du PLC
		/// </summary>
		public int PortPlc { get; set; }

		/// <summary>
		/// Timeout de connection
		/// </summary>
		public int TimeOutConnection { get; set; } = 1000;

		/// <summary>
		/// %MW de départ des signaux d'entrées
		/// </summary>
		public ushort StartRegisterInput { get; set; }

		/// <summary>
		/// %MW de départ des signaux de sorties
		/// </summary>
		public ushort StartRegisterOutput { get; set; }

		/// <summary>
		/// Nombre de %MW des siganux d'entrées
		/// </summary>
		public  int  SizeRegisterInput { get; set; }
		/// <summary>
		/// Nombre de %MW des signaux de sorties
		/// </summary>
		public int SizeRegisterOutput { get; set; }

		/// <summary>
		/// Evenement de changement d'état de la connection Modbus
		/// </summary>
		public event Action<object, ModbusTCPMasterEventArgs> ModbusMasterStateEventArgs;

		#endregion

		#region Properties private

		private ushort[] RegistreInput;

		private ushort[] RegisterOutput;

		private TcpClient tcpClient;

		private IModbusMaster masterModbus;

		private ModbusFactory fmodbus;

		private CancellationTokenSource cancellationToken;

		#endregion

		#region Constructor 
		/// <summary>
		/// Constructeur d'une instance de communication modbus TCP master
		/// </summary>
		/// <param name="ipAddressPlc">Adresse IP du PLC</param>
		/// <param name="portPlc">Port de communication du PLC</param>
		/// <param name="startRegisterInput">%MW de départ des signaux d'entrées</param>
		/// <param name="sizeRegisterInput">%MW de départ des signaux de sorties</param>
		/// <param name="startRegisterOutput">Nombre de %MW des siganux d'entrées</param>
		/// <param name="sizeRegisterOutput">Nombre de %MW des signaux de sorties</param>
		public ModbusTCPMaster( string ipAddressPlc, int portPlc, ushort startRegisterInput = 0, int sizeRegisterInput = 120, ushort startRegisterOutput = 128, int sizeRegisterOutput = 120)
		{

			fmodbus = new ModbusFactory();

			IPadressPlc = ipAddressPlc;
			PortPlc = portPlc;

			StartRegisterInput = startRegisterInput;
			StartRegisterOutput = startRegisterOutput;

			SizeRegisterInput = sizeRegisterInput;
			SizeRegisterOutput = sizeRegisterOutput;

			RegistreInput = new ushort[SizeRegisterInput];
			RegisterOutput = new ushort[SizeRegisterOutput];

			Inputs = new ThreadedBindingList<SignalInput>();
			Outputs = new ThreadedBindingList<Signal>();

		}

		#endregion

		#region Add/Remove signals

		/// <summary>
		/// Ajoute un signal d'entrée
		/// </summary>
		/// <param name="signalInput">Signal d'entrée</param>
		public void AddSignalInput(SignalInput signalInput)
		{
			this.Add(signalInput.Name, signalInput);

			Inputs.Add(signalInput);
		}

		/// <summary>
		/// Ajoute un signal de sortie
		/// </summary>
		/// <param name="signalOutput">Signal de sortie</param>
		public void AddSignalOutput(SignalOutput signalOutput)
		{
			this.Add(signalOutput.Name, signalOutput);
			Outputs.Add(signalOutput);
		}

		/// <summary>
		/// Supprime un signal d'entrée
		/// </summary>
		/// <param name="name"></param>
		public void RemoveSignalInput(string name)
		{
			Inputs.Remove((SignalInput)this[name]);
			this.Remove(name);
		}

		/// <summary>
		/// Supprime un signal de sortie
		/// </summary>
		/// <param name="name">Signal de sortie</param>
		public void RemoveSignalOutput(string name)
		{
			Outputs.Remove((SignalOutput)this[name]);
			this.Remove(name);
		}

		#endregion

		#region Start /Stop
		/// <summary>
		/// Lance la connection modbus
		/// </summary>
		public void Start()
		{
			cancellationToken = new CancellationTokenSource();
			StartModbus();
		}
		/// <summary>
		/// Stop la connection Modbus
		/// </summary>
		public void Stop()
		{
			cancellationToken?.Cancel();
		}

		#endregion

		#region Private Methode

		//Connection a l'automate
		private bool ConnectionClient()
		{
			try
			{
				ModbusMasterStateEventArgs?.Invoke(this, new ModbusTCPMasterEventArgs(StateConnect.Connecting, IPadressPlc, PortPlc));

				tcpClient.Connect(IPadressPlc, 502);

				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		//Cycle de connection et d'échange PC et PLC
		private void StartModbus()
		{
			Task.Run(() =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					try
					{
						//Création d'une instance Client
						tcpClient = new TcpClient();


						//Connection au PLC
						while (!ConnectionClient() & !cancellationToken.IsCancellationRequested)
						{
							ModbusMasterStateEventArgs?.Invoke(this, new ModbusTCPMasterEventArgs(StateConnect.NoClientExist, IPadressPlc, PortPlc));
							Thread.Sleep(5000);
						}

						//Etat connecté
						if (!cancellationToken.IsCancellationRequested)
						{
							System.Diagnostics.Debug.WriteLine("Connection success :" + tcpClient.Client.RemoteEndPoint.ToString());

							ModbusMasterStateEventArgs?.Invoke(this, new ModbusTCPMasterEventArgs(StateConnect.Connected, IPadressPlc, PortPlc));
						}
						

						//Creation d'une instance Modbus Master TPC
						masterModbus = fmodbus.CreateMaster(tcpClient);
						masterModbus.Transport.ReadTimeout = TimeOutConnection;


						//Boucle de lecture / écriture sur le PLC
						while (!cancellationToken.IsCancellationRequested)
						{

							//Lecture des signaus d'entrée
							RegistreInput = masterModbus.ReadHoldingRegisters(1, StartRegisterInput, (ushort)RegistreInput.Length);
							foreach (SignalInput signal in Inputs)
							{
								signal.SetSignal(RegistreInput);
							}

                            foreach (SignalInput signal in Inputs)
                            {
                                signal.CallEventStateChanged();
                            }



                            //Ecriture des signaux de sortie
                            foreach (SignalOutput signal in Outputs)
							{
								signal.SetRegister(RegisterOutput);
							}
							masterModbus.WriteMultipleRegisters(1, StartRegisterOutput, RegisterOutput);

							Thread.Sleep(100);
						}

						//Connection arrêter
						ModbusMasterStateEventArgs?.Invoke(this, new ModbusTCPMasterEventArgs(StateConnect.Stopped, IPadressPlc, PortPlc));

						masterModbus.Dispose();
						tcpClient.Close();
					}

					catch (Exception ex)
					{
						//Deconnection
						ModbusMasterStateEventArgs?.Invoke(this, new ModbusTCPMasterEventArgs(StateConnect.Disconnected, IPadressPlc, PortPlc));
						tcpClient.Close();
						tcpClient.Dispose();

					}
				
				}
			});
		} 

	}

	#endregion

}

