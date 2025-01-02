using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib.Modbus.IO
{
    public abstract class SignalOutput : Signal, INotifyPropertyChanged
    {
        /// <summary>
        /// Definis la valeur du signal
        /// </summary>
        /// <param name="register"></param>
        internal abstract void SetRegister(ushort[] register);
    }




}
