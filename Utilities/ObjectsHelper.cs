
using Artech.Architecture.UI.Framework.Objects;
using Artech.Architecture.UI.Framework.Services;
using System;

namespace Acme.Packages.Menu
{
    internal class ObjectsHelper
    {

        private static void SetDocumentDirty(IGxDocument doc)
        {
            if (UIServices.Environment.InvokeRequired)
                UIServices.Environment.BeginInvoke((Action)(() => ObjectsHelper.SetDocumentDirty(doc)));
            else
                doc.Dirty = true;
        }

        //public static void Unreachables()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    bool success = true;
        //    string str1 = "KBDoctor - Unreachable Objects";
        //    output.StartSection(str1);
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[4]
        //    {
        //"Object",
        //"Type",
        //"Description",
        //"Remove"
        //    });
        //    KBObjectCollection objectCollection1 = new KBObjectCollection();
        //    KBObjectCollection objectCollection2 = new KBObjectCollection();
        //    foreach (KBObject allMember in KBCategory.Get(kb.CurrentModel, "Main Programs").AllMembers)
        //        ObjectsHelper.MarkReachables(output, allMember, objectCollection1);
        //    foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //    {
        //        if (kbObject is ICallableObject | kbObject is Artech.Genexus.Common.Objects.Attribute | kbObject is Table | kbObject is Domain | kbObject is ExternalObject | kbObject is Artech.Genexus.Common.Objects.SDT)
        //            objectCollection2.Add(kbObject);
        //    }
        //    int count1 = objectCollection2.Count;
        //    objectCollection2.RemoveAll((IEnumerable<KBObject>)objectCollection1);
        //    int count2 = objectCollection2.Count;
        //    output.AddLine("(Re)creating KBDoctor.Unreachable category");
        //    KBCategory category = KBCategory.Get(kb.CurrentModel, "KBDoctor.UnReachable");
        //    if ((KBObject)category == (KBObject)null)
        //    {
        //        category = new KBCategory(kb.CurrentModel);
        //        category.Name = "KBDoctor.UnReachable";
        //        category.Description = "Category for unreachable objects";
        //        category.ShowInModelTree = true;
        //        BLServices.TeamDevClient.IgnoreForCommit(category.Model, category.Key);
        //        category.Save();
        //    }
        //    foreach (KBObject allMember in category.AllMembers)
        //    {
        //        output.AddLine("Removing " + allMember.Name + " from  KBDoctor.Unreachable category");
        //        allMember.RemoveCategory((Entity)category);
        //        if (!allMember.GetPropertyValue<bool>("GenerateObject") && allMember is Procedure | allMember is WebPanel | allMember is WorkPanel | allMember is Artech.Genexus.Common.Objects.Transaction | allMember is DataProvider | allMember is Menubar)
        //            allMember.SetPropertyValue("GenerateObject", (object)true);
        //        allMember.Save();
        //    }
        //    foreach (KBObject kbObject in (BaseCollection<KBObject>)objectCollection2)
        //    {
        //        bool flag = false;
        //        IGxDocument documentInfo;
        //        if (UIServices.DocumentManager.IsOpenDocument(kbObject, out documentInfo))
        //        {
        //            documentInfo.Object.AddCategory((Entity)category);
        //            ObjectsHelper.SetDocumentDirty(documentInfo);
        //            UIServices.TrackSelection.OnSelectChange((object)documentInfo.Object, (object)null);
        //        }
        //        else
        //        {
        //            if (!category.ContainsMember(kbObject))
        //            {
        //                kbObject.AddCategory((Entity)category);
        //                flag = true;
        //            }
        //            if (kbObject.GetPropertyValue<bool>("GenerateObject") && kbObject is Procedure | kbObject is WebPanel | kbObject is WorkPanel | kbObject is Artech.Genexus.Common.Objects.Transaction | kbObject is DataProvider)
        //            {
        //                kbObject.SetPropertyValue("GenerateObject", (object)false);
        //                flag = true;
        //            }
        //            string str2 = Functions.linkObject(kbObject);
        //            string str3 = "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;RemoveObject&guid=" + kbObject.Guid.ToString() + "\">Remove</a>";
        //            kbDoctorXmlWriter.AddTableData(new string[4]
        //            {
        //    str2,
        //    kbObject.TypeDescriptor.Name,
        //    kbObject.Description,
        //    str3
        //            });
        //            if (flag)
        //            {
        //                try
        //                {
        //                    output.AddLine(kbObject.TypeDescriptor.Name + "-" + kbObject.Name + " is unreachable (SAVING) ");
        //                    kbObject.Save();
        //                }
        //                catch
        //                {
        //                    output.AddWarningLine("Error saving " + kbObject.TypeDescriptor.Name + "-" + kbObject.Name);
        //                    success = false;
        //                }
        //            }
        //            else
        //                output.AddLine(kbObject.TypeDescriptor.Name + "-" + kbObject.Name + " is unreachable ");
        //        }
        //    }
        //    output.AddLine("");
        //    output.AddLine("Total Objects:" + count1.ToString() + ". Unreachable Objects: " + count2.ToString());
        //    output.EndSection(str1, success);
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //}

        //private static void MarkReachables(
        //  IOutputService output,
        //  KBObject obj,
        //  KBObjectCollection reachablesObjects)
        //{
        //    IKBService kb = UIServices.KB;
        //    ObjectsHelper.WriteOutputLine(output, obj);
        //    reachablesObjects.Add(obj);
        //    foreach (EntityReference reference in obj.GetReferences())
        //    {
        //        KBObject kbObject = KBObject.Get(obj.Model, reference.To);
        //        if (kbObject != (KBObject)null && !reachablesObjects.Contains(kbObject))
        //        {
        //            string name = obj.Name;
        //            output.AddLine(obj.Name + " reference to(" + reference.ReferenceType.ToString() + "):" + kbObject.Name);
        //            ObjectsHelper.MarkReachables(output, kbObject, reachablesObjects);
        //        }
        //    }
        //}

        //private static void WriteOutputLine(IOutputService output, KBObject obj)
        //{
        //    output.AddLine("Processing.. " + obj.TypeDescriptor.DefaultName + "-" + obj.Name + ".");
        //}

        //public static void ObjectsMainsCalled()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Object main called by others";
        //    output.StartSection(str);
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    kbDoctorXmlWriter.AddTableHeader(new string[8]
        //    {
        //"Type",
        //"Object",
        //"Description",
        //"Parm",
        //"#Callers",
        //"Call Protocol",
        //"Generated by Pattern",
        //" Encrypt Parameters "
        //    });
        //    foreach (KBObject allMember in KBCategory.Get(kb.CurrentModel, "Main Programs").AllMembers)
        //    {
        //        int num = 0;
        //        foreach (EntityReference entityReference in allMember.GetReferencesTo(1))
        //            ++num;
        //        string ruleParm = Functions.ExtractRuleParm(allMember);
        //        string propertyValueString = allMember.GetPropertyValueString("USE_ENCRYPTION");
        //        kbDoctorXmlWriter.AddTableData(new string[8]
        //        {
        //  allMember.TypeDescriptor.Name,
        //  Functions.linkObject(allMember),
        //  allMember.Description,
        //  ruleParm,
        //  num.ToString(),
        //  allMember.GetPropertyValueString("CALL_PROTOCOL"),
        //  ObjectsHelper.isGeneratedbyPattern(allMember).ToString(),
        //  propertyValueString
        //        });
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    bool success = true;
        //    output.EndSection(str, success);
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //}

        //public static void ParmWOInOut()
        //{
        //    IKBService kb = UIServices.KB;
        //    List<KBObject> kbObjectList = Concepto.Packages.KBDoctorCore.Sources.API.ObjectsWithoutINOUT(UIServices.KB.CurrentKB, CommonServices.Output);
        //    string title = "KBDoctor - Object with parameters without IN:/OUT:/INOUT:";
        //    string outputFile = Functions.CreateOutputFile(kb, title);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(title);
        //    kbDoctorXmlWriter.AddTableHeader(new string[6]
        //    {
        //"Folder",
        //"Object",
        //"Description",
        //"Param rule",
        //"Timestamp",
        //"Mains"
        //    });
        //    foreach (KBObject kbObject in kbObjectList)
        //    {
        //        string ruleParm = Functions.ExtractRuleParm(kbObject);
        //        string str1 = Functions.linkObject(kbObject);
        //        KBObjectCollection objectCollection = new KBObjectCollection();
        //        string str2 = "";
        //        kbDoctorXmlWriter.AddTableData(new string[6]
        //        {
        //  kbObject.Parent.Name,
        //  str1,
        //  kbObject.Description,
        //  ruleParm,
        //  kbObject.Timestamp.ToString(),
        //  str2
        //        });
        //    }
        //    kbDoctorXmlWriter.AddTableData(new string[4]
        //    {
        //"#Objects with problems ",
        //kbObjectList.Count.ToString(),
        //"",
        //""
        //    });
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //}

        //public static bool isGenerated(KBObject obj)
        //{
        //    if (obj is DataSelector)
        //        return true;
        //    object propertyValue = obj.GetPropertyValue("GenerateObject");
        //    return propertyValue != null && propertyValue.ToString() == "True";
        //}

        //public static void ObjectsWithParmAndCommitOnExit()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - Object with parameters and Commit on Exit = Yes";
        //    output.StartSection(str1);
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[4]
        //    {
        //"Type",
        //"Name",
        //"Description",
        //"isGenerated?"
        //    });
        //    foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //    {
        //        if (kbObject is ICallableObject callableObject)
        //        {
        //            output.AddLine("Processing " + kbObject.TypeDescriptor.Name + " " + kbObject.Name);
        //            bool flag = false;
        //            foreach (Artech.Genexus.Common.Objects.Signature signature in callableObject.GetSignatures())
        //            {
        //                foreach (Parameter parameter in signature.Parameters)
        //                    flag = true;
        //            }
        //            if (flag)
        //            {
        //                object propertyValue = kbObject.GetPropertyValue("CommitOnExit");
        //                if (propertyValue != null && propertyValue.ToString() == "Yes")
        //                {
        //                    string str2 = ObjectsHelper.isGenerated(kbObject) ? "Yes" : string.Empty;
        //                    string str3 = Functions.linkObject(kbObject);
        //                    kbDoctorXmlWriter.AddTableData(new string[4]
        //                    {
        //        kbObject.TypeDescriptor.Name,
        //        str3,
        //        kbObject.Description,
        //        str2
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //}

        //public static void ObjetNotCalled()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - Not referenced objects";
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    output.StartSection(str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[5]
        //    {
        //"Type",
        //"Object",
        //"Remove",
        //"is generated?",
        //"isMain?"
        //    });
        //    bool flag;
        //    do
        //    {
        //        flag = false;
        //        foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //        {
        //            if (kbObject is ICallableObject | kbObject is Artech.Genexus.Common.Objects.Attribute | kbObject is Table | kbObject is Domain | kbObject is ExternalObject | kbObject is Image | kbObject is Artech.Genexus.Common.Objects.SDT)
        //            {
        //                int num = 0;
        //                foreach (EntityReference entityReference in kbObject.GetReferencesTo(1))
        //                    ++num;
        //                if (num == 0)
        //                {
        //                    string str2 = !(kbObject is Artech.Genexus.Common.Objects.Transaction | kbObject is Table | kbObject is Artech.Genexus.Common.Objects.Attribute | kbObject is Domain | kbObject is Image) ? "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;RemoveObject&guid=" + kbObject.Guid.ToString() + "\">Remove</a>" : "";
        //                    string str3 = Functions.linkObject(kbObject);
        //                    string str4 = Utility.IsMain(kbObject) ? "Main" : string.Empty;
        //                    string str5 = ObjectsHelper.isGenerated(kbObject) ? "Yes" : string.Empty;
        //                    if (!Utility.IsMain(kbObject))
        //                    {
        //                        if (str2 != "")
        //                        {
        //                            try
        //                            {
        //                                kbObject.Delete();
        //                                output.AddLine("REMOVING..." + kbObject.Name);
        //                                str2 = "REMOVED!";
        //                                str3 = kbObject.Name;
        //                                flag = true;
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                            }
        //                        }
        //                        kbDoctorXmlWriter.AddTableData(new string[5]
        //                        {
        //          kbObject.TypeDescriptor.Name,
        //          str3,
        //          str2,
        //          str5,
        //          str4
        //                        });
        //                    }
        //                    if (kbObject is Artech.Genexus.Common.Objects.Transaction && kbObject.GetPropertyValue<bool>("GenerateObject"))
        //                    {
        //                        try
        //                        {
        //                            kbObject.SetPropertyValue("GenerateObject", (object)false);
        //                            Concepto.Packages.KBDoctorCore.Sources.API.CleanKBObject(kbObject, output);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    while (flag);
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //}

        //public static void RemovableTransactions()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - Removable Transactions";
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    output.StartSection(str1);
        //    KBDoctorXMLWriter writer = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    writer.AddHeader(str1);
        //    writer.AddTableHeader(new string[5]
        //    {
        //"Table",
        //"Object",
        //"Remove",
        //"is generated?",
        //"isMain?"
        //    });
        //    foreach (Artech.Genexus.Common.Objects.Transaction trn in Artech.Genexus.Common.Objects.Transaction.GetAll(kb.CurrentModel))
        //    {
        //        if (!ObjectsHelper.isGenerated((KBObject)trn))
        //        {
        //            output.AddLine("Procesing... " + trn.Name);
        //            bool isRemovable;
        //            bool isRemovableWithWarning;
        //            string lstTrns;
        //            KBObjectCollection attExclusive;
        //            ObjectsHelper.CheckIfRemovable(output, writer, trn, out isRemovable, out isRemovableWithWarning, out lstTrns, out attExclusive);
        //            Guid guid;
        //            if (isRemovable)
        //            {
        //                output.AddLine("Procesing... " + trn.Name + " REMOVABLE ");
        //                guid = trn.Guid;
        //                string str2 = "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;RemoveObject&guid=" + guid.ToString() + "\">Remove</a>";
        //                writer.AddTableData(new string[5]
        //                {
        //      "",
        //      Functions.linkObject((KBObject) trn),
        //      str2,
        //      trn.Description,
        //      ""
        //                });
        //            }
        //            else if (isRemovableWithWarning)
        //            {
        //                output.AddLine("Procesing... " + trn.Name + " REMOVABLE with warning ");
        //                guid = trn.Guid;
        //                string str3 = "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;RemoveObject&guid=" + guid.ToString() + "\">Remove</a>";
        //                writer.AddTableData(new string[5]
        //                {
        //      "",
        //      Functions.linkObject((KBObject) trn),
        //      str3,
        //      trn.Description,
        //      lstTrns + " WHIT WARNING"
        //                });
        //            }
        //            else if (attExclusive.Count > 0)
        //            {
        //                string str4 = "";
        //                foreach (KBObject kbObject in (BaseCollection<KBObject>)attExclusive)
        //                    str4 = Functions.linkObject(kbObject) + " ";
        //                writer.AddTableData(new string[5]
        //                {
        //      "",
        //      Functions.linkObject((KBObject) trn),
        //      "Exclusive Attributes:" + str4,
        //      trn.Description,
        //      "NOT REMOVABLE"
        //                });
        //            }
        //        }
        //    }
        //    writer.AddFooter();
        //    writer.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //}

        //private static void CheckIfRemovable(
        //  IOutputService output,
        //  KBDoctorXMLWriter writer,
        //  Artech.Genexus.Common.Objects.Transaction trn,
        //  out bool isRemovable,
        //  out bool isRemovableWithWarning,
        //  out string lstTrns,
        //  out KBObjectCollection attExclusive)
        //{
        //    isRemovable = true;
        //    isRemovableWithWarning = true;
        //    lstTrns = "";
        //    attExclusive = new KBObjectCollection();
        //    foreach (Artech.Genexus.Common.Parts.TransactionLevel level in trn.Structure.GetLevels())
        //    {
        //        bool flag = true;
        //        Table associatedTable = level.AssociatedTable;
        //        string name = associatedTable.Name;
        //        KBObjectCollection objectCollection1 = new KBObjectCollection();
        //        KBObjectCollection objectCollection2 = ObjectsHelper.AttributesFromGeneratedTransactions(associatedTable);
        //        KBObjectCollection objectCollection3 = new KBObjectCollection();
        //        KBObjectCollection objectCollection4 = ObjectsHelper.AttributesFromAllTransactionsExceptOne(associatedTable, trn, out lstTrns);
        //        foreach (Artech.Genexus.Common.Parts.TransactionAttribute attribute1 in level.Structure.GetAttributes())
        //        {
        //            Artech.Genexus.Common.Objects.Attribute attribute2 = (Artech.Genexus.Common.Objects.Attribute)attribute1;
        //            if (!objectCollection2.Contains((KBObject)attribute2))
        //            {
        //                output.AddLine("Transaction " + trn.Name + " Table " + name + " LVL " + level.Name + " Attribute " + attribute2.Name + " not in any generated transaction");
        //                isRemovable = false;
        //                flag = false;
        //            }
        //            if (!objectCollection4.Contains((KBObject)attribute2))
        //            {
        //                output.AddLine("Transaction " + trn.Name + " Table " + name + " LVL " + level.Name + " Attribute " + attribute2.Name + " not in any other transaction");
        //                isRemovableWithWarning = false;
        //                flag = false;
        //                attExclusive.Add((KBObject)attribute2);
        //            }
        //        }
        //        if (flag)
        //            writer.AddTableData(new string[5]
        //            {
        //    name,
        //    Functions.linkObject((KBObject) trn),
        //    level.Name,
        //    trn.Description,
        //    "Level Removable"
        //            });
        //    }
        //}

        //internal static void ChangeCommitOnExit()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - Object referenced by object ";
        //    output.StartSection(str1);
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter1 = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter1.AddHeader(str1);
        //    kbDoctorXmlWriter1.AddTableHeader(new string[8]
        //    {
        //"Object",
        //"Type",
        //"Description",
        //"Commit on Exit",
        //"Update DB?",
        //"Commit in Source",
        //"Timestamp",
        //"Last Update"
        //    });
        //    string str2 = "";
        //    string str3 = "";
        //    foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //    {
        //        if (kbObject is Procedure)
        //        {
        //            object propertyValue = kbObject.GetPropertyValue("CommitOnExit");
        //            if (propertyValue != null)
        //                str2 = propertyValue.ToString() == "Yes" ? "YES" : " ";
        //            string str4 = CleanKBHelper.ObjectUpdateDB(kbObject) ? "YES" : "";
        //            Procedure procedure = (Procedure)kbObject;
        //            try
        //            {
        //                str3 = !Functions.ExtractComments(procedure.ProcedurePart.Source.ToString().ToUpper()).Contains("COMMIT") ? "" : "YES ";
        //            }
        //            catch (Exception ex)
        //            {
        //                output.AddErrorLine(ex.Message);
        //            }
        //            finally
        //            {
        //                str3 = "";
        //            }
        //            if (str4 == "" & str2 == "YES")
        //            {
        //                kbObject.SetPropertyValue("CommitOnExit", (object)"No");
        //                kbObject.Save();
        //                KBDoctorXMLWriter kbDoctorXmlWriter2 = kbDoctorXmlWriter1;
        //                string[] datos = new string[8]
        //                {
        //      Functions.linkObject(kbObject),
        //      kbObject.TypeDescriptor.Name,
        //      kbObject.Description,
        //      str2,
        //      str4,
        //      str3,
        //      null,
        //      null
        //                };
        //                DateTime dateTime = kbObject.Timestamp;
        //                datos[6] = dateTime.ToString();
        //                dateTime = kbObject.LastUpdate;
        //                datos[7] = dateTime.ToString();
        //                kbDoctorXmlWriter2.AddTableData(datos);
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter1.AddFooter();
        //    kbDoctorXmlWriter1.Close();
        //    bool success = true;
        //    output.EndSection(str1, success);
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //}

        //internal static void TreeCommit()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Tree Commit  ";
        //    output.StartSection(str);
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    KBDoctorXMLWriter writer = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    writer.AddHeader(str);
        //    writer.AddTableHeader(new string[7]
        //    {
        //"Object",
        //"Commit",
        //"Description",
        //"in New UTL",
        //"Upd/Ins/Del",
        //"TimeStamp",
        //"Modified Tables"
        //    });
        //    string Anidacion = "";
        //    KBObjectCollection yaIncluido = new KBObjectCollection();
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    KBModel currentModel = UIServices.KB.CurrentModel;
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Procedure>());
        //    foreach (Procedure selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //        ObjectsHelper.GraboLlamado(selectObject, Anidacion, yaIncluido, writer);
        //    writer.AddFooter();
        //    writer.Close();
        //    bool success = true;
        //    output.EndSection(str, success);
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //}

