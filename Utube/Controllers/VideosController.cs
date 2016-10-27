using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utube.Models;
using PagedList;
using PagedList.Mvc;



namespace Utube.Controllers
{
    public class VideosController : Controller
    {

        private VideoContext db = new VideoContext();
        public string VKey="AIzaSyAdqVnCZv-Feh76P4ixHH1Sa3nYdePsUZY";

      

        // GET: Videos
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber =( page ?? 1);
            return View(db.Videos.OrderBy((x) => x.Id).ToPagedList(pageNumber,pageSize));
        }

        // GET: Videos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // GET: Videos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Videos/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProfileId,VideoTitle,Vlink,Shared,DateIn,Like,DisLike,Views,Info")] Video video)
        {
            if (ModelState.IsValid)
            {
                video.DateIn = DateTime.Now;
                video.DisLike = 0;
                video.Like = 0;
                video.Views = 0;
                video.Shared = false;
                db.Videos.Add(video);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(video);
        }

        // GET: Videos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: Videos/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProfileId,VideoTitle,Vlink,Shared,DateIn,Like,DisLike,Views,Info")] Video video)
        {
            if (ModelState.IsValid)
            {

                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(video);
        }

        // GET: Videos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Videos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Video video = db.Videos.Find(id);
            db.Videos.Remove(video);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult ShowVideo(int id)
        {
            Video v = db.Videos.Find(id);

            return View(v);
        }
        static RootObject ro = null;
        public ActionResult SearchVideo(string search,int countVideo=1)
        {
       
            string req = "https://www.googleapis.com/youtube/v3/search?part=snippet&order=date&maxResults="+countVideo+"&q=" + search + "&type=video&key=" + VKey;
            Uri uri = new Uri(req);
            WebProxy proxy = new WebProxy("10.3.0.9", 3128);
            proxy.Credentials = new NetworkCredential("07527", "lml0503220233");
            var request = (HttpWebRequest)HttpWebRequest.Create(req);
            request.Proxy = proxy;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string json = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                json = sr.ReadToEnd();
            }
            ro = JsonConvert.DeserializeObject<RootObject>(json);
            return View(ro.items);
        }
        [HttpPost]
        public ActionResult SearchVideo2()
        {            
            string[] Keys = HttpContext.Request.Params.AllKeys;
            foreach(string key in Keys)
            {
               if(key.StartsWith("cb"))
                {
                    string link = key.Substring(2);
                    Item item = ro.items.Where((x) => x.id.videoId == link).FirstOrDefault();
                    Video video = new Video();
                    video.Vlink = "https://www.youtube.com/embed/" + link;
                    if (item.snippet.description != null) video.Info = item.snippet.description;
                    else video.Info="";
                    if (item.snippet.title != null) video.VideoTitle = item.snippet.title;
                    else video.VideoTitle="";
                    video.ProfileId = 1;
                    video.DateIn = DateTime.Now;
                    video.Like = 0;
                    video.DisLike = 0;
                    video.Views = 0;
                    db.Videos.Add(video);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
