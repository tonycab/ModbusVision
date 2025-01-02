using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModbusVisionLib;
using ModbusVisionLib.Modbus.Protocol;
using ModbusVisionLib.Modbus.IO;
using ModbusVisionLib.Modbus.LeCreuset;
using System.Threading;
using System.Globalization;

namespace ModbusVision
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		LeCreusetModbus mb;
		private void Form1_Load(object sender, EventArgs e)
		{
            //Simulation envois de données aléatoirement cohérente
            Random random = new Random(10);

            //Log démarrage application
            LogsManager.Add(EnumCategory.Info, "Application", "Demarrage application");

			//Initialisation datagridvew3 affichage des logs
			dataGridView3.DataSource = LogsManager.GetLogs();
			dataGridView3.Columns[0].Width = 200;
			dataGridView3.Columns[1].Width = 150;
			dataGridView3.Columns[2].Width = 150;
			dataGridView3.RowsAdded += MiseEnFormeLogs;

			//Rémanence des logs dans le datagridvieuw
			dataGridView3.RowsAdded += (o, s) =>
			{
				dataGridView3.FirstDisplayedScrollingRowIndex = dataGridView3.RowCount - 1;
			};

			//Intanciation d'une communication Modbus avec le PLC
			mb = new LeCreusetModbus("10.101.53.35", 502,21128,21000);

			//Abonnement évennement changement d'état de la connection Modbus
			mb.ModbusMasterStateEventArgs += StateModbus;

			//Mise en forme des Rows du datagridvieuw1 et 2 Affichage des signaux
			dataGridView1.RowsAdded += MiseEnForme;
			dataGridView2.RowsAdded += MiseEnForme;

			//Databinding des signaux entrées et sorties Modbus
			dataGridView1.DataSource = mb.Inputs;
			dataGridView2.DataSource = mb.Outputs;


			//Détection d'une demande de trigg du PLC
			mb.EvtTrigger3DTable +=  (s1, s2, s3, s4) =>
			{

				//Trigg pour la caméra 1
				if (s4[0] == "Cam1")
				{
					//Thead camera 1
					Task t = Task.Run(() =>
					 {

						 //Acquistion de l'image
						 this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 1", "Demande de trigger camera 1 "); }));

						 //Temps de simulation d'acquisition d'image
						 Thread.Sleep(2000);

						 this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 1", "Trigger terminér camera 1"); }));
						 //Fin d'acquisition de l'image

						 //Traitement de l'image dans un autre Thread parallèle
						 Task.Run(() =>
								{

									this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 1", "Traitement en cours camera 1"); }));

									Thread.Sleep(2000);

									this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 1", "Données prêtes camera 1"); }));

								

									string state = random.RandonRangeInt(0, 5) > 1 ? "PASS" : "FAIL";

									NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = ".";
									double score = 0;
									double x = 0;
									double y = 0;
									double z = 0;
									double rx = 0;
									double ry = 0;
									double rz = 0;

									if (state == "PASS")
									{
										score = random.RandonRangeDouble(30, 90);
										x = random.RandonRangeDouble(-150, 150);
										y = random.RandonRangeDouble(-150, 150);
										z = random.RandonRangeDouble(-10, 10);
										rx = random.RandonRangeDouble(-10, 10);
										ry = random.RandonRangeDouble(-10, 10);
										rz = random.RandonRangeDouble(-10, 10);
									}

									if (s1 == "0") s1 = "145";
									mb.SendMessage($"/trigger;{s1};{s2};{s3};{state};{score};{x};{y};{z};{rx};{ry};{rz}");

								});
					 });

					//Attente fin d'acquistion d'image
					t.Wait();
				}


				//Trigg pour la caméra 2
				if (s4[0] == "Cam2")
				{

					//Thead camera 2
					Task t = Task.Run(() =>
					 {

						 //Acquistion de l'image
						 this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 2", "Demande de trigger camera 2"); }));

						 //Temps de simulation d'acquisition d'image
						 Thread.Sleep(2500);

						 this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 2", "Trigger terminér camera 2"); }));
						 //Fin d'acquisition de l'image


						 //Traitement de l'image dans un autre Thread parallèle
						 Task.Run(() =>
							{

								this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 2", "Traitement en cours camera 2"); }));

								Thread.Sleep(2000);

								this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 2", "Données prêtes camera 2"); }));



								////Simulation envois de données aléatoirement cohérente
								//Random random = new Random();

								string state = random.RandonRangeInt(0, 5) > 1 ? "PASS" : "FAIL";

								NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = ".";
								double score = 0;
								double x = 0;
								double y = 0;
								double z = 0;
								double rx = 0;
								double ry = 0;
								double rz = 0;

								if (state == "PASS")
								{
									score = random.RandonRangeDouble(30, 90);
									x = random.RandonRangeDouble(-150, 150);
									y = random.RandonRangeDouble(-150, 150);
									z = random.RandonRangeDouble(-10, 10);
									rx = random.RandonRangeDouble(-10, 10);
									ry = random.RandonRangeDouble(-10, 10);
									rz = random.RandonRangeDouble(-10, 10);
								}

								string ExtensionAttaque = "";

								var numberAttaque = random.RandonRangeInt(0, 20);
								ExtensionAttaque += numberAttaque + ";";
								if (numberAttaque > 0)
								{

									for (int i = 0; i < numberAttaque; i++)
									{

										ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";
										ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";
										ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";

									}
								}



								if (s1 == "0") s1 = "20001";
								mb.SendMessage($"/trigger;{s1};{s2};{s3};{state};{score};{x};{y};{z};{rx};{ry};{rz};{ExtensionAttaque}");

							});
					 });

					//Attente fin d'acquistion d'image
					t.Wait();
				}
			};
		}






		//Etat de la connection Modbus
		private void StateModbus(object sender, ModbusTCPMasterEventArgs e)
		{

			this.Invoke(new Action(() => LogsManager.Add(EnumCategory.Communication, "Socket", e)));

			if (e.State == StateConnect.Connecting)
			{
				this.Invoke(new Action(() => { button3.BackColor = Color.LightGreen; }));
				this.Invoke(new Action(() => { button2.BackColor = SystemColors.Control; }));
			}

			if (e.State == StateConnect.Connected)
			{
				this.Invoke(new Action(() => { button3.BackColor = Color.Green; }));
				this.Invoke(new Action(() => { button2.BackColor = SystemColors.Control; }));
			}

			if (e.State == StateConnect.NoClientExist)
			{
				this.Invoke(new Action(() => { button3.BackColor = Color.Yellow; }));
				this.Invoke(new Action(() => { button2.BackColor = SystemColors.Control; }));
			}

			if (e.State == StateConnect.Stopped)
			{
				this.Invoke(new Action(() => { button3.BackColor = SystemColors.Control; }));
				this.Invoke(new Action(() => { button2.BackColor = SystemColors.Control; }));
			}

			if (e.State == StateConnect.Disconnected)
			{
				this.Invoke(new Action(() => { button3.BackColor = SystemColors.Control; }));
				this.Invoke(new Action(() => { button2.BackColor = Color.Red; }));
			}
		}

		//MEthode de mise en forme conditionnelle du type de signaux
		private void MiseEnForme(object sender, DataGridViewRowsAddedEventArgs e)
		{
			DataGridView d = (DataGridView)sender;

			for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
			{
				Signal v = (Signal)d.Rows[i].DataBoundItem;

				if (v.Size == TypeSize.BOOL)
				{
					d.Rows[i].DefaultCellStyle.BackColor = Color.MediumSeaGreen;
				}
				else
				{
					d.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
				}
			}
		}

		//Mise en forme conditionnelle des types de logs
		private void MiseEnFormeLogs(object sender, DataGridViewRowsAddedEventArgs e)
		{
			DataGridView d = (DataGridView)sender;

			for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
			{
				Log v = (Log)d.Rows[i].DataBoundItem;

				if (v.Type == "Camera 1")
				{
					d.Rows[i].DefaultCellStyle.BackColor = Color.MediumSeaGreen;
				}
				else if (v.Type == "Camera 2")
				{
					d.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
				}
			}
		}


		//Effacement des logs
		private void button1_Click_1(object sender, EventArgs e)
		{
			LogsManager.Clear();
		}

		//Lancement de la communication modbus
		private void button3_Click(object sender, EventArgs e)
		{
			mb.IP = textBox1.Text;
			mb?.Start();
		}

		//Arret de la communcation modbus
		private void button2_Click(object sender, EventArgs e)
		{
			mb?.Stop();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			mb.FaultCam1 = mb.FaultCam1? false:true;
		}

		private void button5_Click(object sender, EventArgs e)
		{
			mb.FaultCam2 = mb.FaultCam2 ? false : true;
		}
	}
}
