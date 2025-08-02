using Artech.Architecture.Common.Descriptors;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Helpers.Structure;
using Artech.Common.Properties;
using Artech.Genexus.Common;
using Artech.Genexus.Common.ModelParts;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;
using Artech.Genexus.Common.Parts.SDT;
using Artech.Packages.Patterns;
using Artech.Packages.Patterns.Definition;
using Artech.Packages.Patterns.Engine;
using Artech.Packages.Patterns.Objects;
using Artech.Udm.Framework;
using Artech.Udm.Framework.References;
using LSI.Packages.Extensiones.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Acme.Packages.Menu
{
    public static class Utility
    {
        public static bool isMain(KBObject obj)
        {
            object propertyValue = obj.GetPropertyValue(nameof(isMain));
            return propertyValue != null && propertyValue.ToString() == "True";
        }

        public static bool isGenerated(KBObject obj)
        {
            object propertyValue = obj.GetPropertyValue("GenerateObject");
            return propertyValue != null && propertyValue.ToString() == "True";
        }

        internal static string AddXMLHeader(string fileName)
        {
            return "<?xml version='1.0' encoding='iso-8859-1'?>" + File.ReadAllText(fileName);
        }

        internal static DateTime GetDateTimeNVGDirectory(string FileName)
        {
            string fileName = Path.GetFileName(FileName);
            int startIndex = 4;
            int num1 = int.Parse(fileName.Substring(startIndex, 4));
            int num2 = int.Parse(fileName.Substring(startIndex + 5, 2));
            int num3 = int.Parse(fileName.Substring(startIndex + 8, 2));
            int num4 = int.Parse(fileName.Substring(startIndex + 11, 2));
            int num5 = int.Parse(fileName.Substring(startIndex + 13, 2));
            DateTime timeNvgDirectory = new DateTime();
            if (Utility.ValidoAtributosDateTime(num1, num2, num3, num4, num5, 0))
                timeNvgDirectory = new DateTime(num1, num2, num3, num4, num5, 0);
            return timeNvgDirectory;
        }

        internal static DateTime GetDateTimeWSDLDirectory(string FileName)
        {
            string fileName = Path.GetFileName(FileName);
            int startIndex = 5;
            int num1 = int.Parse(fileName.Substring(startIndex, 4));
            int num2 = int.Parse(fileName.Substring(startIndex + 5, 2));
            int num3 = int.Parse(fileName.Substring(startIndex + 8, 2));
            int num4 = int.Parse(fileName.Substring(startIndex + 11, 2));
            int num5 = int.Parse(fileName.Substring(startIndex + 13, 2));
            DateTime timeWsdlDirectory = new DateTime();
            if (Utility.ValidoAtributosDateTime(num1, num2, num3, num4, num5, 0))
                timeWsdlDirectory = new DateTime(num1, num2, num3, num4, num5, 0);
            return timeWsdlDirectory;
        }

        private static bool ValidoAtributosDateTime(
          int Anio,
          int Mes,
          int Dia,
          int Hora,
          int Minutos,
          int Segundos)
        {
            return Anio >= 2000 && Mes > 0 && Mes <= 12 && Dia <= 31 && Dia > 0 && Hora >= 0 && Hora <= 23 && Minutos >= 0 && Minutos <= 60 && Segundos >= 0 && Segundos <= 60;
        }

        internal static string NvgComparerDirectory(KnowledgeBase KB)
        {
            KB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            string path = Path.Combine(Utility.SpcDirectory(KB), "NvgComparer");
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
            }
            return path;
        }

        internal static string WsdlComparerDirectory(KnowledgeBase KB)
        {
            KB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            string path = Path.Combine(Utility.SpcDirectory(KB), "WsdlComparer");
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
            }
            return path;
        }

        internal static string WsdlDir(KnowledgeBase KB, bool isSource)
        {
            string path1 = Utility.WsdlComparerDirectory(KB);
            string path;
            if (isSource)
            {
                path = Path.Combine(path1, "Source");
            }
            else
            {
                string str = string.Format("{0:yyyy-MM-dd-HHmm}", (object)DateTime.Now);
                path = path1 + "\\WSDL-" + str + "\\";
            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        internal static void ShowKBDoctorResults(string outputFile)
        {
            UIServices.StartPage.OpenPage(outputFile, "KBDoctor", (object)null);
            UIServices.ToolWindows.FocusToolWindow(UIServices.StartPage.ToolWindow.Id);
        }

        internal static string SpcDirectory(KnowledgeBase KB)
        {
            GxModel gxModel = KB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            return KB.Location + string.Format("\\GXSPC{0:D3}\\", (object)gxModel.Model.Id);
        }

        internal static string ObjComparerDirectory(KnowledgeBase KB)
        {
            KB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            string path = Path.Combine(Utility.SpcDirectory(KB), "ObjComparer");
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
            }
            return path;
        }

        internal static int MaxCodeBlock(string source)
        {
            int num1 = 0;
            int num2 = 0;
            using (StringReader stringReader = new StringReader(source))
            {
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    ++num2;
                    if (str.StartsWith("SUB ") || str.StartsWith("EVENT "))
                    {
                        num1 = num1 <= num2 ? num2 : num1;
                        num2 = 1;
                    }
                }
                num1 = num1 <= num2 ? num2 : num1;
            }
            return num1;
        }

        internal static int ComplexityLevel(string source)
        {
            int num = 0;
            using (StringReader stringReader = new StringReader(source))
            {
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    string upper = str.TrimStart().ToUpper();
                    if (upper.StartsWith("DO WHILE") || upper.StartsWith("IF") || upper.StartsWith("DO CASE") || upper.StartsWith("FOR"))
                        ++num;
                }
            }
            return num;
        }

        internal static int MaxNestLevel(string source)
        {
            int num1 = 0;
            int num2 = 0;
            using (StringReader stringReader = new StringReader(source))
            {
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    string upper = str.TrimStart().ToUpper();
                    if (!upper.StartsWith("DO '"))
                    {
                        if (upper.StartsWith("FOR ") || upper.StartsWith("IF ") || upper.StartsWith("DO ") || upper.StartsWith("NEW") || upper.StartsWith("SUB"))
                        {
                            ++num2;
                            num1 = num2 > num1 ? num2 : num1;
                        }
                        else if (upper.StartsWith("ENDFOR") || upper.StartsWith("ENDIF") || upper.StartsWith("ENDDO") || upper.StartsWith("ENDCASE") || upper.StartsWith("ENDNEW") || upper.StartsWith("ENDSUB"))
                            --num2;
                    }
                }
                return num1;
            }
        }

        internal static bool ValidateINOUTinParm(KBObject obj)
        {
            bool flag1 = false;
            if (obj is ICallableObject callableObject)
            {
                foreach (Artech.Genexus.Common.Objects.Signature signature in callableObject.GetSignatures())
                {
                    bool flag2 = false;
                    foreach (Parameter parameter in signature.Parameters)
                    {
                        if (parameter.Accessor.ToString() == "PARM_INOUT")
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (flag2)
                    {
                        RulesPart rulesPart = obj.Parts.Get<RulesPart>();
                        if ((KBPart<KBObject>)rulesPart != (KBPart<KBObject>)null)
                        {
                            Regex regex1 = new Regex("//.*", RegexOptions.None);
                            Regex regex2 = new Regex("parm\\(.*\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                            string source = rulesPart.Source;
                            string input = regex1.Replace(source, "");
                            Match match = regex2.Match(input);
                            if (match != null)
                                flag1 = match.ToString().Split(',').Length != match.ToString().Split(':').Length - 1;
                        }
                    }
                }
            }
            return flag1;
        }

        internal static void AddLineSummary(KnowledgeBase KB, string fileName, string texto)
        {
            using (FileStream fileStream = new FileStream(KB.UserDirectory + "\\" + fileName, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream))
                {
                    streamWriter.WriteLine(DateTime.Now.ToString() + "," + texto);
                    fileStream.Dispose();
                }
            }
        }

        internal static void AddLine(KnowledgeBase KB, string fileName, string texto)
        {
            using (FileStream fileStream = new FileStream(KB.UserDirectory + "\\" + fileName, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream))
                {
                    streamWriter.WriteLine(texto);
                    fileStream.Dispose();
                }
            }
        }

        internal static string ObjectSourceUpper(KBObject obj)
        {
            string str = "";
            try
            {
                if (obj is Procedure)
                    str = obj.Parts.Get<ProcedurePart>().Source;
                if (obj is Artech.Genexus.Common.Objects.Transaction)
                    str = obj.Parts.Get<EventsPart>().Source;
                if (obj is WorkPanel)
                    str = obj.Parts.Get<EventsPart>().Source;
                if (obj is WebPanel)
                    str = obj.Parts.Get<EventsPart>().Source;
            }
            catch (Exception)
            {
            }
            return str.ToUpper();
        }

        internal static KBObjectPart ObjectSourcePart(KBObject obj)
        {
            try
            {
                switch (obj)
                {
                    case Procedure _:
                        return (KBObjectPart)obj.Parts.Get<ProcedurePart>();
                    case Artech.Genexus.Common.Objects.Transaction _:
                        return (KBObjectPart)obj.Parts.Get<EventsPart>();
                    case WorkPanel _:
                        return (KBObjectPart)obj.Parts.Get<EventsPart>();
                    case WebPanel _:
                        return (KBObjectPart)obj.Parts.Get<EventsPart>();
                }
            }
            catch (Exception)
            {
            }
            return (KBObjectPart)null;
        }

        internal static bool isRunable(KBObject obj)
        {
            int num;
            switch (obj)
            {
                case Artech.Genexus.Common.Objects.Transaction _:
                case WorkPanel _:
                case WebPanel _:
                case DataProvider _:
                case DataSelector _:
                case Procedure _:
                    num = 1;
                    break;
                default:
                    num = obj is Menubar ? 1 : 0;
                    break;
            }
            return num != 0;
        }

        internal static bool CanBeBuilt(KBObject obj)
        {
            int num;
            switch (obj)
            {
                case Artech.Genexus.Common.Objects.Transaction _:
                case WebPanel _:
                case Procedure _:
                case DataProvider _:
                    num = 1;
                    break;
                default:
                    num = obj is Menubar ? 1 : 0;
                    break;
            }
            return num != 0;
        }

        internal static string ExtractComments(string source)
        {
            string str1 = "/\\*(.*?)\\*/";
            string str2 = "//(.*?)\\r?\\n";
            string str3 = "\"((\\\\[^\\n]|[^\"\\n])*)\"";
            string str4 = "@(\"[^\"]*\")+";
            string str5 = Regex.Replace(source, str1 + "|" + str2 + "|" + str3 + "|" + str4, (MatchEvaluator)(me => me.Value.StartsWith("/*") || me.Value.StartsWith("//") ? (me.Value.StartsWith("//") ? Environment.NewLine : "") : me.Value), RegexOptions.Singleline).Replace("(", " (").Replace(")", ") ").Replace("\"", "'").Replace("\t", " ").Replace("  ", " ");
            string comments;
            do
            {
                comments = str5;
                str5 = comments.Replace("  ", " ");
            }
            while (comments != str5);
            return comments;
        }

        internal static string CodeCommented(string source)
        {
            return Regex.Match(source.Replace("//\n", "####\n"), "[^\\/](\\/\\*)([\\b\\s]*(msg|do|call|udp|where|if|else|endif|endfor|for|defined by|while|enddo|&[A-Za-z0-9_\\-.\\s]*=))(\\*(?!\\/)|[^*])*(\\*\\/)|(\\/\\/)[\\b\\s]*((msg|do|call|udp|where|if|else|endif|endfor|for|defined by|while|enddo|&[A-Za-z0-9_\\-.\\s]*=)([^\\r\\n]+)?)").Value;
        }

        internal static bool HasCodeCommented(string source)
        {
            string pattern = "[^\\/](\\/\\*)([\\b\\s]*(msg|do|call|udp|where|if|else|endif|endfor|for|defined by|while|enddo|&[A-Za-z0-9_\\-.\\s]*=))(\\*(?!\\/)|[^*])*(\\*\\/)|(\\/\\/)[\\b\\s]*((msg|do|call|udp|where|if|else|endif|endfor|for|defined by|while|enddo|&[A-Za-z0-9_\\-.\\s]*=)([^\\r\\n]+)?)";
            return Regex.Match(source, pattern).Value != "";
        }

        internal static Domain DomainByName(string domainName)
        {
            foreach (Domain domain in Domain.GetAll(UIServices.KB.CurrentModel))
            {
                if (domain.Name == domainName)
                    return domain;
            }
            return (Domain)null;
        }

        internal static string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, "^\\s*$\\n|\\r", "", RegexOptions.Multiline);
        }

        internal static int LineCount(string s)
        {
            int num = 0;
            foreach (char ch in s)
            {
                if (ch == '\n')
                    ++num;
            }
            return num;
        }

        internal static string linkObject(KBObject obj)
        {
            return "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;OpenObject&name=" + obj.Guid.ToString() + "\">" + obj.Name + "</a>";
        }

        internal static string linkFile(string file)
        {
            return "<a href=\"file:///" + file + "\">" + file + "</a>";
        }

        internal static string ExtractRuleParm(KBObject obj)
        {
            RulesPart rulesPart = obj.Parts.Get<RulesPart>();
            string ruleParm = "";
            if ((KBPart<KBObject>)rulesPart != (KBPart<KBObject>)null)
            {
                Regex regex1 = new Regex("//.*", RegexOptions.None);
                Regex regex2 = new Regex("parm\\(.*\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                string source = rulesPart.Source;
                string input = regex1.Replace(source, "");
                Match match = regex2.Match(input);
                ruleParm = match == null ? "" : match.ToString();
            }
            return ruleParm;
        }

        internal static string CleanFileName(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        internal static string CreateOutputFile(KnowledgeBase KB, string title)
        {
            string path = KB.UserDirectory + "\\kbdoctor." + Utility.CleanFileName(title) + ".html";
            if (File.Exists(path))
                File.Delete(path);
            return path;
        }

        internal static bool AttIsSubtype(Artech.Genexus.Common.Objects.Attribute a)
        {
            return a.SuperTypeKey != (EntityKey)null;
        }

        internal static void KillAttribute(Artech.Genexus.Common.Objects.Attribute a)
        {
            IOutputService output = CommonServices.Output;
            foreach (EntityReference entityReference in a.GetReferencesTo())
            {
                KBObject objRef = KBObject.Get(a.Model, entityReference.From);
                if (objRef != (KBObject)null)
                {
                    Utility.CleanVariablesBasedInAttribute(a, output, objRef);
                    Utility.CleanSDT(a, output, objRef);
                    if (!(objRef is DataView))
                    {
                        try
                        {
                            objRef.Save();
                        }
                        catch (Exception ex)
                        {
                            output.AddErrorLine("ERROR: Can't save object: " + objRef.Name + ex.Message);
                        }
                    }
                }
            }
        }

        internal static void CleanVariablesBasedInAttribute(
          Artech.Genexus.Common.Objects.Attribute a,
          IOutputService output,
          KBObject objRef)
        {
            output.AddLine("Cleaning variables references to " + a.Name + " in " + objRef.Name);
            VariablesPart variablesPart = objRef.Parts.Get<VariablesPart>();
            if (!((KBPart<KBObject>)variablesPart != (KBPart<KBObject>)null))
                return;
            foreach (Variable variable in variablesPart.Variables)
            {
                if (!variable.IsStandard && (KBObject)variable.AttributeBasedOn != (KBObject)null && a.Name == variable.AttributeBasedOn.Name)
                {
                    output.AddLine("&" + variable.Name + " based on  " + a.Name);
                    eDBType type = variable.Type;
                    int length = variable.Length;
                    bool signed = variable.Signed;
                    string description = variable.Description;
                    int decimals = variable.Decimals;
                    variable.AttributeBasedOn = (Artech.Genexus.Common.Objects.Attribute)null;
                    variable.Type = type;
                    variable.Decimals = decimals;
                    variable.Description = description;
                    variable.Length = length;
                    variable.Signed = signed;
                }
            }
        }

        internal static KBCategory MainCategory(KBModel model)
        {
            return KBCategory.Get(model, "Main Programs");
        }

        public static EntityKey KeyOfBasedOn_CompatibleConEvo3(SDTItem sdtItem)
        {
            EntityKey entityKey = new EntityKey(Guid.Empty, 0);
            return sdtItem.BasedOn.Key;
        }

        internal static void CleanSDT(Artech.Genexus.Common.Objects.Attribute a, IOutputService output, KBObject objRef)
        {
            if (!(objRef is Artech.Genexus.Common.Objects.SDT))
                return;
            output.AddLine("Cleaning SDT references to " + a.Name + " in " + objRef.Name);
            foreach (SDTItem sdtItem in (IEnumerable<IStructureItem>)objRef.Parts.Get<SDTStructurePart>().Root.Items)
            {
                EntityKey entityKey = Utility.KeyOfBasedOn_CompatibleConEvo3(sdtItem);
                if (sdtItem.BasedOn != null && entityKey == a.Key)
                {
                    output.AddLine("..." + sdtItem.Name + " based on  " + a.Name);
                    eDBType type = sdtItem.Type;
                    int length = sdtItem.Length;
                    bool signed = sdtItem.Signed;
                    string description = sdtItem.Description;
                    int decimals = sdtItem.Decimals;
                    sdtItem.AttributeBasedOn = (Artech.Genexus.Common.Objects.Attribute)null;
                    sdtItem.Type = type;
                    sdtItem.Decimals = decimals;
                    sdtItem.Description = description;
                    sdtItem.Length = length;
                    sdtItem.Signed = signed;
                }
            }
        }

        internal static void CreateModuleNamesFile(KnowledgeBase KB)
        {
            string path = Path.Combine(Utility.SpcDirectory(KB), "NvgComparer") + "\\ModuleNames.txt";
            KBObjectDescriptor objectDescriptor = KBObjectDescriptor.Get("Module");
            List<string> moduleNames = new List<string>();
            foreach (KBObject kbObject in KB.DesignModel.Objects.GetAll(objectDescriptor.Id))
                moduleNames.Add(kbObject.QualifiedName.ToString());
            File.WriteAllLines(path, Utility.SortModulesByLevel(moduleNames).ToArray());
        }

        internal static List<string> SortModulesByLevel(List<string> moduleNames)
        {
            List<string> stringList = new List<string>();
            int num = 1;
            bool flag = false;
            while (!flag)
            {
                flag = true;
                foreach (string moduleName in moduleNames)
                {
                    if (Utility.LevelQualifiedName(moduleName) == num)
                    {
                        flag = false;
                        stringList.Add(moduleName);
                    }
                }
                ++num;
            }
            stringList.Reverse();
            return stringList;
        }

        internal static string GetModuleNamesFilePath(KnowledgeBase KB)
        {
            return Path.Combine(Utility.SpcDirectory(KB), "NvgComparer") + "\\ModuleNames.txt";
        }

        internal static int LevelQualifiedName(string name) => name.Split('.').Length;

        internal static string[] ReadQnameTypeFromNVGFile(string path, IOutputService output)
        {
            StreamReader streamReader = File.Exists(path) ? new StreamReader(path) : throw new ArgumentException("El archivo no existe. " + path);
            int num1 = 0;
            int num2 = 2;
            string Line = "";
            for (; num1 < num2; ++num1)
                Line = streamReader.ReadLine();
            streamReader.Dispose();
            string str = Utility.ReadTypeFromLine(Line);
            string[] strArray1 = Utility.ReadQnameFromLine(Line, output);
            string[] strArray2 = new string[3]
            {
        str,
        strArray1[0],
        strArray1[1]
            };
            streamReader.Dispose();
            return strArray2;
        }

        private static string ReadTypeFromLine(string Line)
        {
            string[] strArray = Line.Split(' ');
            return !(strArray[5] == "Web") ? strArray[5] : strArray[5] + strArray[6];
        }

        private static string[] ReadQnameFromLine(string Line, IOutputService output)
        {
            string[] strArray1 = Line.Split(' ');
            string source = !(strArray1[5] == "Web") ? strArray1[6] : strArray1[7];
            string[] strArray2 = new string[2];
            if (source.Contains<char>('.'))
            {
                string str = "";
                string[] strArray3 = source.Split('.');
                for (int index = 0; index < strArray3.Length - 1; ++index)
                    str = index + 1 >= strArray3.Length - 1 ? str + strArray3[index] : str + strArray3[index] + ".";
                strArray2[0] = str;
                strArray2[1] = strArray3[strArray3.Length - 1];
            }
            else
            {
                strArray2[0] = "";
                strArray2[1] = source;
            }
            return strArray2;
        }

        internal static bool FilesAreEqual(FileInfo first, FileInfo second)
        {
            int count = 8;
            if (first.Length != second.Length)
                return false;
            if (first.FullName == second.FullName)
                return true;
            int num = (int)Math.Ceiling((double)first.Length / (double)count);
            using (FileStream fileStream1 = first.OpenRead())
            {
                using (FileStream fileStream2 = second.OpenRead())
                {
                    byte[] buffer1 = new byte[count];
                    byte[] buffer2 = new byte[count];
                    for (int index = 0; index < num; ++index)
                    {
                        fileStream1.Read(buffer1, 0, count);
                        fileStream2.Read(buffer2, 0, count);
                        if (BitConverter.ToInt64(buffer1, 0) != BitConverter.ToInt64(buffer2, 0))
                            return false;
                    }
                }
            }
            return true;
        }

        internal static string ReturnPicture(Artech.Genexus.Common.Objects.Attribute a)
        {
            return a.Type.ToString() + "(" + a.Length.ToString() + (a.Decimals > 0 ? "." + a.Decimals.ToString() : "") + ")" + (a.Signed ? "-" : "");
        }

        internal static string ReturnPictureVariable(Variable v)
        {
            return v.Type.ToString() + "(" + v.Length.ToString() + (v.Decimals > 0 ? "." + v.Decimals.ToString() : "") + ")" + (v.Signed ? "-" : "");
        }

        internal static string ReturnPictureDomain(Domain d)
        {
            return d.Type.ToString() + "(" + d.Length.ToString() + (d.Decimals > 0 ? "." + d.Decimals.ToString() : "") + ")" + (d.Signed ? "-" : "");
        }

        internal static void SaveObject(IOutputService output, KBObject obj)
        {
            try
            {
                obj.Save();
            }
            catch (Exception ex)
            {
                output.AddErrorLine(ex.Message + " - " + (object)ex.InnerException);
            }
        }

        //internal static void WriteXSLTtoDir(KnowledgeBase KB)
        //{
        //    File.WriteAllText(KB.UserDirectory + "\\KBdoctorEv2.xslt", StringResources.specXEv2);
        //}

        internal static IEnumerable<KBObject> GetObjectsSOAP(KnowledgeBase KB)
        {
            return KB.DesignModel.Objects.GetByPropertyValue("CALL_PROTOCOL", (object)"SOAP");
        }

        internal static void ListAllProperties(KnowledgeBase KB, string generatorName)
        {
            //GxModel gxModel = KB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            //if (gxModel != null)
            //{
            //    foreach (var generator in gxModel.Generators)
            //    {
            //        if (ModelDefinitionsManager.GetGeneratorDefinition(generator.Generator).FriendlyName.Equals(generatorName, StringComparison.OrdinalIgnoreCase))
            //        {
            //            PropertiesObject propertiesObject = PropertiesObject.GetFrom(generator);
            //            if (propertiesObject != null)
            //            {
            //                PropertyDescriptorCollection properties = propertiesObject.GetProperties();
            //                foreach (PropertyDescriptor property in properties)
            //                {
            //                    string propertyName = property.Name;
            //                    string propertyValue = propertiesObject.GetPropertyValueString(propertyName);
            //                    CommonServices.Output.AddLine($"Property: {propertyName}, Value: {propertyValue}");
            //                    Console.WriteLine($"Property: {propertyName}, Value: {propertyValue}");
            //                }
            //            }
            //        }
            //    }
            //}
        }

        internal static void ListAllEnvironmentProperties(KnowledgeBase KB)
        {
            KBEnvironment environment = KB.DesignModel.Environment;
            if (environment != null)
            {
                PropertiesObject propertiesObject = PropertiesObject.GetFrom(environment);
                if (propertiesObject != null)
                {
                    PropertyDescriptorCollection properties = propertiesObject.GetProperties();
                    foreach (PropertyDescriptor property in properties)
                    {
                        string propertyName = property.Name;
                        string propertyValue = propertiesObject.GetPropertyValueString(propertyName);
                        CommonServices.Output.AddLine($"Property: {propertyName}, Value: {propertyValue}");
                        Console.WriteLine($"Property: {propertyName}, Value: {propertyValue}");
                    }
                }
            }
            else
            {
                CommonServices.Output.AddLine("No se pudo obtener el entorno del modelo actual.");
                Console.WriteLine("No se pudo obtener el entorno del modelo actual.");
            }
        }

        internal static void ListAllModelProperties(KnowledgeBase KB)
        {
            KBModel model = KB.DesignModel;
            if (model != null)
            {
                PropertiesObject propertiesObject = PropertiesObject.GetFrom(model);
                if (propertiesObject != null)
                {
                    PropertyDescriptorCollection properties = propertiesObject.GetProperties();
                    foreach (PropertyDescriptor property in properties)
                    {
                        string propertyName = property.Name;
                        string propertyValue = propertiesObject.GetPropertyValueString(propertyName);
                        CommonServices.Output.AddLine($"Property: {propertyName}, Value: {propertyValue}");
                        Console.WriteLine($"Property: {propertyName}, Value: {propertyValue}");
                    }
                }
            }
            else
            {
                CommonServices.Output.AddLine("No se pudo obtener el modelo actual.");
                Console.WriteLine("No se pudo obtener el modelo actual.");
            }
        }

        internal static void ListAllKBProperties(KnowledgeBase KB)
        {
            PropertiesObject propertiesObject = PropertiesObject.GetFrom(KB);
            if (propertiesObject != null)
            {
                PropertyDescriptorCollection properties = propertiesObject.GetProperties();
                foreach (PropertyDescriptor property in properties)
                {
                    string propertyName = property.Name;
                    string propertyValue = propertiesObject.GetPropertyValueString(propertyName);
                    CommonServices.Output.AddLine($"Property: {propertyName}, Value: {propertyValue}");
                    Console.WriteLine($"Property: {propertyName}, Value: {propertyValue}");
                }
            }
        }
        internal static string GetWebRootProperty(KnowledgeBase KB, string generatorName)
        {
            //GxModel gxModel = KB.DesignModel.Environment.TargetModel.GetAs<GxModel>();
            //if (gxModel != null)
            //{
            //    foreach (var generator in gxModel.Generators)
            //    {
                    
            //        CommonServices.Output.AddLine("generator " + generator);

            //        if (ModelDefinitionsManager.GetGeneratorDefinition(generator.Generator).FriendlyName.Equals(generatorName, StringComparison.OrdinalIgnoreCase))
            //        {
            //            PropertiesObject propertiesObject = PropertiesObject.GetFrom(generator);
            //            CommonServices.Output.AddLine("propertiesObject " + propertiesObject.ToString());
            //            if (propertiesObject != null)
            //            {
            //                return propertiesObject.GetPropertyValueString("WebRoot");
            //            }
            //        }
            //    }
            //}
            return string.Empty;
        }



        internal static bool VarHasToBeInDomain(Variable v) => Utility.TypeHasToBeInDomain(v.Type);

        internal static bool AttHasToBeInDomain(Artech.Genexus.Common.Objects.Attribute a)
        {
            return Utility.TypeHasToBeInDomain(a.Type);
        }

        internal static bool TypeHasToBeInDomain(eDBType type)
        {
            return type != eDBType.Boolean && type != eDBType.BITMAP && type != eDBType.BINARY && type != eDBType.GX_SDT && type != eDBType.GX_EXTERNAL_OBJECT && type != eDBType.GX_USRDEFTYP && type != eDBType.GX_BUSCOMP && type != eDBType.GX_BUSCOMP_LEVEL;
        }

        internal static bool IsGeneratedByPattern(KBObject obj)
        {
            bool flag1 = false;
            if (InstanceManager.IsInstanceObject(obj, out PatternDefinition _))
                flag1 = true;
            bool flag2 = false;
            foreach (PatternDefinition pattern in PatternEngine.Patterns)
            {
                if ((KBObject)PatternInstance.Get(obj, pattern.Id) != (KBObject)null)
                    flag2 = true;
            }
            return flag1 | flag2;
        }

        public static List<KBObject> ModuleObjects(Module module)
        {
            List<KBObject> kbObjectList1 = new List<KBObject>();
            foreach (KBObject allMember in module.GetAllMembers())
            {
                if (allMember is Folder)
                {
                    List<KBObject> kbObjectList2 = new List<KBObject>();
                    foreach (KBObject folderObject in Utility.FolderObjects((Folder)allMember))
                        kbObjectList1.Add(folderObject);
                }
                else if (allMember is Module)
                {
                    List<KBObject> kbObjectList3 = new List<KBObject>();
                    foreach (KBObject moduleObject in Utility.ModuleObjects((Module)allMember))
                        kbObjectList1.Add(moduleObject);
                }
                else if (!kbObjectList1.Contains(allMember))
                    kbObjectList1.Add(allMember);
            }
            return kbObjectList1;
        }

        public static bool IsMain(KBObject obj)
        {
            object propertyValue = obj.GetPropertyValue("isMain");
            return propertyValue != null && propertyValue.ToString() == "True";
        }

        public static List<KBObject> FolderObjects(Folder folder)
        {
            List<KBObject> kbObjectList1 = new List<KBObject>();
            foreach (KBObject allObject in folder.AllObjects)
            {
                if (allObject is Folder)
                {
                    List<KBObject> kbObjectList2 = new List<KBObject>();
                    foreach (KBObject folderObject in Utility.FolderObjects((Folder)allObject))
                        kbObjectList1.Add(folderObject);
                }
                else if (!kbObjectList1.Contains(allObject))
                    kbObjectList1.Add(allObject);
            }
            return kbObjectList1;
        }

        public static class HistoryLogger
        {
            private static List<ObjectHistory> historyTable = new List<ObjectHistory>();

            public static void LogChange(KBObject obj, string propertyName, string oldValue, string newValue)
            {
                if (oldValue != newValue)
                {
                    ObjectHistory history = new ObjectHistory
                    {
                        ObjectId = obj.Guid,
                        ObjectName = obj.Name,
                        PropertyName = propertyName,
                        OldValue = oldValue,
                        NewValue = newValue,
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = Environment.UserName
                    };

                    historyTable.Add(history);
                }
            }

            public static IEnumerable<ObjectHistory> GetHistory()
            {
                return historyTable;
            }
        }


        public static bool IsSDT(Variable variable)
        {
            return variable.Type == eDBType.GX_SDT;
        }

        public static bool IsVariableSDT(string variableName)
        {
            // Eliminar el carácter '&' del nombre de la variable si está presente
            string cleanVariableName = RemoveAmpersand(variableName);

            CommonServices.Output.AddLine($"Verificando si la variable '{cleanVariableName}' es un SDT...");

            KBObjectPart currentPart = Entorno.CurrentEditingPart;
            CommonServices.Output.AddLine($"currentPart: {currentPart}");

            if (currentPart != null)
            {
                KBObject kbObject = currentPart.KBObject;
                CommonServices.Output.AddLine($"kbObject: {kbObject.Name}");

                VariablesPart variablesPart = kbObject.Parts.Get<VariablesPart>();
                if (variablesPart != null)
                {
                    Variable variable = variablesPart.Variables.FirstOrDefault(v => v.Name.Equals(cleanVariableName, StringComparison.OrdinalIgnoreCase));
                    if (variable != null)
                    {
                        return Utility.IsSDT(variable);
                    }
                }
            }
            return false;
        }

        public static string RemoveAmpersand(string variable)
        {
            return variable.StartsWith("&") ? variable.Substring(1) : variable;
        }

        public static bool IsInSource()
        {
            KBObjectPart currentPart = Entorno.CurrentEditingPart;
            return currentPart is ISource;
        }

        public static bool IsInRules()
        {
            KBObjectPart currentPart = Entorno.CurrentEditingPart;
            return currentPart is RulesPart;
        }

        public static void ListSDTVariables(KBObject obj)
        {
            VariablesPart variablesPart = obj.Parts.Get<VariablesPart>();
            if (variablesPart != null)
            {
                foreach (Variable variable in variablesPart.Variables)
                {
                    if (IsSDT(variable))
                    {
                        CommonServices.Output.AddLine($"Variable: {variable.Name} es un SDT.");
                        Console.WriteLine($"Variable: {variable.Name} es un SDT.");
                    }
                }
            }
            else
            {
                CommonServices.Output.AddLine("El objeto no tiene una parte de variables.");
                Console.WriteLine("El objeto no tiene una parte de variables.");
            }
        }



    }

}
