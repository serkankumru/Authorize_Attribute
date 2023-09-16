using System;
using System.IO;
using DAL;
using Haberler.MyResult;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Image = DAL.Image;
using Haberler.Auth;
using DAL.Repository;
using Haberler.Extensions;

namespace Haberler.Controllers
{
    [AuthorizeEditor]
    public class NewsController : Controller
    {
        NewsRepository newsRepository;
        ImageRepository imageRepository;
        CategoryRepository categoryRepository;
        CommentRepository commentRepository;
        NewsContenRepository newsContenRepository;
        SliderRepository sliderRepository;

        public NewsController(NewsRepository newsRepository,
                              ImageRepository imageRepository,
                              CategoryRepository categoryRepository,
                              CommentRepository commentRepository,
                              NewsContenRepository newsContenRepository,
                              SliderRepository sliderRepository)
        {
            this.newsRepository = newsRepository;
            this.imageRepository = imageRepository;
            this.categoryRepository = categoryRepository;
            this.commentRepository = commentRepository;
            this.newsContenRepository = newsContenRepository;
            this.sliderRepository = sliderRepository;
        }

        #region News CRUD

        //Create News return ViewBag.Catagory and View  NewsT
        public ActionResult NewsAdd()
        {
            ViewBag.Catagory = categoryRepository.List();
            return View(new NewsT());
        }

        //Edit News get by Id return ViewBag.Catagory and View FindById(id)
        public ActionResult NewsEdit(int id)
        {
            ViewBag.Catagory = categoryRepository.List();
            return View(newsRepository.FindById(id));
        }

        //Delete News get by Id RedirectToAction Editor/Index
        public ActionResult NewsDelete(int id)
        {
            Editor edt = (Editor)Session["Editor"];
            NewsT news = newsRepository.FindById(id);
            if (news.EditorId == edt.Id)
            {
                commentRepository.RemoveNewsItem(news.Id);
                ImageUpload.ImageRemove(newsContenRepository.RemoveNewsItem(news.Id));

                if (sliderRepository.List().Where(x => x.NewsId == news.Id).Count() != 0)
                    sliderRepository.Remove(sliderRepository.List().FirstOrDefault(x => x.NewsId == news.Id).Id);

                newsRepository.Remove(news.Id);
                ImageUpload.ImageRemove(news.Image.Name);
                imageRepository.Remove(news.Image.Id);
            }
            return RedirectToAction("Index", "Editor");
        }

        //Create and Edit News Http Post RedirectToAction Editor/Index
        [HttpPost]
        public ActionResult NewsAdd(NewsT news, HttpPostedFileBase ImagePost)
        {
            Editor edtSession = (Editor)Session["Editor"];
            if (!ModelState.IsValid)
            {
                return View(news);
            }

            if (news.Id == 0)
            {
                int imagesId = ImageUpload.ImageAddNews(ImagePost);
                news.EditorId = edtSession.Id;
                news.CreateDate = DateTime.Now;
                news.Status = true;
                if (imagesId != 0)
                    news.ImagesId = imagesId;
                newsRepository.Add(news);
            }
            else
            {
                NewsT nws = newsRepository.FindById(news.Id);

                int oldImages = nws.ImagesId.Value;
                int postImages = ImageUpload.ImageAddNews(ImagePost);

                nws.ImagesId = nws.ImagesId != postImages && postImages >= oldImages ? postImages : nws.ImagesId;
                nws.Title = news.Title;
                nws.Spot = news.Spot;
                nws.Content = news.Content;
                nws.CatId = news.CatId;

                newsRepository.Update(nws);
                if (oldImages != 2 && postImages != 2)
                {
                    ImageUpload.ImageRemove(imageRepository.FindById(oldImages).Name);
                    imageRepository.Remove(oldImages);
                }
            }
            return RedirectToAction("Content", "Content", new { id = news.Id });
        }
        //News Status Open-Close
        public int NewsStatus(int id)
        {
            Editor edtSession = (Editor)Session["Editor"];
            if (edtSession == null)
                return 0;

            int result = newsRepository.NewsStatus(id);
            return result;
        }

        //Export To Excel Editor News
        public MyFileResult ExportToExcelNews()
        {
            Editor edtSession = (Editor)Session["Editor"];
            var news = newsRepository.List().Where(x => x.EditorId == edtSession.Id).ToList();
            return new MyFileResult(news, edtSession.Name + " " + edtSession.Surname);
        }

        #endregion

        #region Comment

        public ActionResult CommentList(int id)
        {
            return View(commentRepository.List().Where(x => x.NewsId == id).ToList());
        }

        public ActionResult CommentRemove(int id, int newsId)
        {
            commentRepository.Remove(id);
            return View("CommentList", commentRepository.List().Where(x => x.NewsId == newsId).ToList());
        }

        #endregion

    }
}