using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib.Modbus.IO
{
    public class GroupOutputSignal : SignalOutput {


		/// <summary>
		/// Intancie un signal de groupe de sorties
		/// </summary>
		/// <param name="name">Nom du signal</param>
		/// <param name="designation"Désignation du signal></param>
		/// <param name="numberRegister">Numéro de registre de départ</param>
		/// <param name="size">type de signal</param>
		public GroupOutputSignal(string name,string designation, uint numberRegister, TypeSize size)
        {
            Designation = designation;
            Name = name;
            NumberRegister = numberRegister;
            NumberBit = 0;
            Size = size;
            this.SignalType = SignalType.GroupOutput;
        }

		internal override void SetRegister(ushort[] register)
        {
            if (Size == TypeSize.INT)
            {
                register[NumberRegister]= (ushort)State;
            }
            if (Size == TypeSize.UINT)
            {
                register[NumberRegister] = (ushort)State;
            }

            if (Size == TypeSize.DINT) 
            {
                register[NumberRegister] = (ushort)State;
                register[NumberRegister + 1] = (ushort)(State >> 16);
            }
            if (Size == TypeSize.UDINT)
            {
                register[NumberRegister] = (ushort)State;
                register[NumberRegister + 1] = (ushort)(State >> 16);
            }
        }
    }
}






