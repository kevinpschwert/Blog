using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogSite.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;
using static System.String;

namespace BlogSite.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page)
        {
            int pageSize = 3; // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            var blogPost = db.Posts.ToList();

            BlogPost blog = new BlogPost();
            var min = db.Posts.Where(n => n.Body.Length > 100);

            foreach (var x in min.ToList())
            {
                var words = x.Body.Split(' ');
                var totalChar = 0;
                var summary = new List<string>();

                foreach (var n in words)
                {
                    summary.Add(n);
                    totalChar += n.Length + 1;
                    if (totalChar > 300)
                        break;
                }

                var sum = Join(" ", summary) + "...";
                x.Min = sum;
                db.SaveChanges();

            }
            //db.Posts.AsQueryable().ToPagedList(pageNumber, pageSize
            return View(blogPost);
        }

        [Authorize(Roles = "Admin")]
        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }


        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Body")] BlogPost blogPost, HttpPostedFileBase image)
        {
            
                if (image != null && image.ContentLength > 0)
                {
                    //check the file name to make sure its an image
                    var ext = Path.GetExtension(image.FileName).ToLower();
                    if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                        ModelState.AddModelError("image", "Invalid Format.");
                }

                if (image != null)
                {
                    //relative server path
                    var filePath = "~/img/";
                    // path on physical drive on server
                    var absPath = Server.MapPath(filePath);
                    // media url MediaURL relative path
                    blogPost.MediaURL = filePath + image.FileName;
                    //save image
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }

                if (blogPost.MediaURL == null)
                {
                    blogPost.MediaURL = "~/img/mikey.jpg";
                }

            blogPost.Created = DateTimeOffset.Now;

            if (ModelState.IsValid)
            {

                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }


                blogPost.Slug = Slug;
                
                blogPost.Update = DateTimeOffset.Now;

                db.Posts.Add(blogPost);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        Console.WriteLine(error);
                    }
                }
                return View(blogPost);
            }
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Slug,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(blogPost).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            if (image != null && image.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }

            if (image != null)
            {
                //relative server path
                var filePath = "~/img/";
                // path on physical drive on server
                var absPath = Server.MapPath(filePath);
                // media url for relative path
                blogPost.MediaURL = filePath + image.FileName;
                //save image
                image.SaveAs(Path.Combine(absPath, image.FileName));
            }

            if (blogPost.MediaURL == null)
            {
                blogPost.MediaURL = "~/img/mikey.jpg";
                //var blog = db.Posts.Where(x => x.Id == blogPost.Id).ToList();

                //image = db.Posts.Add(blog.());
            }

            blogPost.Created = DateTimeOffset.Now;
            blogPost.Update = DateTimeOffset.Now;

            db.Posts.Attach(blogPost);
            db.Entry(blogPost).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
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
    }
}

//var excluded = new[] { "mediaURL" }
// var entry = context.Entry(obj);
//entry.State = EntityStateModified;
//foreach (var name in excluded
// {
//   entry.Property(name).isModified = false;
// }