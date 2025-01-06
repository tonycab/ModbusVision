using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusVisionLib.Modbus.Protocol;
using ModbusVisionLib.Modbus.IO;
using System.Globalization;
using System.Threading;
using System.ComponentModel;


namespace ModbusVision
{
	public class ExempleModbus 
	{

		#region properties public

		public event Action<int, string, string> EvtTrigger2D;
		public event Action<string, string, string, bool> EvtTrigger3D;
		public event Action<string, string, string> EvtTrigger3Drapide;
		public event Action<string, string, string> EvtDataTrigger;
		public event Action<string, string> EvtLoadCampaign;
		public event Action EvtLoadFreeRunStart;
		public event Action EvtLoadFreeRunStop;
		public event Action<string[]> EvtCalibPose;
		public event Action EvtCalibStart;
		public event Action EvtCalibStop;
		public event Action<string, string> EvtReconstruct3DStart;
		public event Action<string, string> EvtReconstructInPos;
		public event Action EvtReconstructStop;
		public event Action<string, string, string, string[]> EvtReconstructAdd;
		public event Action<string, string, string> EvtCalibAxeExtInPos;
		public event Action EvtCalibAxeExtStart;
		public event Action EvtCalibAxeExtStop;
		public event Action EvtConnectCam;
		public event Action EvtDisconnectCam;
		public event Action EvtGetTempPhoxi;
		public event Action EvtGetVisionState;
		public event Action EvtStateAcknoledgement;
		public event Action EvtRepetabiliteStart;
		public event Action EvtRepetabiliteStop;
		public event Action EvtBackgroundLearning;
		public event Action<string, string, string, string[]> EvtTrigger3DTable;
		public event Action<string[]> EvtConcat3DStart;
		public event Action<string[]> EvtConcat3DAdd;
		public event Action EvtConcat3DStop;
		public event Action<int, string, string> EvtInspectRegions;

		public bool OfflineMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		/// <summary>
		/// Signaux d'entrées
		/// </summary>
		public ThreadedBindingList<SignalInput> Inputs { get; set; }

		/// <summary>
		/// Signaux de sorties
		/// </summary>
		public ThreadedBindingList<Signal> Outputs { get; set; }

		/// <summary>
		/// Changement d'état de la connection Modbus
		/// </summary>
		public event Action<object, ModbusTCPMasterEventArgs> ModbusMasterStateEventArgs;


		/// <summary>
		/// Informe le PLC que la caméra 1 est en défaut
		/// </summary>
		public bool FaultCam1 { get => mb["DefCam1"].State == 1 ? true : false; set => mb["DefCam1"].State = value ? 1 : 0; }


		/// <summary>
		/// Informe le PLC que la caméra 2 est en défaut
		/// </summary>
		public bool FaultCam2 { get => mb["DefCam2"].State == 1 ? true : false; set => mb["DefCam2"].State = value ? 1 : 0; }

		/// <summary>
		/// Taille de l'outil attaque de coulé
		/// </summary>
		public uint SizeToolAttaque { get => (uint)mb["SizeToolAttaque"].State; }

		/// <summary>
		/// Décalage des attaques de coulées
		/// </summary>
		public uint DecalageAttaque { get => (uint)mb["DecalageAttaque"].State; }

		public string IP
		{
			get
			{
				return mb?.IPadressPlc;
			}
			set
			{
				if (mb != null)
				{
					mb.IPadressPlc = value;
				}
			}
		}

		public int Port
		{
			get
			{
				return mb.PortPlc;
			}
			set
			{
				if (mb != null)
				{
					mb.PortPlc = value;
				}
			}
		}



        public ushort StartInputPlc
        {
            get
            {
                return mb.StartRegisterInput;
            }
            set
            {
                if (mb != null)
                {
                    mb.StartRegisterInput = value;
                }
            }
        }

        public ushort StartOutputPlc
        {
            get
            {
                return mb.StartRegisterInput;
            }
            set
            {
                if (mb != null)
                {
                    mb.StartRegisterInput = value;
                }
            }
        }

		public int GetNumTable
		{
			get {
				return mb["NumTable"].State;
			} 
		}



        #endregion

        #region Properties private

        private ModbusTCPMaster mb ;
		private bool faultCam2;


		#endregion

