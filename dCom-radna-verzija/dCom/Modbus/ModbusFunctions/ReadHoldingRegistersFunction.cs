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
            //TO DO: IMPLEMENT
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusCommandParameters modbus = (ModbusCommandParameters)CommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if(response.Length < 9)
            {
                Console.WriteLine("Poruka nije validna");
            }
            else
            {
                for(int i = 0; i < response[9]; i += 2)
                {
                    Tuple<PointType, ushort> tuple = Tuple.Create(PointType.ANALOG_OUTPUT); /////);
                    byte[] bytes = new byte[2];
                    bytes[0] = response[10 + i];
                    bytes[1] = response[9 + i];
                    ret.Add(tuple, (ushort)BitConverter.ToInt16(bytes, 0));
                }
            }

            return ret;

        }
    }
}