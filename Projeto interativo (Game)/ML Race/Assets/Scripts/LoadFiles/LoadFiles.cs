using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

[XmlRoot("PathFiles")]
public class GlobalPaths
{
    [XmlAttribute("server")]
    public string executeServerFilePath;
    [XmlAttribute("predictions")]
    public string predictionsFolderPath;
    [XmlAttribute("assets")]
    public string imageAssetsPath;

    GlobalPaths()
    {

    }

    public GlobalPaths(string server, string predictions, string assets)
    {
        executeServerFilePath = server;
        predictionsFolderPath = predictions;
        imageAssetsPath = assets;
    }
}


public class LoadFiles : MonoBehaviour
{
    [XmlElement("Paths")]
    public static GlobalPaths globalPaths;

    public static void SetPaths()
    {
        globalPaths = new GlobalPaths(
            "C:/Users/Casa Not/Desktop/Senac/Pi 5/trunk/Entrega 3/src/execute.bat",
            "C:/Users/Casa Not/Desktop/Senac/Pi 5/trunk/Entrega 3/predictions/",
            "C:/Users/Casa Not/Desktop/Senac/Pi 5/trunk/Entrega 3/assets/"
            );
    }

    public static void SaveXML()
    {

        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);

        var serializer = new XmlSerializer(typeof(GlobalPaths));
        var encoding = Encoding.GetEncoding("UTF-8");
        StreamWriter stream;
        using (stream = new StreamWriter(Application.streamingAssetsPath + "/Paths.xml", false, encoding))
        {
            serializer.Serialize(stream, globalPaths);
        }

        //var stream = new FileStream(Application.streamingAssetsPath + "/Paths.xml", FileMode.Create);
        //serializer.Serialize(stream, globalPaths);
        //stream.Close();
    }

    public static void LoadXML()
    {
        var serializer = new XmlSerializer(typeof(GlobalPaths));
        var stream = new FileStream(Application.streamingAssetsPath + "/Paths.xml", FileMode.Open);
        GlobalPaths container = serializer.Deserialize(stream) as GlobalPaths;
        stream.Close();

        globalPaths = container;
    }
}
