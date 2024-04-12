using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            byte[] ret = new byte[12];
            ModbusWriteCommandParameters modbus = (ModbusWriteCommandParameters)CommandParameters;

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

            byte[] OutputAddress = BitConverter.GetBytes(modbus.OutputAddress);
            ret[8] = OutputAddress[1];
            ret[9] = OutputAddress[0];

            byte[] Value = BitConverter.GetBytes(modbus.Value);
            ret[10] = Value[1];
            ret[11] = Value[0];

            return ret;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusWriteCommandParameters modbus = (ModbusWriteCommandParameters)CommandParameters;

            if (response.Length <= 9)
            {
                Console.WriteLine("Not valid message!");
            }
            else
            {
                for (int i = 0; i < response[8]; i += 2)
                {
                    Tuple<PointType, ushort> tuple = Tuple.Create(PointType.DIGITAL_OUTPUT, modbus.OutputAddress);
                    byte[] bytes = new byte[1];

                    bytes[0] = response[9 + i];
                    string bits = "";
                    foreach (byte b in bytes)
                    {
                        string bit = Convert.ToString(b, 2).PadLeft(8, '0');
                        bits += bit;
                    }
                    ret.Add(tuple, (ushort)Convert.ToUInt16(bits, 2));
                }
            }

            return ret;
        }
    }
}