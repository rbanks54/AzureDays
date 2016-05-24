using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Paralyser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Paralyser.Controllers
{
    public class ParagraphsController : ApiController
    {
        DocumentClient client;
        string endpointUri = "https://rb-paralyser.documents.azure.com:443/";
        string primaryKey = "YRtGwcjSKYAQnhvhv7sjNEUdm7oUGEf7KfnWUSTsai9jyhNLb2tNiavogHnIaoNWFrs8mRdsaSJf3WgSq9tHDg==";
        string documentDbName = "ParagraphsDb";
        string collectionName = "paragraphs";

        public async Task Post([FromBody]string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            //using (var db = new DatabaseContext())
            //{
            //    var p = new ParagraphText() { Text = text };
            //    db.Paragraphs.Add(p);
            //    db.SaveChanges();
            //}

            await SaveToDocumentDb(text);
        }

        private async Task SaveToDocumentDb(string text)
        {
            var p = new ParagraphDocument() { ID = Guid.NewGuid(), Text = text };
            
            client = new DocumentClient(new Uri(endpointUri), primaryKey);
            await CreateDatabaseIfNotExists(documentDbName);
            await CreateDocumentCollectionIfNotExists(documentDbName, collectionName);
            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(documentDbName, collectionName),p);
        }

        private async Task CreateDatabaseIfNotExists(string databaseName)
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = databaseName });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateDocumentCollectionIfNotExists(string databaseName, string collectionName)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        new DocumentCollection { Id = collectionName },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}