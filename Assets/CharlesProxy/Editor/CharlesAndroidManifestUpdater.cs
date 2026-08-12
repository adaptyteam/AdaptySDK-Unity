using System.IO;
using System.Text;
using System.Xml;
using UnityEditor.Android;
using UnityEngine;

public class CharlesAndroidManifestUpdater : IPostGenerateGradleAndroidProject
{
    public void OnPostGenerateGradleAndroidProject(string basePath)
    {
        // If needed, add condition checks on whether you need to run the modification routine.
        // For example, specific configuration/app options enabled
        var androidManifest = new AndroidManifest(GetManifestPath(basePath));

        // Add your XML manipulation routines
        androidManifest.SetNetworkSecurityConfig();

        //Save the new manifest
        androidManifest.Save();
    }

    public int callbackOrder { get { return 1; } }

    private string _manifestFilePath;

    private string GetManifestPath(string basePath)
    {
        if (string.IsNullOrEmpty(_manifestFilePath))
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _manifestFilePath = pathBuilder.ToString();
        }
        return _manifestFilePath;
    }
}


internal class AndroidXmlDocument : XmlDocument
{
    private string m_Path;
    private XmlNamespaceManager m_NsMgr;
    protected readonly string m_AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

    protected AndroidXmlDocument(string path)
    {
        m_Path = path;
        using (var reader = new XmlTextReader(m_Path))
        {
            reader.Read();
            Load(reader);
        }
        m_NsMgr = new XmlNamespaceManager(NameTable);
        m_NsMgr.AddNamespace("android", m_AndroidXmlNamespace);
    }

    public string Save()
    {
        return SaveAs(m_Path);
    }

    public string SaveAs(string path)
    {
        using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
        {
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
        return path;
    }
}


internal class AndroidManifest : AndroidXmlDocument
{
    private readonly XmlElement m_ApplicationElement;

    public AndroidManifest(string path) : base(path)
    {
        m_ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
    }

    internal void SetNetworkSecurityConfig() {
        m_ApplicationElement.Attributes.Append(CreateAndroidAttribute("networkSecurityConfig", "@xml/network_security_config"));
    }

    private XmlAttribute CreateAndroidAttribute(string key, string value)
    {
        XmlAttribute attr = CreateAttribute("android", key, m_AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }
}
