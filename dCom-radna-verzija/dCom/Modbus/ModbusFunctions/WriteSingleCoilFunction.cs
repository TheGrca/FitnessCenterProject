﻿using Common;
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

            byte[] trans = BitConverter.GetBytes(modbus.TransactionId);
            ret[0] = trans[1];
            ret[1] = trans[0];

            byte[] protocol = BitConverter.GetBytes(modbus.ProtocolId);
            ret[2] = protocol[1];
            ret[3] = protocol[0];

            byte[] length = BitConverter.GetBytes(modbus.Length);
            ret[4] = length[1];
            ret[5] = length[0];
            ret[6] = (byte)(modbus.UnitId);
            ret[7] = (byte)(modbus.FunctionCode);

            byte[] output_address = BitConverter.GetBytes(modbus.OutputAddress);
            ret[8] = output_address[1];
            ret[9] = output_address[0];

            byte[] value = BitConverter.GetBytes(modbus.Value);
            ret[10] = value[1];
            ret[11] = value[0];

            return ret;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusWriteCommandParameters modbus = (ModbusWriteCommandParameters)CommandParameters;

            if(response.Length <= 9)
            {
                Console.WriteLine("Message not valid");
            }
            else
            {
                Tuple<PointType, ushort> tuple = Tuple.Create(PointType.ANALOG_OUTPUT, modbus.OutputAddress);
                byte[] bytes = new byte[2];

                bytes[0] = response[10];
                bytes[1] = response[9];
                ret.Add(tuple, (ushort)BitConverter.ToInt16(bytes, 0));
            }

            return ret;
        }
    }
}