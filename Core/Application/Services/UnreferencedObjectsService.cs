using System;
using System.Collections.Generic;
using System.Linq;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common.Objects;
using Artech.Udm.Framework.References;

namespace Acme.Packages.Menu.Core.Application.Services
{
    public class UnreferencedObjectsService : IUnreferencedObjectsService
    {
        public List<KBObject> GetUnreferencedObjects(KBModel model)
        {
            if (model == null) return new List<KBObject>();

            var reachableObjects = new HashSet<Guid>();
            var queue = new Queue<KBObject>();

            // 1. Identificar objetos MAIN
            foreach (KBObject obj in model.Objects.GetAll())
            {
                if (IsMain(obj))
                {
                    if (!reachableObjects.Contains(obj.Guid))
                    {
                        reachableObjects.Add(obj.Guid);
                        queue.Enqueue(obj);
                    }
                }
            }

            // 2. Recorrer el grafo de llamadas
            while (queue.Count > 0)
            {
                KBObject current = queue.Dequeue();
                
                // Obtener referencias salientes (A quién llama este objeto)
                // Usamos GetReferences con LinkType.UsedObject para capturar llamadas y usos
                foreach (var refItem in current.GetReferences())
                {
                    if (refItem.To == null) continue;

                    // Resolvemos el objeto destino usando la EntityKey (refItem.To)
                    KBObject targetObj = model.Objects.Get(refItem.To);
                    if (targetObj == null) continue;

                    Guid targetGuid = targetObj.Guid;
                    
                    if (!reachableObjects.Contains(targetGuid))
                    {
                        if (IsCallableOrRelevant(targetObj))
                        {
                            reachableObjects.Add(targetGuid);
                            queue.Enqueue(targetObj);
                        }
                    }
                }
            }

            // 3. Obtener todos los objetos relevantes y restar los alcanzables
            var unreferenced = new List<KBObject>();
            foreach (KBObject obj in model.Objects.GetAll())
            {
                if (IsCallableOrRelevant(obj) && !reachableObjects.Contains(obj.Guid))
                {
                    unreferenced.Add(obj);
                }
            }

            return unreferenced;
        }

        public int DeleteUnreferencedObjects(List<KBObject> objectsToDelete)
        {
            if (objectsToDelete == null || objectsToDelete.Count == 0) return 0;
            
            int count = 0;
            foreach (var obj in objectsToDelete)
            {
                try
                {
                    UIServices.Objects.Delete(obj);
                    count++;
                }
                catch { }
            }
            return count;
        }

        private bool IsMain(KBObject obj)
        {
            try
            {
                // Propiedad IsMain existe en objetos invocables
                object val = obj.GetPropertyValue("IsMain");
                if (val is bool b) return b;
                if (val is int i) return i == 1;
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool IsCallableOrRelevant(KBObject obj)
        {
            if (obj == null) return false;
            
            // Filtramos tipos que queremos analizar
            // Procedure, WebPanel, Transaction, SDT, DataProvider, WorkPanel
            if (obj is Procedure || 
                obj is WebPanel || 
                obj is Transaction || 
                obj is DataProvider ||
                obj is WorkPanel) // Agregar más según necesidad
            {
                return true;
            }
            return false;
        }
    }
}