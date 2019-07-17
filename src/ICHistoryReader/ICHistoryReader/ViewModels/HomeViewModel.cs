using System;
using System.Linq;
using System.Threading.Tasks;
using ICHistoryReader.Helpers;
using Windows.Devices.SmartCards;
using Pcsc;
using static ICHistoryReader.Helpers.NfcUtils;
using Pcsc.Common;
using System.Collections.Generic;

namespace ICHistoryReader.ViewModels
{
    public class HomeViewModel : Observable
    {
        #region Properties and Fields

        private SmartCardReader m_cardReader;

        private List<Core.Models.StationCodeData> m_stationCodeList;

        #endregion

        public HomeViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            // First try to find a reader that advertises as being NFC
            var deviceInfo = await SmartCardReaderUtils.GetFirstSmartCardReaderInfo(SmartCardReaderKind.Nfc);
            if (deviceInfo == null)
            {
                // If we didn't find an NFC reader, let's see if there's a "generic" reader meaning we're not sure what type it is
                deviceInfo = await SmartCardReaderUtils.GetFirstSmartCardReaderInfo(SmartCardReaderKind.Any);
            }

            if (deviceInfo == null)
            {
                LogMessage("NFC card reader mode not supported on this device", NotifyType.ErrorMessage);
                return;
            }

            if (!deviceInfo.IsEnabled)
            {
                var msgbox = new Windows.UI.Popups.MessageDialog("Your NFC proximity setting is turned off, you will be taken to the NFC proximity control panel to turn it on");
                msgbox.Commands.Add(new Windows.UI.Popups.UICommand("OK"));
                await msgbox.ShowAsync();

                // This URI will navigate the user to the NFC proximity control panel
                NfcUtils.LaunchNfcProximitySettingsPage();
                return;
            }

            if (m_cardReader == null)
            {
                m_cardReader = await SmartCardReader.FromIdAsync(deviceInfo.Id);
                m_cardReader.CardAdded += cardReader_CardAdded;
                m_cardReader.CardRemoved += cardReader_CardRemoved;
            }

            var path = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + @"\Data\StationCode.csv";
            m_stationCodeList = await Core.Models.StationCodeData.LoadAsync(path);
        }

        public async Task UninitializeAsync()
        {
            // Clean up
            if (m_cardReader != null)
            {
                m_cardReader.CardAdded -= cardReader_CardAdded;
                m_cardReader.CardRemoved -= cardReader_CardRemoved;
                m_cardReader = null;
            }
        }

        private void cardReader_CardRemoved(SmartCardReader sender, CardRemovedEventArgs args)
        {
            LogMessage("Card removed");
        }

        private async void cardReader_CardAdded(SmartCardReader sender, CardAddedEventArgs args)
        {
            await HandleCard(args.SmartCard);
        }

