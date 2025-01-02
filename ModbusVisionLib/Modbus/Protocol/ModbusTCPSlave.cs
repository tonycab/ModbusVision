
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

namespace ModbusVisionLib.Modbus.Protocol
{

    [Obsolete]
    internal class ModbusTCPSlave : Dictionary<string, Signal>
    {
        public ThreadedBindingList<SignalInput> Inputs { get; set; }
        public ThreadedBindingList<Signal> Outputs { get; set; }
        //public BindingList<SignalInput> Inputs { get; set; }
        //public BindingList<Signal> Outputs { get; set; }
        public string IPadress { get; set; }
        public int Port { get; set; }
        public bool StateModbus { get; set; }

        ushort[] RegistreInput { get; set; }

        ushort[] RegisterOutput { get; set; }

        ushort StartRegisterInput { get; set; }

        ushort StartRegisterOutput { get; set; }

        private TcpListener slaveTcpListener;
        private IModbusSlaveNetwork Slave;
        private IModbusSlave Table;

      
        private ModbusFactory fmodbus;

        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public ModbusTCPSlave(string ipAddress, int port = 502, Action<string, string>logger=null, ushort startRegisterInput = 0, int sizeRegisterInput = 128, ushort startRegisterOutput = 128, int sizeRegisterOutput = 128)
        {
            
            fmodbus = new ModbusFactory(null,true, new Loggers(logger));
 
            IPAddress address = IPAddress.Parse(ipAddress);
            Byte[] bytes = address.GetAddressBytes();
            IPAddress ipadresse = new IPAddress(bytes);
            Port = port;
            IPEndPoint ipEndPoint = new IPEndPoint(ipadresse, Port);
            slaveTcpListener = new TcpListener(ipEndPoint);

            StartRegisterInput = startRegisterInput;
            StartRegisterOutput = startRegisterOutput;

            RegistreInput = new ushort[sizeRegisterInput];
            RegisterOutput = new ushort[sizeRegisterOutput];

            Inputs = new ThreadedBindingList<SignalInput>();
            Outputs = new ThreadedBindingList<Signal>();

        }

        public void AddSignalInput(SignalInput signalInput)
        {
            this.Add(signalInput.Name, signalInput);
            Inputs.Add(signalInput);

        }

        public void AddSignalOutput(SignalOutput signalOutput)
        {
            this.Add(signalOutput.Name, signalOutput);
            Outputs.Add(signalOutput);

        }

        public void RemoveSignalInput(string name)
        {
            Inputs.Remove((SignalInput)this[name]);
            this.Remove(name);

        }

        public void RemoveSignalOutput(string name)
        {
            Outputs.Remove((SignalOutput)this[name]);
            this.Remove(name);

        }

        public void Start()
        {
            cancellationToken = new CancellationTokenSource();
            StartModbus();
        }

        public void Stop()
        {

            cancellationToken.Cancel();
            slaveTcpListener.Stop();
            StateModbus = false;
        }

        private void StartModbus()
        {

           
            slaveTcpListener.Start();

            if (Slave == null)
            {            
                //Création d'un SlaveModbus
                Slave = fmodbus.CreateSlaveNetwork(slaveTcpListener);
            }

            if (Table == null)
            {
                //Creation de la table modbus
                Table = fmodbus.CreateSlave(1);
            
                //Ajout de la table modbus 
                Slave.AddSlave(Table);
            }


            //Lance une Task de surveillance de la table modbus
            UpdateSignal(Table, cancellationToken.Token);

            Task.Run(() =>
            {
                try
                {
                    using (cancellationToken.Token.Register(() => StateModbus = false))
                    {
                        //Attente de la fin de la connection
                        Slave.ListenAsync(cancellationToken.Token);//.GetAwaiter().GetResult();

                        StateModbus = true;
                    }
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, cancellationToken.Token);

        }


        //Affichage des valeurs de registe
        public void UpdateSignal(IModbusSlave slave, CancellationToken token)
        {
            Task.Run(() =>
            {

                bool OnUpdate = true;

                while (OnUpdate)
                {

                    if (token.IsCancellationRequested) OnUpdate = false;

                    //Lecture des entrées
                    RegistreInput = slave.DataStore.HoldingRegisters.ReadPoints(StartRegisterInput, (ushort)RegistreInput.Length);

                    foreach (SignalInput signal in Inputs)
                    {
                        signal.SetSignal(RegistreInput);
                    }

                    foreach (SignalOutput signal in Outputs)
                    {
                        signal.SetRegister(RegisterOutput);
                    }

                    //Ecriture des sorties
                    slave.DataStore.HoldingRegisters.WritePoints(StartRegisterOutput, RegisterOutput);

                    Thread.Sleep(50);

                }

            }, token);


        }


    }
}
