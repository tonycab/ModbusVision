using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib.Modbus.Protocol
{
	/// <summary>
	/// Class qui contient les informations d'évènement lors d'un changement d'état de la connection Modbus
	/// </summary>
	public class ModbusTCPMasterEventArgs
	{
		#region Properties public

		public StateConnect State;
		public string IP;
		public int Port;

		#endregion

		#region Constuctor 
		/// <summary>
		/// Constructor class informations event state conection Modbus
		/// </summary>
		/// <param name="stateConnect"></param>
		/// <param name="adresse"></param>
		/// <param name="port"></param>
		public ModbusTCPMasterEventArgs(StateConnect stateConnect, string adresse, int port)
		{

			State = stateConnect;
			IP = adresse;
			Port = port;

		}

		#endregion

		#region Methode public

		/// <summary>
		/// Return string of state information 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Enum.GetName(typeof(StateConnect), State) + " to IP: " + IP + " | Port: " + Port.ToString();
		}

		/// <summary>
		/// Convertion implicite ToString() Methode
		/// </summary>
		/// <param name="s"></param>
		public static implicit operator string(ModbusTCPMasterEventArgs s) => s.ToString();

		#endregion
	}
}
