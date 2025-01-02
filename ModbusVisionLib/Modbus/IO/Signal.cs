using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib.Modbus.IO
{
    public abstract class Signal : INotifyPropertyChanged
    {
     
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;

        /// <summary>
        /// Nom du signal
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    onPropertyChanged(nameof(Name));
                }
            }

        }

        private string designation;

        /// <summary>
        /// Désignation du signal (Facultatif)
        /// </summary>
        public string Designation
        {
            get
            {
                return designation;
            }
            set
            {
                if (value != designation)
                {
                    designation = value;
                    onPropertyChanged(nameof(Designation));
                }
            }

        }

        private SignalType signalType;

        /// <summary>
        /// Type de signal 
        /// </summary>
        public SignalType SignalType
        {
            get
            {
                return signalType;
            }
            set
            {
                if (value != signalType)
                {
                    signalType = value;
                    onPropertyChanged(nameof(SignalType));
                }
            }
        }

        /// <summary>
        /// Numero de registre de départ du signal
        /// </summary>
        public uint NumberRegister { get; set; }

        /// <summary>
        /// Numero de bit du signal 
        /// </summary>
        public int NumberBit { get; set; }

        /// <summary>
        /// Type de signal 
        /// </summary>
        public TypeSize Size { get; set; }

        private int state;

        /// <summary>
        /// Etat / Valeur du signal
        /// </summary>
        public int State
        {
            get
            {
                return state;
            }
            set
            {
                if (value != state)
                {
                    state = value;
                    onPropertyChanged(nameof(State));
                    
                    //this.SignalChanged?.Invoke(this);

                    stateChanged = true;
                }
            }

        }


        private bool stateChanged {  get; set; }

        /// <summary>
        /// Evennement lorsque le signal à changé
        /// </summary>
        public event Action<Signal> SignalChanged;

        /// <summary>
        /// Déclenche l'évennement proprieté à changé
        /// </summary>
        /// <param name="propertyName"></param>
        public  void onPropertyChanged(string propertyName)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(propertyName)));
        }


        /// <summary>
        /// Appel l evennement quand le signal a changer
        /// </summary>
        public virtual void CallEventStateChanged()
        {
            if ( stateChanged == true) {
                this.SignalChanged?.Invoke(this);
                stateChanged = false;
            }
        }


        /// <summary>
        /// Return le nom du signal
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }
    }

    /// <summary>
    /// Enumération des types de signaux
    /// </summary>
    public enum SignalType
    {
        DigitalInput = 0,
        GroupInput = 1,
        DigitalOutput = 2,
        GroupOutput = 3,
        AnalogInput = 4,
        AnalogOutput = 5,
    }

    /// <summary>
    /// Enumération des tailles de signaux
    /// </summary>
    public enum TypeSize
    {
        BOOL = 0,
        INT = 1,
        UINT = 2,
        DINT = 3,
        UDINT = 4,
        REAL = 4,
    }

}