        //private static void GraboLlamado(
        //  Procedure obj,
        //  string Anidacion,
        //  KBObjectCollection yaIncluido,
        //  KBDoctorXMLWriter writer)
        //{
        //    if (yaIncluido.Contains((KBObject)obj))
        //    {
        //        writer.AddTableData(new string[7]
        //        {
        //  Anidacion + Functions.linkObject((KBObject) obj),
        //  "",
        //  "----already included ",
        //  "",
        //  "",
        //  "",
        //  ""
        //        });
        //    }
        //    else
        //    {
        //        string str1 = "";
        //        string str2 = "";
        //        object propertyValue1 = obj.GetPropertyValue("CommitOnExit");
        //        if (propertyValue1 != null)
        //            str1 = propertyValue1.ToString() == "Yes" ? "YES" : " ";
        //        string str3 = CleanKBHelper.ObjectUpdateDB((KBObject)obj) ? "YES" : "";
        //        Procedure procedure = obj;
        //        string str4 = "";
        //        try
        //        {
        //            str4 = Functions.ExtractComments(procedure.ProcedurePart.Source.ToString().ToUpper());
        //        }
        //        catch
        //        {
        //        }
        //        string str5 = !str4.Contains("COMMIT") ? "" : "YES";
        //        if (str1 == "YES" && str3 == "YES" || str5 == "YES")
        //            str2 = "YES";
        //        string str6 = "";
        //        object propertyValue2 = obj.GetPropertyValue("EXECUTE_IN_NEW_UTL");
        //        if (propertyValue2 != null)
        //            str6 = propertyValue2.ToString() == "True" ? "YES" : "";
        //        KBModel model = obj.Model;
        //        IList<KBObject> list = (IList<KBObject>)model.GetReferencesFrom(obj.Key, 1).Where<EntityReference>((Func<EntityReference, bool>)(r => r.ReferenceType == Artech.Udm.Framework.References.ReferenceType.WeakExternal)).Where<EntityReference>((Func<EntityReference, bool>)(r => ReferenceTypeInfo.HasUpdateAccess(r.LinkTypeInfo) | ReferenceTypeInfo.HasDeleteAccess(r.LinkTypeInfo) | ReferenceTypeInfo.HasInsertAccess(r.LinkTypeInfo))).Select<EntityReference, KBObject>((Func<EntityReference, KBObject>)(r => model.Objects.Get(r.To))).ToList<KBObject>();
        //        string str7 = "";
        //        string str8 = "";
        //        foreach (KBObject kbObject in (IEnumerable<KBObject>)list)
        //        {
        //            str7 = str7 + str8 + Functions.linkObject(kbObject);
        //            str8 = ",";
        //        }
        //        writer.AddTableData(new string[7]
        //        {
        //  Anidacion + Functions.linkObject((KBObject) obj),
        //  str2,
        //  obj.Description,
        //  str6,
        //  str3,
        //  obj.Timestamp.ToShortDateString(),
        //  str7
        //        });
        //        yaIncluido.Add((KBObject)obj);
        //        ObjectsHelper.Parse(obj.Model, (KBObject)obj, Anidacion, yaIncluido, writer);
        //    }
        //}

        //public static void Parse(
        //  KBModel modelo,
        //  KBObject obj,
        //  string Anidacion,
        //  KBObjectCollection yaIncluido,
        //  KBDoctorXMLWriter writer)
        //{
        //    IParserEngine engine = (Artech.Architecture.Common.Services.Services.GetService(new Guid("C26F529E-9A69-4df5-B825-9194BA3983A3")) as ILanguageService).CreateEngine();
        //    ProcedurePart part = obj.Parts.Get<ProcedurePart>();
        //    if (!((KBPart<KBObject>)part != (KBPart<KBObject>)null))
        //        return;
        //    ParserInfo info = new ParserInfo((KBObjectPart)part);
        //    int num = 0;
        //    foreach (TokenData token in engine.GetTokens(true, info, part.Source))
        //    {
        //        if (token.Token == 27)
        //            num = token.Id;
        //        if (token.Token == 28)
        //        {
        //            string word = token.Word;
        //            CommonServices.Output.AddLine(Anidacion + num.ToString() + "-" + word);
        //            EntityKey key = new EntityKey(ObjClass.Procedure, token.Id);
        //            ObjectsHelper.GraboLlamado((Procedure)KBObject.Get(obj.Model, key), Anidacion + "____", yaIncluido, writer);
        //        }
        //        if (token.Token == 132)
        //            writer.AddTableData(new string[8]
        //            {
        //    Anidacion + " COMMIT ",
        //    "",
        //    "Commit Explicito",
        //    "",
        //    "",
        //    "",
        //    "",
        //    ""
        //            });
        //    }
        //}

        //private static KBObjectCollection AttributesFromGeneratedTransactions(Table tBL)
        //{
        //    KBObjectCollection objectCollection = new KBObjectCollection();
        //    foreach (Artech.Genexus.Common.Objects.Transaction associatedTransaction in (IEnumerable<Artech.Genexus.Common.Objects.Transaction>)tBL.AssociatedTransactions)
        //    {
        //        if (ObjectsHelper.isGenerated((KBObject)associatedTransaction))
        //        {
        //            foreach (Artech.Genexus.Common.Parts.TransactionLevel level in associatedTransaction.Structure.GetLevels())
        //            {
        //                if ((KBObject)level.AssociatedTable == (KBObject)tBL)
        //                {
        //                    foreach (Artech.Genexus.Common.Parts.TransactionAttribute attribute1 in level.Structure.GetAttributes())
        //                    {
        //                        Artech.Genexus.Common.Objects.Attribute attribute2 = (Artech.Genexus.Common.Objects.Attribute)attribute1;
        //                        objectCollection.Add((KBObject)attribute2);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return objectCollection;
        //}

        //private static KBObjectCollection AttributesFromAllTransactionsExceptOne(
        //  Table tBL,
        //  Artech.Genexus.Common.Objects.Transaction trnToExclude,
        //  out string lstTrn)
        //{
        //    KBObjectCollection objectCollection = new KBObjectCollection();
        //    lstTrn = "";
        //    foreach (Artech.Genexus.Common.Objects.Transaction associatedTransaction in (IEnumerable<Artech.Genexus.Common.Objects.Transaction>)tBL.AssociatedTransactions)
        //    {
        //        if ((KBObject)associatedTransaction != (KBObject)trnToExclude)
        //        {
        //            lstTrn = lstTrn + (object)associatedTransaction.QualifiedName + " ";
        //            foreach (Artech.Genexus.Common.Parts.TransactionLevel level in associatedTransaction.Structure.GetLevels())
        //            {
        //                if ((KBObject)level.AssociatedTable == (KBObject)tBL)
        //                {
        //                    foreach (Artech.Genexus.Common.Parts.TransactionAttribute attribute1 in level.Structure.GetAttributes())
        //                    {
        //                        Artech.Genexus.Common.Objects.Attribute attribute2 = (Artech.Genexus.Common.Objects.Attribute)attribute1;
        //                        objectCollection.Add((KBObject)attribute2);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return objectCollection;
        //}

        //public static void MainTableUsed()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Tables used by mains";
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    output.StartSection(str);
        //    KBDoctorXMLWriter writer = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    writer.AddHeader(str);
        //    writer.AddTableHeader(new string[3]
        //    {
        //"Main",
        //"Table",
        //"Operation"
        //    });
        //    foreach (KBObject allMember in KBCategory.Get(kb.CurrentModel, "Main Programs").AllMembers)
        //    {
        //        StringCollection tableOperation = new StringCollection();
        //        KBObjectCollection objMarked = new KBObjectCollection();
        //        output.AddLine(" ");
        //        output.AddLine(" =============================> " + allMember.Name);
        //        string mainstr = allMember.Name + " " + allMember.GetPropertyValueString("AppLocation");
        //        ObjectsHelper.TablesUsed(output, allMember, tableOperation, objMarked, mainstr, writer);
        //    }
        //    writer.AddFooter();
        //    writer.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str, success);
        //}

        //private static void TablesUsed(
        //  IOutputService output,
        //  KBObject obj,
        //  StringCollection tableOperation,
        //  KBObjectCollection objMarked,
        //  string mainstr,
        //  KBDoctorXMLWriter writer)
        //{
        //    IKBService kb = UIServices.KB;
        //    objMarked.Add(obj);
        //    foreach (EntityReference reference in obj.GetReferences())
        //    {
        //        KBObject kbObject = KBObject.Get(obj.Model, reference.To);
        //        if (kbObject != (KBObject)null && !objMarked.Contains(kbObject))
        //        {
        //            if (Utility.IsMain(kbObject))
        //                break;
        //            bool read;
        //            bool insert;
        //            bool update;
        //            bool delete;
        //            if (reference.ReferenceType == Artech.Udm.Framework.References.ReferenceType.WeakExternal && kbObject is Table && ReferenceTypeInfo.ReadTableInfo(reference.LinkTypeInfo, out read, out insert, out update, out delete, out bool _))
        //            {
        //                if (read)
        //                {
        //                    string str = mainstr + " , " + kbObject.Name + " , SELECT";
        //                    if (!tableOperation.Contains(str))
        //                    {
        //                        output.AddLine(str);
        //                        tableOperation.Add(str);
        //                        writer.AddTableData(new string[3]
        //                        {
        //          mainstr,
        //          kbObject.Name,
        //          "SELECT"
        //                        });
        //                    }
        //                }
        //                if (insert)
        //                {
        //                    string str = mainstr + " , " + kbObject.Name + " , INSERT";
        //                    if (!tableOperation.Contains(str))
        //                    {
        //                        output.AddLine(str);
        //                        tableOperation.Add(str);
        //                        writer.AddTableData(new string[3]
        //                        {
        //          mainstr,
        //          kbObject.Name,
        //          "INSERT"
        //                        });
        //                    }
        //                }
        //                if (update)
        //                {
        //                    string str = mainstr + " , " + kbObject.Name + " , UPDATE";
        //                    if (!tableOperation.Contains(str))
        //                    {
        //                        output.AddLine(str);
        //                        tableOperation.Add(str);
        //                        writer.AddTableData(new string[3]
        //                        {
        //          mainstr,
        //          kbObject.Name,
        //          "UPDATE"
        //                        });
        //                    }
        //                }
        //                if (delete)
        //                {
        //                    string str = mainstr + " , " + kbObject.Name + " , DELETE";
        //                    if (!tableOperation.Contains(str))
        //                    {
        //                        output.AddLine(str);
        //                        tableOperation.Add(str);
        //                        writer.AddTableData(new string[3]
        //                        {
        //          mainstr,
        //          kbObject.Name,
        //          "DELETE"
        //                        });
        //                    }
        //                }
        //            }
        //            ObjectsHelper.TablesUsed(output, kbObject, tableOperation, objMarked, mainstr, writer);
        //        }
        //    }
        //}

        //public static void CreateDeployUnits()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    SpecificationListHelper specificationListHelper = new SpecificationListHelper(kb.CurrentModel.Environment.TargetModel);
        //    string str1 = "KBDoctor - CreateDeployUnits";
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    output.StartSection(str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[4]
        //    {
        //"AppLocation",
        //"Main",
        //"Object",
        //"WIN/WEB"
        //    });
        //    StringCollection tableOperation = new StringCollection();
        //    foreach (KBObject allMember in KBCategory.Get(kb.CurrentModel, "Main Programs").AllMembers)
        //    {
        //        string str2 = (string)allMember.GetProperty("AppLocation").Value;
        //        string Dircopia;
        //        string str3;
        //        if (allMember.GetPropertyValueString("AppGenerator").ToUpper().Contains("WEB"))
        //        {
        //            Dircopia = ".\\Web\\";
        //            str3 = ".dll";
        //        }
        //        else
        //        {
        //            Dircopia = ".\\";
        //            str3 = ".exe";
        //        }
        //        string str4 = "";
        //        if (allMember is Procedure)
        //            str4 = "a";
        //        if (allMember is WorkPanel)
        //            str4 = "u";
        //        if (allMember is Artech.Genexus.Common.Objects.Transaction)
        //            str4 = "";
        //        KBObjectCollection objMarked = new KBObjectCollection();
        //        output.AddLine(" ");
        //        output.AddLine("ECHO  **** " + allMember.Name + " ***** ");
        //        if (allMember.GetPropertyValueString("AppLocation") == "")
        //        {
        //            PromptDescription promptDescription = new PromptDescription("Insert Location for (" + allMember.TypeDescriptor.Name + ") " + allMember.Name + "-" + allMember.Description);
        //            if (promptDescription.ShowDialog() == DialogResult.OK)
        //            {
        //                allMember.SetPropertyValue("AppLocation", (object)promptDescription.Description);
        //                allMember.Save();
        //            }
        //        }
        //        string propertyValueString = allMember.GetPropertyValueString("AppLocation");
        //        string str5 = "XCOPY /y/d " + Dircopia + "bin\\" + str4 + allMember.Name + str3 + " %" + propertyValueString + "%\\bin\\";
        //        output.AddLine(str5);
        //        tableOperation.Add(str5);
        //        int num;
        //        switch (allMember)
        //        {
        //            case Procedure _:
        //            case WorkPanel _:
        //                num = 0;
        //                break;
        //            default:
        //                num = Dircopia == ".\\Web\\" ? 1 : 0;
        //                break;
        //        }
        //        if (num != 0)
        //        {
        //            string str6 = "XCOPY /y/d " + Dircopia + str4 + allMember.Name + ".js %" + propertyValueString + "%";
        //            output.AddLine(str6);
        //            tableOperation.Add(str6);
        //        }
        //        ObjectsHelper.WriteCopyObject(output, allMember, tableOperation, objMarked, propertyValueString, Dircopia);
        //    }
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //}

        //internal static void SplitMainObject()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    output.StartSection("Split Main Object");
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    KBModel currentModel = UIServices.KB.CurrentModel;
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Procedure>());
        //    foreach (Procedure selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //    {
        //        string name = selectObject.Name;
        //        object propertyValue = selectObject.GetPropertyValue("CALL_PROTOCOL");
        //        selectObject.SetPropertyValue("CALL_PROTOCOL", (object)"Internal");
        //        selectObject.SetPropertyValue("isMain", (object)false);
        //        selectObject.SetPropertyValue("ObjectVisibility", (object)ObjectVisibility.Public);
        //        selectObject.Name = name + "_core";
        //        selectObject.Save();
        //        Procedure procedure = new Procedure(currentModel);
        //        foreach (Property property in selectObject.Properties)
        //        {
        //            if (!property.IsDefault)
        //                procedure.SetPropertyValue(property.Name, property.Value);
        //        }
        //        procedure.SetPropertyValue("Name", (object)name);
        //        procedure.SetPropertyValue("isMain", (object)true);
        //        procedure.SetPropertyValue("CALL_PROTOCOL", propertyValue);
        //        selectObject.SetPropertyValue("ObjectVisibility", (object)ObjectVisibility.Private);
        //        string ruleParm = Functions.ExtractRuleParm((KBObject)selectObject);
        //        string str = "";
        //        if (ruleParm != "")
        //        {
        //            procedure.Rules.Source = ruleParm + ";";
        //            str = ruleParm.ToLower().Replace("parm", "").Replace("in:", "").Replace("out:", "").Replace("inout:", "").Replace("(", "").Replace(")", "");
        //        }
        //        procedure.ProcedurePart.Source = name + "_core.call(" + str + ") ";
        //        foreach (Variable variable in selectObject.Variables.Variables)
        //        {
        //            if (!variable.IsStandard)
        //                procedure.Variables.Add(variable);
        //        }
        //        try
        //        {
        //            output.AddWarningLine("Create new: " + procedure.Name + " Protocol: " + propertyValue);
        //            procedure.Save();
        //        }
        //        catch (Exception ex)
        //        {
        //            output.AddErrorLine(ex.Message + " - " + (object)ex.InnerException);
        //            output.AddErrorLine("Can't save object" + name + ". Try to save commented");
        //            output.AddLine("Parm = " + ruleParm + " Parm2= " + str);
        //            procedure.ProcedurePart.Source = "//" + name + "_core.call(" + str + ") ";
        //            procedure.Rules.Source = "//" + ruleParm + ";";
        //            procedure.Save();
        //        }
        //    }
        //    output.EndSection("Split Main Object", true);
        //}

