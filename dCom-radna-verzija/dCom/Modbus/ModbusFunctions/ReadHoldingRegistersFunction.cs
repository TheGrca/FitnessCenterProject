using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            byte[] ret = new byte[12];
            ModbusReadCommandParameters modbus = (ModbusReadCommandParameters)CommandParameters;

            byte[] TransactionId = BitConverter.GetBytes(modbus.TransactionId);
            ret[0] = TransactionId[1];
            ret[1] = TransactionId[0];

            byte[] ProtocolId = BitConverter.GetBytes(modbus.ProtocolId);
            ret[2] = ProtocolId[1];
            ret[3] = ProtocolId[0];

            byte[] Length = BitConverter.GetBytes(modbus.Length);
            ret[4] = Length[1];
            ret[5] = Length[0];

            ret[6] = (byte)(modbus.UnitId);

            ret[7] = (byte)(modbus.FunctionCode);

            byte[] StartAddress = BitConverter.GetBytes(modbus.StartAddress);
            ret[8] = StartAddress[1];
            ret[9] = StartAddress[0];

            byte[] Quantity = BitConverter.GetBytes(modbus.Quantity);
            ret[10] = Quantity[1];
            ret[11] = Quantity[0];

            return ret;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusReadCommandParameters modbus = (ModbusReadCommandParameters)CommandParameters;

            if (response.Length <= 9)
            {
                Console.WriteLine("Not valid message!");
            }
            else
            {
                for (int i = 0; i < response[8]; i += 2)
                {
                    Tuple<PointType, ushort> tuple = Tuple.Create(PointType.ANALOG_OUTPUT, modbus.StartAddress);
                    byte[] bytes = new byte[2];

                    bytes[0] = response[10 + i];
                    bytes[1] = response[9 + i];
                    ret.Add(tuple, (ushort)BitConverter.ToUInt16(bytes, 0));
                }
            }

            return ret;
        }
    }
}