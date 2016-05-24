using Microsoft.Azure.Documents.Client;
using Paralyser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace WordFrequencyService.Controllers
{
    public class FrequencyController : ApiController
    {
        DocumentClient client;
        string endpointUri = "https://rb-paralyser.documents.azure.com:443/";
        string primaryKey = "YRtGwcjSKYAQnhvhv7sjNEUdm7oUGEf7KfnWUSTsai9jyhNLb2tNiavogHnIaoNWFrs8mRdsaSJf3WgSq9tHDg==";
        string documentDbName = "ParagraphsDb";
        string collectionName = "paragraphs";
        Uri collectionUri;

        public FrequencyController()
        {
            client = new DocumentClient(new Uri(endpointUri), primaryKey);
            collectionUri = UriFactory.CreateDocumentCollectionUri(documentDbName, collectionName);
        }

        // GET api/frequency/{word} 
        public string Get(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            //to get word frequency we need to know how many words in total, and how many match the target word
            //get all documents from the doc db and then process
            var totalWords = 0m;
            var matchedWords = 0m;
            var allDocs = LoadAll().Result;
            foreach (var pd in allDocs)
            {
                var words = pd.Text.Split(' ');
                totalWords += words.Count();
                matchedWords += words.Count(w => word.Equals(w,StringComparison.InvariantCultureIgnoreCase));
            }
            return (matchedWords / totalWords).ToString("P");
        }

        private Task<IEnumerable<ParagraphDocument>> LoadAll()
        {
            var feedOptions = new FeedOptions { MaxItemCount = 1000 };

            var query = client.CreateDocumentQuery<ParagraphDocument>(
                        collectionUri, feedOptions)
                        .Where(x => x.Text != null).AsEnumerable(); 
            var response = query.ToArray();

            if (!response.Any())
            {
                return Task.FromResult(Enumerable.Empty<ParagraphDocument>());
            }

            return Task.FromResult(response.AsEnumerable());
        }

    }
}
