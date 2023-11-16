using ObjectDetection;
using Microsoft.ML.Data;
using System.Linq;
using System.Xml.Linq;
using PDFtoImage;

namespace Kriu
{
    class ODetection
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

            //RecreateTempDir();    // istrinam pries paleidima buvusius pav, atkomentuot baigus testavima, dirbant su tikru pdf ir t.t.
            //PdfToImages();        // same as above
            DetectObjects();

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
            //SaveXml(Illustrations);
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
            // Create single instance of sample data from first line of dataset for model input.

            string?[] ImgNames = DirectoryFileNames(tempPath);

            if (ImgNames == null || ImgNames.Length == 0)
            {
                Console.WriteLine("No images found.");
                return;
            }

            //for (int i = 1; i < 9; i++)
            foreach (string? imgName in ImgNames)
            {
                //string imgName = "Screenshot_4" + i + ".png";
                //string path = @"D:\aaa\Dataset Test\" + imgName;
                Console.WriteLine($"File: {imgName}");
                string path = tempPath + @"\" + imgName;
                var image = MLImage.CreateFromFile(path);
                MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
                {
                    Image = image,
                };
                // Make a single prediction on the sample data and print results.
                var predictionResult = MLModel1.Predict(sampleData);
                Console.WriteLine("\n\nPredicted Boxes:\n");
                if (predictionResult.PredictedBoundingBoxes == null)
                {
                    Console.WriteLine("No Predicted Bounding Boxes");
                    return;
                }
                var boxes =
                    predictionResult.PredictedBoundingBoxes.Chunk(4)
                        .Select(x => new { XTop = x[0], YTop = x[1], XBottom = x[2], YBottom = x[3] })
                        .Zip(predictionResult.Score, (a, b) => new { Box = a, Score = b });

                foreach (var item in boxes)
                {
                    Console.WriteLine($"XTop: {item.Box.XTop},YTop: {item.Box.YTop},XBottom: {item.Box.XBottom},YBottom: {item.Box.YBottom}, Score: {item.Score}");
                }
                Console.WriteLine("\n\n");
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
}