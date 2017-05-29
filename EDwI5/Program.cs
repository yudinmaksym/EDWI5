using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.Messages;
using Lucene.Net.Spatial;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using System.IO;
using static Lucene.Net.Search.SimpleFacetedSearch;
using static Lucene.Net.Search.Searcher;
using static Lucene.Net.Search.Query;

namespace EDwI5
{
    class Program
    {

        static void Main(string[] args)
        {
            int counter = 0;
            DirectoryInfo dinfo = new DirectoryInfo(@"E:\Downloads\books");
            var fs = FSDirectory.Open("E:\\Downloads\\books");
            FileInfo[] Files = dinfo.GetFiles("*.txt", SearchOption.AllDirectories);
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            IndexWriter wr = new IndexWriter(fs, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            
            foreach (FileInfo file in Files)
            {
                var keyword = "Title:";
                using (var sr = new StreamReader(file.DirectoryName + "\\" + file))
                {

                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line)) continue;
                        if (line.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            var doc = new Document();
                            counter++;
                            Console.WriteLine(line);
                            doc.Add(new Field("id", counter.ToString(), Field.Store.YES, Field.Index.NO));
                            doc.Add(new Field("Title", line, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.NO));
                            wr.AddDocument(doc);
                            //File.WriteAllText("7.txt", line);
                            Console.WriteLine(doc);

                        }
                    }
                }
            }
            wr.Optimize();
            wr.Commit();
            wr.Dispose();

           
            metka:


                //var reader = wr.GetReader();
                //var searcher = new IndexSearcher(reader);
                Console.WriteLine("Input ur title:");
                string yourinput = Console.ReadLine();

                var mydirectory = FSDirectory.Open(new DirectoryInfo(@"E:\Downloads\books"));
                Analyzer myanalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "Title" }, myanalyzer); // (1)
                Query query = parser.Parse(yourinput);
                var searcher = new IndexSearcher(mydirectory, true);

                TopDocs topDocs = searcher.Search(query, 10);
                int results = topDocs.ScoreDocs.Length;
                for (int i = 0; i < results; i++)
                {
                    ScoreDoc scoreDoc = topDocs.ScoreDocs[i];
                    float score = scoreDoc.Score;
                    int docId = scoreDoc.Doc;
                    Document doc = searcher.Doc(docId);
                    Console.WriteLine("ID:" + doc.Get("id") + " " + doc.Get("Title"));


                }

            goto metka;
            
        }


    }
}
