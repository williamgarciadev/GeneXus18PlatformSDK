using Artech.Architecture.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Types;
using Artech.Udm.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.Packages.Menu.Utilities
{
    public class DataTypeHelper
    {
        public static List<string> GetAllDataTypes()
        {
            KBModel model = UIServices.KB.CurrentModel;
            if (model == null)
            {
                CommonServices.Output.AddLine("No se encontró la KB actual.");
                return new List<string>();
            }

            // Obtener todos los DataTypes en la KB
            List<string> dataTypes = new List<string>();

            try
            {
                var allTypes = DataType.GetTypes(model, typeof(KBObject));

                foreach (var dt in allTypes)
                {
                    dataTypes.Add(dt.Name.ObjectName);
                }

                // Mostrar los Data Types en la consola de salida
                foreach (var dt in dataTypes)
                {
                    CommonServices.Output.AddLine($"Data Type encontrado: {dt}");
                }
            }
            catch (Exception ex)
            {
                CommonServices.Output.AddLine($"Error al obtener los Data Types: {ex.Message}");
            }

            return dataTypes;
        }
    }
}
