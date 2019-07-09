using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ICHistoryReader.Cybernetics
{
    /// <summary>
    /// Polling command
    /// </summary>
    public class Polling : Iso7816.SelectCommand
    {
        public byte[] SystemCode { get; private set; }
        public byte RequestCode { get; private set; }
        public byte TimeSlot { get; private set; }

        public Polling(byte[] systemCode, byte requestCode, byte timeSlot)
            : base(new byte[] { 0x0f, 0x09 } , null)
        {
            SystemCode = systemCode;
            RequestCode = requestCode;
            TimeSlot = timeSlot;
            base.CommandData = GetDataIn(systemCode, requestCode, timeSlot);
        }

        private static byte[] GetDataIn(byte[] systemCode, byte requestCode, byte timeSlot)
        {
            DataWriter dataWriter = new DataWriter();

            dataWriter.WriteByte(6);
            dataWriter.WriteByte(0x00);
            dataWriter.WriteBytes(systemCode);
            dataWriter.WriteByte(requestCode);
            dataWriter.WriteByte(timeSlot);

            return dataWriter.DetachBuffer().ToArray();
        }
    }
}
