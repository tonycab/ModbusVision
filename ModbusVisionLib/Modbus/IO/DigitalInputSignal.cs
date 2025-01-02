using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib.Modbus.IO
{
    public class DigitalInputSignal : SignalInput { 
      
        /// <summary>
        /// Intancie un signal d'entrée
        /// </summary>
        /// <param name="name">Nom du signal</param>
        /// <param name="designation"Désignation du signal></param>
        /// <param name="numberRegister">Numéro de registre de départ</param>
        /// <param name="numberBit">Numéro de bit</param>
        public DigitalInputSignal(string name,string designation, uint numberRegister, int numberBit)
        {
            Designation = designation;
            Name = name;
            NumberRegister = numberRegister;
            NumberBit = numberBit;
            Size = TypeSize.BOOL;
            this.SignalType = SignalType.DigitalInput;
        }


        internal  override void SetSignal(ushort[] register)
        {

            State = register[NumberRegister].GetBitValueInt(NumberBit);
        }

    }


}






