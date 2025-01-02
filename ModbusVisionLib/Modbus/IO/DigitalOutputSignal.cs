using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib.Modbus.IO
{
    public class DigitalOutputSignal : SignalOutput {

		/// <summary>
		/// Intancie un signal de sortie
		/// </summary>
		/// <param name="name">Nom du signal</param>
		/// <param name="designation"Désignation du signal></param>
		/// <param name="numberRegister">Numéro de registre de départ</param>
		/// <param name="numberBit">Numéro de bit</param>
		public DigitalOutputSignal(string name,string designation, uint numberRegister, int numberBit)
        {
            Designation = designation;
            Name = name;
            NumberRegister = numberRegister;
            NumberBit = numberBit;
            Size = TypeSize.BOOL;
            SignalType = SignalType.DigitalOutput;
        }

        internal  override void SetRegister(ushort[] register)
        {
            if (State == 1)
            {
                register[NumberRegister].SetBitValue(NumberBit);

            }
            else
            {
                register[NumberRegister].ResetBitValue(NumberBit);
            }
        }

    }


}