        //public static void CalculateCheckSum()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    SpecificationListHelper specificationListHelper = new SpecificationListHelper(kb.CurrentModel.Environment.TargetModel);
        //    string sectionName = "KBDoctor - Generate objects in text format";
        //    output.StartSection(sectionName);
        //    string str1 = string.Format("{0:yyyy-MM-dd-HHmm}", (object)DateTime.Now);
        //    string str2 = KBDoctorHelper.ObjComparerDirectory(kb) + "\\OBJ-" + str1 + "\\";
        //    Directory.CreateDirectory(str2);
        //    StringCollection stringCollection = new StringCollection();
        //    int num = 0;
        //    foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //    {
        //        if (!(kbObject is Domain | kbObject is Artech.Genexus.Common.Objects.Theme | kbObject is DataView | kbObject is Index | kbObject is KBCategory | kbObject is DataProvider | kbObject is Menubar | kbObject is DataView | kbObject is Diagram | kbObject is Folder | kbObject is Image | kbObject is ExternalObject | kbObject is ThemeClass | kbObject is ThemeColor | kbObject is DataViewIndex))
        //        {
        //            ++num;
        //            if (num % 200 == 0)
        //                output.AddLine(kbObject.GetFullName());
        //            ObjectsHelper.WriteObjectToTextFile(kbObject, str2);
        //        }
        //    }
        //    bool success = true;
        //    output.EndSection(sectionName, success);
        //}

        //public static void GenerateLocationXML()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    SpecificationListHelper specificationListHelper = new SpecificationListHelper(kb.CurrentModel.Environment.TargetModel);
        //    string sectionName = "KBDoctor - Genenrate Location.xml";
        //    string str = kb.CurrentKB.UserDirectory + "\\Location.xml";
        //    if (File.Exists(str))
        //        File.Delete(str);
        //    output.StartSection(sectionName);
        //    output.AddLine("Generate Location.xml template in " + str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(str, Encoding.UTF8);
        //    kbDoctorXmlWriter.WriteStartElement("GXLocations");
        //    KBCategory.Get(kb.CurrentModel, "Main Programs");
        //    foreach (ExternalObject externalObject in (IEnumerable<ExternalObject>)kb.CurrentModel.GetObjects<ExternalObject>())
        //    {
        //        if (externalObject.GetPropertyValueString("ExoType") == "WSDL")
        //        {
        //            kbDoctorXmlWriter.WriteStartElement("GXLocation");
        //            kbDoctorXmlWriter.WriteAttributeString("name", externalObject.Name);
        //            kbDoctorXmlWriter.WriteStartElement("Common");
        //            kbDoctorXmlWriter.WriteElementString("Host", "www.server.com");
        //            kbDoctorXmlWriter.WriteElementString("Port", "80");
        //            kbDoctorXmlWriter.WriteElementString("BaseUrl", "/baseUrl/");
        //            kbDoctorXmlWriter.WriteElementString("Secure", "");
        //            kbDoctorXmlWriter.WriteElementString("Timeout", "60");
        //            kbDoctorXmlWriter.WriteElementString("CancelOnError", "1");
        //            kbDoctorXmlWriter.WriteElementString("Proxyserverhost", "www.proxy.com");
        //            kbDoctorXmlWriter.WriteElementString("Proxyserverport", "80");
        //            kbDoctorXmlWriter.WriteEndElement();
        //            kbDoctorXmlWriter.WriteEndElement();
        //            kbDoctorXmlWriter.WriteStartElement("HTTP");
        //            kbDoctorXmlWriter.WriteStartElement("Authentication");
        //            kbDoctorXmlWriter.WriteElementString("Authenticationmethod", "0=Basic;1:Digest,2:NTML,3:Kerberos");
        //            kbDoctorXmlWriter.WriteElementString("Authenticationrealm", "domain");
        //            kbDoctorXmlWriter.WriteElementString("Authenticationuser", "user");
        //            kbDoctorXmlWriter.WriteElementString("Authenticationpassword", "pass");
        //            kbDoctorXmlWriter.WriteEndElement();
        //            kbDoctorXmlWriter.WriteStartElement("Proxyauthentication");
        //            kbDoctorXmlWriter.WriteElementString("Proxyauthenticationmethod", "0=Basic;1:Digest,2:NTML,3:Kerberos");
        //            kbDoctorXmlWriter.WriteElementString("Proxyauthenticationrealm", "proxydomain");
        //            kbDoctorXmlWriter.WriteElementString("Proxyauthenticationuser", "proxyuser");
        //            kbDoctorXmlWriter.WriteElementString("Proxyauthenticationpassword", "proxypass");
        //            kbDoctorXmlWriter.WriteEndElement();
        //            kbDoctorXmlWriter.WriteEndElement();
        //        }
        //    }
        //    kbDoctorXmlWriter.WriteEndElement();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(str);
        //    bool success = true;
        //    output.EndSection(sectionName, success);
        //}

        //private static void WriteObjectToTextFile(KBObject obj, string newDir)
        //{
        //    string str1 = obj.GetFullName().Replace("'", "").Replace(":", "_").Replace("/", "_").Replace("\\", "_").Replace(" ", "_");
        //    StreamWriter file = new StreamWriter(newDir + str1 + ".txt");
        //    WebPanel webPanel;
        //    switch (obj.TypeDescriptor.Name)
        //    {
        //        case "Attribute":
        //            Artech.Genexus.Common.Objects.Attribute a = (Artech.Genexus.Common.Objects.Attribute)obj;
        //            file.WriteLine(Functions.ReturnPicture(a));
        //            if (a.Formula == null)
        //            {
        //                file.WriteLine("");
        //                break;
        //            }
        //            file.WriteLine(a.Formula.ToString());
        //            break;
        //        case "Procedure":
        //            ProcedurePart procedurePart = obj.Parts.Get<ProcedurePart>();
        //            if ((KBPart<KBObject>)procedurePart != (KBPart<KBObject>)null)
        //            {
        //                file.WriteLine("=== PROCEDURE SOURCE ===");
        //                file.WriteLine(procedurePart.Source);
        //                break;
        //            }
        //            break;
        //        case "SDT":
        //            Artech.Genexus.Common.Objects.SDT sdt = (Artech.Genexus.Common.Objects.SDT)obj;
        //            if ((KBObject)sdt != (KBObject)null)
        //            {
        //                file.WriteLine("=== STRUCTURE ===");
        //                ObjectsHelper.ListStructure(sdt.SDTStructure.Root, 0, file);
        //                break;
        //            }
        //            break;
        //        case "Table":
        //            using (IEnumerator<TableAttribute> enumerator = ((Table)obj).TableStructure.Attributes.GetEnumerator())
        //            {
        //                while (enumerator.MoveNext())
        //                {
        //                    TableAttribute current = enumerator.Current;
        //                    string str2 = (!current.IsKey ? " " : "*") + current.Name + "  " + current.GetPropertiesObject().GetPropertyValueString("DataTypeString") + "-" + current.GetPropertiesObject().GetPropertyValueString("Formula");
        //                    if (current.IsExternalRedundant)
        //                        str2 += " External_Redundant";
        //                    string str3 = str2 + " Null=" + (object)current.IsNullable;
        //                    if (current.IsRedundant)
        //                        str3 += " Redundant";
        //                    file.WriteLine(str3);
        //                }
        //                break;
        //            }
        //        case "Transaction":
        //            StructurePart structurePart = obj.Parts.Get<StructurePart>();
        //            if ((KBPart<KBObject>)structurePart != (KBPart<KBObject>)null)
        //            {
        //                file.WriteLine("=== STRUCTURE ===");
        //                file.WriteLine(structurePart.ToString());
        //            }
        //            EventsPart eventsPart1 = obj.Parts.Get<EventsPart>();
        //            if ((KBPart<KBObject>)eventsPart1 != (KBPart<KBObject>)null)
        //            {
        //                file.WriteLine("=== EVENTS SOURCE ===");
        //                file.WriteLine(eventsPart1.Source);
        //                break;
        //            }
        //            break;
        //        case "WebComponent":
        //            webPanel = (WebPanel)obj;
        //            EventsPart eventsPart2 = obj.Parts.Get<EventsPart>();
        //            if ((KBPart<KBObject>)eventsPart2 != (KBPart<KBObject>)null)
        //            {
        //                file.WriteLine("=== EVENTS SOURCE ===");
        //                file.WriteLine(eventsPart2.Source);
        //                break;
        //            }
        //            break;
        //        case "WebPanel":
        //            webPanel = (WebPanel)obj;
        //            EventsPart eventsPart3 = obj.Parts.Get<EventsPart>();
        //            if ((KBPart<KBObject>)eventsPart3 != (KBPart<KBObject>)null)
        //            {
        //                file.WriteLine("=== EVENTS SOURCE ===");
        //                file.WriteLine(eventsPart3.Source);
        //                break;
        //            }
        //            break;
        //        case "WorkPanel":
        //            EventsPart eventsPart4 = obj.Parts.Get<EventsPart>();
        //            if ((KBPart<KBObject>)eventsPart4 != (KBPart<KBObject>)null)
        //            {
        //                file.WriteLine("=== EVENTS SOURCE ===");
        //                file.WriteLine(eventsPart4.Source);
        //                break;
        //            }
        //            break;
        //    }
        //    file.Close();
        //}

        //private static void ListStructure(SDTLevel level, int tabs, StreamWriter file)
        //{
        //    ObjectsHelper.WriteTabs(tabs, file);
        //    file.Write(level.Name);
        //    if (level.IsCollection)
        //        file.Write(", collection: {0}", (object)level.CollectionItemName);
        //    file.WriteLine();
        //    foreach (SDTItem sdtItem in level.GetItems<SDTItem>())
        //        ObjectsHelper.ListItem(sdtItem, tabs + 1, file);
        //    foreach (SDTLevel level1 in level.GetItems<SDTLevel>())
        //        ObjectsHelper.ListStructure(level1, tabs + 1, file);
        //}

        //private static void ListItem(SDTItem item, int tabs, StreamWriter file)
        //{
        //    ObjectsHelper.WriteTabs(tabs, file);
        //    string str = item.Type.ToString().Substring(0, 1) + "(" + item.Length.ToString() + (item.Decimals > 0 ? "." + item.Decimals.ToString() : "") + ")" + (item.Signed ? "-" : "");
        //    file.WriteLine("{0}, {1}, {2} {3}", (object)item.Name, (object)str, (object)item.Description, item.IsCollection ? (object)(", collection " + item.CollectionItemName) : (object)"");
        //}

        //private static void WriteTabs(int tabs, StreamWriter file)
        //{
        //    while (tabs-- > 0)
        //        file.Write('\t');
        //}

        //private static void WriteCopyObject(
        //  IOutputService output,
        //  KBObject obj,
        //  StringCollection tableOperation,
        //  KBObjectCollection objMarked,
        //  string mainstr,
        //  string Dircopia)
        //{
        //    IKBService kb = UIServices.KB;
        //    string str = string.Format("XCOPY /y/d {0}bin\\{1}.dll %{2}%\\bin\\", (object)Dircopia, (object)obj.Name, (object)mainstr);
        //    int num1;
        //    switch (obj)
        //    {
        //        case Procedure _:
        //        case WorkPanel _:
        //            num1 = 0;
        //            break;
        //        default:
        //            num1 = !tableOperation.Contains(str) ? 1 : 0;
        //            break;
        //    }
        //    if (num1 != 0)
        //    {
        //        output.AddLine(str);
        //        output.AddLine(string.Format("XCOPY /y/d {0}{1}.js  %{2}%\\ ", (object)Dircopia, (object)obj.Name, (object)mainstr));
        //        tableOperation.Add(str);
        //    }
        //    objMarked.Add(obj);
        //    foreach (EntityReference reference in obj.GetReferences())
        //    {
        //        KBObject kbObject = KBObject.Get(obj.Model, reference.To);
        //        if (kbObject != (KBObject)null && !objMarked.Contains(kbObject))
        //        {
        //            if (Utility.IsMain(kbObject))
        //                break;
        //            int num2;
        //            switch (kbObject)
        //            {
        //                case Artech.Genexus.Common.Objects.Transaction _:
        //                case WorkPanel _:
        //                case WebPanel _:
        //                case Menubar _:
        //                case Procedure _:
        //                case DataProvider _:
        //                    num2 = 1;
        //                    break;
        //                default:
        //                    num2 = kbObject is DataSelector ? 1 : 0;
        //                    break;
        //            }
        //            if (num2 != 0)
        //                ObjectsHelper.WriteCopyObject(output, kbObject, tableOperation, objMarked, mainstr, Dircopia);
        //        }
        //    }
        //}

        //public static void ObjectsComplex()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Complex Objects";
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    output.StartSection(str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    kbDoctorXmlWriter.AddTableHeader(new string[4]
        //    {
        //"Object",
        //"Description",
        //"Type",
        //"Size"
        //    });
        //    foreach (string file in Directory.GetFiles(KBDoctorHelper.SpcDirectory(kb), "*.SP0", SearchOption.AllDirectories))
        //    {
        //        long length = new FileInfo(file).Length;
        //        string withoutExtension = Path.GetFileNameWithoutExtension(file);
        //        foreach (KBObject kbObject in UIServices.KB.CurrentModel.Objects.GetByPartialName((IEnumerable<string>)new string[1]
        //        {
        //  "Objects"
        //        }, withoutExtension))
        //        {
        //            if (kbObject.Name == withoutExtension && length > 200000L)
        //            {
        //                kbDoctorXmlWriter.AddTableData(new string[4]
        //                {
        //      Functions.linkObject(kbObject),
        //      kbObject.Description,
        //      kbObject.TypeDescriptor.Name,
        //      length.ToString("N0")
        //                });
        //                output.AddLine(file);
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str, success);
        //}

        //public static void ObjectsLegacyCode()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string[] titles = new string[15]
        //    {
        //"Object",
        //"Description",
        //"Type",
        //" call",
        //" udp",
        //" create",
        //".false",
        //".true",
        //"new",
        //"defined",
        //"delete",
        //".and.",
        //".or.",
        //".not.",
        //".like."
        //    };
        //    string str = "KBDoctor - Objects - Legacy Code";
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    output.StartSection(str);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    kbDoctorXmlWriter.AddTableHeader(titles);
        //    int num1 = 0;
        //    foreach (KBObject kbObject in UIServices.KB.CurrentModel.Objects.GetAll())
        //    {
        //        int num2;
        //        switch (kbObject)
        //        {
        //            case Artech.Genexus.Common.Objects.Transaction _:
        //            case WebPanel _:
        //            case Procedure _:
        //                num2 = 1;
        //                break;
        //            default:
        //                num2 = kbObject is WorkPanel ? 1 : 0;
        //                break;
        //        }
        //        if (num2 != 0 && ObjectsHelper.isGenerated(kbObject))
        //        {
        //            output.AddLine(kbObject.Name);
        //            string comments = Functions.ExtractComments(Functions.RemoveEmptyLines(ObjectsHelper.ObjectSource(kbObject)));
        //            bool flag = false;
        //            string[] datos = new string[titles.Length];
        //            datos[0] = Functions.linkObject(kbObject);
        //            datos[1] = kbObject.Description;
        //            datos[2] = kbObject.TypeDescriptor.Name;
        //            for (int index = 3; index < titles.Length; ++index)
        //            {
        //                if (comments.Contains(titles[index].ToUpper()))
        //                {
        //                    datos[index] = "   **  ";
        //                    flag = true;
        //                }
        //                else
        //                    datos[index] = "";
        //            }
        //            if (flag)
        //            {
        //                kbDoctorXmlWriter.AddTableData(datos);
        //                ++num1;
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str, success);
        //    Functions.AddLineSummary(str + ".txt", num1.ToString());
        //}

