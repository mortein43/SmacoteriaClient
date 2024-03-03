using System.Xml.Linq;

namespace Smacoteria.Controls;

public static class RsourcesControl
{

    public static void AddResourceToProjectFile(string projectFilePath, string newResourcePath)
    {
        projectFilePath += "\\Smacoteria.csproj";
        string itemGroupName = "ItemGroup";

        try
        {
            XDocument projectFile = XDocument.Load(projectFilePath);

            XElement itemGroup = projectFile.Descendants(itemGroupName)
                                             .FirstOrDefault(ig => ig.Elements("Resource")
                                                                     .Any(e => e.Attribute("Include").Value == newResourcePath));

            if (itemGroup == null)
            {
                itemGroup = projectFile.Descendants(itemGroupName)
                                       .FirstOrDefault(ig => ig.Elements("Resource").Any());

                if (itemGroup != null)
                {
                    XElement newResourceElement = new XElement("Resource", new XAttribute("Include", newResourcePath));
                    itemGroup.Add(newResourceElement);
                    projectFile.Save(projectFilePath);

                }
                else
                {
                }
            }
            else
            {
            }
        }
        catch (Exception ex)
        {
        }
    }
}
