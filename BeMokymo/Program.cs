using System;
using System.IO;
using System.Xml.Linq;
using PadPaveiksliukais;
using PDFtoImage;

class PadPaveiksliukas
{
    public static string dataPath = Directory.GetCurrentDirectory() + @"\data";
    public static string tempPath = Directory.GetCurrentDirectory() + @"\temp"; //darbo metu atsiradusiem pav
    public static string resultsPath = Directory.GetCurrentDirectory() + @"\results";

    public static void Main()
    {
        if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
        if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
        if (!Directory.Exists(resultsPath)) Directory.CreateDirectory(resultsPath);

        List<Illustration> Illustrations = new List<Illustration>();

        //RecreateTempDir();    //istrinam pries paleidima buvusius pav
        //PdfToImages();
        //DetectObjects();

        // testavimui start ---------------
        for (int i = 1; i < 3; i++)
        {
            Illustrations.Add(new Illustration(
                number: i,
                name: $"Vardas_{i}",
                type: $"Tipas_{i}",
                page: i,
                left: 10,
                top: 20,
                right: 30,
                bottom: 40));
        }
        // testavimui end ---------------
        SaveXml(Illustrations);

    }

    static void RecreateTempDir()
    {
        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }
        Directory.CreateDirectory(tempPath);
    }

    static string?[] DirectoryFileNames(string path)
    {
        string?[] files = Directory.GetFiles(path)
                                     .Select(Path.GetFileName)
                                     .ToArray();
        //Console.WriteLine("Directory path: {0}", path);
        //Console.WriteLine("Files:");
        //foreach (var item in files) Console.WriteLine(item);
        return files;
    }

    static string? DataFileName(string dataDirPath)
    {
        string?[] files = DirectoryFileNames(dataPath);
        if (files != null)
        {
            return files[0];
        }
        else
        {
            return null;
        }
    }

    static void PdfToImages()
    {
        string? pdfName = DataFileName(dataPath);
        if (pdfName == null)
        {
            Console.WriteLine("Data file not found.");
            return;
        }
        Console.WriteLine("Found data file: {0}", pdfName);
        Console.WriteLine("Using .pdf to create images... ");
        Byte[] bytes = File.ReadAllBytes(dataPath + @"\" + pdfName);
        string content = Convert.ToBase64String(bytes);
        int sk = Conversion.GetPageCount(content);
        for (int i = 0; i < sk; i++)
        {
            string imgName = "page_" + (i + 1);
            Console.WriteLine(imgName);
            Conversion.SavePng(tempPath + @"\" + imgName + ".png", content, page: i);
        }
        Console.WriteLine("done.");
    }

    static void DetectObjects()
    {
        string?[] imageNames = DirectoryFileNames(tempPath);

        imageNames.Order();

        if (imageNames != null)
        {
            foreach (string? imageName in imageNames)
            {
                Console.WriteLine(imageName);
            }
        }
    }

    static void SaveXml(List<Illustration> Illustrations)
    {
        XDocument xmlDocument = CreateXmlFile(Illustrations);

        xmlDocument.Save(resultsPath + @"\" + "results.xml");
    }

    static XDocument CreateXmlFile(List<Illustration> Illustrations)
    {
        XDocument document = new XDocument
            (
                new XElement("Graphics")
            );

        foreach (Illustration item in Illustrations)
        {
            XElement elem = new XElement("illustration");
            List<XElement> elements = new List<XElement>();
            elements.Add(new XElement("source",
                            new XAttribute("page", item.Page),
                            new XAttribute("left", item.Left),
                            new XAttribute("top", item.Top),
                            new XAttribute("right", item.Right),
                            new XAttribute("bottom", item.Bottom)));
            elements.Add(new XElement("target",
                new XAttribute("name", item.Name),
                new XAttribute("type", item.Type),
                new XAttribute("number", item.Number)));
            elem.Add(elements);
            document.Root.Add(elem);
        }
        return document;
    }
}