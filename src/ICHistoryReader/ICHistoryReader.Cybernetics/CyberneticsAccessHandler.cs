using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;
using Pcsc;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ICHistoryReader.Cybernetics
{
    /// <summary>
    /// Access handler class for Felica based ICC. It provides wrappers for different Felica 
    /// commands
    /// </summary>
    public class AccessHandler
    {
        /// <summary>
        /// connection object to smart card
        /// </summary>
        private SmartCardConnection connectionObject { set; get; }
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="ScConnection">
        /// connection object to a Felica ICC
        /// </param>
        public AccessHandler(SmartCardConnection ScConnection)
        {
            connectionObject = ScConnection;
        }
        /// <summary>
        /// Polling command
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="requestCode"></param>
        /// <param name="timeSlot"></param>
        /// <returns></returns>
        public async Task<byte[]> PollingAsync(byte[] systemCode, byte requestCode, byte timeSlot)
        {
            if (systemCode.Length != 2)
            {
                throw new NotSupportedException();
            }

            var packet = new Polling(systemCode, requestCode, timeSlot);
            var resBuf = await connectionObject.TransmitAsync(packet.CommandData.AsBuffer());
            var res = resBuf.ToArray();

            return res;

            //if (!apduRes.Succeeded)
            //{
            //    throw new Exception("Failure reading Felica card, " + apduRes.ToString());
            //}

            //return apduRes.ResponseData;
        }
        /// <summary>
        /// Wrapper method to write data to the felica card
        /// </summary>
        /// <param name="serviceCount">
        /// The number of service
        /// </param>
        /// <param name="serviceCodeList">
        /// The service code list in little endian format
        /// </param>
        /// </param>
        /// <param name="blockCount">
        /// The number of blocks to read
        /// </param>
        /// </param>
        /// <param name="blockList">
        /// The list of blocks to be read. Must be multiples of 2 or 3.
        /// </param>
        /// /// <param name="blockData">
        /// The data to write for the corresponding blocks. Must be multiple of 16 of the block count.
        /// </param>
        public async Task WriteAsync(byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList, byte[] blockData)
        {
            if (serviceCount != 1 || serviceCodeList.Length != 2)
            {
                throw new NotSupportedException();
            }

            if (blockData.Length != blockCount * 16)
            {
                throw new InvalidOperationException("Invalid blockData size");
            }

            var apduRes = await connectionObject.TransceiveAsync(new Felica.Update(serviceCount, serviceCodeList, blockCount, blockList, blockData));

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure writing Felica card, " + apduRes.ToString());
            }
        }
        /// <summary>
        /// Wrapper method to perform transparent transceive data to the felica card
        /// </summary>
        /// <param name="commandData">
        /// The command to send to the felica card
        /// </param>
        /// <returns>
        /// byte array of the read data
        /// </returns>
        public async Task<byte[]> TransparentExchangeAsync(byte[] commandData)
        {
            byte[] responseData = await connectionObject.TransparentExchangeAsync(commandData);

            return responseData;
        }
        /// <summary>
        /// Wrapper method get the Felica UID
        /// </summary>
        /// <returns>
        /// byte array UID
        /// </returns>
        public async Task<byte[]> GetUidAsync()
        {
            var apduRes = await connectionObject.TransceiveAsync(new Felica.GetUid());

            if (!apduRes.Succeeded)
            {
                throw new Exception("Failure getting UID of Felica card, " + apduRes.ToString());
            }

            return apduRes.ResponseData;
        }
    }
}