		#region Constructor
		/// <summary>
		/// Constructeur d'une instance Modbus pour l'affaire Le Creuset
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <param name="logger"></param>
		public ExempleModbus(string ip, int port,ushort startInput, ushort startOutput)
		{

			CultureInfo.CurrentCulture = new CultureInfo("fr-FR", false);

			mb = new ModbusTCPMaster(ip, port, startInput, 120, startOutput, 120);

			//IP = ip;
			//Port = port;

			//StartInputPlc = startInput;
			//StartOutputPlc = startOutput;

            //Evennement changement d'etat de la connection Modbus
            mb.ModbusMasterStateEventArgs += ModbusMasterStateEvent;

			Inputs = mb.Inputs;
			Outputs = mb.Outputs;

			//-----------------------------------GENERALITE-----------------------------------------

			//Définition des entrées digital (Generalité)
			mb.AddSignalInput(new DigitalInputSignal("Life", "Bit de vie communication", 0, 0));

			//Définition des sorties digital (Generalité)
			mb.AddSignalOutput(new DigitalOutputSignal("LifeEcho", "Echo bit de vie communication", 0, 0));
			mb.AddSignalOutput(new DigitalOutputSignal("DefCam1", "Defaut camera 1", 0, 1));
			mb.AddSignalOutput(new DigitalOutputSignal("DefCam2", "Defaut camera 1", 0, 2));

			//------------------------------------CAMERA 1------------------------------------------

			//Définition des entrées digital (Caméra 1)
			mb.AddSignalInput(new DigitalInputSignal("TriggCam1", "Trigger camera 1", 2, 0));
			mb.AddSignalInput(new DigitalInputSignal("AcqDataCam1", "Acq récepetion données", 2, 1));

			//Définition des sortie digital (Caméra 1)
			mb.AddSignalOutput(new DigitalOutputSignal("EndTrigCam1", "Trigger terminé camera 1", 2, 0));
			mb.AddSignalOutput(new DigitalOutputSignal("DataCam1", "Données prêtes", 2, 1));


			//Définition des entrées groups (Caméra 1)
			mb.AddSignalInput(new GroupInputSignal("TicketCam1", "Numero de ticket camera 1", 3, TypeSize.UINT));
			mb.AddSignalInput(new GroupInputSignal("ProgCam1", "Numero de programme camera 1", 4, TypeSize.UINT));
			mb.AddSignalInput(new GroupInputSignal("ModCam1", "Numero de model camera 1", 5, TypeSize.UINT));


			//Définition des sorties groups (Caméra 1)
			mb.AddSignalOutput(new GroupOutputSignal("DataTicketCam1", "Data Numero de ticket camera 1", 3, TypeSize.UINT));
			mb.AddSignalOutput(new GroupOutputSignal("DataPrgCam1", "Data Numero de programme camera 1", 4, TypeSize.UINT));
			mb.AddSignalOutput(new GroupOutputSignal("DataModelCam1", "Data Numero de modele camera 1", 5, TypeSize.UINT));
			mb.AddSignalOutput(new GroupOutputSignal("DataStateCam1", "Data state camera 1", 6, TypeSize.UINT));
			mb.AddSignalOutput(new GroupOutputSignal("DataScoreCam1", "Data score camera 1", 7, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataXCam1", "Data position X camera 1", 8, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataYCam1", "Data position Y camera 1", 9, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataZCam1", "Data position Z camera 1", 10, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataRxCam1", "Data position Rx camera 1", 11, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataRyCam1", "Data position Ry camera 1", 12, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataRzCam1", "Data position Rz camera 1", 13, TypeSize.INT));

			//------------------------------------CAMERA 2------------------------------------------

			//Définition des entrées digital (Caméra 2)
			mb.AddSignalInput(new DigitalInputSignal("TriggCam2", "Trigger camera 2", 22, 0));
			mb.AddSignalInput(new DigitalInputSignal("AcqDataCam2", "Acq récepetion données", 22, 1));

			//Définition des sortie digital (Caméra 2)
			mb.AddSignalOutput(new DigitalOutputSignal("EndTrigCam2", "Trigger terminé camera 2", 22, 0));
			mb.AddSignalOutput(new DigitalOutputSignal("DataCam2", "Données prêtes", 22, 1));

			//Définition des entrées groups (Caméra 2)
			mb.AddSignalInput(new GroupInputSignal("TicketCam2", "Numero de ticket camera 2", 23, TypeSize.UINT));
			mb.AddSignalInput(new GroupInputSignal("ProgCam2", "Numero de programme camera 2", 24, TypeSize.UINT));
			mb.AddSignalInput(new GroupInputSignal("ModCam2", "Numero de model camera 2", 25, TypeSize.UINT));

			//Définition des sorties groups (Caméra 2)
			mb.AddSignalOutput(new GroupOutputSignal("DataTicketCam2", "Data Numero de ticket camera 2", 23, TypeSize.UINT));
			mb.AddSignalOutput(new GroupOutputSignal("DataPrgCam2", "Data Numero de programme camera 2", 24, TypeSize.UINT));
			mb.AddSignalOutput(new GroupOutputSignal("DataModelCam2", "Data Numero de modele camera 2", 25, TypeSize.UINT));
			mb.AddSignalOutput(new GroupOutputSignal("DataStateCam2", "Data state camera 2", 26, TypeSize.UINT));
			mb.AddSignalOutput(new GroupOutputSignal("DataScoreCam2", "Data score camera 2", 27, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataXCam2", "Data position X camera 2", 28, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataYCam2", "Data position Y camera 2", 29, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataZCam2", "Data position Z camera 2", 30, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataRxCam2", "Data position Rx camera 2", 31, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataRyCam2", "Data position Ry camera 2", 32, TypeSize.INT));
			mb.AddSignalOutput(new GroupOutputSignal("DataRzCam2", "Data position Rz camera 2", 33, TypeSize.INT));


			//Attaque de coulées
			//Définition des entrées groups
			mb.AddSignalInput(new GroupInputSignal("SizeToolAttaque", "Taille outil attaque", 41, TypeSize.UINT));
			mb.AddSignalInput(new GroupInputSignal("DecalageAttaque", "Distance décalage attaque", 42, TypeSize.UINT));


            mb.AddSignalInput(new GroupInputSignal("NumTable", "NumeroTable", 43, TypeSize.UINT));

            //Définition des entrées groups (Caméra 2)
            mb.AddSignalOutput(new GroupOutputSignal("NumberAttaque", "Nnombre d'attaque", 41, TypeSize.UINT));


			for (int i = 0; i < 20; i++)
			{
				mb.AddSignalOutput(new GroupOutputSignal("X_Attaque" + (i + 1), "Data Coordonnée X - Attaque " + (i + 1), 42 + (uint)i * 3, TypeSize.INT));
				mb.AddSignalOutput(new GroupOutputSignal("Y_Attaque" + (i + 1), "Data Coordonnée Y - Attaque " + (i + 1), 43 + (uint)i * 3, TypeSize.INT));
				mb.AddSignalOutput(new GroupOutputSignal("Z_Attaque" + (i + 1), "Data Coordonnée Z - Attaque " + (i + 1), 44 + (uint)i * 3, TypeSize.INT));
			}

			//Bit de vie communication
			mb["Life"].SignalChanged += (s) =>
			{
				mb["LifeEcho"].State = s.State;
			};

			//Trigger camera 1
			mb["TriggCam1"].SignalChanged += (s) =>
			{
				Task.Run(() =>
				{
					if (s.State == 1)
					{
                        //if (mb["TicketCam1"].State > 1 && mb["TicketCam1"].State < 10000) return;
                        mb["DataCam1"].State = 0;

                        EvtTrigger3DTable?.Invoke(mb["TicketCam1"].State.ToString(), mb["ProgCam1"].State.ToString(), mb["ModCam1"].State.ToString(), new string[] { "Cam1" });
						mb["EndTrigCam1"].State = 1;
					}
					else
					{
						mb["EndTrigCam1"].State = 0;
					}
				});

			};

			//Acq data caméra 1
			mb["AcqDataCam1"].SignalChanged += (s) =>
			{
				if (s.State == 1)
				{

					mb["DataTicketCam1"].State = 0;
					mb["DataPrgCam1"].State = 0;
					mb["DataModelCam1"].State = 0;
					mb["DataStateCam1"].State = 0;
					mb["DataScoreCam1"].State = 0;
					mb["DataXCam1"].State = 0;
					mb["DataYCam1"].State = 0;
					mb["DataZCam1"].State = 0;
					mb["DataRxCam1"].State = 0;
					mb["DataRyCam1"].State = 0;
					mb["DataRzCam1"].State = 0;

					mb["DataCam1"].State = 0;
				}
			};

			//Trigger camera 2
			mb["TriggCam2"].SignalChanged += (s) =>
			{
				Task.Run(() =>
				{
					if (s.State == 1)
					{
                        //if (mb["TicketCam2"].State > 20000 && mb["TicketCam2"].State < 30000) return;
                        mb["DataCam2"].State = 1;
                        EvtTrigger3DTable?.Invoke(mb["TicketCam2"].State.ToString(), mb["ProgCam2"].State.ToString(), mb["ModCam2"].State.ToString(), new string[] { "Cam2" });
						mb["EndTrigCam2"].State = 1;
					}
					else
					{
						mb["EndTrigCam2"].State = 0;
					}
				});
			};

			//Acq data caméra 2
			mb["AcqDataCam2"].SignalChanged += (s) =>
			{
				if (s.State == 1)
				{
					mb["DataTicketCam2"].State = 0;
					mb["DataPrgCam2"].State = 0;
					mb["DataModelCam2"].State = 0;
					mb["DataStateCam2"].State = 0;
					mb["DataScoreCam2"].State = 0;
					mb["DataXCam2"].State = 0;
					mb["DataYCam2"].State = 0;
					mb["DataZCam2"].State = 0;
					mb["DataRxCam2"].State = 0;
					mb["DataRyCam2"].State = 0;
					mb["DataRzCam2"].State = 0;

					for (int i = 0; i < 20; i++)
					{
						mb["X_Attaque" + (i + 1)].State = 0;
						mb["Y_Attaque" + (i + 1)].State = 0;
						mb["Z_Attaque" + (i + 1)].State = 0;
					}


					mb["DataCam2"].State = 0;
				}
			};

		}

		#endregion

		#region Methode mublic

		public void Start()
		{
			mb.Start();
		}

		public void Stop()
		{
			mb.Stop();
		}

		public void SendMessage(string message)
		{
			string[] m = message.Split(';');

			//Camera 1
			if (m[0] == "/trigger" && int.Parse(m[1]) > 1 && int.Parse(m[1]) < 10000)
			{
				try
				{
					mb["DataTicketCam1"].State = int.Parse(m[1]);
					mb["DataPrgCam1"].State = int.Parse(m[2]);
					mb["DataModelCam1"].State = int.Parse(m[3]);
					mb["DataStateCam1"].State = m[4] == "PASS" ? 1 : 0;
					//mb["DataStateCam1"].State = int.Parse(m[4]);
					mb["DataScoreCam1"].State = (int)(double.Parse(m[5]) * 10);

					NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = ".";

					mb["DataXCam1"].State = (int)(double.Parse(m[6]) * 100);
					mb["DataYCam1"].State = (int)(double.Parse(m[7]) * 100);
					mb["DataZCam1"].State = (int)(double.Parse(m[8]) * 100);
					mb["DataRxCam1"].State = (int)(double.Parse(m[9]) * 100);
					mb["DataRyCam1"].State = (int)(double.Parse(m[10]) * 100);
					mb["DataRzCam1"].State = (int)(double.Parse(m[11]) * 100);

					mb["DataCam1"].State = 1;
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e.Message);

				}
			}


			//Camera 2
			if (m[0] == "/trigger" && int.Parse(m[1]) > 20000 && int.Parse(m[1]) < 30000)
			{
				mb["DataTicketCam2"].State = int.Parse(m[1]);
				mb["DataPrgCam2"].State = int.Parse(m[2]);
				mb["DataModelCam2"].State = int.Parse(m[3]);

				mb["DataStateCam2"].State = m[4] == "PASS" ? 1 : 0;
				//mb["DataStateCam2"].State = int.Parse(m[4]);
				mb["DataScoreCam2"].State = (int)(double.Parse(m[5]) * 10);

				NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = ".";

				mb["DataXCam2"].State = (int)(double.Parse(m[6]) * 100);
				mb["DataYCam2"].State = (int)(double.Parse(m[7]) * 100);
				mb["DataZCam2"].State = (int)(double.Parse(m[8]) * 100);
				mb["DataRxCam2"].State = (int)(double.Parse(m[9]) * 100);
				mb["DataRyCam2"].State = (int)(double.Parse(m[10]) * 100);
				mb["DataRzCam2"].State = (int)(double.Parse(m[11]) * 100);


				mb["NumberAttaque"].State = int.Parse(m[12]);
				for (int i = 0; i < int.Parse(m[12]); i++)
				{
					mb["X_Attaque" + (i + 1)].State = (int)(double.Parse(m[13 + (uint)i * 3]) * 10);
					mb["Y_Attaque" + (i + 1)].State = (int)(double.Parse(m[14 + (uint)i * 3]) * 10);
					mb["Z_Attaque" + (i + 1)].State = (int)(double.Parse(m[15 + (uint)i * 3]) * 10);
				}

				mb["DataCam2"].State = 1;

			}


		}

		#endregion

		#region Methide private

		private void ModbusMasterStateEvent(object arg1, ModbusTCPMasterEventArgs arg2)
		{
			ModbusMasterStateEventArgs?.Invoke(arg1, arg2);
		}
		#endregion

	}
}

