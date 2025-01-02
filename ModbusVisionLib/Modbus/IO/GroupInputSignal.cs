using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib.Modbus.IO
{
    public class GroupInputSignal : SignalInput {

		/// <summary>
		/// Intancie un signal de groupe d'entrées 
		/// </summary>
		/// <param name="name">Nom du signal</param>
		/// <param name="designation"Désignation du signal></param>
		/// <param name="numberRegister">Numéro de registre de départ</param>
		/// <param name="size">type de signal</param>
		public GroupInputSignal(string name,string designation, uint numberRegister, TypeSize size)
        {
            Designation = designation;
            Name = name;
            NumberRegister = numberRegister;
            NumberBit = 0;
            Size = size;
            this.SignalType = SignalType.GroupInput;
        }

        internal  override void SetSignal(ushort[] register)
        {
            if(Size == TypeSize.INT)
            {
                State = register[NumberRegister];
            }
            if (Size == TypeSize.DINT)
            {
                var r = register[NumberRegister+1] << 16;
                r = r | register[NumberRegister];

                State = r;
            }

            if (Size == TypeSize.UINT)
            {
                State = register[NumberRegister];
            }
            if (Size == TypeSize.UDINT)
            {
                var r = register[NumberRegister + 1] << 16;
                r = r | register[NumberRegister];

                State = r;
            }

        }

    }


}






