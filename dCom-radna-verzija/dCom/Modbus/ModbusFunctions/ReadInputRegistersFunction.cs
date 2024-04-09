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
            byte[] ret = new byte[12];

            ModbusReadCommandParameters modbus = (ModbusReadCommandParameters)CommandParameters;

            byte[] trans = BitConverter.GetBytes(modbus.TransactionId);
            ret[0] = trans[1];
            ret[1] = trans[0];

            byte[] protc = BitConverter.GetBytes(modbus.ProtocolId);
            ret[2] = protc[1];
            ret[3] = protc[0];

            byte[] length = BitConverter.GetBytes(modbus.Length);
            ret[4] = length[1];
            ret[5] = length[0];

            ret[6] = (byte)(modbus.UnitId);

            ret[7] = (byte)(modbus.FunctionCode);

            byte[] start_address = BitConverter.GetBytes(modbus.StartAddress);
            ret[8] = start_address[1];
            ret[9] = start_address[0];

            byte[] quantity = BitConverter.GetBytes(modbus.Quantity);
            ret[10] = quantity[1];
            ret[11] = quantity[0];

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
                for (int i = 0; i < response[8]; i+=2)
                {
                    Tuple<PointType, ushort> tuple = Tuple.Create(PointType.ANALOG_INPUT, modbus.StartAddress);
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