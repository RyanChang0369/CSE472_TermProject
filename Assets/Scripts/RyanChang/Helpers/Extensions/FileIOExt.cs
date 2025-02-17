using System;
using System.IO;

/// <summary>
/// Contains methods pertaining to file IO.
/// </summary>
public static class FileIOExt
{
    /// <summary>
    /// Name of project, to be used to name the save files folder.
    /// </summary>
    private const string PROJECT_NAME = "Undefined";

    /// <summary>
    /// Write to a save file.
    /// </summary>
    /// <param name="output">What to write to the file.</param>
    /// <param name="fileName">Name of the file to write.</param>
    public static void WriteSaveFile(string output, string fileName)
    {
        string filePath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
                PROJECT_NAME, fileName);

        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), PROJECT_NAME));
        }

        using (StreamWriter sw = new(filePath))
        {
            sw.Write(output);
        }
    }

    /// <summary>
    /// Reads from the save file.
    /// </summary>
    /// <param name="fileName">Name of save file.</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static string ReadSaveFile(string fileName)
    {
        string filePath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
                PROJECT_NAME, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Cannot find " + fileName);
        }

        using (StreamReader sr = new(filePath))
        {
            return sr.ReadToEnd();
        }
    }
}