using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Hangout.Client {
	public class Mockery {
		private List<Assembly> mLoadedAssemblies;
		private static Dictionary<string, string> mConvertOperators;

		/// 'Output' is human readable information that describes
		/// the operations internal to the parser.
		private Action<string[]> mReportOutput = null;

		public Mockery(Action<string[]> reportOutput, IEnumerable<Assembly> assembliesToMock) {
			mReportOutput = reportOutput;
			mLoadedAssemblies = new List<Assembly>(assembliesToMock);
		}

		public void UpdateOutput() {
			if (mReportOutput != null) {
				try {
					List<string> output = new List<string>();
					output.Add("Loaded Symbols:");
					foreach (Assembly assy in mLoadedAssemblies) {
						foreach (System.Type t in assy.GetExportedTypes()) {
							string topLine = "";
							if (t.IsAbstract) {
								topLine += "abstract ";
							}
							if (t.IsPublic) {
								topLine += "public ";
							}

							if (t.IsClass) {
								topLine += "class ";
							} else if (t.IsInterface) {
								topLine += "interface ";
							} else if (t.IsEnum) {
								topLine += "enum ";
							}

							topLine += t.Namespace + "." + t.Name;
							output.Add(topLine);

							foreach (MemberInfo mb in t.GetMembers()) {
								output.Add("\t\t" + mb);
							}

							output.Add("");
							output.Add("");
						}
					}
					mReportOutput(output.ToArray());
				} catch (ReflectionTypeLoadException e) {
					ReportReflectionTypeLoadException(e);
				}
			}
		}

		private void ReportReflectionTypeLoadException(ReflectionTypeLoadException reflectionException) {
			if (mReportOutput != null) {
				List<string> exceptionOutput = new List<string>();
				exceptionOutput.Add("Error loading Assembly:");
				foreach (Exception e in reflectionException.LoaderExceptions) {
					exceptionOutput.AddRange(e.Message.ToString().Split('\n'));
					exceptionOutput.AddRange(e.ToString().Split('\n'));
				}
				mReportOutput(exceptionOutput.ToArray());
			}
		}

		public void Clear() {
			mLoadedAssemblies.Clear();
			if (mReportOutput != null) {
				mReportOutput(new string[0]);
			}
		}

		public void SaveMockery(string rootPath) {
			mConvertOperators = new Dictionary<string, string>();
			mConvertOperators.Add("op_Equality", "operator ==");
			mConvertOperators.Add("op_Inequality", "operator !=");
			mConvertOperators.Add("op_Addition", "operator +");
			mConvertOperators.Add("op_Subtraction", "operator -");
			mConvertOperators.Add("op_Multiply", "operator *");
			mConvertOperators.Add("op_Division", "operator /");
			mConvertOperators.Add("op_Implicit", "implicit operator");
			mConvertOperators.Add("op_Explicit", "explicit operator");
			mConvertOperators.Add("op_UnaryNegation", "operator !");

			DirectoryInfo rootDirectory = new DirectoryInfo(rootPath);
			foreach (Assembly assy in mLoadedAssemblies) {
				DirectoryInfo currentDirectory = rootDirectory.CreateSubdirectory(assy.FullName.Split(',')[0]);
				foreach (System.Type t in assy.GetExportedTypes()) {
					using (StreamWriter sw = new StreamWriter(currentDirectory.ToString() + @"/" + t.ToString().Split('.')[1] + ".cs")) {

						// Write the 'using' lines
						sw.WriteLine("using System;");
						sw.WriteLine("using System.Collections;");
						sw.WriteLine("using System.Collections.Generic;");
						sw.WriteLine("using System.Runtime.Serialization;");

						sw.WriteLine();
						sw.WriteLine();

						if (t.Namespace != null) {
							sw.WriteLine("namespace " + t.Namespace + " {");
						}

						if (t.IsClass) {
							if (t.IsSubclassOf(typeof(MulticastDelegate))) {
								WriteDelegate(sw, t);
							} else {
								WriteClassOrStruct(sw, t, Kind.Class);
							}
						} else if (t.IsInterface) {
							WriteInterface(sw, t);
						} else if (t.IsEnum) {
							WriteEnum(sw, t);
						} else {
							WriteClassOrStruct(sw, t, Kind.Struct);
						}

						if (t.Namespace != null) {
							sw.WriteLine("}");
						}
					}
				}
			}
		}
		
		private string GetTypeDefault(System.Type t) {
			if( !t.IsEnum ) {
				return "default(" + GetTypeName(t) + ")";
			} else {
				return "(" + GetTypeName(t) + ") 0";
			}
		}
		
		private string GetTypeName(System.Type t) { 
			return GetTypeName(t, true);
		}
		
		private string GetTypeName(System.Type t, bool includeNamespace) {			
			// Filter out any namespace special characters (+)
			string result = "";
			if( includeNamespace && t.Namespace != null ) {
				result += t.Namespace + ".";
			}
			
			result += t.Name;

			if( result.Contains("`") ) { // This case seems to be a bug in KeyValuePair<T, U>, it doesn't register as a generic type
				result = result.Substring(0, result.IndexOf("`"));
				
				
			} else if( t.IsGenericType ){
				
				
				result += "<";
				
				bool first = true;
				foreach( System.Type genericArgument in t.GetGenericArguments() ) {
					if( first ) {
						first = false;
					} else {
						result += ", ";
					}
					
					result += GetTypeName(genericArgument);
				}
				
				result += ">";
			}
			
			if( result == "System.Void" ) {
				result = "void";
			}
			result = result.Replace("&", "");
			if( result.Contains("`") || result.Contains("+") || result.Contains("&") ) {
				throw new Exception("Bug happened while getting the type name for: " + result);
			}
			return result;
		}

		private void WriteDelegate(StreamWriter s, System.Type t) {
			s.WriteLine("public delegate void " + GetTypeName(t, false) + "();");
		}

		private void WriteMethodModifiers(StreamWriter s, MethodBase mb) {
			s.Write("\n\tpublic ");
			if (mb.IsStatic) {
				s.Write("static ");
			}

			if (mb.IsVirtual) {
				MethodInfo methodInfo = mb as MethodInfo;
				if( methodInfo == null || methodInfo.GetBaseDefinition().DeclaringType == methodInfo.DeclaringType) {
					s.Write("virtual ");
				} else {
					s.Write("override ");
				}
			}
		}

		private void WriteAttributes(System.Type t, StreamWriter s) {
			foreach (System.Object attr in t.GetCustomAttributes(false)) {
				System.Type attribType = attr.GetType();
				
				
				s.Write("[");
				s.Write(GetTypeName(attribType));
				
				ConstructorInfo[] constructors = attribType.GetConstructors(BindingFlags.Public);
				if( constructors.Length > 0 ) {
					
					s.Write("(");
					bool first = true;
					foreach(ParameterInfo pi in constructors[0].GetParameters() ) {
						if( first ) {
							first = false;
						} else {
							s.Write(", ");
						}
						GetTypeDefault(pi.ParameterType);
					}

					s.Write(")");
				}
				s.Write("]\n");
			}
		}

		private enum Kind { Class, Struct }
		
		private void WriteFunctionBody(StreamWriter s, MethodBase mb, List<string> outVarsToBeSet) {
			s.Write("{\n");
			
			if( !mb.IsStatic ) {
				s.WriteLine("\t\t//Mock Data:");
				
				if( mb.IsConstructor ) {
					s.WriteLine("\t\t\tmFunctionCallCounts = new Dictionary<string, int>();");
				} else {
					s.WriteLine("\t\tif(mFunctionCallCounts == null) {");
					s.WriteLine("\t\t\tmFunctionCallCounts = new Dictionary<string, int>();");
					s.WriteLine("\t\t}");
				}

				s.WriteLine("\t\tif(!mFunctionCallCounts.ContainsKey( \"" + mb + "\" )){");
				s.WriteLine("\t\t\tmFunctionCallCounts.Add( \"" + mb + "\", 0 );");
				s.WriteLine("\t\t}");
				s.WriteLine("\t\tmFunctionCallCounts[\"" + mb + "\"]++;");
			}
			
			foreach( string outVarLine in outVarsToBeSet ) {
				s.WriteLine("\t\t" + outVarLine);
			}
			
			MethodInfo methodInfo = mb as MethodInfo;
			if (methodInfo != null && methodInfo.ReturnType != typeof(void)) {
				s.Write("\t\treturn default(");
				s.Write(GetTypeName(methodInfo.ReturnType));
				s.Write(");\n");
			}
			s.Write("\t}\n");
		}
		
		private List<string> WriteParams(StreamWriter s, MethodBase mb) {
			s.Write("(");

			List<string> outVarsToBeSet = new List<string>(); // All the out params need to be set in the function
			bool first = true;
			foreach (ParameterInfo pi in mb.GetParameters()) {
				if (!first) {
					s.Write(", ");
				} else {
					s.Write(" ");
				}

				first = false;
				string paramString;
				if (pi.Name == "object") {
					paramString = GetTypeName(pi.ParameterType) + " MOCKERY_RENAMED_object";
				} else {
					paramString = GetTypeName(pi.ParameterType) + " " + pi.Name;
				}
				if (pi.IsOut) {
					paramString = "out " + paramString;
					outVarsToBeSet.Add(pi.Name + " = " + GetTypeDefault(pi.ParameterType) + ";" );
				} else if (pi.ToString().Contains("&")) {
					paramString = "ref " + paramString;
				}
				s.Write(paramString.Replace("&", ""));
			}

			s.Write(" )");
			
			return outVarsToBeSet;
		}

		private void WriteClassOrStruct(StreamWriter s, System.Type t, Kind k) {
			WriteAttributes(t, s);
			s.Write("public ");
			if (t.IsAbstract) {
				s.Write("abstract ");
			}
			s.Write(k.ToString().ToLower());
			s.Write(" ");

			s.Write(GetTypeName(t, false));

			List<string> implements = new List<string>(); 
			
			if( t.BaseType != null && t.BaseType != typeof(object) && t.BaseType != typeof(ValueType) && t.BaseType.IsPublic ) {
				implements.Add(GetTypeName(t.BaseType));
			}
			
			foreach(System.Type interfaceType in t.GetInterfaces()) {			
				if (interfaceType.Name.StartsWith("_") || t.BaseType.IsPublic) {
					// Ignore the special IL classes like _Exception and _Attribute,
					// the class will still inherit from System.Exception or System.Attribute
					continue;
				}
				
				implements.Add(GetTypeName(interfaceType));
			}
			
			if( implements.Count > 0 ) {
				s.Write(" : ");
				bool first = true;
				foreach( string implement in implements ) {
					if( first ) {
						first = false;
					} else {
						s.Write(", ");
					}
					
					s.Write(implement);
				}
			}

			s.Write(" {\n");
			s.WriteLine("\t// Mock data:");
			s.WriteLine("\tprivate Dictionary<string, int> mFunctionCallCounts;");
			s.WriteLine("\tpublic Dictionary<string, int> functionCallCounts {");
			s.WriteLine("\t\tget { ");
			s.WriteLine("\t\t\tif(mFunctionCallCounts == null) {");
			s.WriteLine("\t\t\t\tmFunctionCallCounts = new Dictionary<string, int>();");
			s.WriteLine("\t\t\t}");
			s.WriteLine("\t\t\treturn mFunctionCallCounts;");
			s.WriteLine("\t\t}");
			s.WriteLine("\t}");

			WriteMemberFunctions(s, t);
			
			s.WriteLine("}");
		}
		
		private void WriteMemberFunctions(StreamWriter s, System.Type t) {
			Dictionary<string, List<MethodInfo>> properties = new Dictionary<string, List<MethodInfo>>();

			List<MethodBase> mbs = new List<MethodBase>(t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public));
			mbs.AddRange(t.GetConstructors());

			foreach (MethodBase mb in mbs) {
				// Hack
				// If this is a property, handle it after the 'normal' functions are written
				// This isn't generic, it'll only work with code bases that use camel case
				if (mb.Name.StartsWith("get_") || mb.Name.StartsWith("set_")) {
					string propertyName = mb.Name.Remove(0, 4);
					if (!properties.ContainsKey(propertyName)) {
						properties.Add(propertyName, new List<MethodInfo>());
					}
					properties[propertyName].Add((MethodInfo)mb);
					continue;
				}

				WriteMethodModifiers(s, mb);
				
				// If the method is an overloaded operator, replace the
				//  name with the better formatted name (conversions are set
				//  up in the beginning of SaveMockery)
				if( mb.IsConstructor ) {
					s.Write(t.Name);
				} else {
					MethodInfo mi = (MethodInfo)mb; // The non-constructor functions should always be MethodInfos
					if (mb.Name == "op_Implicit" || mb.Name == "op_Explicit") {
						
						if (!mConvertOperators.ContainsKey(mb.Name)) {
							throw new Exception("mConvertOperators must have entries for both 'op_Implicit' and 'op_Explicit'");
						}

						s.Write(mConvertOperators[mi.Name]);

						s.Write(" ");
						s.Write(GetTypeName(mi.ReturnType));
					} else {
						s.Write(GetTypeName(mi.ReturnType));
						s.Write(" ");

						if (mConvertOperators.ContainsKey(mi.Name)) {
							s.Write(mConvertOperators[mi.Name]);
						} else {
							s.Write(mi.Name);
						}
					}	
				}

				List<string> outVarsToBeSet = WriteParams(s, mb);
				WriteFunctionBody(s, mb, outVarsToBeSet);
			}

			// Write the properties
			foreach (KeyValuePair<string, List<MethodInfo>> property in properties) {
				WriteMethodModifiers(s, property.Value[0]);
				s.Write(GetTypeName(property.Value[0].ReturnType));
				s.Write(" ");
				s.Write(property.Key);
				s.Write(" {\n");
				bool hasGet = false;
				bool hasSet = false;
				bool isStatic = false;
				foreach (MethodInfo mb in property.Value) {
					if( mb.IsStatic ) {
						isStatic = true;
					}
					if (mb.Name.StartsWith("get_")) {
						hasGet = true;
					} else if (mb.Name.StartsWith("set_")) {
						hasSet = true;
					}
				}

				if (hasGet) {
					s.Write("\t\tget {\n");
					
					if( !isStatic ) {
						s.WriteLine("\t\t\tif(mFunctionCallCounts == null) {");
						s.WriteLine("\t\t\t\tmFunctionCallCounts = new Dictionary<string, int>();");
						s.WriteLine("\t\t\t}");

						s.WriteLine("\t\t\tif(!mFunctionCallCounts.ContainsKey( \"get_" + property.Key + "\" )){");
						s.WriteLine("\t\t\t\tmFunctionCallCounts.Add( \"get_" + property.Key + "\", 0 );");
						s.WriteLine("\t\t\t}");
						s.WriteLine("\t\t\tmFunctionCallCounts[\"get_" + property.Key + "\"]++;");	
					}
					
					s.Write("\t\t\treturn default(");
					s.Write(GetTypeName(property.Value[0].ReturnType));
					s.Write(");\n");
					s.Write("\t\t}\n");
				}
				if (hasSet) {
					s.WriteLine("\t\tset {");
					
					if( !isStatic ) {
						s.WriteLine("\t\t\tif(mFunctionCallCounts == null) {");
						s.WriteLine("\t\t\t\tmFunctionCallCounts = new Dictionary<string, int>();");
						s.WriteLine("\t\t\t}");

						s.WriteLine("\t\t\tif(!mFunctionCallCounts.ContainsKey( \"set_" + property.Key + "\" )){");
						s.WriteLine("\t\t\t\tmFunctionCallCounts.Add( \"set_" + property.Key + "\", 0 );");
						s.WriteLine("\t\t\t}");
						s.WriteLine("\t\t\tmFunctionCallCounts[\"set_" + property.Key + "\"]++;");
					}
					
					s.WriteLine("\t\t}");
				}
				s.Write("\t}\n");
			}
		}

		private void WriteInterface(StreamWriter s, System.Type t) {
			s.Write("public interface ");
			s.Write(t.Name);
			if (t.BaseType != typeof(object) && t.BaseType != null) {
				s.Write(" : ");
				s.Write(t.BaseType.Name);
			}
			s.Write(" {\n");

			foreach (MemberInfo mb in t.GetMembers()) {
				s.Write(mb);
				s.Write(";\n");
			}
			s.WriteLine("}");
		}

		private void WriteEnum(StreamWriter s, System.Type t) {
			s.WriteLine("public enum " + t.Name + " {");
			bool first = true;
			foreach (MemberInfo mb in t.GetMembers()) {

				if (mb.ToString().Contains(t.Name)) {
					if (mb.ToString().Contains("(")) {
						continue; // Skip functions
					}
					string[] possibleType = mb.ToString().Split('.');
					if (possibleType.Length > 1) {
						if (!first) {
							s.Write(",\n");
						}

						first = false;
						s.Write("\t" + possibleType[1].Split(' ')[1]);
					}
				}
			}
			s.WriteLine();
			s.WriteLine("}");
		}

		public int LoadedTypes {
			get {
				int result = 0;
				foreach (Assembly assembly in mLoadedAssemblies) {
					try {
						result += assembly.GetExportedTypes().Length;
					} catch (ReflectionTypeLoadException e) {
						ReportReflectionTypeLoadException(e);
					}
				}

				return result;
			}
		}
	}
}