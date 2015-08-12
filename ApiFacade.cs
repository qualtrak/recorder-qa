using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using Qualtrak.Coach.Integration.Core.Contracts;
using Qualtrak.Coach.Integration.Core.DTO;

namespace QATestRecorder
{
    public class ApiFacade : IApiFacade
    {
        private string _defaultFile = null;
        private static List<RecordingUser> Recordings = new List<RecordingUser>();  

        public ApiFacade()
        {
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["coach:service:defaultmediafile"]))
            {
                _defaultFile = WebConfigurationManager.AppSettings["coach:service:defaultmediafile"];
            }
        }

        public async Task<List<RecorderUserInfo>> GetUsersAsync(string tenantCode, string username = null, string password = null)
        {
            var list = new List<RecorderUserInfo>();

            for (var i = 0; i < 1000; i++)
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

            return await Task.FromResult(list);
        }

        public async Task<List<RecordingInfo>> GetRecordingsAsync(int limit, string tenantCode, string userId, List<SearchCriteria> searchCriteria, string username = null,
            string password = null)
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
                recordingsProcessedSoFar++;

                if (recordingsProcessedSoFar > limit) break;
            }

            return await Task.FromResult(list);
        }

        public async Task<List<RecordingUser>> GetRecordingsForUsersAsync(int limit, string tenantCode, List<string> userIds, List<SearchCriteria> searchCriteria, string username = null,
            string password = null)
        {
            var result = new List<RecordingUser>();

            foreach (var userId in userIds)
            {
                List<RecordingInfo> recordings = await this.GetRecordingsAsync(limit, tenantCode, userId, searchCriteria, username, password);
                List<RecordingUser> recordingUsers = recordings.Select(x => new RecordingUser { RecordingId = x.RecordingID, RecorderUserId = x.RecorderUserID }).ToList();
                result.AddRange(recordingUsers);
                Recordings.AddRange(recordingUsers);
                recordings.Clear();
                recordingUsers.Clear();
            }

            return result;
        }

        public async Task<List<RecordingInfo>> GetRecordingsForRecordingIdsAsync(List<string> recordingIds, string tenantCode, string username = null,
            string password = null)
        {
            var list = new List<RecordingInfo>();

            foreach (var recordingId in recordingIds)
            {
                string userId = Recordings.Where(x => x.RecordingId == recordingId).Select(x => x.RecorderUserId).FirstOrDefault();

                var recoding = new RecordingInfo
                {
                    TenantCode = int.Parse(tenantCode),
                    RecordingID = recordingId,
                    RecorderUserID = userId,
                    RecordingDate = DateTime.Now.AddDays(-1),
                    Metadata = "{ \"metadata\" : [{\"label\" : \"Claim No\", \"value\" : \"123456\", \"field\" : \"claim_no\", \"type\" : \"number\"},{\"label\" : \"Caller Id\", \"value\" : \"004401232312311\", \"field\" : \"caller_id\", \"type\" : \"number\" }, {\"label\" : \"Account No\", \"value\" : \"12121231314AB\", \"field\" : \"account_no\", \"type\" : \"string\" }, {\"label\" : \"A really long label description\", \"value\" : \"A really long piece of call metadata information 1234567890 1234567890\", \"field\" : \"notes\", \"type\" : \"string\" }] }",
                    RecordingFileName = _defaultFile ?? "a.wmv"
                };

                list.Add(recoding);
            }

            return await Task.FromResult(list);
       }

        public async Task<string> GetRecordingUrlAsync(string recordingId, string recordingOriginalUrl, string username = null,
            string password = null)
        {
            return await Task.FromResult(recordingOriginalUrl);
        }

        public async Task PostEvaluationScoreAsync(string tenantCode, string evaluationId, string headlineScore, string extraScore,
            string userId, string recordingId, string username = null, string password = null)
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> GetStreamAsync(string url)
        {
            Stream stream = new MemoryStream(1000000 * 10);

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            using (var client = new HttpClient() { BaseAddress = new Uri(url, UriKind.Absolute) })
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

            return await Task.FromResult(stream);
        }

    }
}
