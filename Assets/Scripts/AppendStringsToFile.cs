using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AppendStringsToFile : MonoBehaviour
{
    [Header("Đường dẫn tới file .txt (đầy đủ)")]
    public string filePath;

    [Header("Chuỗi sẽ được thêm vào (các giá trị cách nhau bởi dấu phẩy)")]
    [TextArea(3, 10)]
    public string csvValues; // chuỗi 1 dòng như "10,20,30,..."

    [ContextMenu("Thêm vào file")]
    public void AppendToFile()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("❌ File không tồn tại: " + filePath);
            return;
        }

        string[] valuesToAppend = csvValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (valuesToAppend.Length == 0)
        {
            Debug.LogError("❌ Chuỗi csvValues rỗng hoặc không hợp lệ.");
            return;
        }

        Debug.Log($"Số giá trị trong csvValues: {valuesToAppend.Length}");

        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length != valuesToAppend.Length)
        {
            Debug.LogError($"❌ Số dòng trong file ({lines.Length}) và số giá trị nhập vào ({valuesToAppend.Length}) không khớp.");
            return;
        }

        try
        {
            for (int i = 0; i < lines.Length; i++)
            {
                string value = valuesToAppend[i].Trim();
                if (string.IsNullOrEmpty(value) || !float.TryParse(value, out _))
                {
                    Debug.LogError($"❌ Giá trị không hợp lệ tại vị trí {i}: {value}");
                    return;
                }
                lines[i] += "," + value;
            }

            File.WriteAllLines(filePath, lines);
            Debug.Log("✅ Ghi file thành công.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Lỗi khi ghi file: {ex.Message}");
        }
    }
    
    
    [ContextMenu("Xóa cột cuối")]
    public void RemoveLastColumn()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("❌ File không tồn tại: " + filePath);
            return;
        }

        try
        {
            string fileContent = File.ReadAllText(filePath);
            string updatedContent = RemoveLastColumn(fileContent, ',');
            File.WriteAllText(filePath, updatedContent);
            Debug.Log("✅ Đã xóa cột cuối và ghi lại vào file.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Lỗi khi xóa cột cuối: {ex.Message}");
        }
    }
    public string RemoveLastColumn(string csvText, char separator = '\t')
    {
        var lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> updatedLines = new List<string>();

        foreach (var line in lines)
        {
            var columns = line.Split(separator)
                              .Select(c => c.Trim())
                              .ToArray();

            if (columns.Length > 1)
            {
                updatedLines.Add(string.Join(separator.ToString(), columns.Take(columns.Length - 1)));
            }
            else
            {
                updatedLines.Add(string.Join(separator.ToString(), columns));
            }
        }

        return string.Join("\n", updatedLines);
    }
}