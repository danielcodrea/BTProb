using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Server;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void SqlStoredProcedureJobs ()
    {
        //remove Certificate policy - for POC scope
        ServicePointManager.ServerCertificateValidationCallback =
                        ((sender, certificate, chain, sslPolicyErrors) => true);

        var jobsIDs = new List<int>();
        
         HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:44377/api/v1/jobs/xml");//63227 - 44377
        request.Method = "GET";
        request.ContentLength = 0;
        request.Credentials = CredentialCache.DefaultCredentials;
        request.ContentType = "application/xml";
        request.Accept = "application/xml";


        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            using (Stream receiveStream = response.GetResponseStream())
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    string strContent = readStream.ReadToEnd();
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(strContent);
                    SqlPipe pipe = SqlContext.Pipe;
                    SqlMetaData[] cols = new SqlMetaData[1];
                    cols[0] = new SqlMetaData("ID", SqlDbType.Int);

                    for (int i = 0; i < xdoc.ChildNodes.Count; i++)
                    {
                        int id = int.Parse(xdoc.ChildNodes[i].Attributes["ID"].Value);
                        if (id == 0)
                        {
                            break;
                        }
                        jobsIDs.Add(id);
                    }

                    pipe.SendResultsEnd();
                }
            }
        }

        foreach (int jobid in jobsIDs)
        {
            HttpWebRequest processRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:63227/api/v1/directprocess/processjob/?jobId={jobid}");
            processRequest.Method = "GET";
            processRequest.ContentLength = 0;
            processRequest.Credentials = CredentialCache.DefaultCredentials;
            processRequest.ContentType = "application/xml";
            processRequest.Accept = "application/xml";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream receiveStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        string strContent = readStream.ReadToEnd();
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(strContent);

                        for (int i = 0; i < xdoc.ChildNodes.Count; i++)
                        {
                            int id = int.Parse(xdoc.ChildNodes[i].Attributes["ID"].Value);
                            jobsIDs.Add(id);
                        }

                    }
                }
            }
        }
    }
}
