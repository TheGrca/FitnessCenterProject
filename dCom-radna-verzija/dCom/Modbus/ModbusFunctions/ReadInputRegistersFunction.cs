using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            Console.WriteLine("Request started");
            byte[] recVal = new byte[12];
            ModbusReadCommandParameters  modbus = (ModbusReadCommandParameters)CommandParameters;
            byte[] temp = BitConverter.GetBytes(modbus.TransactionId);
            recVal[0] = temp[0];
            recVal[1] = temp[1];

            byte[] tempP = BitConverter.GetBytes(modbus.ProtocolId);
            recVal[2] = tempP[0];
            recVal[3] = tempP[1];

            byte[] tempL = BitConverter.GetBytes(modbus.Length);
            recVal[4] = tempL[0];
            recVal[5] = tempL[1];
            recVal[6] = (byte)(modbus.UnitId);
            recVal[7] = (byte)(modbus.FunctionCode);

            byte[] tempSA = BitConverter.GetBytes(modbus.StartAddress);
            recVal[8] = tempSA[0];
            recVal[9] = tempSA[1];

            byte[] tempQ = BitConverter.GetBytes(modbus.Quantity);
            recVal[10] = tempQ[0];
            recVal[11] = tempQ[1];

            Console.WriteLine("Request ended");
            return recVal;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusReadCommandParameters modbus = (ModbusReadCommandParameters)CommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();

            for(int i = 0; i < response[8]; i += 2)
            {
                Tuple<PointType, ushort> tuple = Tuple.Create((PointType)response[7], modbus.StartAddress);
                byte[] bytes = BitConverter.GetBytes(response[9 + i]);
                byte temp1 = bytes[0];
                byte temp2 = bytes[1];
                bytes[0] = temp2;
                bytes[1] = temp1;
                ret.Add(tuple, (ushort)BitConverter.ToInt16(bytes, 0));
            }

            return ret;
        }
    }
}