        /// <summary>
        /// Sample code to hande a couple of different cards based on the identification process
        /// </summary>
        /// <returns>None</returns>
        private async Task HandleCard(SmartCard card)
        {
            try
            {
                // Clear the messages
                //MainPage.Current.NotifyUser(String.Empty, NotifyType.StatusMessage, true);

                // Connect to the card
                using (SmartCardConnection connection = await card.ConnectAsync())
                {
                    // Try to identify what type of card it was
                    IccDetection cardIdentification = new IccDetection(card, connection);
                    await cardIdentification.DetectCardTypeAync();
                    LogMessage("Connected to card\r\nPC/SC device class: " + cardIdentification.PcscDeviceClass.ToString());
                    LogMessage("Card name: " + cardIdentification.PcscCardName.ToString());
                    LogMessage("ATR: " + BitConverter.ToString(cardIdentification.Atr));

                    if ((cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass) &&
                        (cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralightC
                        || cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralight
                        || cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralightEV1))
                    {
                        // Handle MIFARE Ultralight
                        MifareUltralight.AccessHandler mifareULAccess = new MifareUltralight.AccessHandler(connection);

                        // Each read should get us 16 bytes/4 blocks, so doing
                        // 4 reads will get us all 64 bytes/16 blocks on the card
                        for (byte i = 0; i < 4; i++)
                        {
                            byte[] response = await mifareULAccess.ReadAsync((byte)(4 * i));
                            LogMessage("Block " + (4 * i).ToString() + " to Block " + (4 * i + 3).ToString() + " " + BitConverter.ToString(response));
                        }

                        byte[] responseUid = await mifareULAccess.GetUidAsync();
                        LogMessage("UID:  " + BitConverter.ToString(responseUid));
                    }
                    else if (cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.MifareDesfire)
                    {
                        // Handle MIFARE DESfire
                        Desfire.AccessHandler desfireAccess = new Desfire.AccessHandler(connection);
                        Desfire.CardDetails desfire = await desfireAccess.ReadCardDetailsAsync();

                        LogMessage("DesFire Card Details:  " + Environment.NewLine + desfire.ToString());
                    }
                    else if (cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass
                        && cardIdentification.PcscCardName == Pcsc.CardName.FeliCa)
                    {
                        // Handle Felica
                        LogMessage("Felica card detected");
                        var cyberneticsAccess = new Cybernetics.AccessHandler(connection);
                        var cardinfo = null as Cybernetics.CardInfo;
                        var histories = new List<Cybernetics.HistoryInfo>();
                        var gateHistories = new List<Cybernetics.GateHistoryInfo>();
                        var sfHistory = null as Cybernetics.SFEntryInfo;

                        var IDm = await cyberneticsAccess.GetUidAsync(); // ok
                        LogMessage("IDm: " + BitConverter.ToString(IDm));
                        await cyberneticsAccess.SelectFileAsync(new byte[] { 0x8B, 0x00 });
                        var block008B = await cyberneticsAccess.ReadBinaryAsync(0x00);
                        LogMessage("Card Type:  " + BitConverter.ToString(block008B));
                        cardinfo = Cybernetics.CardInfo.FromBytes(block008B);
                        await cyberneticsAccess.SelectFileAsync(new byte[] { 0x0F, 0x09 });
                        for (byte block=0; block < 20; ++block)
                        {
                            var block090F = await cyberneticsAccess.ReadBinaryAsync(block);
                            LogMessage("History:  " + BitConverter.ToString(block090F));
                            histories.Add(Cybernetics.HistoryInfo.FromBytes(block090F));
                        }
                        var histList = new List<ICHistoryReader.Core.Cybernetics.HistoryInfo>();
                        var prevHist = null as ICHistoryReader.Core.Cybernetics.HistoryInfo;
                        foreach (var hist in histories)
                        {
                            var h = ICHistoryReader.Core.Cybernetics.HistoryInfo.From(hist, m_stationCodeList);
                            histList.Add(h);
                            if (prevHist != null)
                            {
                                // 前の履歴から精算額を計算
                                prevHist.Amount = h.Balance - prevHist.Balance;
                            }
                            prevHist = h;
                        }
                        histList = histList.SkipLast(1).ToList();
                        await cyberneticsAccess.SelectFileAsync(new byte[] { 0x8F, 0x10 });
                        for (byte block = 0; block < 3; ++block)
                        {
                            var block108F = await cyberneticsAccess.ReadBinaryAsync(block);
                            LogMessage("Gate:  " + BitConverter.ToString(block108F));
                            gateHistories.Add(Cybernetics.GateHistoryInfo.FromBytes(block108F));
                        }
                        await cyberneticsAccess.SelectFileAsync(new byte[] { 0xCB, 0x10 });
                        var temp = new List<byte>();
                        for (byte block = 0; block < 2; ++block)
                        {
                            var block10CB = await cyberneticsAccess.ReadBinaryAsync(block);
                            LogMessage("SF:  " + BitConverter.ToString(block10CB));
                            temp.AddRange(block10CB);
                        }
                        sfHistory = Cybernetics.SFEntryInfo.FromBytes(temp.ToArray());
                    }
                    else if (cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass
                        && (cardIdentification.PcscCardName == Pcsc.CardName.MifareStandard1K || cardIdentification.PcscCardName == Pcsc.CardName.MifareStandard4K))
                    {
                        // Handle MIFARE Standard/Classic
                        LogMessage("MIFARE Standard/Classic card detected");
                        var mfStdAccess = new MifareStandard.AccessHandler(connection);
                        var uid = await mfStdAccess.GetUidAsync();
                        LogMessage("UID:  " + BitConverter.ToString(uid));

                        ushort maxAddress = 0;
                        switch (cardIdentification.PcscCardName)
                        {
                            case Pcsc.CardName.MifareStandard1K:
                                maxAddress = 0x3f;
                                break;
                            case Pcsc.CardName.MifareStandard4K:
                                maxAddress = 0xff;
                                break;
                        }
                        await mfStdAccess.LoadKeyAsync(MifareStandard.DefaultKeys.FactoryDefault);

                        for (ushort address = 0; address <= maxAddress; address++)
                        {
                            var response = await mfStdAccess.ReadAsync(address, Pcsc.GeneralAuthenticate.GeneralAuthenticateKeyType.MifareKeyA);
                            LogMessage("Block " + address.ToString() + " " + BitConverter.ToString(response));
                        }
                    }
                    else if (cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass
                        && (cardIdentification.PcscCardName == Pcsc.CardName.ICODE1 ||
                            cardIdentification.PcscCardName == Pcsc.CardName.ICODESLI ||
                            cardIdentification.PcscCardName == Pcsc.CardName.iCodeSL2))
                    {
                        // Handle ISO15693
                        LogMessage("ISO15693 card detected");
                        var iso15693Access = new Iso15693.AccessHandler(connection);
                        var uid = await iso15693Access.GetUidAsync();
                        LogMessage("UID:  " + BitConverter.ToString(uid));
                    }
                    else
                    {
                        // Unknown card type
                        // Note that when using the XDE emulator the card's ATR and type is not passed through, so we'll
                        // end up here even for known card types if using the XDE emulator

                        // Some cards might still let us query their UID with the PC/SC command, so let's try:
                        var apduRes = await connection.TransceiveAsync(new Pcsc.GetUid());
                        if (!apduRes.Succeeded)
                        {
                            LogMessage("Failure getting UID of card, " + apduRes.ToString());
                        }
                        else
                        {
                            LogMessage("UID:  " + BitConverter.ToString(apduRes.ResponseData));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage("Exception handling card: " + ex.ToString(), NotifyType.ErrorMessage);
            }
        }
    }
}
