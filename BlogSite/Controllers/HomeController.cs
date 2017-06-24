using BlogSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogSite.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            var post = db.Posts.ToList();
            return View(post);
            //ViewBag.Search = searchStr(put the searchStr as a parameter)
            //    var blogList = IndexSearch(searchStr);
            //return View(blogList.ToPadedList(pagnemuber, pagesize));
            //Html.PagedListPage(Model page => Url.Action("Index", new { page, searchStr = ViewBag.Search }), PagedListRednerOptions.TwitterBootstrapPager) --> This goes in the View;

        }

        public ActionResult MainIndex()
        {
            return View();
        }

        public ActionResult Map()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string searchStr, int? page)
        {
            var result = db.Posts.Where(p => p.Body.Contains(searchStr))
                .Union(db.Posts.Where(p => p.Title.Contains(searchStr)))
                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Body.Contains(searchStr))))
                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.DisplayName.Contains(searchStr))))
                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.FirstName.Contains(searchStr))))
                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.LastName.Contains(searchStr))))
                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.UserName.Contains(searchStr))))
                .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.Email.Contains(searchStr))))
                .Union(db.Posts.Where(p => p.Comments.Any(c => c.UpdateReason.Contains(searchStr))));

            //var listPosts = db.Posts.AsQueryable();
            //listPosts = listPosts.Where(p => p.Title.Contains(searchStr) ||
            //                                 p.Body.Contains(searchStr) ||
            //                                 p.Comments.Any(c => c.Body.Contains(searchStr) ||
            //                                                     c.Author.FirstName.Contains(searchStr) ||
            //                                                     c.Author.LastName.Contains(searchStr) ||
            //                                                     c.Author.UserName.Contains(searchStr) ||
            //                                                     c.Author.Email.Contains(searchStr)));

            //if (searchStr != null)
            //{
            //    do Search linq above
            //}
            //else
            //{
            //    result = db.Posts.AsQueryable();
            //    return result.OrderByDescending(p => p.Created);
            //}

            return View(result);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Photo()
        {
            return View();
        }
    }
}