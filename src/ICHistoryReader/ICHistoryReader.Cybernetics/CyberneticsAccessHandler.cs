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
        public async Task<byte[]> GetUidAsync()
        {
            var resApdu = await connectionObject.TransceiveAsync(new GetData(GetData.GetDataDataType.Uid));

            if (!resApdu.Succeeded)
            {
                throw new Exception("Failure reading Felica card, " + resApdu.ToString());
            }

            return resApdu.ResponseData;
        }
        public async Task SelectFileAsync(byte[] serviceCode)
        {
            if (serviceCode.Length != 2)
            {
                throw new NotSupportedException();
            }

            var resApdu = await connectionObject.TransceiveAsync(new SelectFile(serviceCode));

            if (!resApdu.Succeeded)
            {
                throw new Exception("Failure reading Felica card, " + resApdu.ToString());
            }

            return;
        }
        public async Task<byte[]> ReadBinaryAsync(byte blockOffset)
        {
            var resApdu = await connectionObject.TransceiveAsync(new ReadBinary(blockOffset));

            if (!resApdu.Succeeded)
            {
                throw new Exception("Failure reading Felica card, " + resApdu.ToString());
            }

            return resApdu.ResponseData;
        }
        /// <summary>
        /// Polling command
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="requestCode"></param>
        /// <param name="timeSlot"></param>
        /// <returns></returns>
        public async Task<PollingResponse> PollingAsync(byte[] systemCode, byte requestCode, byte timeSlot)
        {
            if (systemCode.Length != 2)
            {
                throw new NotSupportedException();
            }

            //var result = (PollingResponse)await connectionObject.TransparentExchangeAsync(new Polling(systemCode, requestCode, timeSlot).CommandData);
            var req = new Polling(systemCode, requestCode, timeSlot).CommandData;
            var result = (PollingResponse)(await connectionObject.TransparentExchangeAsync(req));

            if (!result.Succeeded)
            {
                throw new Exception("Failure reading Felica card, " + result.ToString());
            }

            return result;
        }
        /// <summary>
        /// Request service
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="requestCode"></param>
        /// <param name="timeSlot"></param>
        /// <returns></returns>
        public async Task<RequestServiceResponse> RequestServiceAsync(byte[] idm, byte nodeCount, byte[] nodeCodeList)
        {
            if (idm.Length != 8)
            {
                throw new NotSupportedException();
            }
            if (nodeCodeList.Length != 2 * nodeCount)
            {
                throw new NotSupportedException();
            }

            var result = (RequestServiceResponse)await connectionObject.TransparentExchangeAsync((new RequestService(idm, nodeCount, nodeCodeList)).CommandData);

            if (!result.Succeeded)
            {
                throw new Exception("Failure reading Felica card, " + result.ToString());
            }

            return result;
        }
        /// <summary>
        /// Read without encryption
        /// </summary>
        /// <param name="idm"></param>
        /// <param name="serviceCount"></param>
        /// <param name="serviceCodeList"></param>
        /// <param name="blockCount"></param>
        /// <param name="blockList"></param>
        /// <returns></returns>
        public async Task<ReadWithoutEncryptionResponse> ReadWithoutEncryptionAsync(byte[] idm, byte serviceCount, byte[] serviceCodeList, byte blockCount, byte[] blockList)
        {
            if (idm.Length != 8)
            {
                throw new NotSupportedException();
            }
            if (serviceCount != 1 || serviceCodeList.Length !=  2 * serviceCount)
            {
                throw new NotSupportedException();
            }
            if (blockList.Length < 2 * blockCount || 3 * blockCount < blockList.Length)
            {
                throw new NotSupportedException();
            }

            var result = (ReadWithoutEncryptionResponse)await connectionObject.TransparentExchangeAsync((new ReadWithoutEncryption(idm, serviceCount, serviceCodeList, blockCount, blockList)).CommandData);

            if (!result.Succeeded)
            {
                throw new Exception("Failure reading Felica card, " + result.ToString());
            }

            return result;
        }
    }
}
