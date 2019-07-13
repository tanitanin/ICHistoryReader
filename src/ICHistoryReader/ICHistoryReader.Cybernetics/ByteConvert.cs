using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICHistoryReader.Cybernetics
{
    static class ByteConvert
    {
        public static ushort ToUint16(byte[] bytes, bool littleEndian = true)
        {
            if (bytes.Length != 2)
                throw new NotSupportedException();
            if (littleEndian)
            {
                var high = (ushort)bytes[1] << 8;
                return (ushort)(high | bytes[0]);
            }
            else
            {
                var high = (ushort)bytes[0] << 8;
                return (ushort)(high | bytes[1]);
            }
        }
        public static uint ToUint32(byte[] bytes, bool littleEndian = true)
        {
            if (bytes.Length != 4)
                throw new NotSupportedException();
            if (littleEndian)
            {
                var b1 = (uint)bytes[3] << 24;
                var b2 = (uint)bytes[2] << 16;
                var b3 = (uint)bytes[1] << 8;
                var b4 = (uint)bytes[0] << 0;
                return (uint)(b1 | b2 | b3 | b4);
            }
            else
            {
                var b1 = (uint)bytes[0] << 24;
                var b2 = (uint)bytes[1] << 16;
                var b3 = (uint)bytes[2] << 8;
                var b4 = (uint)bytes[3] << 0;
                return (uint)(b1 | b2 | b3 | b4);
            }
        }
        public static int FromBCD(byte data)
        {
            var k2 = ((data & 0xF0) >> 4) * 10;
            var k1 = (data & 0x0F);
            return k2 + k1;
        }
        public static int FromBCD(ushort data)
        {
            var k4 = ((data & 0xF000) >> 24) * 1000;
            var k3 = ((data & 0x0F00) >> 16) * 100;
            var k2 = ((data & 0x00F0) >> 4) * 10;
            var k1 = (data & 0x000F);
            return  k4 + k3 + k2 + k1;
        }
        public static DateTime ToDate(byte[] bytes)
        {
            if (bytes.Length != 2)
                throw new NotSupportedException();
            var origin = ByteConvert.ToUint16(bytes, false);
            var YY = (int)(origin & 0xFE00) >> 9;
            var MM = (int)(origin & 0x01E0) >> 5;
            var DD = (int)(origin & 0x001F) >> 0;
            try
            {
                return new DateTime(YY, MM, DD);
            }
            catch { }
            return DateTime.MinValue;
        }
        public static TimeSpan ToTimeBCD(byte[] bytes)
        {
            if (bytes.Length != 2)
                throw new NotSupportedException();
            var origin = ByteConvert.ToUint16(bytes, false);
            var hh = ByteConvert.FromBCD((byte)((origin & 0xFF00) >> 8));
            var mm = ByteConvert.FromBCD((byte)(origin & 0x00FF));
            try
            {
                return new TimeSpan(hh, mm, 0);
            }
            catch { }
            return TimeSpan.MinValue;
        }
        public static DateTime ToDateTimeBCD(byte[] bytes)
        {
            if (bytes.Length != 4)
                throw new NotSupportedException();
            var origin = ByteConvert.ToUint32(bytes, false);
            var YY = (int)(origin & 0xFE000000) >> 25;
            var MM = (int)(origin & 0x01E00000) >> 21;
            var DD = (int)(origin & 0x001F0000) >> 16;
            var hh = ByteConvert.FromBCD((byte)((origin & 0x0000FF00) >> 8));
            var mm = ByteConvert.FromBCD((byte)(origin & 0x000000FF));
            try
            {
                return new DateTime(YY, MM, DD, hh, mm, 0);
            }
            catch { }
            return DateTime.MinValue;
        }
    }
}
