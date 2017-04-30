<Query Kind="Program">
  <NuGetReference>NLog.Config</NuGetReference>
  <Namespace>NLog</Namespace>
  <Namespace>NLog.Common</Namespace>
  <Namespace>NLog.Conditions</Namespace>
  <Namespace>NLog.Config</Namespace>
  <Namespace>NLog.Filters</Namespace>
  <Namespace>NLog.Fluent</Namespace>
  <Namespace>NLog.Internal</Namespace>
  <Namespace>NLog.Internal.Fakeables</Namespace>
  <Namespace>NLog.LayoutRenderers</Namespace>
  <Namespace>NLog.LayoutRenderers.Wrappers</Namespace>
  <Namespace>NLog.Layouts</Namespace>
  <Namespace>NLog.LogReceiverService</Namespace>
  <Namespace>NLog.Targets</Namespace>
  <Namespace>NLog.Targets.Wrappers</Namespace>
  <Namespace>NLog.Time</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Diagnostics</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Text.RegularExpressions</Namespace>
  <CopyLocal>true</CopyLocal>
</Query>

class Program
{
	private static Logger logger = LogManager.GetCurrentClassLogger();

	static void Main()
	{

		var fileContents = UserApplicationDataHelper.ValidateUserData("NvidaDriverDateChecker", "LastCheckedDate.txt");

		WebClient webClient = new WebClient();
		List<string> contentList = new List<string>();
		string s = webClient.DownloadString("http://www.geforce.com/whats-new/tag/drivers");

		MatchCollection matches = Regex.Matches(s, @"(<span class=""date-display-single"".*?>.*?</span>)", RegexOptions.Singleline);

		foreach (Match match in matches)
		{
			string value = match.Groups[1].Value;
			// Remove inner tags from text.
			string cleanText = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
			contentList.Add(cleanText);
		}

		var latest = contentList.FirstOrDefault();
		var msg = string.Empty;

		if (string.IsNullOrEmpty(latest))
		{
			msg = "Could not retrieve latest date!";
		}
		else if (string.IsNullOrWhiteSpace(fileContents))
		{
			msg = string.Format("No previous date recorded, writing {0}", latest);			
		}
		else
		{
			var doDatesMatch = (fileContents == latest);
			msg = string.Format("{0}ew Driver, previous date : {1} {2} newest date : {3}", (doDatesMatch ? "No n" : "N"), fileContents,(doDatesMatch ? "matches" : "doesn't match"), latest);
		}
		
		if (!string.IsNullOrWhiteSpace(latest))
		{
			UserApplicationDataHelper.WriteToFile(latest);
		}

		logger.Debug(msg);
		msg.Dump();
		logger.Trace("\nEnd Run\n");

#if CMD
   "Press any key to exit".Dump();
   Console.ReadKey();
#else
		
#endif

	}


}

public static class UserApplicationDataHelper
{
	private static Logger logger = LogManager.GetCurrentClassLogger();
	private static string applicationDirectory = "";
	private static string applicationFile = "";

	public static string ValidateUserData(string ApplicationDirectoryName, string FileName)
	{
		applicationDirectory =
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationDirectoryName);

		logger.Trace(string.Format("Validate User directory: {0}", applicationDirectory));
		CreateDirectory(applicationDirectory);
		applicationFile = Path.Combine(applicationDirectory, FileName);
		logger.Trace(string.Format("Validate file: {0}", applicationFile));
		WriteToFile();
		string results = ReadFile(applicationFile);
		logger.Trace(string.Format("Read file: {0} characters read from {1}", results.Length, applicationFile));
		return results;
	}

	public static string ReadFile()
	{
		return ReadFile(applicationFile);
	}

	public static string ReadFile(string filePath = "")
	{

		var results = string.Empty;

		if (!File.Exists(filePath))
		{
			var errorMessage = string.Format("File not found, While attempting a read : {0}", filePath);
			var ex = new FileNotFoundException(errorMessage);
			logger.Error(ex);
			throw ex;
		}

		try
		{
			results = File.ReadAllText(filePath);
		}
		catch (Exception ex)
		{
			logger.Error(ex, string.Format("Error attempting to read from {0}", filePath));
			throw;
		}

		return results;
	}
	public static void WriteToFile(string contents = "")
	{
		WriteToFile(applicationFile, contents);
	}

	public static void WriteToFile(string filePath, string contents = "")
	{

		try
		{
			bool doesFileExist = File.Exists(filePath);

			logger.Trace(string.Format("File does{0} exist : {1}", (doesFileExist ? "" : " not"), filePath));

			if (doesFileExist)
			{
				if (!string.IsNullOrEmpty(contents))
				{
					logger.Trace(string.Format("Contents exist, writing {0} characters to {1}", contents.Length, filePath));
					File.WriteAllText(filePath, contents);
				}
				else
				{
					logger.Debug("File exist, but no contents to write, Do nothing");
				}
			}
			else
			{
				logger.Trace(string.Format("Contents of {0} characters used to create {1}", contents.Length, filePath));
				File.WriteAllText(filePath, contents);
			}

		}
		catch (Exception ex)
		{
			logger.Error(ex, String.Format("Failed to create or write to file: {0}", filePath));
			throw;
		}
	}

	public static void CreateDirectory()
	{
		CreateDirectory(applicationDirectory);
	}

	public static void CreateDirectory(string directoryPath)
	{
		if (!Directory.Exists(directoryPath))
		{
			logger.Trace(String.Format("Directory not found, attempt to create Directory: {0}", directoryPath));
			try
			{
				Directory.CreateDirectory(directoryPath);
			}
			catch (Exception ex)
			{
				logger.Error(ex, String.Format("Failed to create Directory: {0}", directoryPath));
				throw;
			}

		}
		else
		{
			logger.Trace(String.Format("Directory found : {0}", directoryPath));
		}
	}


}