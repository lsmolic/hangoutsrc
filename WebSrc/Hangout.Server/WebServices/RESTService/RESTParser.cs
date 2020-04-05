using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Hangout.Shared;
using System.Configuration;

namespace Hangout.Server.WebServices
{
	public class RESTParser
	{
		private ServicesLog mServiceLog = new ServicesLog("RESTParser");
		private const string mServicesRoot = "Hangout.Server.WebServices.";
		private string mAssemblyAltLocation = "";
		private bool mDebugMode = (ConfigurationManager.AppSettings["DebugMode"] == "true") ? true : false;

		public RESTParser(string assemblyAltLocation)
		{
			mAssemblyAltLocation = assemblyAltLocation.Trim();
		}

		public RESTParser()
		{

		}

		/// <summary>
		/// Process a REST Service call
		/// </summary>
		/// <param name="restCommand">RESTCommand</param>
		/// <returns>xml string containing the response</returns>
		public string ProcessRestService(RESTCommand restCommand)
		{
			Globals.AddCurrentRemotingRequest();
			if (mDebugMode)
			{
				mServiceLog.Log.Debug("Total Remoting Service Requests: " + Globals.GetTotalRemotingRequests);
			}
			mServiceLog.Log.Debug("++++ Current Remoting Service Requests: " + Globals.GetCurrentRemotingRequests);
			string response = "";

			try
			{
				Type commandClassType = GetCommandClassType(restCommand.Noun);

				if (commandClassType == null)
				{
					throw new ArgumentException("Service noun could not be found: " + restCommand.Noun);
				}

				MethodInfo methodInfo = commandClassType.GetMethod(restCommand.Verb);

				if (methodInfo == null)
				{
					throw new ArgumentException("Service verb could not be found: " + restCommand.Verb);
				}
				// loop though each parameter in the method and look for argIndex corresponding
				// query parameter in the REST request URL.
				string argString = "";
				string delimiter = "";
				int numArgs = methodInfo.GetParameters().Length;
				object[] args = new object[numArgs];

				for (int argIndex = 0; argIndex < numArgs; argIndex++)
				{
					ParameterInfo parameterInfo = methodInfo.GetParameters()[argIndex];
					argString += delimiter + parameterInfo.Name + "=" + Util.GetHttpValue(parameterInfo.Name);
					delimiter = "; ";
					if (parameterInfo.ParameterType == typeof(int))
					{
						string arg = GetParameterValue(restCommand.Parameters, parameterInfo.Name);
						int p = Int32.Parse(arg);
						args[argIndex] = p;
					}
					else if (parameterInfo.ParameterType == typeof(string))
					{
						args[argIndex] = GetParameterValue(restCommand.Parameters, parameterInfo.Name);
					}
					else if (parameterInfo.ParameterType == typeof(PostFile))
					{
						PostFile postFile = new PostFile();
						postFile = restCommand.PostFile;
						args[argIndex] = postFile;
					}
					else if (parameterInfo.ParameterType == typeof(Session.UserId))
					{
						string stringUserId = GetParameterValue(restCommand.Parameters, "userId");
						int intUserId = System.Convert.ToInt32(stringUserId);
						Session.UserId userId = new Session.UserId(intUserId);
						args[argIndex] = userId;
					}
					else if (parameterInfo.ParameterType == typeof(RESTCommand))
					{
						args[argIndex] = restCommand;
					}
					else
					{
						throw new ArgumentException("unsupported argument type");
					}
				}
				if (mDebugMode)
				{
					mServiceLog.Log.Debug("ProcessRestService (remoting)(Invoke Method) (" + restCommand.Noun + ")(" + restCommand.Verb + ") args: " + argString);
				}
				Object invokeParam1 = Activator.CreateInstance(commandClassType);

				object reflectionObject = (object)methodInfo.Invoke(invokeParam1, args);

				StringWriterWithEncoding stringWriter = new StringWriterWithEncoding(Encoding.UTF8);
				XmlTextWriterFormattedNoDeclaration xmlTextWriter = new XmlTextWriterFormattedNoDeclaration(stringWriter);

				new XmlSerializer(methodInfo.ReturnType).Serialize(xmlTextWriter, reflectionObject);
				Globals.RemoveCurrentRemotingRequest();
				if (mDebugMode)
				{
					mServiceLog.Log.Debug("---- Current Remoting Service Requests: " + Globals.GetCurrentRemotingRequests);

				}
				response = stringWriter.ToString();
			}

			catch (Exception ex)
			{
				Globals.RemoveCurrentRemotingRequest();
				if (mDebugMode)
				{
					mServiceLog.Log.Debug("---- Current Remoting Service Requests: " + Globals.GetCurrentRemotingRequests);
				}
				mServiceLog.Log.Error("ProcessRequest (remoting) ERROR:" + ex.Message);
				//logError("ProcessRequest", ex);
				//response = CreateErrorDoc(ex.Message);
			}

			return response;
		}