        //public static void EditLegacyCodeToReplace()
        //{
        //    Process.Start("notepad.exe", ObjectsHelper.CreateFileWithTextToTeplace());
        //}

        //public static void EditReviewObjects()
        //{
        //    Concepto.Packages.KBDoctorCore.Sources.API.InitializeIniFile(UIServices.KB.CurrentKB);
        //    Process.Start("notepad.exe", UIServices.KB.CurrentKB.UserDirectory + "\\KBDoctor.ini");
        //}

        //private static string CreateFileWithTextToTeplace()
        //{
        //    string path = UIServices.KB.CurrentKB.UserDirectory + "\\Replace.txt";
        //    if (!File.Exists(path))
        //        File.WriteAllText(path, Comparer.Replace);
        //    return path;
        //}

        //public static void ChangeLegacyCode()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    bool success = true;
        //    Regex[] regexArray = new Regex[9]
        //    {
        //new Regex("([\\b]*)([^.])([c][a][l][l][(])([\\s]*[a-z][0-9a-z_]+)[\\s]*[,]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        //new Regex("([\\b]*)([^.])([u][d][p][(])([\\s]*[a-z][0-9a-z_]+)[\\s]*[,]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        //new Regex("([\\b]*)([^.])([c][r][e][a][t][e][(])([\\s]*[a-z][0-9a-z_]+)[\\s]*[,]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        //new Regex("([\\b]*)([^.])([l][i][n][k][(])([\\s]*[a-z][0-9a-z_]+)[\\s]*[,]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        //new Regex("([\\b]*)([^.])([c][a][l][l][(])([\\s]*[a-z][0-9a-z_]+)[\\s]*[,]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        //new Regex("([\\b]*)([^.])([u][d][p][(])([\\s]*[a-z][0-9a-z_]+)[\\s]*[,]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        //new Regex("([\\b]*)([^.])([c][r][e][a][t][e][(])([\\s]*[a-z][0-9a-z_]+)[\\s]*[,]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        //new Regex("([\\b]*)([^.])([l][i][n][k][(])([\\s]*[a-z][0-9a-z_]+)[\\s]*[,]", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        //new Regex("^([c][a][l][l][(])([\\s]*[a-z][0-9a-z_]+)(,|\\))", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled)
        //    };
        //    string[] strArray1 = new string[9]
        //    {
        //"$1$2$4.$3",
        //"$1$2$4.$3",
        //"$1$2$4.$3",
        //"$1$2$4.$3",
        //"$1$2$4.$3)",
        //"$1$2$4.$3)",
        //"$1$2$4.$3)",
        //"$1$2$4.$3)",
        //"$2.$1"
        //    };
        //    string[] strArray2 = File.ReadAllLines(ObjectsHelper.CreateFileWithTextToTeplace());
        //    string[,] strArray3 = new string[100, 2];
        //    int index1 = 0;
        //    foreach (string str in strArray2)
        //    {
        //        if (!str.StartsWith("#") && str.Contains("|"))
        //        {
        //            string[] strArray4 = str.Split('|');
        //            strArray3[index1, 0] = strArray4[0];
        //            strArray3[index1, 1] = strArray4[1];
        //            ++index1;
        //        }
        //    }
        //    string sectionName = "KBDoctor - Change code to improve readability";
        //    output.StartSection(sectionName);
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    KBModel currentModel = UIServices.KB.CurrentModel;
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Procedure>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Artech.Genexus.Common.Objects.Transaction>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<WebPanel>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<WorkPanel>());
        //    foreach (KBObject selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //    {
        //        int num;
        //        switch (selectObject)
        //        {
        //            case Artech.Genexus.Common.Objects.Transaction _:
        //            case WebPanel _:
        //            case Procedure _:
        //                num = 1;
        //                break;
        //            default:
        //                num = selectObject is WorkPanel ? 1 : 0;
        //                break;
        //        }
        //        if (num != 0 && ObjectsHelper.isGenerated(selectObject) && !ObjectsHelper.isGeneratedbyPattern(selectObject))
        //        {
        //            string str1 = ObjectsHelper.ObjectSource(selectObject);
        //            string str2 = str1;
        //            output.AddLine("Object " + selectObject.Name);
        //            ObjectsHelper.ChangeUDPCallWhenNecesary(selectObject);
        //            for (int index2 = 0; index2 < 9; ++index2)
        //                str2 = regexArray[index2].Replace(str2, strArray1[index2]);
        //            if (!str2.ToLower().Contains("java") && !str2.ToLower().Contains("csharp"))
        //            {
        //                for (int index3 = 0; index3 < strArray3.Length / 2; ++index3)
        //                    str2 = str2.Replace(strArray3[index3, 0], strArray3[index3, 1], StringComparison.InvariantCultureIgnoreCase);
        //                if (str1 != str2)
        //                {
        //                    try
        //                    {
        //                        output.AddLine("..Saving.." + selectObject.Name);
        //                        ObjectsHelper.SaveNewSource(selectObject, str2);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        output.AddErrorLine(ex.Message);
        //                        output.AddErrorLine("========= newsource ===============");
        //                        output.AddLine(str2);
        //                        success = false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    output.EndSection(sectionName, success);
        //}

        //private static string ReplaceLegacyCode(string newSource, string original, string changeto)
        //{
        //    newSource = newSource.Replace(original, changeto, StringComparison.CurrentCultureIgnoreCase);
        //    return newSource;
        //}

        //private static void SaveNewSource(KBObject obj, string newSource)
        //{
        //    if (obj is Procedure)
        //    {
        //        Procedure procedure = (Procedure)obj;
        //        procedure.ProcedurePart.Source = newSource;
        //        procedure.Save();
        //    }
        //    if (obj is WebPanel)
        //    {
        //        WebPanel webPanel = (WebPanel)obj;
        //        webPanel.Events.Source = newSource;
        //        webPanel.Save();
        //    }
        //    if (obj is Artech.Genexus.Common.Objects.Transaction)
        //    {
        //        Artech.Genexus.Common.Objects.Transaction transaction = (Artech.Genexus.Common.Objects.Transaction)obj;
        //        transaction.Events.Source = newSource;
        //        transaction.Save();
        //    }
        //    if (!(obj is WorkPanel))
        //        return;
        //    WorkPanel workPanel = (WorkPanel)obj;
        //    workPanel.Events.Source = newSource;
        //    workPanel.Save();
        //}

        //private static string ReplaceOneLegacy(string myString, string v)
        //{
        //    string str1 = "";
        //    int length = myString.ToLower().IndexOf(v);
        //    int startIndex = length + v.Length;
        //    string str2 = myString.Substring(startIndex, myString.Length - startIndex).Trim().Replace("(", "");
        //    string str3 = "";
        //    string[] strArray = str2.Split(',', ')', ' ');
        //    int index = 0;
        //    if (index < strArray.Length)
        //        str3 = strArray[index];
        //    if (str3 != "" && str3.Substring(0, 1) != "&")
        //    {
        //        string str4 = str2.Substring(str3.Length, str2.Length - str3.Length);
        //        if (str4 != "" && str4.Substring(0, 1) == ",")
        //            str4 = str4.Substring(1, str4.Length - 1);
        //        str1 = myString.Substring(0, length) + " " + str3 + "." + v.Trim() + "(" + str4;
        //    }
        //    return str1;
        //}

