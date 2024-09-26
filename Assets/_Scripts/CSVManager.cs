using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    // File path to the CSV
    private string filePath;

    private void Start()
    {
        // Set the file path (you can customize this path)
        filePath = Path.Combine(Application.dataPath, "data.csv");

        // Example data to write to the CSV
        List<string[]> dataToWrite = new()
        {
            new[] { "Name", "Age", "Score" },
            new[] { "John", "23", "100" },
            new[] { "Jane", "29", "90" },
            new[] { "Alice", "25", "95" }
        };

        // Write the data
        WriteToCSV(dataToWrite);

        // Read the data back
        List<string[]> dataRead = ReadFromCSV(filePath);

        // Output the read data to the console
        foreach (var row in dataRead)
        {
            Debug.Log(string.Join(",", row));
        }
    }

    // Method to write data to a CSV file
    public void WriteToCSV(List<string[]> data)
    {
        using (StreamWriter sw = new(filePath))
        {
            foreach (var line in data)
            {
                sw.WriteLine(string.Join(",", line));
            }
        }

    }

    public void AppendToCSV(List<string[]> data)
    {
        // Use StreamWriter with append set to true
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            foreach (var line in data)
            {
                sw.WriteLine(string.Join(",", line));
            }
        }

        Debug.Log($"Data successfully appended to {filePath}");
    }

    // Method to read data from a CSV file
    public List<string[]> ReadFromCSV(string path)
    {
        List<string[]> data = new List<string[]>();

        if (File.Exists(path))
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    data.Add(values);
                }
            }

            Debug.Log($"Data successfully read from {path}");
        }
        else
        {
            Debug.LogError($"File not found: {path}");
        }

        return data;
    }
}
