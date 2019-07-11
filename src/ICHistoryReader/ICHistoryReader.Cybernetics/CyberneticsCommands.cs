using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ICHistoryReader.Cybernetics
{
    enum RequestType : byte
    {
        Polling = 0x00,
        RequestService = 0x02,
        ReadWithoutEncryption = 0x06,
    }
    enum ResponseType : byte
    {
        Polling = 0x01,
        RequestService = 0x03,
        ReadWithoutEncryption = 0x07,
    }
    public class SelectFile : Iso7816.ApduCommand
    {
        public SelectFile(byte[] serviceCode)
            : base((byte)Iso7816.Cla.ReservedForPts, (byte)Iso7816.Ins.SelectFile, 0x00, 0x01, serviceCode, 0x02)
        { }
    }
    public class SelectFileResponse : Iso7816.ApduResponse
    {
        public SelectFileResponse()
            : base()
        { }
    }
    public class ReadBinary : Iso7816.ApduCommand
    {
        public ReadBinary(byte blockOffset)
            : base((byte)Iso7816.Cla.ReservedForPts, (byte)Iso7816.Ins.ReadBinary, 0x00, blockOffset, null, 0x00)
        { }
    }
    public class ReadBinaryResponse : Iso7816.ApduResponse
    {
        public ReadBinaryResponse()
            : base()
        { }
        public override bool Succeeded
        {
            get
            {
                if (base.Succeeded)
                {
                    if (ResponseData.Length != 16)
                        throw new System.InvalidOperationException("Invalid value size for ICC response");
                    return true;
                }
                return false;
            }
        }
    }
    /// <summary>
    /// Polling command
    /// </summary>
    public class Polling : Pcsc.TransparentExchange
    {
        public byte[] SystemCode { get; private set; }
        public byte RequestCode { get; private set; }
        public byte TimeSlot { get; private set; }

        public Polling(byte[] systemCode, byte requestCode, byte timeSlot)
            : base(null)
        {
            SystemCode = systemCode;
            RequestCode = requestCode;
            TimeSlot = timeSlot;
            base.CommandData = GetDataIn(systemCode, requestCode, timeSlot);
        }

        private static byte[] GetDataIn(byte[] systemCode, byte requestCode, byte timeSlot)
        {
            DataWriter dataWriter = new DataWriter();

            var length = 1 + 1 + systemCode.Length + 1 + 1;
            dataWriter.WriteByte((byte)length);
            dataWriter.WriteByte((byte)RequestType.Polling);
            dataWriter.WriteBytes(systemCode);
            dataWriter.WriteByte(requestCode);
            dataWriter.WriteByte(timeSlot);

            return dataWriter.DetachBuffer().ToArray();
        }
    }
    /// <summary>
    /// Polling response
    /// </summary>
    public class PollingResponse
    {
        byte[] rawResponse;
        public PollingResponse(byte[] rawResponse)
        {
            this.rawResponse = rawResponse;
        }
        public static explicit operator PollingResponse(byte[] rawResponse)
        {
            return new PollingResponse(rawResponse);
        }
        public bool Succeeded
        {
            get
            {
                byte[] value = this.rawResponse;
                if (value.Length != 18 && value.Length != 20)
                    throw new System.InvalidOperationException("Invalid value size for ICC response");

                return rawResponse[0] == (byte)ResponseType.Polling;
            }
        }
        public byte[] IDm
        {
            get
            {
                return this.rawResponse.Skip(1).Take(8).ToArray();
            }
        }
        public byte[] PMm
        {
            get
            {
                return this.rawResponse.Skip(9).Take(8).ToArray();
            }
        }
    }
    /// <summary>
    /// Request service command
    /// </summary>
    public class RequestService : Pcsc.TransparentExchange
    {
        public byte[] IDm { get; private set; }
        public byte NodeCount { get; private set; }
        public byte[] NodeCodeList { get; private set; }

        public RequestService(byte[] idm, byte nodeCount, byte[] nodeCodeList)
            : base(null)
        {
            IDm = idm;
            NodeCount = nodeCount;
            NodeCodeList = nodeCodeList;
            base.CommandData = GetDataIn(IDm, NodeCount, NodeCodeList);
        }

        private static byte[] GetDataIn(byte[] idm, byte nodeCount, byte[] nodeCodeList)
        {
            DataWriter dataWriter = new DataWriter();

            var length = 1 + 1 + idm.Length + 1 + nodeCodeList.Length;
            dataWriter.WriteByte((byte)length);
            dataWriter.WriteByte((byte)RequestType.RequestService);
            dataWriter.WriteBytes(idm);
            dataWriter.WriteByte(nodeCount);
            dataWriter.WriteBytes(nodeCodeList);

            return dataWriter.DetachBuffer().ToArray();
        }
    }
    /// <summary>
    /// Request service response
    /// </summary>
    public class RequestServiceResponse
    {
        byte[] rawResponse;
        public RequestServiceResponse(byte[] rawResponse)
        {
            this.rawResponse = rawResponse;
        }
        public static explicit operator RequestServiceResponse(byte[] rawResponse)
        {
            return new RequestServiceResponse(rawResponse);
        }
        public bool Succeeded
        {
            get
            {
                byte[] value = this.rawResponse;
                if (value.Length < 10 || value.Length != 10 + 2 * NodeCount)
                    throw new System.InvalidOperationException("Invalid value size for ICC response");

                return rawResponse[0] == (byte)ResponseType.RequestService;
            }
        }
        public byte[] IDm
        {
            get => this.rawResponse.Skip(1).Take(8).ToArray();
        }
        public byte NodeCount
        {
            get => this.rawResponse[9];
        }
        public byte[] NodeKeyVersionList
        {
            get => this.rawResponse.Skip(10).ToArray();
        }
    }
    /// <summary>
    /// Read without encryption command
    /// </summary>
    public class ReadWithoutEncryption : Pcsc.TransparentExchange
    {
        public byte[] IDm { get; private set; }
        public byte ServiceCount { get; private set; }
        public byte[] ServiceCodeList { get; private set; }
        public byte BlockCount { get; private set; }
        public byte[] BlockList { get; private set; }

        public ReadWithoutEncryption(byte[] idm, byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList)
            : base(null)
        {
            IDm = idm;
            ServiceCount = serviceCount;
            ServiceCodeList = serviceCodeList;
            BlockCount = blockCount;
            BlockList = blockList;
            base.CommandData = GetDataIn(IDm, ServiceCount, ServiceCodeList, BlockCount, BlockList);
        }

        private static byte[] GetDataIn(byte[] idm, byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList)
        {
            DataWriter dataWriter = new DataWriter();

            var length = 1 + 1 + idm.Length + 1 + serviceCodeList.Length + 1 + blockList.Length;
            dataWriter.WriteByte((byte)length);
            dataWriter.WriteByte((byte)RequestType.ReadWithoutEncryption);
            dataWriter.WriteBytes(idm);
            dataWriter.WriteByte(serviceCount);
            dataWriter.WriteBytes(serviceCodeList);
            dataWriter.WriteByte(blockCount);
            dataWriter.WriteBytes(blockList);

            return dataWriter.DetachBuffer().ToArray();
        }
    }
    /// <summary>
    /// Read without encryption response
    /// </summary>
    public class ReadWithoutEncryptionResponse
    {
        byte[] rawResponse;
        public ReadWithoutEncryptionResponse(byte[] rawResponse)
        {
            this.rawResponse = rawResponse;
        }
        public static explicit operator ReadWithoutEncryptionResponse(byte[] rawResponse)
        {
            return new ReadWithoutEncryptionResponse(rawResponse);
        }
        public bool Succeeded
        {
            get
            {
                byte[] value = this.rawResponse;
                if (value.Length < 12)
                    throw new System.InvalidOperationException("Invalid value size for ICC response");
                if (StatusFlag1 == 0x00 && value.Length != 12 + 16 * BlockCount)
                    throw new System.InvalidOperationException("Invalid value size for ICC response");

                return rawResponse[0] == (byte)ResponseType.ReadWithoutEncryption;
            }
        }
        public byte[] IDm
        {
            get => this.rawResponse.Skip(1).Take(8).ToArray();
        }
        public byte StatusFlag1
        {
            get => this.rawResponse[9];
        }
        public byte StatusFlag2
        {
            get => this.rawResponse[10];
        }
        public byte BlockCount
        {
            get => this.rawResponse[11];
        }
        public byte[] BlockData
        {
            get => this.rawResponse.Skip(12).ToArray();
        }
    }
}