        //public static void ObjectsWithConstants()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Objects with Constants";
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    output.StartSection(str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    kbDoctorXmlWriter.AddTableHeader(new string[4]
        //    {
        //"Object",
        //"Description",
        //"Line",
        //"Constant"
        //    });
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Procedure>());
        //    ILanguageService service = Artech.Architecture.Common.Services.Services.GetService(new Guid("C26F529E-9A69-4df5-B825-9194BA3983A3")) as ILanguageService;
        //    IParserEngine2 engine = GenexusBLServices.Language.CreateEngine() as IParserEngine2;
        //    foreach (KBObject selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //    {
        //        ProcedurePart part = selectObject.Parts.Get<ProcedurePart>();
        //        if ((KBPart<KBObject>)part != (KBPart<KBObject>)null)
        //        {
        //            ParserInfo parserInfo = new ParserInfo((KBObjectPart)part);
        //        }
        //    }
        //}

        //public static void CountTableAccess()
        //{
        //    IKBService kb = UIServices.KB;
        //    KBModel model = kb.CurrentModel;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Count Table Access per Object";
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    output.StartSection(str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    kbDoctorXmlWriter.AddTableHeader(new string[9]
        //    {
        //"Object",
        //"Description",
        //"Type",
        //"Module",
        //"Inserts",
        //"Updates",
        //"Delete",
        //"Read",
        //"Total"
        //    });
        //    foreach (KBObject kbObject in UIServices.KB.CurrentModel.Objects.GetAll())
        //    {
        //        if (Functions.isRunable(kbObject))
        //        {
        //            output.AddLine(kbObject.Name);
        //            int count1 = model.GetReferencesFrom(kbObject.Key, 1).Where<EntityReference>((Func<EntityReference, bool>)(r => r.ReferenceType == Artech.Udm.Framework.References.ReferenceType.WeakExternal)).Where<EntityReference>((Func<EntityReference, bool>)(r => ReferenceTypeInfo.HasUpdateAccess(r.LinkTypeInfo))).Select<EntityReference, KBObject>((Func<EntityReference, KBObject>)(r => model.Objects.Get(r.To))).ToList<KBObject>().Count;
        //            int count2 = model.GetReferencesFrom(kbObject.Key, 1).Where<EntityReference>((Func<EntityReference, bool>)(r => r.ReferenceType == Artech.Udm.Framework.References.ReferenceType.WeakExternal)).Where<EntityReference>((Func<EntityReference, bool>)(r => ReferenceTypeInfo.HasInsertAccess(r.LinkTypeInfo))).Select<EntityReference, KBObject>((Func<EntityReference, KBObject>)(r => model.Objects.Get(r.To))).ToList<KBObject>().Count;
        //            int count3 = model.GetReferencesFrom(kbObject.Key, 1).Where<EntityReference>((Func<EntityReference, bool>)(r => r.ReferenceType == Artech.Udm.Framework.References.ReferenceType.WeakExternal)).Where<EntityReference>((Func<EntityReference, bool>)(r => ReferenceTypeInfo.HasDeleteAccess(r.LinkTypeInfo))).Select<EntityReference, KBObject>((Func<EntityReference, KBObject>)(r => model.Objects.Get(r.To))).ToList<KBObject>().Count;
        //            int count4 = model.GetReferencesFrom(kbObject.Key, 1).Where<EntityReference>((Func<EntityReference, bool>)(r => r.ReferenceType == Artech.Udm.Framework.References.ReferenceType.WeakExternal)).Where<EntityReference>((Func<EntityReference, bool>)(r => ReferenceTypeInfo.HasReadAccess(r.LinkTypeInfo))).Select<EntityReference, KBObject>((Func<EntityReference, KBObject>)(r => model.Objects.Get(r.To))).ToList<KBObject>().Count;
        //            int num = count1 + count2 + count3 + count4;
        //            kbDoctorXmlWriter.AddTableData(new string[9]
        //            {
        //    Functions.linkObject(kbObject),
        //    kbObject.TypeDescriptor.Name,
        //    kbObject.Description,
        //    kbObject.Module.Name,
        //    count1.ToString(),
        //    count2.ToString(),
        //    count3.ToString(),
        //    count4.ToString(),
        //    num.ToString()
        //            });
        //        }
        //    }
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str, success);
        //}

        //public static void ObjectsRefactoringCandidates()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - Refactoring candidates";
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    output.StartSection(str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[16]
        //    {
        //"Object",
        //"Description",
        //"Type",
        //"Module",
        //"Folder",
        //"Parm/IN:OUT:",
        //"#Parameters",
        //"Code commented?",
        //"% Comments",
        //"# Comments",
        //"# Lines",
        //"Max Nest",
        //"Longest Code Block",
        //"Cyclomatic Complexity",
        //"Candidate",
        //"Complexity Index"
        //    });
        //    int num1 = 0;
        //    int num2 = 0;
        //    foreach (KBObject kbObject in UIServices.KB.CurrentModel.Objects.GetAll())
        //    {
        //        int num3;
        //        switch (kbObject)
        //        {
        //            case Artech.Genexus.Common.Objects.Transaction _:
        //            case WebPanel _:
        //            case Procedure _:
        //                num3 = 1;
        //                break;
        //            default:
        //                num3 = kbObject is WorkPanel ? 1 : 0;
        //                break;
        //        }
        //        if (num3 != 0 && ObjectsHelper.isGenerated(kbObject) && !ObjectsHelper.isGeneratedbyPattern(kbObject))
        //        {
        //            output.AddLine(kbObject.Name);
        //            string source = Functions.RemoveEmptyLines(Functions.ObjectSourceUpper(kbObject));
        //            string str2 = Functions.RemoveEmptyLines(Functions.ExtractComments(source));
        //            int linesSource;
        //            int linesComment;
        //            float PercentComment;
        //            ObjectsHelper.CountCommentsLines(source, str2, out linesSource, out linesComment, out PercentComment);
        //            int MaxCodeBlock = Functions.MaxCodeBlock(str2);
        //            int MaxNestLevel = Functions.MaxNestLevel(str2);
        //            int ComplexityLevel = Functions.ComplexityLevel(str2);
        //            string ParmINOUT = Functions.ValidateINOUTinParm(kbObject) ? "Error" : "";
        //            int num4 = ObjectsHelper.ParametersCountObject(kbObject);
        //            string str3 = "";
        //            if (ParmINOUT == "Error" || MaxNestLevel > 6 || ComplexityLevel > 30 || MaxCodeBlock > 500 || num4 > 6)
        //                str3 = "*";
        //            int complexityIndex = ObjectsHelper.CalculateComplexityIndex(MaxCodeBlock, MaxNestLevel, ComplexityLevel, ParmINOUT);
        //            string name = kbObject.Parent.Name;
        //            string str4 = Functions.CodeCommented(source).Replace("'", "").Replace(">", "").Replace("<", "");
        //            kbDoctorXmlWriter.AddTableData(new string[16]
        //            {
        //    Functions.linkObject(kbObject),
        //    kbObject.Description,
        //    kbObject.TypeDescriptor.Name,
        //    kbObject.Module.Name,
        //    name,
        //    ParmINOUT,
        //    num4.ToString(),
        //    str4,
        //    PercentComment.ToString("0"),
        //    linesComment.ToString(),
        //    linesSource.ToString(),
        //    MaxNestLevel.ToString(),
        //    MaxCodeBlock.ToString(),
        //    ComplexityLevel.ToString(),
        //    str3,
        //    complexityIndex.ToString()
        //            });
        //            ++num2;
        //            num1 += complexityIndex;
        //        }
        //    }
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    int num5 = num1 / num2;
        //    kbDoctorXmlWriter.AddTableHeader(new string[5]
        //    {
        //"Totals Objects= ",
        //num2.ToString(),
        //" Complexity Index Sum= ",
        //num1.ToString(),
        //" Complexity Index Average= " + num5.ToString()
        //    });
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //    Functions.AddLineSummary(str1 + ".txt", "Totals Objects= " + num2.ToString() + " Complexity Index Sum= " + num1.ToString() + " Complexity Index Average= " + num5.ToString());
        //}

        //private static int ParametersCountObject(KBObject obj)
        //{
        //    int num = 0;
        //    if (obj is ICallableObject callableObject)
        //    {
        //        foreach (Artech.Genexus.Common.Objects.Signature signature in callableObject.GetSignatures())
        //            num = signature.ParametersCount;
        //    }
        //    return num;
        //}

        //public static void ObjectsDiagnostics()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - Diagnostics";
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    output.StartSection(str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[7]
        //    {
        //"Object",
        //"Description",
        //"Type",
        //"Error Code",
        //"Error Description",
        //"Observation",
        //"Solution"
        //    });
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    KBModel currentModel = UIServices.KB.CurrentModel;
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Procedure>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Artech.Genexus.Common.Objects.Transaction>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<WebPanel>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<WorkPanel>());
        //    foreach (KBObject selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //    {
        //        if (ObjectsHelper.isGenerated(selectObject) && !ObjectsHelper.isGeneratedbyPattern(selectObject))
        //        {
        //            string source = Functions.RemoveEmptyLines(Functions.ObjectSourceUpper(selectObject));
        //            string str2 = Functions.RemoveEmptyLines(Functions.ExtractComments(source));
        //            ObjectsHelper.CountCommentsLines(source, str2, out int _, out int _, out float _);
        //            int MaxCodeBlock = Functions.MaxCodeBlock(str2);
        //            int MaxNestLevel = Functions.MaxNestLevel(str2);
        //            int ComplexityLevel = Functions.ComplexityLevel(str2);
        //            string ParmINOUT = Functions.ValidateINOUTinParm(selectObject) ? "Error" : "";
        //            if (ParmINOUT == "Error")
        //            {
        //                string str3 = "kbd0001";
        //                string str4 = "Missing IN:OUT:INOUT: in parm rule";
        //                string ruleParm = Functions.ExtractRuleParm(selectObject);
        //                string newParm = CleanKBHelper.ChangeRuleParmWithIN(selectObject);
        //                CleanKBHelper.SaveObjectNewParm(output, selectObject, ruleParm, newParm);
        //                kbDoctorXmlWriter.AddTableData(new string[7]
        //                {
        //      Functions.linkObject(selectObject),
        //      selectObject.Description,
        //      selectObject.TypeDescriptor.Name,
        //      str3,
        //      str4,
        //      ruleParm,
        //      newParm
        //                });
        //            }
        //            string str5 = "";
        //            if (ParmINOUT == "Error" || MaxNestLevel > 10 || ComplexityLevel > 30 || MaxCodeBlock > 500)
        //                str5 = "*";
        //            int complexityIndex = ObjectsHelper.CalculateComplexityIndex(MaxCodeBlock, MaxNestLevel, ComplexityLevel, ParmINOUT);
        //            if (complexityIndex > 500)
        //            {
        //                string str6 = "kbd0002";
        //                string str7 = "KBDoctor Complexity Index too high";
        //                string str8 = "ComplexityIndex = " + complexityIndex.ToString();
        //                string str9 = "";
        //                kbDoctorXmlWriter.AddTableData(new string[7]
        //                {
        //      Functions.linkObject(selectObject),
        //      selectObject.Description,
        //      selectObject.TypeDescriptor.Name,
        //      str6,
        //      str7,
        //      str8,
        //      str9
        //                });
        //            }
        //            string str10 = ObjectsHelper.VariablesNotBasedAttributesOrDomain(selectObject);
        //            if (str10 != "")
        //            {
        //                string str11 = "kbd0003";
        //                string str12 = "Variables no based in Attributes or Domain";
        //                string str13 = str10;
        //                string str14 = "";
        //                kbDoctorXmlWriter.AddTableData(new string[7]
        //                {
        //      Functions.linkObject(selectObject),
        //      selectObject.Description,
        //      selectObject.TypeDescriptor.Name,
        //      str11,
        //      str12,
        //      str13,
        //      str14
        //                });
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //}

        //public static void ObjectsUDPCallables()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - UDP CALLABLE";
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    output.StartSection(str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[7]
        //    {
        //"Object",
        //"Description",
        //"Type",
        //"Folder",
        //"SaveDate",
        //"Param",
        //"Referenced with Call"
        //    });
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    KBModel currentModel = UIServices.KB.CurrentModel;
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Procedure>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<WorkPanel>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<WebPanel>());
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Artech.Genexus.Common.Objects.Transaction>());
        //    foreach (KBObject selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //    {
        //        output.AddLine(selectObject.Name);
        //        string str2 = ObjectsHelper.ChangeUDPCallWhenNecesary(selectObject);
        //        if (str2 != "")
        //        {
        //            string ruleParm = Functions.ExtractRuleParm(selectObject);
        //            kbDoctorXmlWriter.AddTableData(new string[7]
        //            {
        //    Functions.linkObject(selectObject),
        //    selectObject.Description,
        //    selectObject.TypeDescriptor.Name,
        //    " " + selectObject.Parent.Name,
        //    " " + selectObject.VersionDate.ToShortDateString(),
        //    ruleParm,
        //    str2
        //            });
        //        }
        //    }
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.AddTableFooterOnly();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //}

        //private static string ChangeUDPCallWhenNecesary(KBObject obj)
        //{
        //    string str = "";
        //    if (ObjectsHelper.isGenerated(obj) && !ObjectsHelper.isGeneratedbyPattern(obj))
        //    {
        //        foreach (EntityReference reference in obj.GetReferences(1))
        //        {
        //            KBObject kbObject = KBObject.Get(obj.Model, reference.To);
        //            if (ObjectsHelper.OnlyLastParameterIsOut(kbObject) && ObjectsHelper.ReplaceCallForUdpInObject(obj, kbObject.Name))
        //                str = str + " " + Functions.linkObject(kbObject);
        //        }
        //    }
        //    return str;
        //}

        //private static string ReplaceCallForUDP(KBObject obj)
        //{
        //    IOutputService output = CommonServices.Output;
        //    bool flag = false;
        //    string str = "";
        //    foreach (EntityReference entityReference in obj.GetReferencesTo(1))
        //    {
        //        KBObject kbObject = KBObject.Get(obj.Model, entityReference.From);
        //        if (Functions.ExtractComments(Functions.RemoveEmptyLines(Functions.ObjectSourceUpper(kbObject))).Replace("\t", "").Replace(" ", "").Contains("CALL(" + obj.Name.ToUpper()))
        //        {
        //            flag = true;
        //            output.AddLine(kbObject.Name + " ---> " + obj.Name);
        //            str = str + " " + Functions.linkObject(kbObject);
        //            ObjectsHelper.ReplaceCallForUdpInObject(kbObject, obj.Name);
        //        }
        //    }
        //    return str;
        //}

        //private static bool ReplaceCallForUdpInObject(KBObject obj, string name)
        //{
        //    IOutputService output = CommonServices.Output;
        //    string input = ObjectsHelper.ObjectSource(obj);
        //    string newSource = new Regex(string.Format("(\\s+)?(call\\([\\s\\b]*({0})([\\s\\b]*)(,(.*)(,[\\s\\b]*)(&([a-z0-9\\-]+))|,(.*)(&([a-z0-9\\-]+)))[\\s\\b]*\\)|(({0})(.call) ?\\(((.*)(,[\\s\\b]*)(&([a-z0-9\\-]+))|(.*)(&([a-z0-9\\-] +)))[\\s\\b]*\\)))", (object)name), RegexOptions.IgnoreCase).Replace(input, "$1$8$11$19$22 = $3$14.udp($6$17)");
        //    bool flag = false;
        //    if (input != newSource)
        //    {
        //        ObjectsHelper.SaveNewSource(obj, newSource);
        //        output.AddLine("=====================================");
        //        output.AddLine("Modified .. " + obj.Name);
        //        flag = true;
        //    }
        //    return flag;
        //}

        //private static bool OnlyLastParameterIsOut(KBObject obj)
        //{
        //    bool flag;
        //    if (obj is ICallableObject callableObject)
        //    {
        //        flag = true;
        //        foreach (Artech.Genexus.Common.Objects.Signature signature in callableObject.GetSignatures())
        //        {
        //            int parametersCount = signature.ParametersCount;
        //            Parameter[] array = signature.Parameters.ToArray<Parameter>();
        //            if (array[parametersCount - 1].Accessor.ToString() != "PARM_OUT")
        //            {
        //                flag = false;
        //            }
        //            else
        //            {
        //                for (int index = 0; index < parametersCount - 1; ++index)
        //                {
        //                    if (array[index].Accessor.ToString() != "PARM_IN")
        //                        flag = false;
        //                }
        //            }
        //        }
        //    }
        //    else
        //        flag = false;
        //    return flag;
        //}

        //private static string CompareCallParameters(KBObject obj)
        //{
        //    IOutputService output = CommonServices.Output;
        //    string comments = Functions.ExtractComments(Functions.RemoveEmptyLines(ObjectsHelper.ObjectSource(obj)));
        //    string str1 = "";
        //    foreach (EntityReference reference in obj.GetReferences())
        //    {
        //        KBObject objRef = KBObject.Get(obj.Model, reference.To);
        //        if (ObjectsHelper.IsCallalable(objRef))
        //        {
        //            StringCollection stringCollection1 = ObjectsHelper.InspectCall(objRef);
        //            using (StringReader stringReader = new StringReader(comments))
        //            {
        //                string str2;
        //                while ((str2 = stringReader.ReadLine()) != null)
        //                {
        //                    if (str2.Contains(objRef.Name))
        //                    {
        //                        string str3 = str2.Replace("\t", " ").Replace(" ", "");
        //                        str3.Replace(".call(", "(", StringComparison.CurrentCultureIgnoreCase);
        //                        str3.Replace(".udp(", "(", StringComparison.CurrentCultureIgnoreCase);
        //                        str3.Replace("call(", "(", StringComparison.CurrentCultureIgnoreCase);
        //                        str3.Replace("udp(", "(", StringComparison.CurrentCultureIgnoreCase);
        //                        str3.Replace(objRef.Name, "Ñ", StringComparison.CurrentCultureIgnoreCase);
        //                        StringCollection stringCollection2 = ObjectsHelper.ProcessingObjectCall(obj, str3);
        //                        for (int index = 0; index < stringCollection2.Count; ++index)
        //                        {
        //                            if (stringCollection2[index] != stringCollection1[index])
        //                                output.AddLine("Diferencia: " + str3 + " Parametro: " + index.ToString() + " - " + stringCollection2[index] + "-" + stringCollection1[index]);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return str1;
        //}

        //private static StringCollection ProcessingObjectCall(KBObject obj, string line)
        //{
        //    IOutputService output = CommonServices.Output;
        //    StringCollection stringCollection = new StringCollection();
        //    if (line.Contains("="))
        //    {
        //        string[] strArray = line.Split('=');
        //        line = strArray[1] + strArray[0];
        //    }
        //    char[] chArray = new char[4] { '=', ',', ')', '(' };
        //    foreach (string name in line.Split(chArray))
        //    {
        //        if (name != "")
        //        {
        //            if (name.StartsWith("&"))
        //            {
        //                stringCollection.Add(ObjectsHelper.TypeOfVariable(name.Replace("&", ""), obj));
        //            }
        //            else
        //            {
        //                foreach (KBObject a in UIServices.KB.CurrentModel.Objects.GetByName("Attributes", new Guid?(new Guid("adbb33c9-0906-4971-833c-998de27e0676")), name))
        //                {
        //                    if (a != (KBObject)null && a is Artech.Genexus.Common.Objects.Attribute)
        //                    {
        //                        string str = ObjectsHelper.TypeOfAttribute((Artech.Genexus.Common.Objects.Attribute)a);
        //                        stringCollection.Add(str);
        //                        if (str == "")
        //                            output.AddLine("__________no se pudo______________" + name);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return stringCollection;
        //}

        //private static StringCollection InspectCall(KBObject objRef)
        //{
        //    IOutputService output = CommonServices.Output;
        //    StringCollection stringCollection = new StringCollection();
        //    if (objRef is ICallableObject callableObject)
        //    {
        //        foreach (Artech.Genexus.Common.Objects.Signature signature in callableObject.GetSignatures())
        //        {
        //            foreach (Parameter parameter in signature.Parameters)
        //            {
        //                string str = ObjectsHelper.TypeOfParm(parameter, objRef);
        //                stringCollection.Add(str);
        //            }
        //        }
        //    }
        //    return stringCollection;
        //}

        //private static string TypeOfParm(Parameter parm, KBObject objRef)
        //{
        //    return !parm.IsAttribute ? ObjectsHelper.TypeOfVariable(parm.Name, objRef) : ObjectsHelper.TypeOfAttribute((Artech.Genexus.Common.Objects.Attribute)parm.Object);
        //}

        //private static string TypeOfAttribute(Artech.Genexus.Common.Objects.Attribute a)
        //{
        //    string str = "/" + Functions.ReturnPicture(a);
        //    if ((KBObject)a.DomainBasedOn != (KBObject)null)
        //        str = str + "/" + a.DomainBasedOn.Name + "//";
        //    return str;
        //}

        //private static string TypeOfVariable(string varname, KBObject objRef)
        //{
        //    string str = "";
        //    VariablesPart variablesPart = objRef.Parts.Get<VariablesPart>();
        //    if ((KBPart<KBObject>)variablesPart != (KBPart<KBObject>)null)
        //    {
        //        foreach (Variable variable in variablesPart.Variables)
        //        {
        //            if (variable.Name == varname)
        //            {
        //                str = "/" + Functions.ReturnPictureVariable(variable);
        //                if ((KBObject)variable.DomainBasedOn != (KBObject)null)
        //                    str = str + "/" + variable.DomainBasedOn.Name + "//";
        //            }
        //        }
        //    }
        //    return str;
        //}

        //private static int CalculateComplexityIndex(
        //  int MaxCodeBlock,
        //  int MaxNestLevel,
        //  int ComplexityLevel,
        //  string ParmINOUT)
        //{
        //    int num = 0;
        //    if (ParmINOUT == "Error")
        //        num += 100;
        //    return num + MaxNestLevel * MaxNestLevel + ComplexityLevel * 10 + MaxCodeBlock * 2;
        //}

        //private static void CountCommentsLines(
        //  string source,
        //  string sourceWOComments,
        //  out int linesSource,
        //  out int linesComment,
        //  out float PercentComment)
        //{
        //    linesSource = Functions.LineCount(source);
        //    int num = Functions.LineCount(sourceWOComments);
        //    linesComment = linesSource - num;
        //    PercentComment = linesSource == 0 ? 0.0f : (float)(linesComment * 100 / linesSource);
        //}

        //public static string ObjectSource(KBObject obj)
        //{
        //    string str = "";
        //    if (obj is Procedure)
        //        str = obj.Parts.Get<ProcedurePart>().Source;
        //    if (obj is Artech.Genexus.Common.Objects.Transaction)
        //        str = obj.Parts.Get<EventsPart>().Source;
        //    if (obj is WorkPanel)
        //        str = obj.Parts.Get<EventsPart>().Source;
        //    if (obj is WebPanel)
        //        str = obj.Parts.Get<EventsPart>().Source;
        //    return str;
        //}

        //private static void ParseSource(
        //  string source,
        //  out int MaxCodeBlock,
        //  out int MaxNestLevel,
        //  out int ComplexityLevel)
        //{
        //    string upper1 = source.ToUpper();
        //    ComplexityLevel = 0;
        //    using (StringReader stringReader = new StringReader(upper1))
        //    {
        //        string str;
        //        while ((str = stringReader.ReadLine()) != null)
        //        {
        //            string upper2 = str.TrimStart().ToUpper();
        //            if (upper2.StartsWith("DO WHILE") || upper2.StartsWith("IF") || upper2.StartsWith("DO CASE") || upper2.StartsWith("FOR"))
        //                ++ComplexityLevel;
        //        }
        //    }
        //    MaxNestLevel = 0;
        //    int num1 = 0;
        //    using (StringReader stringReader = new StringReader(upper1))
        //    {
        //        string str;
        //        while ((str = stringReader.ReadLine()) != null)
        //        {
        //            string upper3 = str.TrimStart().ToUpper();
        //            if (!upper3.StartsWith("DO '"))
        //            {
        //                if (upper3.StartsWith("FOR ") || upper3.StartsWith("IF ") || upper3.StartsWith("DO ") || upper3.StartsWith("NEW") || upper3.StartsWith("SUB"))
        //                {
        //                    ++num1;
        //                    MaxNestLevel = num1 > MaxNestLevel ? num1 : MaxNestLevel;
        //                }
        //                else if (upper3.StartsWith("ENDFOR") || upper3.StartsWith("ENDIF") || upper3.StartsWith("ENDDO") || upper3.StartsWith("ENDCASE") || upper3.StartsWith("ENDNEW") || upper3.StartsWith("ENDSUB"))
        //                    --num1;
        //            }
        //        }
        //    }
        //    if (num1 != 0)
        //        Console.WriteLine(num1.ToString());
        //    MaxCodeBlock = 0;
        //    int num2 = 0;
        //    using (StringReader stringReader = new StringReader(upper1))
        //    {
        //        string str;
        //        while ((str = stringReader.ReadLine()) != null)
        //        {
        //            ++num2;
        //            if (str.StartsWith("SUB ") || str.StartsWith("EVENT "))
        //            {
        //                MaxCodeBlock = MaxCodeBlock <= num2 ? num2 : MaxCodeBlock;
        //                num2 = 1;
        //            }
        //        }
        //        MaxCodeBlock = MaxCodeBlock <= num2 ? num2 : MaxCodeBlock;
        //    }
        //}

        //public static void ListProcedureCallWebPanelTransaction()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - List Procedure that call Webpanel or Transaction";
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    output.StartSection(str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    kbDoctorXmlWriter.AddTableHeader(new string[4]
        //    {
        //"Object",
        //"Description",
        //"Reference",
        //"Description"
        //    });
        //    foreach (KBObject kbObject1 in UIServices.KB.CurrentModel.Objects.GetAll())
        //    {
        //        if (kbObject1 is Procedure && kbObject1.Name != "ListPrograms" && !kbObject1.Name.Contains("Dummy"))
        //        {
        //            foreach (EntityReference reference in kbObject1.GetReferences())
        //            {
        //                KBObject kbObject2 = KBObject.Get(kbObject1.Model, reference.To);
        //                int num;
        //                if (kbObject2 != (KBObject)null)
        //                {
        //                    switch (kbObject2)
        //                    {
        //                        case Artech.Genexus.Common.Objects.Transaction _:
        //                        case WorkPanel _:
        //                            num = 1;
        //                            break;
        //                        default:
        //                            num = kbObject2 is WebPanel ? 1 : 0;
        //                            break;
        //                    }
        //                }
        //                else
        //                    num = 0;
        //                if (num != 0)
        //                {
        //                    kbDoctorXmlWriter.AddTableData(new string[4]
        //                    {
        //        Functions.linkObject(kbObject1),
        //        kbObject1.Description,
        //        Functions.linkObject(kbObject2),
        //        kbObject2.Description
        //                    });
        //                    output.AddLine(kbObject1.Name + " => " + kbObject2.Name);
        //                }
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str, success);
        //}

        //public static void RemoveObject(object[] parameters)
        //{
        //    Guid guid = Guid.Empty;
        //    foreach (Dictionary<string, string> parameter in parameters)
        //    {
        //        foreach (string g in parameter.Values)
        //        {
        //            try
        //            {
        //                guid = new Guid(g);
        //            }
        //            catch (FormatException ex)
        //            {
        //                guid = Guid.Empty;
        //            }
        //        }
        //    }
        //    KBObject kbObject = UIServices.KB.CurrentModel.Objects.Get(guid);
        //    string name1 = kbObject.TypeDescriptor.Name;
        //    string name2 = kbObject.Name;
        //    if (MessageBox.Show(string.Format("Are you sure you want to delete " + name1.Trim() + " {0}?", (object)name2), "Remove object", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        //        return;
        //    try
        //    {
        //        kbObject.Delete();
        //        int num = (int)MessageBox.Show(name1 + " " + name2 + " was successfully removed.");
        //    }
        //    catch (GxException ex)
        //    {
        //        int num = (int)MessageBox.Show(ex.Message, "Could not remove " + name1, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //    }
        //}

        //public static void OpenObject(object[] parameters)
        //{
        //    foreach (Dictionary<string, string> parameter in parameters)
        //    {
        //        int num = 0;
        //        foreach (string g in parameter.Values)
        //        {
        //            if (num == 1)
        //                UIServices.DocumentManager.OpenDocument(UIServices.KB.CurrentModel.Objects.Get(new Guid(g)), OpenDocumentOptions.CurrentVersion);
        //            ++num;
        //        }
        //    }
        //}

        //public static void OpenObjectRules(object[] parameters)
        //{
        //    foreach (Dictionary<string, string> parameter in parameters)
        //    {
        //        int num = 0;
        //        foreach (string g in parameter.Values)
        //        {
        //            if (num == 1)
        //            {
        //                UIServices.KB.CurrentModel.Objects.Get(new Guid(g));
        //                OpenDocumentOptions currentVersion = OpenDocumentOptions.CurrentVersion;
        //            }
        //            ++num;
        //        }
        //    }
        //}

        //public static void ObjectsWithVarNotBasedOnAtt()
        //{
        //    IKBService kb = UIServices.KB;
        //    string str = "KBDoctor - Object with variables not based on attribute/domain";
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    IOutputService output = CommonServices.Output;
        //    output.StartSection(str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    kbDoctorXmlWriter.AddTableHeader(new string[6]
        //    {
        //"Type",
        //"Name",
        //"Variable",
        //"Picture",
        //"Attribute",
        //"Domain"
        //    });
        //    foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //    {
        //        bool flag = false;
        //        if (ObjectsHelper.isGenerated(kbObject) && kbObject is Procedure)
        //        {
        //            output.AddLine("Procesing.... " + kbObject.Name + " - " + kbObject.TypeDescriptor.Name);
        //            string propertyValue1 = (string)kbObject.GetPropertyValue("ATT_PICTURE");
        //            VariablesPart variablesPart = kbObject.Parts.Get<VariablesPart>();
        //            if ((KBPart<KBObject>)variablesPart != (KBPart<KBObject>)null)
        //            {
        //                foreach (Variable variable in variablesPart.Variables)
        //                {
        //                    if (!variable.IsStandard)
        //                    {
        //                        string name1 = (KBObject)variable.AttributeBasedOn == (KBObject)null ? "" : variable.AttributeBasedOn.Name;
        //                        string name2 = (KBObject)variable.DomainBasedOn == (KBObject)null ? "" : variable.DomainBasedOn.Name;
        //                        string propertyValue2 = (string)variable.GetPropertyValue("ATT_PICTURE");
        //                        if (name1 == "" && name2 == "")
        //                        {
        //                            if (variable.Name.ToLower() == "archivo" && variable.Type == eDBType.CHARACTER && variable.Length == 50)
        //                                variable.DomainBasedOn = Functions.DomainByName("Archivo");
        //                            if (variable.Name.ToLower() == "in" && variable.Type == eDBType.VARCHAR && variable.Length >= 9999)
        //                                variable.DomainBasedOn = Functions.DomainByName("XMLContenido");
        //                            if (variable.Name.ToLower() == "out" && variable.Type == eDBType.VARCHAR && variable.Length >= 9999)
        //                                variable.DomainBasedOn = Functions.DomainByName("XMLContenido");
        //                            if ((KBObject)variable.DomainBasedOn != (KBObject)null)
        //                            {
        //                                variable.Name.ToLower();
        //                                kbDoctorXmlWriter.AddTableData(new string[6]
        //                                {
        //              kbObject.TypeDescriptor.Name,
        //              Functions.linkObject(kbObject),
        //              variable.Name,
        //              propertyValue2,
        //              name1,
        //              name2
        //                                });
        //                                flag = true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            if (flag)
        //                kbObject.Save();
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str, success);
        //}

        //public static string VariablesNotBasedAttributesOrDomain(KBObject obj)
        //{
        //    KBModel currentModel = UIServices.KB.CurrentModel;
        //    string str = "";
        //    VariablesPart variablesPart = obj.Parts.Get<VariablesPart>();
        //    if ((KBPart<KBObject>)variablesPart != (KBPart<KBObject>)null)
        //    {
        //        foreach (Variable variable in variablesPart.Variables)
        //        {
        //            if (!variable.IsStandard && (KBObject)variable.AttributeBasedOn == (KBObject)null && (KBObject)variable.DomainBasedOn == (KBObject)null && variable.Type != eDBType.GX_USRDEFTYP && variable.Type != eDBType.GX_SDT && variable.Type != eDBType.GX_EXTERNAL_OBJECT && variable.Type != eDBType.Boolean)
        //            {
        //                str = str + variable.Name + " " + variable.Type.ToString().ToLower() + "(" + variable.Length.ToString() + ")<br>" + Environment.NewLine;
        //                Functions.AddLineSummary("ObjectsVariableSinDom.Txt", obj.Name + "," + variable.Name + "," + Functions.ReturnPictureVariable(variable));
        //            }
        //        }
        //    }
        //    return str;
        //}

        //public static void AssignDomainToVariable(object[] parameters)
        //{
        //    IKBService kb = UIServices.KB;
        //    string domainName = "";
        //    string name = "";
        //    string str = "";
        //    int id = 0;
        //    foreach (Dictionary<string, string> parameter in parameters)
        //    {
        //        int num = 0;
        //        foreach (string s in parameter.Values)
        //        {
        //            switch (num)
        //            {
        //                case 1:
        //                    name = s;
        //                    break;
        //                case 2:
        //                    str = s;
        //                    break;
        //                case 3:
        //                    id = int.Parse(s);
        //                    break;
        //                case 4:
        //                    domainName = s;
        //                    break;
        //            }
        //            ++num;
        //        }
        //    }
        //    if (!(name != "") || !(domainName != "") || !(str != "") || id == 0)
        //        return;
        //    Domain domain = Functions.DomainByName(domainName);
        //    foreach (KBObject kbObject in UIServices.KB.CurrentModel.Objects.GetByName("Objects", new Guid?(), name))
        //    {
        //        kbObject.Parts.Get<VariablesPart>().GetVariable(id).DomainBasedOn = domain;
        //        IGxDocument documentInfo;
        //        if (UIServices.DocumentManager.IsOpenDocument(kbObject, out documentInfo))
        //        {
        //            ObjectsHelper.SetDocumentDirty(documentInfo);
        //            UIServices.TrackSelection.OnSelectChange((object)documentInfo.Object, (object)null);
        //            int num = (int)MessageBox.Show("Object open, save to complete the operation.");
        //        }
        //        else
        //        {
        //            kbObject.Save();
        //            int num = (int)MessageBox.Show("Variable assigned.");
        //        }
        //    }
        //}

        //public static void AssignAttributeToVariable(object[] parameters)
        //{
        //    IKBService kb = UIServices.KB;
        //    string name = "";
        //    string str = "";
        //    int id1 = 0;
        //    int id2 = 0;
        //    foreach (Dictionary<string, string> parameter in parameters)
        //    {
        //        int num = 0;
        //        foreach (string s in parameter.Values)
        //        {
        //            switch (num)
        //            {
        //                case 1:
        //                    name = s;
        //                    break;
        //                case 2:
        //                    str = s;
        //                    break;
        //                case 3:
        //                    id1 = int.Parse(s);
        //                    break;
        //                case 4:
        //                    id2 = int.Parse(s);
        //                    break;
        //            }
        //            ++num;
        //        }
        //    }
        //    if (!(name != "") || id2 == 0 || !(str != "") || id1 == 0)
        //        return;
        //    Artech.Genexus.Common.Objects.Attribute attribute = Artech.Genexus.Common.Objects.Attribute.Get(UIServices.KB.CurrentModel, id2);
        //    foreach (KBObject kbObject in UIServices.KB.CurrentModel.Objects.GetByName("Objects", new Guid?(), name))
        //    {
        //        kbObject.Parts.Get<VariablesPart>().GetVariable(id1).AttributeBasedOn = attribute;
        //        IGxDocument documentInfo;
        //        if (UIServices.DocumentManager.IsOpenDocument(kbObject, out documentInfo))
        //        {
        //            ObjectsHelper.SetDocumentDirty(documentInfo);
        //            UIServices.TrackSelection.OnSelectChange((object)documentInfo.Object, (object)null);
        //            int num = (int)MessageBox.Show("Object open, save to complete the operation.");
        //        }
        //        else
        //        {
        //            kbObject.Save();
        //            int num = (int)MessageBox.Show("Variable assigned.");
        //        }
        //    }
        //}

        //public static void IndexWithNotRefAtt()
        //{
        //    IKBService kb = UIServices.KB;
        //    string str1 = "KBDoctor - Index with not referenced attributes";
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    IOutputService output = CommonServices.Output;
        //    output.StartSection(str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[7]
        //    {
        //"Table",
        //"Name",
        //"Attribute",
        //"Composition",
        //"Remove Index",
        //"Remove attribute from the index",
        //"Remove attribute and next attributes from the index"
        //    });
        //    KBObjectCollection objectCollection1 = ObjectsHelper.Unreachables("Attribute");
        //    KBObjectCollection objectCollection2 = new KBObjectCollection();
        //    foreach (KBObject kbObject in Index.GetAll(kb.CurrentModel))
        //    {
        //        if (((Index)kbObject).Source == IndexSource.User && (KBObject)((Index)kbObject).Table != (KBObject)null)
        //            objectCollection2.Add(kbObject);
        //    }
        //    foreach (Artech.Genexus.Common.Objects.Attribute attribute in (BaseCollection<KBObject>)objectCollection1)
        //    {
        //        KBObjectCollection items = new KBObjectCollection();
        //        foreach (Index index in (BaseCollection<KBObject>)objectCollection2)
        //        {
        //            bool flag = false;
        //            string str2 = "";
        //            foreach (IndexMember member in index.IndexStructure.Members)
        //            {
        //                if (str2 != "")
        //                    str2 += ", ";
        //                str2 += member.Attribute.Name;
        //                if (member.Attribute.Id == attribute.Id)
        //                    flag = true;
        //            }
        //            if (flag)
        //            {
        //                items.Add((KBObject)index);
        //                string str3 = "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;RemoveIndexAttribute&mode=1&indId=" + (object)index.Id + "&attId=" + (object)attribute.Id + "\">Remove Index</a>";
        //                string str4 = "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;RemoveIndexAttribute&mode=2&indId=" + (object)index.Id + "&attId=" + (object)attribute.Id + "\">Remove attribute from the index</a>";
        //                string str5 = "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;RemoveIndexAttribute&mode=3&indId=" + (object)index.Id + "&attId=" + (object)attribute.Id + "\">Remove attribute and next attributes from the index</a>";
        //                kbDoctorXmlWriter.AddTableData(new string[7]
        //                {
        //      index.Table.Name,
        //      index.Name,
        //      attribute.Name,
        //      str2,
        //      str3,
        //      str4,
        //      str5
        //                });
        //            }
        //        }
        //        objectCollection2.RemoveAll((IEnumerable<KBObject>)items);
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //}

        //public static KBObjectCollection Unreachables(string type)
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    KBObjectCollection objectCollection1 = new KBObjectCollection();
        //    KBObjectCollection objectCollection2 = new KBObjectCollection();
        //    foreach (KBObject allMember in KBCategory.Get(kb.CurrentModel, "Main Programs").AllMembers)
        //        ObjectsHelper.MarkReachables(output, allMember, objectCollection1);
        //    if (type == "Attribute")
        //    {
        //        foreach (KBObject kbObject in Artech.Genexus.Common.Objects.Attribute.GetAll(kb.CurrentModel))
        //            objectCollection2.Add(kbObject);
        //    }
        //    else
        //    {
        //        foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //        {
        //            if (kbObject is ICallableObject || kbObject is Artech.Genexus.Common.Objects.Attribute)
        //                objectCollection2.Add(kbObject);
        //        }
        //    }
        //    objectCollection2.RemoveAll((IEnumerable<KBObject>)objectCollection1);
        //    return objectCollection2;
        //}

        //public static void RemoveIndexAttribute(object[] parameters)
        //{
        //    IKBService kb = UIServices.KB;
        //    int num1 = 0;
        //    int id = 0;
        //    int num2 = 0;
        //    foreach (Dictionary<string, string> parameter in parameters)
        //    {
        //        int num3 = 0;
        //        foreach (string s in parameter.Values)
        //        {
        //            switch (num3)
        //            {
        //                case 1:
        //                    num1 = int.Parse(s);
        //                    break;
        //                case 2:
        //                    id = int.Parse(s);
        //                    break;
        //                case 3:
        //                    num2 = int.Parse(s);
        //                    break;
        //            }
        //            ++num3;
        //        }
        //    }
        //    if (num1 == 0 || id == 0 || num2 == 0)
        //        return;
        //    Index index1 = Index.Get(kb.CurrentModel, id);
        //    Table table = index1.Table;
        //    string name = index1.Name;
        //    switch (num1)
        //    {
        //        case 1:
        //            if (MessageBox.Show(string.Format("Are you sure you want to remove the index {0}?", (object)index1.Name), "Remove attribute", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //            {
        //                foreach (TableIndex index2 in table.TableIndexes.Indexes)
        //                {
        //                    if (index2.Index.Id == id)
        //                    {
        //                        table.TableIndexes.Indexes.Remove(index2);
        //                        break;
        //                    }
        //                }
        //                index1.Delete();
        //                table.Save();
        //                int num4 = (int)MessageBox.Show("Index " + name + " was successfully removed.");
        //                goto label_57;
        //            }
        //            else
        //                goto label_57;
        //        case 2:
        //            using (IEnumerator<IndexMember> enumerator = index1.IndexStructure.Members.GetEnumerator())
        //            {
        //                while (enumerator.MoveNext())
        //                {
        //                    IndexMember current = enumerator.Current;
        //                    if (current.Attribute.Id == num2 && MessageBox.Show(string.Format("Are you sure you want to remove the attribute " + current.Attribute.Name + " from the index {0}?", (object)index1.Name), "Remove attribute", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //                    {
        //                        index1.IndexStructure.Members.Remove(current);
        //                        index1.Save();
        //                        int num5 = (int)MessageBox.Show("Index " + name + " was successfully updated.");
        //                        break;
        //                    }
        //                }
        //                break;
        //            }
        //        case 3:
        //            ArrayList arrayList = new ArrayList();
        //            bool flag = false;
        //            foreach (IndexMember member in index1.IndexStructure.Members)
        //            {
        //                if (member.Attribute.Id == num2)
        //                {
        //                    if (MessageBox.Show(string.Format("Are you sure you want to remove the attribute " + member.Attribute.Name + " and the next attributes from the index {0}?", (object)index1.Name), "Remove attribute", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //                    {
        //                        arrayList.Add((object)member);
        //                        flag = true;
        //                    }
        //                }
        //                else if (flag)
        //                    arrayList.Add((object)member);
        //            }
        //            if (flag)
        //            {
        //                foreach (IndexMember indexMember in arrayList)
        //                    index1.IndexStructure.Members.Remove(indexMember);
        //                index1.Save();
        //                int num6 = (int)MessageBox.Show("Index " + name + " was successfully updated.");
        //            }
        //            goto default;
        //    }
        //    label_57:;
        //}

        //public static void ObjectsWithVarsNotUsed()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - Object with variables not used";
        //    string varName = "";
        //    output.StartSection(str1);
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    kbDoctorXmlWriter.AddTableHeader(new string[3]
        //    {
        //"Type",
        //"Name",
        //"Clean"
        //    });
        //    foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //    {
        //        VariablesPart variablesPart = kbObject.Parts.Get<VariablesPart>();
        //        if ((KBPart<KBObject>)variablesPart != (KBPart<KBObject>)null && ObjectsHelper.isGenerated(kbObject))
        //        {
        //            output.AddLine("Procesing.." + kbObject.Name);
        //            string str2 = "";
        //            foreach (Variable variable in variablesPart.Variables)
        //            {
        //                if (!variable.IsStandard)
        //                {
        //                    bool flag = false;
        //                    ProcedurePart procedurePart = kbObject.Parts.Get<ProcedurePart>();
        //                    if ((KBPart<KBObject>)procedurePart != (KBPart<KBObject>)null)
        //                        flag = ObjectsHelper.VarUsedInText(procedurePart.Source, varName);
        //                    if (!flag)
        //                    {
        //                        RulesPart rulesPart = kbObject.Parts.Get<RulesPart>();
        //                        if ((KBPart<KBObject>)rulesPart != (KBPart<KBObject>)null)
        //                            flag = ObjectsHelper.VarUsedInText(rulesPart.Source, varName);
        //                    }
        //                    if (!flag)
        //                    {
        //                        ConditionsPart conditionsPart = kbObject.Parts.Get<ConditionsPart>();
        //                        if ((KBPart<KBObject>)conditionsPart != (KBPart<KBObject>)null)
        //                            flag = ObjectsHelper.VarUsedInText(conditionsPart.Source, varName);
        //                    }
        //                    if (!flag)
        //                    {
        //                        EventsPart eventsPart = kbObject.Parts.Get<EventsPart>();
        //                        if ((KBPart<KBObject>)eventsPart != (KBPart<KBObject>)null)
        //                            flag = ObjectsHelper.VarUsedInText(eventsPart.Source, varName);
        //                    }
        //                    if (!flag)
        //                    {
        //                        WebFormPart wF = kbObject.Parts.Get<WebFormPart>();
        //                        if ((KBPart<KBObject>)wF != (KBPart<KBObject>)null)
        //                            flag = ObjectsHelper.VarUsedInWebForm(wF, variable.Id);
        //                    }
        //                    if (!flag)
        //                    {
        //                        if (str2 != "")
        //                            str2 = str2 + "&varid" + (object)variable.Id + "=";
        //                        str2 += (string)(object)variable.Id;
        //                    }
        //                }
        //            }
        //            if (str2 != "")
        //            {
        //                string str3 = "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;CleanVarsNotUsed&ObjName=" + kbObject.Name + "&varid=" + str2 + "\">Clean vars not used</a>";
        //                kbDoctorXmlWriter.AddTableData(new string[3]
        //                {
        //      kbObject.TypeDescriptor.Name,
        //      kbObject.Name,
        //      str3
        //                });
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str1, success);
        //}

        //public static bool VarUsedInText(string reglas, string varName)
        //{
        //    bool flag = false;
        //    if (reglas == null)
        //        return false;
        //    Regex regex1 = new Regex("//.*", RegexOptions.None);
        //    Regex regex2 = new Regex("/\\*.*\\*/", RegexOptions.Singleline);
        //    Regex regex3 = new Regex(varName + "\\W+", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //    reglas = regex1.Replace(reglas, "");
        //    reglas = regex2.Replace(reglas, "");
        //    if (regex3.Match(reglas).Success)
        //        flag = true;
        //    return flag;
        //}

        //public static bool VarUsedInWebForm(WebFormPart wF, int varId)
        //{
        //    return wF.GetVariable(varId) != (Variable)null;
        //}

        //public static void CleanVarsNotUsed()
        //{
        //    string sectionName = "KBDoctor - Clean variables not used by DVelop Software.";
        //    IOutputService output = CommonServices.Output;
        //    output.StartSection(sectionName);
        //    Concepto.Packages.KBDoctorCore.Sources.API.CleanAllKBObjectVariables(UIServices.KB.CurrentKB, output);
        //    output.EndSection(sectionName, true);
        //}

        //private static void SetDocumentDirty(IGxDocument doc)
        //{
        //    if (UIServices.Environment.InvokeRequired)
        //        UIServices.Environment.BeginInvoke((System.Action)(() => ObjectsHelper.SetDocumentDirty(doc)));
        //    else
        //        doc.Dirty = true;
        //}

        //public static bool IsCallalable(KBObject obj)
        //{
        //    int num;
        //    switch (obj)
        //    {
        //        case Artech.Genexus.Common.Objects.Transaction _:
        //        case Procedure _:
        //        case WebPanel _:
        //        case WorkPanel _:
        //        case DataProvider _:
        //        case Menubar _:
        //            num = 1;
        //            break;
        //        default:
        //            num = obj is DataSelector ? 1 : 0;
        //            break;
        //    }
        //    return num != 0;
        //}

        //public static bool isGeneratedbyPattern(KBObject obj)
        //{
        //    return obj == (KBObject)null || obj.GetPropertyValue<bool>("IsGeneratedObject");
        //}

        //public static void ResetWINForm()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Reset WIN Forms";
        //    output.StartSection(str);
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    KBDoctorXMLWriter writer = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    writer.AddHeader(str);
        //    writer.AddTableHeader(new string[2] { "Type", "Name" });
        //    foreach (Artech.Genexus.Common.Objects.Transaction transaction in Artech.Genexus.Common.Objects.Transaction.GetAll(kb.CurrentModel))
        //    {
        //        Artech.Genexus.Common.Objects.Transaction obj = transaction;
        //        if (ObjectsHelper.isGenerated((KBObject)obj))
        //        {
        //            output.AddLine("Procesing.." + obj.Name);
        //            new List<KBObjectPart>()
        //  {
        //    obj.Parts[typeof (WinFormPart).GUID]
        //  }.ForEach((Action<KBObjectPart>)(part =>
        //  {
        //      if (!part.Default.CanCalculateDefault())
        //          return;
        //      part.Default.SilentSetIsDefault(true);
        //      try
        //      {
        //          obj.Save();
        //      }
        //      catch (Exception ex)
        //      {
        //          output.AddErrorLine(ex.Message);
        //      }
        //      writer.AddTableData(new string[2]
        //      {
        //      obj.TypeDescriptor.Name,
        //      Functions.linkObject((KBObject) obj)
        //      });
        //  }));
        //        }
        //    }
        //    writer.AddFooter();
        //    writer.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    bool success = true;
        //    output.EndSection(str, success);
        //}

        //public static void BuildObjectAndReferences()
        //{
        //    IKBService kb = UIServices.KB;
        //    KBModel currentModel = UIServices.KB.CurrentModel;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - Build Objects with references";
        //    output.StartSection(str1);
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    KBDoctorXMLWriter writer = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    writer.AddHeader(str1);
        //    writer.AddTableHeader(new string[4]
        //    {
        //"Object",
        //"Description",
        //"Visibility",
        //"Is Referenced by"
        //    });
        //    KBObjectCollection objToBuild = new KBObjectCollection();
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    string str2 = "";
        //    foreach (KBObject selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //    {
        //        if (KBObjectHelper.IsSpecifiable(selectObject) && !objToBuild.Contains(selectObject))
        //        {
        //            objToBuild.Add(selectObject);
        //            writer.AddTableData(new string[4]
        //            {
        //    selectObject.QualifiedName.ToString(),
        //    selectObject.Description,
        //    selectObject.IsPublic ? "Public" : "",
        //    ""
        //            });
        //        }
        //        ModulesHelper.AddObjectsReferenceTo(selectObject, objToBuild, writer);
        //    }
        //    foreach (KBObject kbObject in (BaseCollection<KBObject>)objToBuild)
        //        str2 = str2 + kbObject.Name + ";";
        //    writer.AddTableData(new string[1] { str2 });
        //    writer.AddFooter();
        //    writer.Close();
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //    GenexusUIServices.Build.BuildWithTheseOnly(objToBuild.Keys);
        //    do
        //    {
        //        Application.DoEvents();
        //    }
        //    while (GenexusUIServices.Build.IsBuilding);
        //    output.AddLine(str2);
        //    output.EndSection(str1, true);
        //}

        //public static void BuildObjectWithProperty()
        //{
        //    IKBService kb = UIServices.KB;
        //    KBModel currentModel = UIServices.KB.CurrentModel;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Build Objects with property";
        //    output.StartSection(str);
        //    Functions.CreateOutputFile(kb, str);
        //    WebPanel webPanel = new WebPanel(currentModel);
        //    output.AddLine("===== ENCRYPT URL PARAMETERS ========");
        //    foreach (KBObject kbObject in currentModel.Objects.GetByPropertyValue("USE_ENCRYPTION", (object)"SITE"))
        //        output.AddLine(kbObject.Name);
        //    output.AddLine("===== SOAP ========");
        //    foreach (KBObject kbObject in currentModel.Objects.GetByPropertyValue("CALL_PROTOCOL", (object)"SOAP"))
        //        output.AddLine(kbObject.Name);
        //    output.AddLine("===== HTTP ========");
        //    foreach (KBObject kbObject in currentModel.Objects.GetByPropertyValue("CALL_PROTOCOL", (object)"HTTP"))
        //        output.AddLine(kbObject.Name);
        //}

        //public static void ListAPIObjects()
        //{
        //    IKBService kb = UIServices.KB;
        //    Dictionary<string, KBObjectCollection> dictionary = new Dictionary<string, KBObjectCollection>();
        //    string str1 = "KBDoctor - List API Objects ";
        //    Functions.CreateOutputFile(kb, str1);
        //    IOutputService output = CommonServices.Output;
        //    output.StartSection(str1);
        //    string contents = "";
        //    SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
        //    int num = 0;
        //    foreach (KBObject kbObject in kb.CurrentModel.Objects.GetAll())
        //    {
        //        if (kbObject != (KBObject)null && ObjectsHelper.isGenerated(kbObject))
        //        {
        //            bool flag = false;
        //            if (kbObject is DataProvider && kbObject.GetPropertyValue<bool>("idISWEBSERVICE"))
        //                flag = true;
        //            if (kbObject is Artech.Genexus.Common.Objects.Transaction || kbObject is WebPanel)
        //                flag = true;
        //            if (kbObject.TypeDescriptor.Name == "MasterPage")
        //                flag = false;
        //            string str2 = kbObject.QualifiedName.ToString();
        //            bool propertyValue = kbObject.GetPropertyValue<bool>("IsMain");
        //            if (kbObject is Procedure & propertyValue)
        //            {
        //                str2 = kbObject.QualifiedName.ModuleName + (kbObject.QualifiedName.ModuleName == "" ? "a" : ".a") + kbObject.QualifiedName.ObjectName;
        //                flag = true;
        //            }
        //            if (kbObject is WorkPanel & propertyValue)
        //                flag = true;
        //            if (kbObject is ExternalObject || kbObject is Artech.Genexus.Common.Objects.SDT)
        //                flag = true;
        //            string str3 = kbObject.GetPropertyValueString("CALL_PROTOCOL");
        //            if (str3 == "")
        //                str3 = "Internal";
        //            if (flag)
        //                sortedDictionary[str3 + "\t" + kbObject.Name] = str2;
        //            ++num;
        //            if (num % 100 == 0)
        //                output.AddLine(kbObject.TypeDescriptor.Name + "," + kbObject.Name + "," + kbObject.Description);
        //        }
        //    }
        //    bool success = true;
        //    string str4 = KBDoctorHelper.SpcDirectory(kb);
        //    string str5 = string.Format("{0:yyyy-MM-dd-HHmm}", (object)DateTime.Now);
        //    foreach (KeyValuePair<string, string> keyValuePair in sortedDictionary)
        //        contents = contents + keyValuePair.Value + "\r\n";
        //    string path = str4 + "\\API3-" + str5 + ".txt";
        //    File.WriteAllText(path, contents);
        //    output.AddLine("URL/URI file generated in " + path);
        //    output.EndSection(str1, success);
        //}

        //public static void ObjectsWithTheSameSignature()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Objects with the same signature";
        //    output.StartSection(str);
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    SelectObjectOptions selectObjectOptions = new SelectObjectOptions()
        //    {
        //        MultipleSelection = true,
        //        ObjectTypes = {
        //  KBObjectDescriptor.Get<Artech.Genexus.Common.Objects.Transaction>()
        //}
        //    };
        //    HashSet<int> classes;
        //    Hashtable[] Classes_types;
        //    Concepto.Packages.KBDoctorCore.Sources.API.GetClassesTypesWithTheSameSignature(kb.CurrentModel.Objects.GetAll(), out classes, out Classes_types);
        //    kbDoctorXmlWriter.AddTableHeader(new string[3]
        //    {
        //"Class",
        //"Object",
        //"Datatype params"
        //    });
        //    foreach (int num in classes)
        //    {
        //        Hashtable hashtable = Classes_types[num - 1];
        //        foreach (string key in (IEnumerable)hashtable.Keys)
        //        {
        //            List<KBObject> kbObjectList = hashtable[(object)key] as List<KBObject>;
        //            if (kbObjectList.Count > 1)
        //            {
        //                foreach (KBObject kbObject in kbObjectList)
        //                    kbDoctorXmlWriter.AddTableData(new string[3]
        //                    {
        //        key,
        //        kbObject.Name,
        //        Functions.ExtractRuleParm(kbObject)
        //                    });
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    bool success = true;
        //    output.EndSection(str, success);
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //}

        //public static void ObjectsWithTheSameSignatureAssociated()
        //{
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str = "KBDoctor - Objects with the same signature associated to a transaction";
        //    output.StartSection(str);
        //    string outputFile = Functions.CreateOutputFile(kb, str);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str);
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Artech.Genexus.Common.Objects.Transaction>());
        //    List<KBObject> objects = new List<KBObject>();
        //    HashSet<EntityKey> entityKeySet = new HashSet<EntityKey>();
        //    foreach (Entity selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //    {
        //        foreach (EntityReference reference in selectObject.GetReferences())
        //        {
        //            KBObject kbObject = KBObject.Get(kb.CurrentModel, reference.To);
        //            ICallableObject callableObject = kbObject as ICallableObject;
        //            if (!entityKeySet.Contains(reference.To) && callableObject != null)
        //            {
        //                objects.Add(kbObject);
        //                entityKeySet.Add(reference.To);
        //            }
        //        }
        //    }
        //    HashSet<int> classes;
        //    Hashtable[] Classes_types;
        //    Concepto.Packages.KBDoctorCore.Sources.API.GetClassesTypesWithTheSameSignature((IEnumerable<KBObject>)objects, out classes, out Classes_types);
        //    kbDoctorXmlWriter.AddTableHeader(new string[3]
        //    {
        //"Class",
        //"Object",
        //"Datatype params"
        //    });
        //    foreach (int num in classes)
        //    {
        //        Hashtable hashtable = Classes_types[num - 1];
        //        foreach (string key in (IEnumerable)hashtable.Keys)
        //        {
        //            List<KBObject> kbObjectList = hashtable[(object)key] as List<KBObject>;
        //            if (kbObjectList.Count > 1)
        //            {
        //                foreach (KBObject kbObject in kbObjectList)
        //                    kbDoctorXmlWriter.AddTableData(new string[3]
        //                    {
        //        key,
        //        kbObject.Name,
        //        Functions.ExtractRuleParm(kbObject)
        //                    });
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    bool success = true;
        //    output.EndSection(str, success);
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //}

        //public static void TestParser()
        //{
        //    Hashtable hashtable = new Hashtable();
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FNONE, (object)"FNONE");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FOB, (object)" '(' Open Bracket");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FFN, (object)" 'Function(' Fuction call");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FNA, (object)" Name Attribute");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FNC, (object)" Name Cconstant");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FCB, (object)" ')' Close Bracket");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FPL, (object)" '+''-' Plus Minus oper.");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FPR, (object)" '*''/' Product Divis. oper.");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FCM, (object)" ',' Comma separate parms.");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FNT, (object)" 'NOT' NOT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FAN, (object)" 'AND' 'OR' AND OR");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FRE, (object)" '<' '>' '=' Relational oper.");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.EXP, (object)" Expression");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.SUM, (object)" Sum");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.COU, (object)" Count");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.AVE, (object)" Average");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.MAX, (object)" Maximum");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.MIN, (object)" Minimum");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FIF, (object)" IF ...");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FSC, (object)" Semicolon ';'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FOT, (object)" Otherwise");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.ERR_TOKEN, (object)"ERR_TOKEN");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FEN, (object)" EOExpression");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FCO, (object)" Comment (') for rules / commands");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FUV, (object)" User Variable (&) for rules/commands");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FUA, (object)" User Variable Array '&xx('");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FCN, (object)" Continuation Line / White spaces");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FAM, (object)" String to replace '&' with '&&'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FCL, (object)" CLass id (used for calls)");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FOI, (object)" Object Id (used for calls)");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FCT, (object)" ConTrol ID/Name (for properties)");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FCI, (object)" Control type Id (combo/edit/etc.)");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FMT, (object)" control id/name (for MeThods) (Used only in specifier)");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FBI, (object)" BInary info in value (used to save bin data in obj_info)");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FDC, (object)" Date constante (used only in dYNQ by now)");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FCV, (object)" Control Variable (the var associated with the control (Used only in specifier)");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FWH, (object)" WHEN (GXW) / WHERE (DKL) ...");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FNS, (object)" Name space ...");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FON, (object)" ON ...");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FBC, (object)" Comentario de bloque");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FOR, (object)" ORDER ...");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TKN_TRUE, (object)" TRUE");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TKN_FALSE, (object)" FALSE");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TKN_NONE, (object)" NONE, para expresión FOR EACH ... ORDER NONE ... ENDFOR");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.PRM, (object)" Parámetro, utilizado en DYNQ");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FND, (object)" Name Domain");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FLV, (object)" LEVEL token");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TKN_NEW, (object)" NEW token");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FSDTCLS, (object)" Structure Class");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TKN_NULL, (object)" NULL");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TKN_IN, (object)" IN");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.SSL, (object)" SUBSELECT : used by generators; reserved it for Gx.");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.FEX, (object)" Exception name");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TMSGID, (object)" Message id");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TNCNT, (object)" Token Name Constant NonTranslatable");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TFOR, (object)" For token, defined to be used with Lookup Deklarit's rule");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TDEPENDENCIES, (object)" Dependencies token, new condition for rules.");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TRULE, (object)" Rule token");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TBY, (object)" 'By' token");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TGIVEN, (object)" 'Given' token");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TWHERE, (object)" 'Where' token -GeneXus, Deklarit uses FWH");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TDEFINEDBY, (object)" 'Defined by' token");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TSECTION, (object)" [Web], [Win], [Web], [Text]");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TINDP, (object)" Used for token 'in <dataselector>'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.OPENSQUAREBRACKET, (object)"OPENSQUAREBRACKET");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.CLOSESQUAREBRACKET, (object)"CLOSESQUAREBRACKET");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.OUTPUTNAME, (object)"OUTPUTNAME");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.OUTPUTDYNAMICSYM, (object)"OUTPUTDYNAMICSYM");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.INPUT, (object)"INPUT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.OUTPUTPROPERTY, (object)"OUTPUTPROPERTY");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.OBJREFERENCE, (object)"OBJREFERENCE");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TUSING, (object)"TUSING");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TSIGN, (object)" Now that rules supports comments, define the TSIGN token to specified the sign of an expression (e.g. '-1')");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.TEXO, (object)"TEXO");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEJE, (object)" 'Eject'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTNSK, (object)" 'NoSkip'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTLNN, (object)" 'Lineno'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTPRC, (object)"DTPRC");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTCLL, (object)" 'Call'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTDBA, (object)"DTDBA");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTCOB, (object)"DTCOB");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTASG, (object)" Assignment");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTPRI, (object)"DTPRI");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTIF, (object)"IF");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTELS, (object)" 'Else'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEIF, (object)" 'Endif'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTNPR, (object)"Defined by");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTDEL, (object)" 'Delete'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTDO, (object)" 'Do'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEDO, (object)" 'Enddo'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTWHE, (object)"Where");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTNEW, (object)"New");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTRET, (object)"Return");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTHEA, (object)"DTHEA");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTBEG, (object)"DTBEG");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTFOR, (object)" 'ForEach'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEND, (object)"DTEND");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTPL, (object)"DTPL");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTMT, (object)"DTMT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTMB, (object)"DTMB");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTSRC, (object)"DTSRC");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTENW, (object)"End New");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEFO, (object)" 'EndFor'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTWDU, (object)" 'When Duplicate'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTWNO, (object)" 'When None'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTCP, (object)"DTCP");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTCMM, (object)"Commit");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTXFE, (object)"DTXFE");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTXFF, (object)"DTXFF");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTXNW, (object)"DTXNW");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTXEF, (object)"DTXEF");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTXEN, (object)"DTXEN");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTDBY, (object)"DTDBY");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEXF, (object)" 'Exit' from a 'Do While'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEXD, (object)"DTEXD");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTMSG, (object)"Msg - Message");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTFOO, (object)"DTFOO");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTPRO, (object)" 'Sub' 'subroutine'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEPR, (object)" 'EndSub'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTDOP, (object)" Do 'subroutine'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEVT, (object)"DTEVT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEEV, (object)"DTEEV");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTREF, (object)"DTREF");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTFLN, (object)"DTFLN");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEFL, (object)"DTEFL");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTCNF, (object)"DTCNF");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTDOC, (object)"Do case");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTCAS, (object)"Case 'Condition''");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTECA, (object)"EndCase");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTLOA, (object)"DTLOA");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTLVL, (object)"DTLVL");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTRBK, (object)" Comando ROLLBACK");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTSBM, (object)" Comando SUBMIT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTGRA, (object)"DTGRA");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTERH, (object)" Commando Error_Handler");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTVB, (object)" Comando VB");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTFSL, (object)"DTFSL");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTDMY, (object)"Reserved for spec RPC");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTOTH, (object)"Otherwise");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEFS, (object)" Reserved for End for each selected line");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTJAV, (object)" Comando JAVA");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTSQL, (object)" Comando SQL");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTFLS, (object)"DTFLS");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTFSS, (object)"DTFSS");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEFF, (object)"DTEFF");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTLNK, (object)" Comando LINK");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTAPL, (object)" Asignación del tipo +=");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTAMI, (object)" Asignación del tipo -=");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTAMU, (object)" Asignación del tipo *=");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTADI, (object)" Asignación del tipo /=");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTFIN, (object)" FOR <var> IN <array>");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEFI, (object)" END' del token anterior");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTFFT, (object)" FOR <var>=<exp> TO <exp> STEP <exp>");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEFT, (object)" END' del token anterior");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTIN, (object)" Comando IN de FOR var IN array");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTTO, (object)" Comando TO de FOR EACH var=exp TO exp");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTSTP, (object)" Comando STEP de FOR var=exp TO exp STEP exp");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTCSH, (object)" Comando CSHARP");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTON, (object)" Comando ON");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTWHN, (object)" Comando WHEN");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTOPD, (object)" Comando OPTION DISTINCT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTUSG, (object)" Comando USING de FOR EACH ... ENDFOR");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTPOPUP, (object)" Comando POPUP()");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.BLOCKING, (object)" Comando BLOCKING");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.OUTPUTELEMENT, (object)"OUTPUTELEMENT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.OPENCURLYBRACKET, (object)"OPENCURLYBRACKET");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.CLOSECURLYBRACKET, (object)"CLOSECURLYBRACKET");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.PRINT, (object)"PRINT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.INSERT, (object)"INSERT");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.SUBGROUP, (object)"SUBGROUP");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.ENDSUBGROUP, (object)"ENDSUBGROUP");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTStub, (object)" 'public sub'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTJavaScript, (object)" 'javascript' command - not implemented yet! - reserved number");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTEndStub, (object)"DTEndStub");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTCallStub, (object)"DTCallStub");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTRuby, (object)" Comando 'RUBY <LINE>'");
        //    hashtable.Add((object)ObjectsHelper.TokensIds.DTREDUNDANCY, (object)" Used to give redundancy info to the specifier");
        //    IKBService kb = UIServices.KB;
        //    IOutputService output = CommonServices.Output;
        //    string str1 = "KBDoctor - TESTEO DE PARSER ";
        //    output.StartSection(str1);
        //    string outputFile = Functions.CreateOutputFile(kb, str1);
        //    KBDoctorXMLWriter kbDoctorXmlWriter = new KBDoctorXMLWriter(outputFile, Encoding.UTF8);
        //    kbDoctorXmlWriter.AddHeader(str1);
        //    SelectObjectOptions options = new SelectObjectOptions();
        //    options.MultipleSelection = true;
        //    options.ObjectTypes.Add(KBObjectDescriptor.Get<Procedure>());
        //    IParserEngine engine = (Artech.Architecture.Common.Services.Services.GetService(new Guid("C26F529E-9A69-4df5-B825-9194BA3983A3")) as ILanguageService).CreateEngine();
        //    kbDoctorXmlWriter.AddTableHeader(new string[5]
        //    {
        //"OBJECT",
        //"COMMAND",
        //"TOKEN",
        //"Id",
        //"Row"
        //    });
        //    foreach (KBObject selectObject in (IEnumerable<KBObject>)UIServices.SelectObjectDialog.SelectObjects(options))
        //    {
        //        kbDoctorXmlWriter.AddTableData(new string[4]
        //        {
        //  selectObject.Name,
        //  "",
        //  "",
        //  ""
        //        });
        //        ProcedurePart part = selectObject.Parts.Get<ProcedurePart>();
        //        if ((KBPart<KBObject>)part != (KBPart<KBObject>)null)
        //        {
        //            ParserInfo info = new ParserInfo((KBObjectPart)part);
        //            foreach (TokenData token in engine.GetTokens(true, info, part.Source))
        //            {
        //                string str2 = hashtable[(object)(ObjectsHelper.TokensIds)token.Token] as string;
        //                if (token.Token >= 100)
        //                    kbDoctorXmlWriter.AddTableData(new string[4]
        //                    {
        //        "",
        //        str2,
        //        "",
        //        token.Token.ToString()
        //                    });
        //                else
        //                    kbDoctorXmlWriter.AddTableData(new string[4]
        //                    {
        //        "",
        //        "",
        //        str2,
        //        token.Token.ToString()
        //                    });
        //            }
        //        }
        //    }
        //    kbDoctorXmlWriter.AddFooter();
        //    kbDoctorXmlWriter.Close();
        //    bool success = true;
        //    output.EndSection(str1, success);
        //    KBDoctorHelper.ShowKBDoctorResults(outputFile);
        //}

        //public enum TokensIds
        //{
        //    FNONE = -1, // 0xFFFFFFFF
        //    FOB = 0,
        //    FFN = 1,
        //    FNA = 2,
        //    FNC = 3,
        //    FCB = 4,
        //    FPL = 5,
        //    FPR = 6,
        //    FCM = 7,
        //    FNT = 8,
        //    FAN = 9,
        //    FRE = 10, // 0x0000000A
        //    EXP = 11, // 0x0000000B
        //    SUM = 12, // 0x0000000C
        //    COU = 13, // 0x0000000D
        //    AVE = 14, // 0x0000000E
        //    MAX = 15, // 0x0000000F
        //    MIN = 16, // 0x00000010
        //    FIF = 17, // 0x00000011
        //    FSC = 18, // 0x00000012
        //    FOT = 19, // 0x00000013
        //    ERR_TOKEN = 20, // 0x00000014
        //    FEN = 21, // 0x00000015
        //    FCO = 22, // 0x00000016
        //    FUV = 23, // 0x00000017
        //    FUA = 24, // 0x00000018
        //    FCN = 25, // 0x00000019
        //    FAM = 26, // 0x0000001A
        //    FCL = 27, // 0x0000001B
        //    FOI = 28, // 0x0000001C
        //    FCT = 29, // 0x0000001D
        //    FCI = 30, // 0x0000001E
        //    FMT = 31, // 0x0000001F
        //    FBI = 32, // 0x00000020
        //    FDC = 33, // 0x00000021
        //    FCV = 34, // 0x00000022
        //    FWH = 35, // 0x00000023
        //    FNS = 36, // 0x00000024
        //    FON = 37, // 0x00000025
        //    FBC = 38, // 0x00000026
        //    FOR = 39, // 0x00000027
        //    TKN_TRUE = 40, // 0x00000028
        //    TKN_FALSE = 41, // 0x00000029
        //    TKN_NONE = 42, // 0x0000002A
        //    PRM = 43, // 0x0000002B
        //    FND = 44, // 0x0000002C
        //    FLV = 45, // 0x0000002D
        //    TKN_NEW = 46, // 0x0000002E
        //    FSDTCLS = 47, // 0x0000002F
        //    TKN_NULL = 48, // 0x00000030
        //    TKN_IN = 49, // 0x00000031
        //    SSL = 50, // 0x00000032
        //    FEX = 51, // 0x00000033
        //    TMSGID = 52, // 0x00000034
        //    TNCNT = 53, // 0x00000035
        //    TFOR = 54, // 0x00000036
        //    TDEPENDENCIES = 55, // 0x00000037
        //    TRULE = 56, // 0x00000038
        //    TBY = 57, // 0x00000039
        //    TGIVEN = 58, // 0x0000003A
        //    TWHERE = 59, // 0x0000003B
        //    TDEFINEDBY = 60, // 0x0000003C
        //    TSECTION = 61, // 0x0000003D
        //    TINDP = 62, // 0x0000003E
        //    OPENSQUAREBRACKET = 63, // 0x0000003F
        //    CLOSESQUAREBRACKET = 64, // 0x00000040
        //    OUTPUTNAME = 65, // 0x00000041
        //    OUTPUTDYNAMICSYM = 66, // 0x00000042
        //    INPUT = 67, // 0x00000043
        //    OUTPUTPROPERTY = 68, // 0x00000044
        //    OBJREFERENCE = 69, // 0x00000045
        //    TUSING = 70, // 0x00000046
        //    TSIGN = 71, // 0x00000047
        //    TEXO = 72, // 0x00000048
        //    DTEJE = 100, // 0x00000064
        //    DTNSK = 101, // 0x00000065
        //    DTLNN = 102, // 0x00000066
        //    DTPRC = 103, // 0x00000067
        //    DTCLL = 104, // 0x00000068
        //    DTDBA = 105, // 0x00000069
        //    DTCOB = 106, // 0x0000006A
        //    DTASG = 107, // 0x0000006B
        //    DTPRI = 108, // 0x0000006C
        //    DTIF = 109, // 0x0000006D
        //    DTELS = 110, // 0x0000006E
        //    DTEIF = 111, // 0x0000006F
        //    DTNPR = 112, // 0x00000070
        //    DTDEL = 113, // 0x00000071
        //    DTDO = 114, // 0x00000072
        //    DTEDO = 115, // 0x00000073
        //    DTWHE = 116, // 0x00000074
        //    DTNEW = 117, // 0x00000075
        //    DTRET = 118, // 0x00000076
        //    DTHEA = 119, // 0x00000077
        //    DTBEG = 120, // 0x00000078
        //    DTFOR = 121, // 0x00000079
        //    DTEND = 122, // 0x0000007A
        //    DTPL = 123, // 0x0000007B
        //    DTMT = 124, // 0x0000007C
        //    DTMB = 125, // 0x0000007D
        //    DTSRC = 126, // 0x0000007E
        //    DTENW = 127, // 0x0000007F
        //    DTEFO = 128, // 0x00000080
        //    DTWDU = 129, // 0x00000081
        //    DTWNO = 130, // 0x00000082
        //    DTCP = 131, // 0x00000083
        //    DTCMM = 132, // 0x00000084
        //    DTXFE = 133, // 0x00000085
        //    DTXFF = 134, // 0x00000086
        //    DTXNW = 135, // 0x00000087
        //    DTXEF = 136, // 0x00000088
        //    DTXEN = 137, // 0x00000089
        //    DTDBY = 138, // 0x0000008A
        //    DTEXF = 139, // 0x0000008B
        //    DTEXD = 140, // 0x0000008C
        //    DTMSG = 141, // 0x0000008D
        //    DTFOO = 142, // 0x0000008E
        //    DTPRO = 143, // 0x0000008F
        //    DTEPR = 144, // 0x00000090
        //    DTDOP = 145, // 0x00000091
        //    DTEVT = 146, // 0x00000092
        //    DTEEV = 147, // 0x00000093
        //    DTREF = 148, // 0x00000094
        //    DTFLN = 149, // 0x00000095
        //    DTEFL = 150, // 0x00000096
        //    DTCNF = 151, // 0x00000097
        //    DTDOC = 152, // 0x00000098
        //    DTCAS = 153, // 0x00000099
        //    DTECA = 154, // 0x0000009A
        //    DTLOA = 155, // 0x0000009B
        //    DTLVL = 156, // 0x0000009C
        //    DTRBK = 157, // 0x0000009D
        //    DTSBM = 158, // 0x0000009E
        //    DTGRA = 159, // 0x0000009F
        //    DTERH = 160, // 0x000000A0
        //    DTVB = 161, // 0x000000A1
        //    DTFSL = 162, // 0x000000A2
        //    DTDMY = 163, // 0x000000A3
        //    DTOTH = 164, // 0x000000A4
        //    DTEFS = 165, // 0x000000A5
        //    DTJAV = 166, // 0x000000A6
        //    DTSQL = 167, // 0x000000A7
        //    DTFLS = 168, // 0x000000A8
        //    DTFSS = 169, // 0x000000A9
        //    DTEFF = 170, // 0x000000AA
        //    DTLNK = 171, // 0x000000AB
        //    DTAPL = 172, // 0x000000AC
        //    DTAMI = 173, // 0x000000AD
        //    DTAMU = 174, // 0x000000AE
        //    DTADI = 175, // 0x000000AF
        //    DTFIN = 176, // 0x000000B0
        //    DTEFI = 177, // 0x000000B1
        //    DTFFT = 178, // 0x000000B2
        //    DTEFT = 179, // 0x000000B3
        //    DTIN = 180, // 0x000000B4
        //    DTTO = 181, // 0x000000B5
        //    DTSTP = 182, // 0x000000B6
        //    DTCSH = 183, // 0x000000B7
        //    DTON = 184, // 0x000000B8
        //    DTWHN = 185, // 0x000000B9
        //    DTOPD = 186, // 0x000000BA
        //    DTUSG = 187, // 0x000000BB
        //    DTPOPUP = 188, // 0x000000BC
        //    BLOCKING = 189, // 0x000000BD
        //    OUTPUTELEMENT = 190, // 0x000000BE
        //    OPENCURLYBRACKET = 191, // 0x000000BF
        //    CLOSECURLYBRACKET = 192, // 0x000000C0
        //    PRINT = 193, // 0x000000C1
        //    INSERT = 194, // 0x000000C2
        //    SUBGROUP = 195, // 0x000000C3
        //    ENDSUBGROUP = 196, // 0x000000C4
        //    DTStub = 197, // 0x000000C5
        //    DTJavaScript = 198, // 0x000000C6
        //    DTEndStub = 199, // 0x000000C7
        //    DTCallStub = 200, // 0x000000C8
        //    DTRuby = 201, // 0x000000C9
        //    DTREDUNDANCY = 397, // 0x0000018D
        //}
    }
}
