using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib.Modbus.IO
{
	public class SignalModbus<T> : INotifyPropertyChanged 
	{

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

		private T signalValue;

		public T Value
		{
			get
			{
				return signalValue;
			}
			set
			{
				if (value.Equals(signalValue))
				{
					signalValue = value;
					onPropertyChanged(nameof(signalValue));
					this.SignalChanged?.Invoke(this);
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

		internal  void SetSignal(ushort[] register)
		{
			//if (T = )
			//{
			//	Object a = register[NumberRegister];
			//	signalValue = (T)a;
			//}
			//if (Size == TypeSize.DINT)
			//{
			//	var r = register[NumberRegister + 1] << 16;
			//	r = r | register[NumberRegister];

			//	signalValue = r;
			//}

			//if (Size == TypeSize.UINT)
			//{
			//	State = register[NumberRegister];
			//}
			//if (Size == TypeSize.UDINT)
			//{
			//	var r = register[NumberRegister + 1] << 16;
			//	r = r | register[NumberRegister];

			//	State = r;
			//}

		}

		/// <summary>
		/// Evennement lorsque le signal à changé
		/// </summary>
		public event Action<SignalModbus<T>> SignalChanged;

		/// <summary>
		/// Implementation PropertyChanged  for interface INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		public void onPropertyChanged(string propertyName)
		{

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(propertyName)));
		}



	}
}
