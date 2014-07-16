using System;
using System.IO;
using System.Xml;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkFileReader
    {
        private readonly string _file;

        public SparkFileReader(string file)
        {
            _file = file;
        }

        // <viewdata model="FubuMVC.Diagnostics.DashboardModel" />

        public Parsing Read()
        {
            var parsing = new Parsing();

            using (var reader = new StreamReader(_file))
            {
                string line;
                // Read and display lines from the file until the end of  
                // the file is reached. 
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("<viewdata"))
                    {
                        var document = new XmlDocument();
                        document.LoadXml(line.Trim());
                        parsing.ViewModelType = document.DocumentElement.GetAttribute("model");


                    }
                }
            }


            return parsing;

        }
    }
}