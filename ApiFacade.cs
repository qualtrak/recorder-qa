namespace QATestRecorder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Web.Configuration;
    using Qualtrak.Coach.DTO.Integration;
    using Qualtrak.Coach.DTO.Integration.Contracts;

    public class ApiFacade : IApiFacade
    {
        private string _defaultFile = null;

        public ApiFacade()
        {
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["coach:service:defaultmediafile"]))
            {
                _defaultFile = WebConfigurationManager.AppSettings["coach:service:defaultmediafile"];
            }
        }

        public List<RecorderUserInfo> GetUsers(string tenantCode, string username, string password)
        {
            var list = new List<RecorderUserInfo>();
          
            for(var i=0;i<1000; i++) 
            {
                RecorderUserInfo user = new RecorderUserInfo
                {
                    TenantCode = int.Parse(tenantCode),
                    RecorderUserID = i.ToString(),
                    RecorderAcountID = i.ToString(),
                    Username = string.Format("agent.{0}", i),
                    FirstName = "Agent",
                    LastName = i.ToString(),
                    Mail = string.Format("agent.{0}@qualtrak.com", i)
                };

                list.Add(user);
            }
           
            return list;
        }

        public List<RecordingInfo> GetRecordings(int limit, string tenantCode, string userId, List<SearchCriteria> critera, string username, string password)
        {
            int recordingsProcessedSoFar = 0;
            var list = new List<RecordingInfo>();

            for (var i = 0; i < 1000; i++) 
            {
                RecordingInfo recoding = new RecordingInfo
                {
                    TenantCode = int.Parse(tenantCode),
                    RecorderUserID = userId,
                    RecordingID = i.ToString(),
                    RecordingDate = DateTime.Now.AddDays(-1),
                    Metadata = "{ \"metadata\" : [{\"label\" : \"Claim No\", \"value\" : \"123456\", \"field\" : \"claim_no\", \"type\" : \"number\"},{\"label\" : \"Caller Id\", \"value\" : \"004401232312311\", \"field\" : \"caller_id\", \"type\" : \"number\" }, {\"label\" : \"Account No\", \"value\" : \"12121231314AB\", \"field\" : \"account_no\", \"type\" : \"string\" }, {\"label\" : \"A really long label description\", \"value\" : \"A really long piece of call metadata information 1234567890 1234567890\", \"field\" : \"notes\", \"type\" : \"string\" }] }",
                    RecordingFileName = _defaultFile ?? "a.wmv"
                };

                list.Add(recoding);
                recordingsProcessedSoFar ++;

                if (recordingsProcessedSoFar > limit) break;
            }

            return list;
        }

        public List<RecordingInfo> GetRecordingsForUsers(int limit, string tenantCode, List<string> userIds, List<SearchCriteria> searchCriteria, string username,
            string password)
        {
            var result = new List<RecordingInfo>();

            foreach (var userId in userIds)
            {
                result.AddRange(this.GetRecordings(limit, tenantCode, userId, searchCriteria, username, password));
            }

            return result;
        }


        public string GetRecordingUrl(string recordingId, string recordingOriginalUrl, string username, string password)
        {
            return recordingOriginalUrl;
        }

        public void PostEvaluationScore(string tenantCode, string username, string password, string evaluationId,
            string headlineScore, string extraScore, string userId, string recordingId)
        {
            
        }

        public Stream GetStream(string url)
        {
            Stream stream = new MemoryStream(1000000 * 10);

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            using (var client = new HttpClient(){BaseAddress = new Uri(url, UriKind.Absolute)})
            {
                try
                {
                    var response = client.GetByteArrayAsync("").Result;
                    stream = new MemoryStream(response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return stream;
        }
    }
}
