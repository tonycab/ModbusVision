using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusVisionLib
{
    static class Extension
    {

        /// <summary>
        /// retourne la valeur du bit spécifié
        /// </summary>
        /// <param name="value">ushort type</param>
        /// <param name="bitNumber">numéro de bit entre 0 - 15</param>
        /// <returns></returns>
        public static bool GetBitValue(this ushort value, int bitNumber)
        {
            var v = (value >> bitNumber & 1);

            return v == 1 ? true : false;

        }
        /// <summary>
        /// retourne la valeur du bit spécifié
        /// </summary>
        /// <param name="value">ushort type</param>
        /// <param name="bitNumber">numéro de bit entre 0 - 15</param>
        /// <returns></returns>
        public static int GetBitValueInt(this ushort value, int bitNumber)
        {
            var v = (value >> bitNumber & 1);

            return v;

        }
        /// <summary>
        /// Set le bit spécifié
        /// </summary>
        /// <param name="value">ushort type</param>
        /// <param name="bitNumber">numéro de bit entre 0 - 15</param>
        public static void SetBitValue(ref this ushort value, int bitNumber)
        {

            var v = (1 << bitNumber);

            value = (ushort)(v | value);

        }

        /// <summary>
        /// Reset le bit spécifié
        /// </summary>
        /// <param name="value">ushort type</param>
        /// <param name="bitNumber">numéro de bit entre 0 - 15</param>
        public static void ResetBitValue(ref this ushort value, int bitNumber)
        {

            var v = ~(1 << bitNumber);

            value = (ushort)(v & value);

        }
        /// <summary>
        /// Inverse le bit spécifé
        /// </summary>
        /// <param name="value">ushort type</param>
        /// <param name="bitNumber">numéro de bit entre 0 - 15</param>
        public static void InvertBitValue(ref this ushort value, int bitNumber)
        {

            if (value.GetBitValue(bitNumber))
            {
                value.ResetBitValue(bitNumber);
            }
            else { value.SetBitValue(bitNumber); }
        }

    }
}
