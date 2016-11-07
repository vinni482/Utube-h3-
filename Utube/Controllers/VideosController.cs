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
using Microsoft.AspNet.Identity;

namespace Utube.Controllers
{
    public class VideosController : Controller
    {

        private VideoContext db = new VideoContext();
        public string VKey="AIzaSyAdqVnCZv-Feh76P4ixHH1Sa3nYdePsUZY";

        // GET: Videos
        public ActionResult Index(int? page)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            if (userId == null) return RedirectToAction("SharedVideo");
            Guid g = Guid.Parse(userId);
            var p = db.Profiles.Where((x) => x.UserId == g).FirstOrDefault();
            int pageSize = 10;
            int pageNumber =( page ?? 1);

            ViewBag.Emails = AllUsers.UsersOnline;
              
            return View(db.Videos.Where((x) => x.Profileid == p.Id).OrderBy((x) => x.Id).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult SharedVideo(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(db.Videos.Where((x) => x.Shared == true).OrderBy((x) => x.Id).ToPagedList(pageNumber, pageSize));
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
        public ActionResult Create([Bind(Include = "Id,Profileid,Videotitle,Vlink,Shared,Datein,Like,Dislike,Views,Info")] Video video)
        {
            if (ModelState.IsValid)
            {
                video.Datein = DateTime.Now;
                video.Dislike = 0;
                video.Like = 0;
                video.Views = 0;
                video.Shared = false;
                var userId = HttpContext.User.Identity.GetUserId();
                if (userId == null) return RedirectToAction("SharedVideo");
                Guid g = Guid.Parse(userId);
                var p = db.Profiles.Where((x) => x.UserId ==g).FirstOrDefault();
                video.Profileid = p.Id;
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
        public ActionResult Edit([Bind(Include = "Id,Profileid,Videotitle,Vlink,Shared,Datein,Like,Dislike,Views,Info")] Video video)
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
            proxy.Credentials = new NetworkCredential("06755", "iwillbehappy");
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
                    if (item.snippet.title != null) video.Videotitle = item.snippet.title;
                    else video.Videotitle="";
                    var userId = HttpContext.User.Identity.GetUserId();
                    if (userId == null) return RedirectToAction("SharedVideo");
                    Guid g = Guid.Parse(userId);
                    var p = db.Profiles.Where((x) => x.UserId == g).FirstOrDefault();
                    video.Profileid = p.Id; 
                    video.Datein = DateTime.Now;
                    video.Like = 0;
                    video.Dislike = 0;
                    video.Views = 0;
                    db.Videos.Add(video);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        // GET: Videos/Delete/5
        public ActionResult AddVideo(int? id)
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
        [HttpPost]
        public ActionResult AddVideo()
        {

            string[] Keys = HttpContext.Request.Params.AllKeys;
            foreach (string key in Keys)
            {
                if (key.StartsWith("input"))
                {
                    int id = Convert.ToInt32(key.Substring(5));
                    Video video = db.Videos.Find(id);
                    Video video2 = new Video();               
                    var userId = HttpContext.User.Identity.GetUserId();
                    if (userId == null) return RedirectToAction("SharedVideo");
                    Guid g = Guid.Parse(userId);
                    var p = db.Profiles.Where((x) => x.UserId == g).FirstOrDefault();
                    video2.Profileid = p.Id;
                    video2.Datein = DateTime.Now;
                    video2.Like = 0;
                    video2.Dislike = 0;
                    video2.Views = 0;
                    video2.Vlink = video.Vlink;
                    video2.Videotitle = video.Videotitle;
                    db.Videos.Add(video2);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetProfileName(int? id)
        {
            Profile profile = null;
            if(id != null)
            {
                profile = db.Profiles.Where(A => A.Id == id).FirstOrDefault();
            }
            return PartialView("GetProfileName", profile.Name);
        }

        public string IncrementLike(int? id)
        {
            Guid g = Guid.Parse(HttpContext.User.Identity.GetUserId());
            string str = string.Empty;
            if (id != null)
            {
                Video video = db.Videos.Where(a => a.Id == id).FirstOrDefault();
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    LimitLike likes = new LimitLike
                    {
                        UserId = g,
                        VideoId = video.Id
                    };
                    if(!db.LimitLikes.Contains(likes))
                    {
                        db.LimitLikes.Add(likes);
                        video.Like++;
                        db.SaveChanges();
                    }
                }
                str = video.Like.ToString();
                db.SaveChanges();
            }
            return str;
        }

        public ActionResult WebGrid()
        {
            var list = db.Videos.ToList();
            return View(list);
        }
    }
}
