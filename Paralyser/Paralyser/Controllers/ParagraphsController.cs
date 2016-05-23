using Paralyser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Paralyser.Controllers
{
    public class ParagraphsController : ApiController
    {
        public void Post([FromBody]string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            using (var db = new DatabaseContext())
            {
                var p = new ParagraphText() { Text = text };
                db.Paragraphs.Add(p);
                db.SaveChanges();
            }
        }
    }
}