		/// <summary>
		/// GetThe Parameter Value given the NameValueCollection paramaters and a key
		/// </summary>
		/// <param name="parameters">key value pair of parameters</param>
		/// <param name="key">Key index into NameValueCollection parameters</param>
		/// <returns>string parameter value</returns>
		public string GetParameterValue(NameValueCollection parameters, string key)
		{
			string value = "";

			try
			{
				value = parameters[key];
			}

			catch { }

			return value;
		}


		/// <summary>
		/// Given the noun retreives the type for the class that holds the noun. 
		/// </summary>
		/// <param name="commandNoun">The command noun (class)</param>
		/// <returns>return the command noun class type</returns>
		public Type GetCommandClassType(string commandNoun)
		{
			Type commandClassType = null;

			Assembly serviceAssembly = null;

			// Assembly execAssembly = Assembly.GetExecutingAssembly();
			// string  dllToLoad = execAssembly.Location.Replace("RESTService", commandNoun); 
			// AssemblyName an = AssemblyName.GetAssemblyName(dllToLoad);
			// serviceAssembly = Assembly.Load(an);

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;

			try
			{
				serviceAssembly = Assembly.Load(mServicesRoot + commandNoun);
			}

			catch { }


			foreach (Type type in serviceAssembly.GetExportedTypes())
			{
				// see if the class implements Hangout.Server.WebServices.IRESTService
				if (type.IsClass && typeof(Hangout.Server.WebServices.IRESTService).IsAssignableFrom(type))
				{
					commandClassType = type;
					break;
				}
			}

			return commandClassType;
		}

		Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly loadedAssembly = null;

			try
			{
				Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();

				foreach (Assembly currentAssembly in currentAssemblies)
				{
					if (currentAssembly.FullName == args.Name)
					{
						loadedAssembly = currentAssembly;
						break;
					}
				}

				string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				loadedAssembly = LoadTheAssembly(currentPath + "\\", args.Name);

				if (loadedAssembly == null)
				{
					loadedAssembly = LoadTheAssembly(mAssemblyAltLocation, args.Name);
				}
			}

			catch (Exception ex)
			{
				// logError("CurrentDomainAssemblyResolve", ex);
			}

			return loadedAssembly;
		}

		private Assembly LoadTheAssembly(string resolutionPath, string assemblyName)
		{
			Assembly loadedAssembly = null;

			try
			{
				if (loadedAssembly == null)
				{
					string assemblyToLoad = Path.GetFullPath(resolutionPath + assemblyName + ".dll");
					loadedAssembly = Assembly.LoadFrom(assemblyToLoad);
				}
			}

			// catch errors so we can just return null and move on to the next location
			catch { }

			return loadedAssembly;
		}
	}


	/// <summary>
	/// String Writer that allows the Encoding to be set
	/// </summary>
	public class StringWriterWithEncoding : StringWriter
	{
		Encoding encoding;

		public StringWriterWithEncoding(Encoding encoding)
		{
			this.encoding = encoding;
		}

		public override Encoding Encoding
		{
			get { return encoding; }
		}
	}
}
